using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SomeWebApp.Application.Auth;
using SomeWebApp.Application.Checker;
using SomeWebApp.Application.Interfaces;
using SomeWebApp.Core.Auth;
using SomeWebApp.Core.Entities;
using SomeWebApp.Models.Auth;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SomeWebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IChecker _checker;

        public AuthController(ILogger<AuthController> logger, IConfiguration configuration, IUnitOfWork unitOfWork,
            IAuthService authService, IPasswordHasher passwordHasher, IChecker checker)
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
                return BadRequest("User exists");
            }

            var passwordHash = _passwordHasher.Hash(signup_dto.password);
            var newUser = new UserModel(signup_dto.nickname, signup_dto.email, passwordHash);
            
            var affectedRows = await _unitOfWork.Users.AddAsync(newUser);
            if (affectedRows == 0)
            {
                // TODO: log
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

            // TODO: password verification is 401
            if (user == null ||
                !_passwordHasher.Check(user.Password, login_dto.password).Verified)
            {
                return Unauthorized();
            }

            //var roles = await _unitOfWork.Roles.GetRoleNamesByUserIDAsync(user.ID);

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Nickname),
            };
            //add:role
            //edit:role
            //delete:role
            //claims.AddRange(roles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));

            var newTokens = new TokensModel(
                _authService.GenerateAccessToken(claims),
                _authService.GenerateRefreshToken());

            RefreshSessionModel newRefreshSession =
                new RefreshSessionModel(user.ID, newTokens.RefreshToken, newTokens.CreationTimestamp);

            // TODO: temporarily
            await _unitOfWork.RefreshSessions.DeleteAllByUserIDAsync(user.ID);

            await _unitOfWork.RefreshSessions.AddAsync(newRefreshSession);
            
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

            var session = await _unitOfWork.RefreshSessions.GetByNicknameAndRefreshTokenAsync(nickname, refreshToken);
            
            if (session == null ||
                session.CreationTimestamp.AddDays(_configuration.GetValue<double>("Tokens:Lifecycle:RefreshToken.days")) <= DateTimeOffset.UtcNow)
            {
                return BadRequest("Invalid refresh session");
            }
            
            TokensModel newTokens = new TokensModel(
                _authService.GenerateAccessToken(principal.Claims),
                _authService.GenerateRefreshToken());

            RefreshSessionModel newRefreshSession = new RefreshSessionModel(session.ID_User, newTokens.RefreshToken, newTokens.CreationTimestamp);

            await _unitOfWork.RefreshSessions.DeleteAsync(session.ID);
            await _unitOfWork.RefreshSessions.AddAsync(newRefreshSession);

            return new ObjectResult(newTokens);
        }

        [HttpPost, Route("signout")]
        [Authorize]
        public async Task<IActionResult> Signout([FromBody] string refresh_token)
        {
            var nickname = User.Identity.Name;
            
            var session = await _unitOfWork.RefreshSessions.GetByNicknameAndRefreshTokenAsync(nickname, refresh_token);
            
            if (session == null ||
                session.CreationTimestamp.AddDays(_configuration.GetValue<double>("Tokens:Lifecycle:RefreshToken.days")) <= DateTimeOffset.UtcNow)
            {
                return BadRequest("Invalid client request");
            }

            var affectedRows = await _unitOfWork.RefreshSessions.DeleteByUserIDAndRefreshTokenAsync(session.ID_User, session.RefreshToken);
            
            if (affectedRows == 0)
            {
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
