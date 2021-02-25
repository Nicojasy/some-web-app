using SomeWebApp.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;
using SomeWebApp.Core.Entities;
using Dapper;
using MySql.Data.MySqlClient;

namespace SomeWebApp.Infrastructure.Repository
{
    public class RefreshSessionRepository : IRefreshSessionRepository
    {
        private readonly IConfiguration Configuration;

        public RefreshSessionRepository(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }

        private string connectionString
        {
            get => Configuration.GetConnectionString("MySQL");
        }

        //SELECT
        public async Task<RefreshSessionModel> GetByNicknameAndRefreshTokenAsync(string nickname, string refresh_token)
        {
            var sql = "SELECT * FROM refresh_sessions r JOIN users u ON r.ID_User = u.ID " +
                "WHERE u.Nickname = @nickname and r.RefreshToken = @refresh_token;";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var result = await connection.QueryFirstOrDefaultAsync<RefreshSessionModel>(sql, new { nickname, refresh_token});

            return result;
         }

        //INSERT
        public async Task<int> AddAsync(RefreshSessionModel entity)
        {
            var sql = "Insert into refresh_sessions (ID_User,RefreshToken,CreationTimestamp) " +
                "VALUES (@ID_User,@RefreshToken,@CreationTimestamp);";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            int result = await connection.ExecuteAsync(sql,
                new
                {
                    entity.ID_User,
                    entity.RefreshToken,
                    CreationTimestamp = entity.CreationTimestamp.ToString("yyyy-MM-dd HH:mm:ss")
                }) ;

            return result;
        }

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

        //DELETE
        public async Task<int> DeleteAsync(UInt64 id)
        {
            var sql = "DELETE FROM refresh_sessions WHERE ID = @id;";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            int result = await connection.ExecuteAsync(sql, new { id });

            return result;
        }

        public async Task<int> DeleteByUserIDAndRefreshTokenAsync(UInt64 id_user, string refresh_token)
        {
            var sql = "DELETE FROM refresh_sessions WHERE ID_User = @id_user and RefreshToken = @refresh_token";
            
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            int result = await connection.ExecuteAsync(sql, new { id_user, refresh_token });

            return result;
        }

        public async Task<int> DeleteAllByUserIDAsync(UInt64 id_user)
        {
            var sql = "DELETE FROM refresh_sessions WHERE ID_User = @id_user";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            int result = await connection.ExecuteAsync(sql, new { id_user});

            return result;
        }
    }
}