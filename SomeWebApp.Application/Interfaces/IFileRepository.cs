using SomeWebApp.Core.Entities;
using System.Threading.Tasks;

namespace SomeWebApp.Application.Interfaces
{
    public interface  IFileRepository : IGenericRepository<FileModel>
    {
        Task<int> AddAsync(FileModel entity);
    }
}
