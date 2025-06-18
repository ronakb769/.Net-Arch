using LearnArchitecture.Core.Entities;
using LearnArchitecture.Core.Helper.Constants;
using LearnArchitecture.Core.Models.RequestModels;
using LearnArchitecture.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearnArchitecture.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppSettingsController : ControllerBase
    {
        private readonly IAppSettingsService _appSettingsService;
        private readonly ILogger<AppSettingsController> _logger;

        public AppSettingsController(IAppSettingsService appSettingsService, ILogger<AppSettingsController> logger)
        {
            this._appSettingsService = appSettingsService;
            this._logger = logger;
        }

        [HttpGet("GetCaptchaStatus")]
       
        public async Task<IActionResult> GetCaptchaStatus()
        {
            const string methodName = nameof(GetCaptchaStatus);
            try
            {
                _logger.LogInformation($"{methodName} called app settings controller");
                var value = await _appSettingsService.GetCaptchaStatus();
                return Ok(value);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, $"Exception occurred in {methodName} called from app settings controller");
                throw;
            }
        }

        [HttpGet("GetAllAppSettings")]

        public async Task<IActionResult> GetAllAppSettings(AppSettingPagingRequestModel request)
        {
            const string methodName = nameof(GetAllAppSettings);
            try
            {
                _logger.LogInformation($"{methodName} called app settings controller");
                var value = await _appSettingsService.GetSettingValue(request);
                return Ok(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred in {methodName} called from app settings controller");
                throw;
            }
        }

        [HttpGet("GetAppSettingById")]

        public async Task<IActionResult> GetAppSettingById(int id)
        {
            const string methodName = nameof(GetAppSettingById);
            try
            {
                _logger.LogInformation($"{methodName} called app settings controller");
                var value = await _appSettingsService.GetAppSettingById(id);
                return Ok(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred in {methodName} called from app settings controller");
                throw;
            }
        }

        [HttpPost("UpdateAppSettings")]

        public async Task<IActionResult> UpdateAppSettings(AppSettings appSettings)
        {
            const string methodName = nameof(UpdateAppSettings);
            try
            {
                _logger.LogInformation($"{methodName} called app settings controller");
                var value = await _appSettingsService.UpdateAppSettings(appSettings);
                return Ok(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred in {methodName} called from app settings controller");
                throw;
            }
        }
    }
}
