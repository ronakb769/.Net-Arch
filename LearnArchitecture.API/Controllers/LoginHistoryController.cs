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
        [HttpGet("GetLoginHistory")]
        [Authorize]
        public async Task<IActionResult> GetLoginHistory()
        {
            try
            {
                _logger.LogInformation("Get Login History called from LoginHistoryController");
                var data = await _loginHistoryService.GetLoginHistory();
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving login history.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
    }
}
