using Azure.Core;
using LearnArchitecture.Core.Models.RequestModels;
using LearnArchitecture.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LearnArchitecture.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginHistoryController : ControllerBase
    {
        private readonly ILogger<LoginHistoryController> _logger;
        private readonly ILoginHistoryService _loginHistoryService; 
        public LoginHistoryController(ILogger<LoginHistoryController> logger, ILoginHistoryService _loginHistoryService)
        {
            this._logger = logger;
            this._loginHistoryService = _loginHistoryService;
        }
        [HttpPost("GetLoginHistory")]
        [Authorize]
        public async Task<IActionResult> GetLoginHistory(LoginHistoryPagingRequestModel request)
        {
            try
            {
                _logger.LogInformation("Get Login History called from LoginHistoryController");
                var data = await _loginHistoryService.GetLoginHistory(request);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving login history.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpGet("GetAllUserNames")]
        [Authorize]
        public async Task<IActionResult> GetAllUserNames()
        {
            try
            {
                _logger.LogInformation("Get all user Names called from LoginHistoryController");
                var data = await _loginHistoryService.GetAllUserNames();
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving Users Name");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
    }
}
