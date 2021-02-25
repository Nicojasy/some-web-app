using System;
using System.Threading.Tasks;
using SomeWebApp.Core.Entities;

namespace SomeWebApp.Application.Interfaces
{
    public interface IRefreshSessionRepository : IGenericRepository<RefreshSessionModel>
    {
        Task<RefreshSessionModel> GetByNicknameAndRefreshTokenAsync(string nickname, string refresh_token);
        Task<int> DeleteAsync(UInt64 id);
        Task<int> DeleteByUserIDAndRefreshTokenAsync(UInt64 id_user, string refresh_token);
        Task<int> DeleteAllByUserIDAsync(UInt64 id_user);

        Task<int> AddAsync(RefreshSessionModel entity);
        Task<UInt64> AddWithReturnIDAsync(RefreshSessionModel entity);
    }
}
