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

        Task<int> UpdateIsDeletedAsync(UInt64 id, bool isDeleted);
        Task<int> UpdateIsDeletedAsync(string nickname, bool isDeleted);
        Task<int> UpdatePasswordAsync(UInt64 user_id, string new_password);
        Task<int> UpdateBioAsync(UInt64 user_id, string new_bio);

        Task<int> AddAsync(UserModel entity);
        Task<UInt64> AddWithReturnIDAsync(UserModel entity);
    }
}