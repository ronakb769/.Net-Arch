using LearnArchitecture.Core.Entities;
using LearnArchitecture.Core.Helper;
using LearnArchitecture.Core.Helper.Constants;
using LearnArchitecture.Core.Models.ResponseModel;
using LearnArchitecture.Data.IRepository;
using LearnArchitecture.Data.Repository;
using LearnArchitecture.Services.IServices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Services.Services
{
    public class DashboardService:IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;
        private readonly ILogger<DashboardService> _logger;
        public DashboardService(IDashboardRepository dashboardRepository, ILogger<DashboardService> logger)
        {
            this._dashboardRepository = dashboardRepository;
            this._logger = logger;
        }

        public async Task<ApiResponse<DashboardResponseModel>> GetUserCount(AuthClaim authClaim)
        {
            const string methodName = nameof(GetUserCount);
            try
            {
                _logger.LogInformation($"{methodName} from dashboard service");

                var dashboardData = await _dashboardRepository.GetUserCount();

                if (dashboardData == null)
                    return ResponseBuilder.Fail<DashboardResponseModel>("No dashboard data found",HttpStatusCode.NotFound);

                return ResponseBuilder.Success(dashboardData, "dashboard data retrieved successfully");

            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, $"Exception in {methodName} from dashboard service");
                return ResponseBuilder.Fail<DashboardResponseModel>("An error occurred while retrieving dashboard data", HttpStatusCode.BadRequest);
            }
        }
    }
}
