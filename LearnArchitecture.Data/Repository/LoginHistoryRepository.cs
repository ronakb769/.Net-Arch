using LearnArchitecture.Core.Entities;
using LearnArchitecture.Core.Models.ResponseModel;
using LearnArchitecture.Data.Context;
using LearnArchitecture.Data.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Data.Repository
{
    public class LoginHistoryRepository:ILoginHistoryRepository
    {
        private readonly LearnArchitectureDbContext _dbContext;
        private readonly ILogger<LoginHistoryRepository> _logger;

        public LoginHistoryRepository(LearnArchitectureDbContext dbContext, ILogger<LoginHistoryRepository> _logger)
        {
            this._dbContext = dbContext;
            this._logger = _logger;
        }

        public async Task<List<LoginHistoryResponseModel>> GetLoginHistory()
        {
            const string methodName = nameof(GetLoginHistory);
            try
            {
                _logger.LogInformation($"{methodName} called from role Repository");

                var lstLoginHistory =      await(from lh in  _dbContext.LoginHistory
                                         join u in _dbContext.Users 
                                         on lh.userId equals u.userId
                                         select new LoginHistoryResponseModel 
                                         { 
                                            loginHistoryId = lh.loginHistoryId,
                                             userId = lh.userId,
                                             userName = u.firstName + " " + u.lastName,
                                             loginDate = lh.loginDate,
                                             loginTime = lh.loginTime,
                                             logoutTime = lh.logoutTime,
                                             ipAddress = lh.ipAddress
                                         }).OrderByDescending(lh => lh.loginDate)
                                           .ThenByDescending(lh => lh.loginTime)
                                           .ToListAsync();

                return lstLoginHistory;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName} from role Repository");
                throw;
            }
        }
    }
}
