using System;
using System.Threading.Tasks;
using SomeWebApp.Core.Entities;

namespace SomeWebApp.Application.Interfaces
{
    public interface IUserRepository : IGenericRepository<UserModel>
    {
        Task<UserModel> GetByIdAsync(UInt64 id);
        Task<UserModel> GetByNicknameAsync(string nickname);
        Task<UserModel> GetByNicknameAndPasswordAsync(string nickname, string password);
        Task<int> GetCountUsersByNicknameOrEmailAsync(string nickname, string password);
        Task<UserModel> GetByLoginAsync(string login);

        Task<int> AddAsync(UserModel entity);
        Task<UInt64> AddWithReturnIDAsync(UserModel entity);
    }
}