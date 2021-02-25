using SomeWebApp.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;
using SomeWebApp.Core.Entities;
using Dapper;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace SomeWebApp.Infrastructure.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IConfiguration Configuration;

        public RoleRepository(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }

        private string connectionString
        {
            get => Configuration.GetConnectionString("MySQL");
        }

        //SELECT
        public async Task<RoleModel> GetRoleByNameAsync(string name)
        {
            var sql = "SELECT * FROM roles " +
                "WHERE Name = @name;";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var result = await connection.QueryFirstOrDefaultAsync(sql, new { name });

            return result;
        }

        public async Task<PermissionModel> GetPermissionByNameAsync(string name)
        {
            var sql = "SELECT * FROM permissions " +
                "WHERE Name = @name;";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var result = await connection.QueryFirstOrDefaultAsync(sql, new { name });

            return result;
        }

        public async Task<IEnumerable<string>> GetRoleNamesByUserIDAsync(UInt64 user_id)
        {
            var sql = "SELECT r.Name FROM user_roles ur " +
                "JOIN roles r ON ur.ID_Role = r.ID " +
                "WHERE ur.ID = @user_id;";
            
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var result = await connection.QueryAsync<string>(sql, new { user_id });

            return result;
        }

        //INSERT
        public async Task<int> AddRolesAsync(RoleModel entity)
        {
            var sql = "INSERT INTO roles (Name, Discription) " +
                "VALUES (@Name, @Discription);";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            int result = await connection.ExecuteAsync(sql,
                new
                {
                    entity.Name,
                    entity.Discription
                });

            return result;
        }
        
        public async Task<int> AddUserRoleAsync(UserRoleModel entity)
        {
            var sql = "INSERT INTO user_roles (ID_User, ID_Role) " +
                "VALUES (@ID_User, @ID_Role);";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            int result = await connection.ExecuteAsync(sql,
                new
                {
                    entity.ID_User,
                    entity.ID_Role
                });

            return result;
        }

        public async Task<int> AddPermissionAsync(RoleModel entity)
        {
            var sql = "INSERT INTO roles (Name, Discription) " +
                "VALUES (@Name, @Discription);";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            int result = await connection.ExecuteAsync(sql,
                new
                {
                    entity.Name,
                    entity.Discription
                });

            return result;
        }

        public async Task<int> AddRolePermissionAsync(RolePermissionModel entity)
        {
            var sql = "INSERT INTO role_permissions (ID_Role, ID_Permission) " +
                "VALUES (@ID_Role, @ID_Permission);";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            int result = await connection.ExecuteAsync(sql,
                new
                {
                    entity.ID_Role,
                    entity.ID_Permission
                });

            return result;
        }
        /*
        public async Task<UInt64> AddWithReturnIDAsync(RefreshSessionModel entity)
        {
            var sql = "Insert into refresh_sessions (ID_User,RefreshToken,CreationTimestamp) " +
                "VALUES (@ID_User,@RefreshToken,@CreationTimestamp); " +
                "SELECT LAST_INSERT_ID();";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            UInt64 result = await connection.QueryFirstOrDefaultAsync(sql,
                new
                {
                    entity.ID_User,
                    entity.RefreshToken,
                    CreationTimestamp = entity.CreationTimestamp.ToString("yyyy-MM-dd HH:mm:ss")
                });

            return result;
        }
        */

        //DELETE
        public async Task<int> DeleteRoleAndUserRoleLinksAsync(UInt64 id)
        {
            var sql = "DELETE FROM roles WHERE ID = @id; " +
                "DELETE FROM user_roles WHERE ID_Role = @id";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            int result = await connection.ExecuteAsync(sql, new { id });

            return result;
        }

        public async Task<int> DeletePermissionAndRolePermissionLinksAsync(UInt64 id)
        {
            var sql = "DELETE FROM permissions WHERE ID = @id; " +
                "DELETE FROM role_permissions WHERE ID_Role = @id";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            int result = await connection.ExecuteAsync(sql, new { id });

            return result;
        }
    }
}