using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SomeWebApp.Application.Interfaces;
using SomeWebApp.Core.Entities;
using SomeWebApp.LogExtension;
using SomeWebApp.Models.Role;
using System;
using System.Threading.Tasks;

namespace SomeWebApp.Controllers
{
    [ApiController]
    [Route("api/roles")]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public RolesController(ILogger<AuthController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }


        [HttpGet, Route("{role_id}")]
        public async Task<IActionResult> GetRole(UInt64 role_id)
        {
            var role = await _unitOfWork.Roles.GetRoleAsync(role_id);
            if (role == null)
            {
                _logger.InfoDatabaseQueryReturnedNull($"Get role by id => role_id: '{role_id}'");
                return BadRequest();
            }
            return Ok(role);
        }

        [HttpPost, Route("createrole")]
        public async Task<IActionResult> CreateRole([FromBody] RoleDto role)
        {
            var newRole = new RoleModel(role.Name, role.Discription);

            var affectedRows = await _unitOfWork.Roles.AddRoleAsync(newRole);
            
            if (affectedRows == 0)
            {
                _logger.WarnDatabaseQueryReturnedZeroAffectedRows($"Add role => role_name: '{role.Name}', role_discription: '{role.Discription}'");
                return BadRequest();
            }
            return Ok();
        }

        [HttpPost, Route("{role_id}/permissions/{permission_id}/addpermissionlink")]
        public async Task<IActionResult> AddPermissionLinkForRole(UInt64 role_id, UInt64 permission_id)
        {
            var newLink = new RolePermissionModel(role_id, permission_id);

            var affectedRows = await _unitOfWork.Roles.AddRolePermissionAsync(newLink);

            if (affectedRows == 0)
            {
                _logger.WarnDatabaseQueryReturnedZeroAffectedRows($"Add role_permission => role_id: '{role_id}', permission_id: '{permission_id}'");
                return BadRequest();
            }
            return Ok();
        }

        [HttpDelete, Route("{role_id}")]
        public async Task<IActionResult> DeleteRole(UInt64 role_id)
        {
            var role = await _unitOfWork.Roles.GetRoleAsync(role_id);
            if (role == null)
            {
                _logger.InfoDatabaseQueryReturnedNull($"Get role by id => role_id: '{role_id}'");
                return BadRequest("Roll not exists");
            }
            
            var affectedRows = await _unitOfWork.Roles.DeleteRoleWithLinksAsync(role.ID);
            if (affectedRows == 0)
            {
                _logger.InfoDatabaseQueryReturnedZeroAffectedRows($"Delete role with links => role_id: '{role_id}'");
                return BadRequest();
            }
            return Ok();
        }

        [HttpDelete, Route("{role_id}/permissions/{permission_id}")]
        public async Task<IActionResult> DeleteRolePermissionLink(UInt64 role_id, UInt64 permission_id)
        {
            var affectedRows = await _unitOfWork.Roles.DeleteRolePermissionAsync(role_id, permission_id);
            if (affectedRows == 0)
            {
                _logger.InfoDatabaseQueryReturnedZeroAffectedRows($"Delete role_permission => role_id: '{role_id}', permission_id: '{permission_id}'");
                return BadRequest();
            }
            return Ok();
        }

        [HttpPost, Route("{role_id}/newname")]
        public async Task<IActionResult> UpdateNameOfRole(UInt64 role_id, [FromBody] string nickname)
        {
            var affectedRows = await _unitOfWork.Roles.UpdateNameOfRoleAsync(role_id, nickname);
            if (affectedRows == 0)
            {
                _logger.InfoDatabaseQueryReturnedZeroAffectedRows($"Update name of role => role_id: '{role_id}', nickname: '{nickname}'");
                return BadRequest();
            }
            return Ok();
        }

        [HttpPost, Route("{role_id}/new-discription")]
        public async Task<IActionResult> UpdateDiscriptionOfRole(UInt64 role_id, [FromBody] string discription)
        {
            var affectedRows = await _unitOfWork.Roles.UpdateDiscriptionOfRoleAsync(role_id, discription);
            if (affectedRows == 0)
            {
                _logger.InfoDatabaseQueryReturnedZeroAffectedRows($"Update discription of role => role_id: '{role_id}', name: '{discription}'");
                return BadRequest();
            }
            return Ok();
        }

        // TODO: update as PATCH
        // json: [{"op": "replace", "path":"discription", value: "new_d"},{..}]
        [HttpPost, Route("permissions/{permission_id}/new-discription")]
        public async Task<IActionResult> UpdateDiscriptionOfPermission(UInt64 permission_id, [FromBody] string discription)
        {
            var affectedRows = await _unitOfWork.Roles.UpdateDiscriptionOfPermissionAsync(permission_id, discription);
            if (affectedRows == 0)
            {
                _logger.InfoDatabaseQueryReturnedZeroAffectedRows($"Update dicription of permission => permission_id: '{permission_id}', discription: '{discription}'");
                return BadRequest();
            }
            return Ok();
        }

        [HttpPost, Route("permissions/add-permission")]
        public async Task<IActionResult> AddPermission([FromBody] PermissionDto permission)
        {
            var newPermission = new RoleModel(permission.Name, permission.Discription);

            var affectedRows = await _unitOfWork.Roles.AddPermissionAsync(newPermission);

            if (affectedRows == 0)
            {
                _logger.WarnDatabaseQueryReturnedZeroAffectedRows($"Add permission => permission_name: '{permission.Name}', permission_discription: '{permission.Discription}'");
                return BadRequest();
            }
            return Ok();
        }

        [HttpDelete, Route("permissions/{permission_id}")]
        [Authorize]
        public async Task<IActionResult> DeletePermission(UInt64 permission_id)
        {
            var permission = await _unitOfWork.Roles.GetPermissionByIDAsync(permission_id);
            if (permission == null)
            {
                _logger.InfoDatabaseQueryReturnedNull($"Get permission by id => permission_id: '{permission_id}'");
                return BadRequest("Permission not exists");
            }

            var affectedRows = await _unitOfWork.Roles.DeletePermissionWithLinksAsync(permission.ID);
            if (affectedRows == 0)
            {
                _logger.InfoDatabaseQueryReturnedZeroAffectedRows($"Delete permission => permission_id: '{permission_id}'");
                return BadRequest();
            }
            return Ok();
        }
    }
}
