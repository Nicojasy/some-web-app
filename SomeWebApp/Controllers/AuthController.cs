using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SomeWebApp.Application.Auth;
using SomeWebApp.Application.Checker;
using SomeWebApp.Application.Interfaces;
using SomeWebApp.Core.Auth;
using SomeWebApp.Core.Entities;
using SomeWebApp.LogExtension;
using SomeWebApp.Models.Auth;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SomeWebApp.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IEnteredDataChecker _checker;

        public AuthController(ILogger<AuthController> logger, IConfiguration configuration, IUnitOfWork unitOfWork,
            IAuthService authService, IPasswordHasher passwordHasher, IEnteredDataChecker checker)
        {
            _logger = logger;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _authService = authService;
            _passwordHasher = passwordHasher;
            _checker = checker;
        }

        [HttpPost, Route("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupDto signup_dto)
        {
            if (_checker.CheckNicknameAndEmailAndPassword(signup_dto.nickname, signup_dto.email,signup_dto.password))
            {
                return BadRequest("Invalid client request");
            }

            var foundUsers = await _unitOfWork.Users.GetCountUsersByNicknameOrEmailAsync(signup_dto.nickname, signup_dto.email);
            if (foundUsers != default)
            {
                // TODO: nickname or email exists?
                return BadRequest("User exists");
            }

            var passwordHash = _passwordHasher.Hash(signup_dto.password);
            var newUser = new UserModel(signup_dto.nickname, signup_dto.email, passwordHash);
            
            var affectedRows = await _unitOfWork.Users.AddAsync(newUser);
            if (affectedRows == 0)
            {
                _logger.WarnDatabaseQueryReturnedZeroAffectedRows($"Add user => user_nickname: '{signup_dto.nickname}', user_email: '{signup_dto.email}', user_password: '{signup_dto.password}', user_passwordHash: '{passwordHash}'");
                return BadRequest();
            }

            return Ok();
        }

        [HttpPost, Route("signin")]
        public async Task<IActionResult> Signin([FromBody] LoginDto login_dto)
        {
            if (login_dto.login == null || login_dto.password == null)
            {
                return BadRequest("Invalid client request");
            }

            var user = await _unitOfWork.Users.GetByLoginAsync(login_dto.login);

            if (user == null)
            {
                return Unauthorized();
            }

            try
            {
                var check = _passwordHasher.Check(user.Password, login_dto.password);

                // TODO: if necessary, add a counter of attempts to change the password
                if (!check.Verified)
                {
                    if (check.NeedsUpgrade)
                        _logger.WarnNeedsUpgrade($"Needs upgrade password => user_password: {user.Password}");

                    return Unauthorized();
                }
            }
            catch (FormatException ex)
            {
                _logger.WarnBadPasswordFormatWithException(user.Password, ex);
                return BadRequest();
            }
            
            // TODO: null return?
            var scopes = await _unitOfWork.Roles.GetPermissionNamesByUserIDAsync(user.ID);
            
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Nickname),
                new Claim("scope", scopes != null ? string.Join(' ', scopes) : string.Empty)
            };

            var newTokens = new TokensModel(
                _authService.GenerateAccessToken(claims),
                _authService.GenerateRefreshToken());

            RefreshSessionModel newRefreshSession =
                new RefreshSessionModel(user.ID, newTokens.RefreshToken, newTokens.CreationTimestamp);

            // TODO: temporarily
            await _unitOfWork.RefreshSessions.DeleteAllByUserIDAsync(user.ID);

            var affectedRows = await _unitOfWork.RefreshSessions.AddAsync(newRefreshSession);

            if (affectedRows == 0)
            {
                _logger.WarnDatabaseQueryReturnedZeroAffectedRows($"Add refresh_session => id_user: '{newRefreshSession.ID_User}', refresh_token: '{newRefreshSession.RefreshToken}', CreationTimestamp: '{newRefreshSession.CreationTimestamp}'");
                return BadRequest();
            }

            return Ok(newTokens);
        }
        
        [HttpPost, Route("refresh")]
        [Authorize]
        public async Task<IActionResult> Refresh([FromBody] RefreshDto tokenApiModel)
        {
            if (tokenApiModel is null)
            {
                return BadRequest("Invalid client request");
            }
            
            string accessToken = tokenApiModel.access_token;
            string refreshToken = tokenApiModel.refresh_token;
            ClaimsPrincipal principal = _authService.GetPrincipalFromExpiredToken(accessToken);
            
            string nickname = principal?.Identity?.Name;
            if (nickname == null)
            {
                return BadRequest("Invalid client request");
            }

            // TODO: improve the code
            //delete
            var session = await _unitOfWork.RefreshSessions.GetByNicknameAndRefreshTokenAsync(nickname, refreshToken);
            
            if (session == null ||
                session.CreationTimestamp.AddDays(_configuration.GetValue<int>("Tokens:Lifecycle:RefreshToken.days")) <= DateTimeOffset.UtcNow)
            {
                return BadRequest("Invalid refresh session");
            }
            //update
            await _unitOfWork.RefreshSessions.DeleteAsync(session.ID);

            TokensModel newTokens = new TokensModel(
                _authService.GenerateAccessToken(principal.Claims),
                _authService.GenerateRefreshToken());

            RefreshSessionModel newRefreshSession =
                new RefreshSessionModel(session.ID_User, newTokens.RefreshToken, newTokens.CreationTimestamp);

            var affectedRows = await _unitOfWork.RefreshSessions.AddAsync(newRefreshSession);

            if (affectedRows == 0)
            {
                _logger.WarnDatabaseQueryReturnedZeroAffectedRows($"Add refresh_session => id_user: '{session.ID_User}', refresh_token: '{session.RefreshToken}', creation_timestamp: '{session.CreationTimestamp}'");
                return BadRequest();
            }

            return new ObjectResult(newTokens);
        }

        [HttpPost, Route("signout")]
        [Authorize]
        public async Task<IActionResult> Signout([FromBody] string refresh_token)
        {
            var nickname = User.Identity.Name;

            // TODO: improve the code
            //delete
            var session = await _unitOfWork.RefreshSessions.GetByNicknameAndRefreshTokenAsync(nickname, refresh_token);
            
            if (session == null ||
                session.CreationTimestamp.AddDays(_configuration.GetValue<int>("Tokens:Lifecycle:RefreshToken.days")) <= DateTimeOffset.UtcNow)
            {
                return BadRequest("Invalid client request");
            }
            //update
            var affectedRows = await _unitOfWork.RefreshSessions.DeleteByUserIDAndRefreshTokenAsync(session.ID_User, session.RefreshToken);
            
            if (affectedRows == 0)
            {
                _logger.InfoDatabaseQueryReturnedZeroAffectedRows($"Delete refresh_session => id_user: '{session.ID_User}', refresh_token: '{session.RefreshToken}', creation_timestamp: '{session.CreationTimestamp}'");
                return BadRequest("Session not found");
            }

            return NoContent();
        }


        /*
        [HttpPost, Route("revoke")]
        [Authorize]
        public async Task<IActionResult> Revoke([FromBody] string refresh_token)
        {
            var nickname = User.Identity.Name;

            var sessions = await unitOfWork.RefreshSessions.SearchUserRefreshSessionsByNickname(nickname);

            // TODO: revoke from session not via refresh_token
            RefreshTokenModel sessionRefreshToken = sessions?.refreshTokens.FirstOrDefault(t=>t.RefreshToken== refresh_token);
            
            if (sessionRefreshToken == null ||
                sessionRefreshToken.CreationTimestamp.AddDays(Convert.ToDouble(configuration["Tokens:Lifecycle:RefreshToken.days"])) <= DateTimeOffset.UtcNow)
            {
                return BadRequest("Invalid client request");
            }

            await unitOfWork.RefreshSessions.DeleteRefreshSessionAsync(sessions.ID_User, sessionRefreshToken.RefreshToken);

            return NoContent();
        }
        */
    }
}
