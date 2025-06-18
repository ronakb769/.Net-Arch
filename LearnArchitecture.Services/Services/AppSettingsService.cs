using LearnArchitecture.Core.Entities;
using LearnArchitecture.Core.Helper;
using LearnArchitecture.Core.Helper.Constants;
using LearnArchitecture.Data.IRepository;
using LearnArchitecture.Data.Repository;
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
    public class AppSettingsService:IAppSettingsService
    {
        private readonly IAppSettngsRepository _appSettingsRepository;
        private readonly ILogger<AppSettingsService> _logger;

        public AppSettingsService(IAppSettngsRepository appSettingsRepository, ILogger<AppSettingsService> logger)
        {
            this._appSettingsRepository = appSettingsRepository;
            this._logger = logger;
        }

        public async Task<ApiResponse<List<AppSettings>>> GetSettingValue()
        {
            const string methodName = nameof(GetSettingValue);
            try
            {
                _logger.LogInformation($"{methodName} called from app setting service");
                var appSettings = await _appSettingsRepository.GetSettingValue();
                if (appSettings == null)
                    return ResponseBuilder.Fail<List<AppSettings>>("No app settings found", HttpStatusCode.NotFound);

                return ResponseBuilder.Success(appSettings, "app settings retrieved successfully");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName} from app setting service");
                return ResponseBuilder.Fail<List<AppSettings>>("An error occurred while retrieving app settings");
            }
        }

        public async Task<ApiResponse<AppSettings>> GetAppSettingById(int id)
        {
            const string methodName = nameof(GetAppSettingById);
            try
            {
                _logger.LogInformation($"{methodName} called from app setting service");
                var appSettings = await _appSettingsRepository.GetAppSettingById(id);
                if (appSettings == null)
                    return ResponseBuilder.Fail<AppSettings>("No app settings found", HttpStatusCode.NotFound);

                return ResponseBuilder.Success(appSettings, "app settings retrieved successfully");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName} from app setting service");
                return ResponseBuilder.Fail<AppSettings>("An error occurred while retrieving app settings");
            }
        }

        public async Task<ApiResponse<bool>> UpdateAppSettings(AppSettings appSetting)
        {
            const string methodName = nameof(UpdateAppSettings);
            try
            {
                _logger.LogInformation($"{methodName} called from app setting service");
                var existingAppSettings = await _appSettingsRepository.GetAppSettingById(appSetting.Id);
                if (existingAppSettings == null)
                    return ResponseBuilder.Fail<bool>("No app settings found", HttpStatusCode.NotFound);
                else
                {
                    existingAppSettings.Id = appSetting.Id;
                    existingAppSettings.Key = appSetting.Key;
                    existingAppSettings.Value = appSetting.Value;
                    existingAppSettings.description = appSetting.description;

                    var isUpdated = await _appSettingsRepository.UpdateAppSettings(existingAppSettings);

                    return ResponseBuilder.Success(isUpdated, "app settings updated successfully");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName} from app setting service");
                return ResponseBuilder.Fail<bool>("An error occurred while retrieving app settings");
            }
        }
    }
}
