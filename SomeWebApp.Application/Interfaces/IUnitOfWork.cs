namespace SomeWebApp.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IFileRepository Files { get; }
        IUserRepository Users { get; }
        IRefreshSessionRepository RefreshSessions { get; }
        IRoleRepository Roles { get; }
    }
}