using SomeWebApp.Application.Interfaces;
using SomeWebApp.Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;
using SomeWebApp.Application.Auth;
using SomeWebApp.Infrastructure.Auth;
using SomeWebApp.Application.Checker;
using SomeWebApp.Infrastructure.Checker;
using Microsoft.Extensions.Configuration;

namespace SomeWebApp.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration Configuration)
        {
            //auth
            services.AddTransient<IAuthService, TokenService>();
            //registration
            services.AddTransient<IPasswordHasher, PasswordHasher>();
            //checker
            services.AddSingleton<IEnteredDataChecker, EnteredDataChecker>();
            
            //unitofwork
            services.AddTransient<IFileRepository, FileRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IRefreshSessionRepository, RefreshSessionRepository>();
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
        }
    }
}
