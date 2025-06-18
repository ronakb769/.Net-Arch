using LearnArchitecture.Core.Entities;
using LearnArchitecture.Core.Models.RequestModels;
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

        public async Task<PagingResponseModel<LoginHistoryResponseModel>> GetLoginHistory(LoginHistoryPagingRequestModel request)
        {
            const string methodName = nameof(GetLoginHistory);
            try
            {
                _logger.LogInformation($"{methodName} called from role Repository");

                var query = from lh in _dbContext.LoginHistory
                            join u in _dbContext.Users on lh.userId equals u.userId
                            select new LoginHistoryResponseModel
                            {
                                loginHistoryId = lh.loginHistoryId,
                                userId = lh.userId,
                                userName = u.firstName + " " + u.lastName,
                                loginDate = lh.loginDate,
                                loginTime = lh.loginTime,
                                logoutTime = lh.logoutTime,
                                ipAddress = lh.ipAddress
                            };

                // Filtering
                if (request.SelectedUserNames != null && request.SelectedUserNames.Any())
                {
                    query = query.Where(x => request.SelectedUserNames.Contains(x.userName));
                }

                if (request.FromDate.HasValue)
                {
                    var fromDateOnly = DateOnly.FromDateTime(request.FromDate.Value);
                    query = query.Where(x => x.loginDate >= fromDateOnly);
                }

                if (request.ToDate.HasValue)
                {
                    var toDateOnly = DateOnly.FromDateTime(request.ToDate.Value);
                    query = query.Where(x => x.loginDate <= toDateOnly);
                }

                int totalRecords = await query.CountAsync();

                // Sorting
                query = request.SortColumn?.ToLower() switch
                {
                    "username" => request.SortDirection == "desc"
                        ? query.OrderByDescending(x => x.userName)
                        : query.OrderBy(x => x.userName),

                    "logindate" => request.SortDirection == "desc"
                        ? query.OrderByDescending(x => x.loginDate)
                        : query.OrderBy(x => x.loginDate),

                    "logintime" => request.SortDirection == "desc"
                        ? query.OrderByDescending(x => x.loginTime)
                        : query.OrderBy(x => x.loginTime),

                    "logouttime" => request.SortDirection == "desc"
                        ? query.OrderByDescending(x => x.logoutTime)
                        : query.OrderBy(x => x.logoutTime),

                    _ => query.OrderByDescending(x => x.loginDate).ThenByDescending(x => x.loginTime)
                };

                // Paging
                var data = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                return new PagingResponseModel<LoginHistoryResponseModel>
                {
                    Data = data,
                    TotalRecords = totalRecords
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName} from role Repository");
                throw;
            }
        }

        public async Task<List<string>> GetAllUserNames()
        {
            try
            {
                var userNames = await _dbContext.Users.Where(x => x.isActive && !x.isDelete).Select(x=>x.firstName + " "+x.lastName).ToListAsync();
                return userNames;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
