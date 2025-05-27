using LearnArchitecture.Core.Helper.Attributes;
using LearnArchitecture.Core.Helper.Constants;
using LearnArchitecture.Services.IServices;
using LearnArchitecture.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearnArchitecture.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;
        private readonly ILogger<PermissionController> _logger;
        public PermissionController(IPermissionService permissionService, ILogger<PermissionController> logger)
        {
            this._permissionService = permissionService;
            this._logger = logger;
        }

        [HttpGet("GetAllPermission")]
        [Authorize]
        public async Task<IActionResult> GetAllPermission()
        {
            const string methodName = nameof(GetAllPermission);
            try
            {

                _logger.LogInformation($"{methodName} called from permission controller");
                var JWTAuthClaim = HttpContext.Items["AuthClaim"] as AuthClaim;
                var data = await _permissionService.GetAllPermission(JWTAuthClaim);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred in {methodName} called from permission controller");
                throw;
            }
        }
    }
}
