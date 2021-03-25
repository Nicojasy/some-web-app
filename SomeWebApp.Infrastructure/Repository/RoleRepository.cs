using SomeWebApp.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;
using SomeWebApp.Core.Entities;
using Dapper;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using SomeWebApp.Core.Repository;

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

        public async Task<RoleModel> GetRoleAsync(UInt64 role_id)
        {
            var sql = "SELECT * FROM roles " +
                "WHERE ID = @role_id;";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var result = await connection.QueryFirstOrDefaultAsync(sql, new { role_id });

            return result;
        }

        public async Task<RoleModel> GetRoleByNameAsync(string name)
        {
            var sql = "SELECT * FROM roles " +
                "WHERE Name = @name " +
                "LIMIT 1;";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var result = await connection.QueryFirstOrDefaultAsync(sql, new { name });

            return result;
        }

        public async Task<IEnumerable<string>> GetPermissionNamesByUserIDAsync(UInt64 user_id)
        {
            var sql = "SELECT p.Name FROM permissions p " +
                "JOIN role_permissions rp ON p.ID = rp.ID_Permission " +
                "JOIN user_roles ur ON rp.ID_Role = ur.ID_Role " +
                "WHERE ur.ID_User = @user_id;";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var result = await connection.QueryAsync<string>(sql, new { user_id });

            return result;
        }

        public async Task<IEnumerable<PermissionModel>> GetPermissionsByUserIDAsync(UInt64 user_id)
        {
            var sql = "SELECT p.* FROM permissions p JOIN role_permissions rp ON p.ID = rp.ID_Permission " +
                "WHERE rp.ID_User = @user_id;";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var result = await connection.QueryAsync<PermissionModel>(sql, new { user_id });

            return result;
        }

        public async Task<IEnumerable<RoleModel>> GetRolesByUserIDAsync(UInt64 user_id)
        {
            var sql = "SELECT roles.* FROM roles r JOIN user_roles ur ON r.ID = ur.ID_Roles " +
                "WHERE ur.ID_User = @user_id;";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var result = await connection.QueryAsync<RoleModel>(sql, new { user_id });

            return result;
        }

        // FIXME: fix GetRolesWithPermissionsByUserIDAsync()
        public async Task<IEnumerable<RoleWithPermissionsModel>> GetRolesWithPerissionsByUserIDAsync(UInt64 user_id)
        {
            /*
            var query = @"
select a.*, g.* from Article a left join Groups g on g.Id = a.IdGroup    
select * from Barcode";
            //NOTE: IdGroup should exists in your Article class.
            IEnumerable<Article> articles = null;
            using (var multi = connection.QUeryMultiple(query))
            {
                articles = multi.Read<Article, Group, Article>((a, g) =>
                { a.Group = g; return a; });
                if (articles != null)
                {
                    var barcodes = multi.Read<Barcode>().ToList();
                    foreach (var article in articles)
                    {
                        article.Barcode = barcodes.Where(x => x.IdArticle = article.Id).ToList();
                    }
                }
            }
            */

            var sql = "SELECT r.* FROM roles r JOIN user_roles ur ON r.ID = ur.ID_Roles " +
                "WHERE ur.ID_User = @user_id; " +
                "SELECT * FROM ";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var result = await connection.QueryAsync<RoleWithPermissionsModel>(sql, new { user_id });

            return result;
        }

        public async Task<PermissionModel> GetPermissionByIDAsync(UInt64 permission_id)
        {

            var sql = "SELECT * FROM permissions " +
                "WHERE ID = @permission_id;";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var result = await connection.QueryFirstOrDefaultAsync(sql, new { permission_id });

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

        //UPDATE
        public async Task<int> UpdateNameOfRoleAsync(UInt64 role_id, string name)
        {
            var sql = "UPDATE roles SET Name = @name WHERE ID = @role_id;";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var result = await connection.ExecuteAsync(sql, new { role_id, name });

            return result;
        }

        public async Task<int> UpdateDiscriptionOfRoleAsync(UInt64 role_id, string discription)
        {
            var sql = "UPDATE roles SET Discription = @discription WHERE ID = @role_id;";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var result = await connection.ExecuteAsync(sql, new { role_id, discription });

            return result;
        }

        public async Task<int> UpdateDiscriptionOfPermissionAsync(UInt64 permission_id, string discription)
        {
            var sql = "UPDATE permissions SET Discription = @discription WHERE ID = @permission_id;";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var result = await connection.ExecuteAsync(sql, new { permission_id, discription });

            return result;
        }

        //INSERT
        public async Task<int> AddRoleAsync(RoleModel entity)
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
        public async Task<int> DeleteRoleWithLinksAsync(UInt64 id)
        {
            var sql = "DELETE FROM user_roles WHERE ID_Role = @id; " +
                "DELETE FROM permissions WHERE ID_Role = @id; " +
                "DELETE FROM roles WHERE ID = @id;";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            int result = await connection.ExecuteAsync(sql, new { id });

            return result;
        }

        public async Task<int> DeletePermissionWithLinksAsync(UInt64 id)
        {
            var sql = "DELETE FROM role_permissions WHERE ID_Role = @id; " +
                "DELETE FROM permissions WHERE ID = @id;";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            int result = await connection.ExecuteAsync(sql, new { id });

            return result;
        }
        public async Task<int> DeleteRolePermissionAsync(UInt64 role_id, UInt64 permission_id)
        {

            var sql = "DELETE FROM role_permissions " +
                "WHERE ID_Role = @role_id AND ID_Permission = @permission_id;";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            int result = await connection.ExecuteAsync(sql, new { role_id, permission_id });

            return result;
        }

        public async Task<int> DeleteRolePermissionAsync(string role_name, string permission_name)
        {

            var sql = "DELETE FROM role_permissions " +
                "WHERE ID_Role = ANY(SELECT ID FROM roles WHERE Name = @role_name) " +
                "AND ID_Permission = ANY(SELECT ID FROM permissions WHERE Name = @permission_name);";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            int result = await connection.ExecuteAsync(sql, new { role_name, permission_name });

            return result;
        }
    }
}