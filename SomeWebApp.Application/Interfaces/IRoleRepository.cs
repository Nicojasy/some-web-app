using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SomeWebApp.Core.Entities;
using SomeWebApp.Core.Repository;

namespace SomeWebApp.Application.Interfaces
{
    public interface IRoleRepository : IGenericRepository<RoleModel>
    {
        Task<RoleModel> GetRoleAsync(UInt64 role_id);
        Task<RoleModel> GetRoleByNameAsync(string name);
        Task<IEnumerable<string>> GetPermissionNamesByUserIDAsync(UInt64 user_id);
        Task<IEnumerable<PermissionModel>> GetPermissionsByUserIDAsync(UInt64 user_id);
        Task<IEnumerable<RoleModel>> GetRolesByUserIDAsync(UInt64 user_id);
        Task<IEnumerable<RoleWithPermissionsModel>> GetRolesWithPerissionsByUserIDAsync(UInt64 user_id);
        Task<PermissionModel> GetPermissionByIDAsync(UInt64 permission_id);
        Task<PermissionModel> GetPermissionByNameAsync(string name);
        Task<IEnumerable<string>> GetRoleNamesByUserIDAsync(UInt64 user_id);

        Task<int> UpdateNameOfRoleAsync(UInt64 role_id, string name);
        Task<int> UpdateDiscriptionOfRoleAsync(UInt64 role_id, string discription);
        Task<int> UpdateDiscriptionOfPermissionAsync(UInt64 permission_id, string discription);

        Task<int> AddRoleAsync(RoleModel entity);
        Task<int> AddUserRoleAsync(UserRoleModel entity);
        Task<int> AddPermissionAsync(RoleModel entity);
        Task<int> AddRolePermissionAsync(RolePermissionModel entity);
        
        Task<int> DeleteRoleWithLinksAsync(UInt64 id);
        Task<int> DeletePermissionWithLinksAsync(UInt64 id);
        Task<int> DeleteRolePermissionAsync(UInt64 role_id, UInt64 permission_id);
        Task<int> DeleteRolePermissionAsync(string role_name, string permission_name);
    }
}