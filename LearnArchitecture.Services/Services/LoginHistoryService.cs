using LearnArchitecture.Core.Entities;
using LearnArchitecture.Core.Helper;
using LearnArchitecture.Core.Models.ResponseModel;
using LearnArchitecture.Data.IRepository;
using LearnArchitecture.Services.IServices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Services.Services
{
    public class LoginHistoryService: ILoginHistoryService
    {
        private readonly ILoginHistoryRepository _loginHistoryRepository;
        private readonly ILogger<LoginHistoryService> _logger;

        public LoginHistoryService(ILoginHistoryRepository _loginHistoryRepository, ILogger<LoginHistoryService> _logger)
        {
            this._loginHistoryRepository = _loginHistoryRepository;
            this._logger = _logger;
        }

        public async Task<ApiResponse<List<LoginHistoryResponseModel>>> GetLoginHistory()
        {
            const string methodName = nameof(GetLoginHistory);
            try
            {
                _logger.LogInformation($"{methodName} called from login history service");
                var loginHistory = await _loginHistoryRepository.GetLoginHistory();
                if (loginHistory == null || !loginHistory.Any())
                {
                    return ResponseBuilder.Fail<List<LoginHistoryResponseModel>>("No users found", HttpStatusCode.NotFound);
                }

                return ResponseBuilder.Success(loginHistory, "Users retrieved successfully");

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName} from login history service");
                return ResponseBuilder.Fail<List<LoginHistoryResponseModel>>("An error occurred while retrieving login history");
            }
        }
    }
}
