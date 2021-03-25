using SomeWebApp.Application.Interfaces;
using SomeWebApp.Core.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;

namespace SomeWebApp.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration Configuration;
        public UserRepository(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }

        private string connectionString
        {
            get => Configuration.GetConnectionString("MySQL");
        }

        //SELECT
        public async Task<UserModel> GetByNicknameAsync(string nickname)
        {
            var sql = "SELECT * FROM users WHERE Nickname = @nickname;";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<UserModel>(sql, new { nickname });

            return result;
        }

        public async Task<UserModel> GetByNicknameAndPasswordAsync(string nickname, string password)
        {
            var sql = "SELECT * FROM users WHERE Nickname = @nickname and Password = @password;";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<UserModel>(sql, new { nickname, password });

            return result;
        }

        public async Task<int> GetCountUsersByNicknameOrEmailAsync(string nickname, string email)
        {
            var sql = "SELECT COUNT(*) FROM users WHERE Nickname = @nickname or Email = @email;";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var result = await connection.QueryFirstOrDefaultAsync<int>(sql, new { nickname, email });

            return result;
        }

        public async Task<UserModel> GetByLoginAsync(string login)
        {
            var sql = "SELECT * FROM users WHERE Nickname = @nickname or Email = @email;";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var result = await connection.QueryFirstOrDefaultAsync<UserModel>(sql, new { nickname = login, email = login });

            return result;
        }

        public async Task<UserModel> GetByIdAsync(UInt64 id)
        {
            var sql = "SELECT * FROM users WHERE ID = @id";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<UserModel>(sql, new { id });

            return result;
        }

        //INSERT
        public async Task<int> AddAsync(UserModel entity)
        {
            var sql = "INSERT INTO users (Email,Password,Nickname,BIO,Photo,IsDeleted,RegistrationTimestamp) " +
                "VALUES (@Email,@Password,@Nickname,@BIO,@Photo,@IsDeleted,@RegistrationTimestamp);";
            
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            int result = await connection.ExecuteAsync(sql,
                new
                {
                    entity.Email,
                    entity.Password,
                    entity.Nickname,
                    entity.Bio,
                    entity.ID_Photo,
                    entity.IsDeleted,
                    RegistrationTimestamp = entity.RegistrationTimestamp.ToString("yyyy-MM-dd HH:mm:ss")
                });
            
            return result;
        }

        public async Task<UInt64> AddWithReturnIDAsync(UserModel entity)
        {
            var sql = "INSERT INTO users (Email,Password,Nickname,BIO,Photo,IsDeleted,RegistrationTimestamp) " +
                "VALUES (@Email,@Password,@Nickname,@BIO,@Photo,@IsDeleted,@RegistrationTimestamp); " +
                "SELECT LAST_INSERT_ID();";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var result = await connection.QueryFirstOrDefaultAsync<UInt64>(sql,
                new
                {
                    entity.Email,
                    entity.Password,
                    entity.Nickname,
                    entity.Bio,
                    entity.ID_Photo,
                    entity.IsDeleted,
                    RegistrationTimestamp = entity.RegistrationTimestamp.ToString("yyyy-MM-dd HH:mm:ss")
                });

            return result;
        }

        public async Task<int> UpdateIsDeletedAsync(UInt64 id, bool isDeleted)
        {
            var sql = "UPDATE users SET IsDeleted = @isDeleted " +
                "WHERE ID = @id and IsDeleted != @isDeleted;";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var result = await connection.ExecuteAsync(sql, new { id, isDeleted });

            return result;
        }

        public async Task<int> UpdateIsDeletedAsync(string nickname, bool isDeleted)
        {
            var sql = "UPDATE users SET IsDeleted = @isDeleted " +
                "WHERE Nickname = @nickname and IsDeleted != @isDeleted;";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var result = await connection.ExecuteAsync(sql, new { nickname, isDeleted });

            return result;
        }

        public async Task<int> UpdatePasswordAsync(UInt64 user_id, string new_password)
        {
            var sql = "UPDATE users SET Password = @new_password WHERE ID = @user_id;";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var result = await connection.ExecuteAsync(sql, new { user_id, new_password});

            return result;
        }

        public async Task<int> UpdateBioAsync(UInt64 user_id, string new_bio)
        {
            var sql = "UPDATE users SET Bio = @new_bio WHERE ID = @user_id;";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var result = await connection.ExecuteAsync(sql, new { user_id, new_bio });

            return result;
        }
    }
}