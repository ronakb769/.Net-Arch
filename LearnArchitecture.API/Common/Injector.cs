using LearnArchitecture.Data.IRepository;
using LearnArchitecture.Data.Repository;
using LearnArchitecture.Services.IServices;
using LearnArchitecture.Services.Services;

namespace LearnArchitecture.API.Common
{
    public class Injector
    {
        public static void APIInitializer(IServiceCollection services)
        {
            //BusinessLogic
            services.AddTransient<ILoginService, LoginService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IPermissionService, PermissionService>();
            services.AddTransient<IDashboardService, DashboardService>();
            services.AddTransient<ILoginHistoryService, LoginHistoryService>();

            //Repository
            services.AddTransient<ILoginRepository, LoginRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<IPermissionRepository, PermissionRepository>();
            services.AddTransient<IErrorLogRepository, ErrorLogRepository>();
            services.AddTransient<IDashboardRepository, DashboardRepository>();
            services.AddTransient<ILoginHistoryRepository, LoginHistoryRepository>();
        }
    }
}
