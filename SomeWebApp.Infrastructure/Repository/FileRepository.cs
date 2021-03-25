using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using SomeWebApp.Application.Interfaces;
using SomeWebApp.Core.Entities;
using System.Threading.Tasks;

namespace SomeWebApp.Infrastructure.Repository
{
    public class FileRepository : IFileRepository
    {
        private readonly IConfiguration Configuration;

        public FileRepository(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }

        private string connectionString
        {
            get => Configuration.GetConnectionString("MySQL");
        }

        public async Task<int> AddAsync(FileModel entity)
        {
            var sql = "INSERT INTO files (Name, MachineName, Path, Extension, Size, UploadTimestamp) " +
                "VALUES (@Name, @MachineName, @Path, @Extension, @Size, @UploadTimestamp);";

            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            int result = await connection.ExecuteAsync(sql,
                new
                {
                    entity.Name,
                    entity.MachineName,
                    entity.Path,
                    entity.Extension,
                    entity.Size,
                    UploadTimestamp = entity.UploadTimestamp.ToString("yyyy-MM-dd HH:mm:ss")
                });

            return result;
        }
    }
}
