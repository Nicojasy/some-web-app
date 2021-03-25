using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SomeWebApp.Application.Auth;
using SomeWebApp.Application.Interfaces;
using SomeWebApp.Core.Entities;
using SomeWebApp.LogExtension;
using SomeWebApp.Models.User;
using System;
using System.Threading.Tasks;

namespace SomeWebApp.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<AuthController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public UsersController(ILogger<AuthController> logger, IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
        {
            _passwordHasher = passwordHasher;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [HttpGet, Route("")]
        public async Task<IActionResult> GetUsers()
        {
            // TODO: pagination

            return Ok(/*users*/);
        }

        [HttpGet, Route("{user_id}")]
        public async Task<IActionResult> GetUser(UInt64 user_id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(user_id);
            if (user == null)
            {
                _logger.InfoDatabaseQueryReturnedNull($"Get user by user_id => user_id: {user_id}");
                return BadRequest();
            }
            return Ok(user);
        }

        [HttpGet, Route("{user_id}/roleswithpermissions")]
        public async Task<IActionResult> GetRolesWithPermissionsOfUser(UInt64 user_id)
        {
            var rolesWithPermissions = await _unitOfWork.Roles.GetRolesWithPerissionsByUserIDAsync(user_id);
            if (rolesWithPermissions == null)
            {
                return Ok();
            }
            return Ok(rolesWithPermissions);
        }

        [HttpPost, Route("{user_id}/roles/{role_id}/addrolelink")]
        public async Task<IActionResult> AddPermissionLinkForRole(UInt64 user_id, UInt64 role_id)
        {
            var newLink = new UserRoleModel(user_id, role_id);

            var affectedRows = await _unitOfWork.Roles.AddUserRoleAsync(newLink);

            if (affectedRows == 0)
            {
                _logger.WarnDatabaseQueryReturnedZeroAffectedRows($"Add user_role => user_id: '{user_id}', role_id: '{role_id}'");
                return BadRequest();
            }
            return Ok();
        }

        [HttpDelete, Route("{user_id}")]
        public async Task<IActionResult> DeleteUser(UInt64 user_id)
        {
            var affectedRows = await _unitOfWork.Users.UpdateIsDeletedAsync(user_id, true);
            if (affectedRows == 0)
            {
                _logger.InfoDatabaseQueryReturnedZeroAffectedRows($"Update user isDeleted to true (delete user) => user_id: '{user_id}'");
                return BadRequest();
            }
            return Ok();
        }

        [HttpPost, Route("{user_id}/restore")]
        public async Task<IActionResult> RestoreUser(UInt64 user_id)
        {
            var affectedRows = await _unitOfWork.Users.UpdateIsDeletedAsync(user_id, false);
            if (affectedRows == 0)
            {
                _logger.InfoDatabaseQueryReturnedZeroAffectedRows($"Update user isDeleted to false (restore user) => user_id: '{user_id}'");
                return BadRequest();
            }
            return Ok();
        }

        [HttpPost, Route("{user_id}/newpassword")]
        public async Task<IActionResult> NewPasswordOfUser(UInt64 user_id,
            [FromBody] ChangePasswordDto password_dto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(user_id);

            if (user == null)
            {
                return BadRequest();
            }

            try
            {
                var check = _passwordHasher.Check(user.Password, password_dto.OldPassword);
                // TODO: if necessary, add a counter of attempts to change the password
                if (!check.Verified)
                {
                    if (check.NeedsUpgrade)
                        _logger.WarnNeedsUpgrade($"Needs upgrade password => user_password: {user.Password}");

                    _logger.InfoPasswordsMismatch(user.Password, password_dto.OldPassword);
                    return BadRequest();
                }
            }
            catch(FormatException ex)
            {
                _logger.WarnBadPasswordFormatWithException(user.Password, ex);
                return BadRequest();
            }

            var hash = _passwordHasher.Hash(password_dto.NewPassword);
            var affectedRows = await _unitOfWork.Users.UpdatePasswordAsync(user_id, hash);
            if (affectedRows == 0)
            {
                _logger.InfoDatabaseQueryReturnedZeroAffectedRows($"Update user password => user_id: '{user_id}', new_password: '{password_dto.NewPassword}', hash: '{hash}'");
                return BadRequest();
            }
            return Ok();
        }

        [HttpPost, Route("{user_id}/newbio")]
        public async Task<IActionResult> NewBioOfUser(UInt64 user_id, [FromBody] string bio)
        {
            if (bio == null)
            {
                return BadRequest();
            }

            var affectedRows = await _unitOfWork.Users.UpdateBioAsync(user_id, bio);
            if (affectedRows == 0)
            {
                _logger.InfoDatabaseQueryReturnedZeroAffectedRows($"Update user bio => user_id: '{user_id}', user_bio: '{bio}'");
                return BadRequest();
            }
            return Ok();
        }
    }
}
