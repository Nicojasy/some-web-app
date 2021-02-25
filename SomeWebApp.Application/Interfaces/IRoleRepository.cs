using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SomeWebApp.Core.Entities;

namespace SomeWebApp.Application.Interfaces
{
    public interface IRoleRepository : IGenericRepository<RoleModel>
    {
        Task<RoleModel> GetRoleByNameAsync(string name);
        Task<PermissionModel> GetPermissionByNameAsync(string name);
        Task<IEnumerable<string>> GetRoleNamesByUserIDAsync(UInt64 user_id);

        Task<int> AddRolesAsync(RoleModel entity);
        Task<int> AddUserRoleAsync(UserRoleModel entity);
        Task<int> AddPermissionAsync(RoleModel entity);
        Task<int> AddRolePermissionAsync(RolePermissionModel entity);
        
        Task<int> DeleteRoleAndUserRoleLinksAsync(UInt64 id);
        Task<int> DeletePermissionAndRolePermissionLinksAsync(UInt64 id);
    }
}