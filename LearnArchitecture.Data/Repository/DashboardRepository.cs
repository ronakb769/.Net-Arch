using LearnArchitecture.Core.Helper.Constants;
using LearnArchitecture.Core.Models.ResponseModel;
using LearnArchitecture.Data.Context;
using LearnArchitecture.Data.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Data.Repository
{
    public class DashboardRepository:IDashboardRepository
    {
        private readonly LearnArchitectureDbContext _dbContext;
        private readonly ILogger<RoleRepository> _logger;

        public DashboardRepository(LearnArchitectureDbContext dbContext, ILogger<RoleRepository> logger)
        {
            this._dbContext = dbContext;
            this._logger = logger;
        }

        public async Task<DashboardResponseModel> GetUserCount()
        {
            const string methodName = nameof(GetUserCount);
            try
            {
                _logger.LogInformation($"{methodName} called from dashboard Repository");
                var data = await (from u in _dbContext.Users
                                  join urm in _dbContext.UserRoleMapping on u.userId equals urm.userId
                                  join r in _dbContext.Role on urm.roleId equals r.roleId
                                  where u.isActive == true
                                  select new
                                  {
                                      User = u,
                                      Role = r,
                                      UserRoleMapping = urm
                                  }).ToListAsync();

                var dashboardData  = new DashboardResponseModel();
                dashboardData.ActiveUser = data.Count(x => x.User.isActive == true && x.User.isDelete == false);
                dashboardData.SuperAdminUser = data.Count(x => x.User.isActive == true && x.User.isDelete == false && x.Role.roleName == RoleConstants.SuperAdmin);
                dashboardData.AdminUser = data.Count(x => x.User.isActive == true && x.User.isDelete == false && x.Role.roleName == RoleConstants.Admin);
                dashboardData.NormalUser = data.Count(x => x.User.isActive == true && x.User.isDelete == false && x.Role.roleName == RoleConstants.User);
                dashboardData.InActiveUser = data.Count(x => x.User.isActive == true && x.User.isDelete == true );

                return dashboardData;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, $"Exception in {methodName} from dashboard Repository");
                throw;
            }
            
        }
    }
}
