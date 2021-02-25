using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SomeWebApp.Application.Interfaces;
using SomeWebApp.Models.Role;
using System;
using System.Threading.Tasks;

namespace SomeWebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public RoleController(ILogger<AuthController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [HttpGet, Route("getroleswithpermissionsofuser")]
        [Authorize]
        public async Task<IActionResult> GetRolesWithPermissionsOfUser([FromHeader] UInt64 user_id)
        {
            return Ok();
        }

        [HttpPost, Route("addrole")]
        [Authorize]
        public async Task<IActionResult> AddRole([FromBody] RoleDto role)
        {
            return Ok();
        }

        [HttpPost, Route("addpermissionforrole")]
        [Authorize]
        public async Task<IActionResult> AddPermissionForRole([FromBody] UInt64 role, [FromBody] PermissionDto permission)
        {
            return Ok();
        }

        [HttpPost, Route("deleterole")]
        [Authorize]
        public async Task<IActionResult> DeleteRole([FromBody] string role_name)
        {
            if (string.IsNullOrEmpty(role_name))
            {
                return BadRequest("Invalid client request");
            }

            var role = await _unitOfWork.Roles.GetRoleByNameAsync(role_name);
            if (role == null)
            {
                return BadRequest("Roll not exists");
            }

            var affectedRows = await _unitOfWork.Roles.DeleteRoleAndUserRoleLinksAsync(role.ID);
            if (affectedRows == 0)
            {
                // TODO: log
            }
            return Ok();
        }


        [HttpPost, Route("deletepermissionforrole")]
        [Authorize]
        public async Task<IActionResult> DeletePermissionForRole([FromBody] string role_name, [FromBody] )
        {
            return Ok();
        }

        /*
        [HttpPost, Route("deletepermission")]
        [Authorize]
        public async Task<IActionResult> DeletePermission([FromBody] string permission_name)
        {
            if (string.IsNullOrEmpty(permission_name))
            {
                return BadRequest("Invalid client request");
            }

            var permission = await _unitOfWork.Roles.GetPermissionByNameAsync(permission_name);
            if (permission == null)
            {
                return BadRequest("Roll not exists");
            }

            var affectedRows = await _unitOfWork.Roles.DeletePermissionAndRolePermissionLinksAsync(permission.ID);
            if (affectedRows == 0)
            {
                // TODO: log
            }
            return Ok();
        }
        */
    }
}
