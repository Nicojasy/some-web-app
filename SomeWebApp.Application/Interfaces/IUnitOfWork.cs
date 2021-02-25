namespace SomeWebApp.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        IRefreshSessionRepository RefreshSessions { get; }
        IRoleRepository Roles { get; }
    }
}