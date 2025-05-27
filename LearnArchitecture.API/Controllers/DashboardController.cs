using LearnArchitecture.Core.Helper.Constants;
using LearnArchitecture.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearnArchitecture.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly ILogger<DashboardController> _logger;
        public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger)
        {
            this._dashboardService = dashboardService;
            this._logger = logger;
        }

        [HttpGet("GetUserCount")]
        [Authorize]
        public async Task<IActionResult> GetUserCount()
        {
            const string methodName = nameof(GetUserCount);
            try
            {

                _logger.LogInformation($"{methodName} called from Dashboard controller");
                var JWTAuthClaim = HttpContext.Items["AuthClaim"] as AuthClaim;
                var data = await _dashboardService.GetUserCount(JWTAuthClaim);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred in {methodName} called from Dashboard controller");
                throw;
            }
        }
    }
}
