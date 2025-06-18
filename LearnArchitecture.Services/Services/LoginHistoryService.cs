using LearnArchitecture.Core.Entities;
using LearnArchitecture.Core.Helper;
using LearnArchitecture.Core.Models.RequestModels;
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

        public async Task<ApiResponse<PagingResponseModel<LoginHistoryResponseModel>>> GetLoginHistory(LoginHistoryPagingRequestModel request)
        {
            const string methodName = nameof(GetLoginHistory);
            try
            {
                _logger.LogInformation($"{methodName} called from login history service");
                var loginHistory = await _loginHistoryRepository.GetLoginHistory(request);
                if (loginHistory == null || !loginHistory.Data.Any())
                {
                    return ResponseBuilder.Fail<PagingResponseModel<LoginHistoryResponseModel>>("No record found", HttpStatusCode.NotFound);
                }

                return ResponseBuilder.Success(loginHistory, "login history retrieved successfully");

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName} from login history service");
                return ResponseBuilder.Fail<PagingResponseModel<LoginHistoryResponseModel>>("An error occurred while retrieving login history");
            }
        }
        public async Task<ApiResponse<List<string>>> GetAllUserNames()
        {
            const string methodName = nameof(GetAllUserNames);
            try
            {
                _logger.LogInformation($"{methodName} called from login history service");
                var userNames = await _loginHistoryRepository.GetAllUserNames();
                if (userNames == null || !userNames.Any())
                {
                    return ResponseBuilder.Fail<List<string>>("No users found", HttpStatusCode.NotFound);
                }

                return ResponseBuilder.Success(userNames, "Users name retrieved successfully");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName} from login history service");
                return ResponseBuilder.Fail<List<string>>("An error occurred while retrieving users Name");
            }
        }
    }
}
