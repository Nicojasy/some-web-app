using SomeWebApp.Application.Interfaces;

namespace SomeWebApp.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(IUserRepository _userRepository, IRefreshSessionRepository _refreshSessionRepository,
            IRoleRepository _roleRepository)
        {
            Users = _userRepository;
            RefreshSessions = _refreshSessionRepository;
            Roles = _roleRepository;
        }

        public IUserRepository Users { get; }
        public IRefreshSessionRepository RefreshSessions { get; }
        public IRoleRepository Roles{ get; }
    }
}
