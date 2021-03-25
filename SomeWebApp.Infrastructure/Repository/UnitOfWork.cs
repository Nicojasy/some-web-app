using SomeWebApp.Application.Interfaces;

namespace SomeWebApp.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(IFileRepository _fileRepository, IUserRepository _userRepository,
            IRefreshSessionRepository _refreshSessionRepository, IRoleRepository _roleRepository)
        {
            Files = _fileRepository;
            Users = _userRepository;
            RefreshSessions = _refreshSessionRepository;
            Roles = _roleRepository;
        }

        public IFileRepository Files{ get; }
        public IUserRepository Users { get; }
        public IRefreshSessionRepository RefreshSessions { get; }
        public IRoleRepository Roles{ get; }
    }
}
