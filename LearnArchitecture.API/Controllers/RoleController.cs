using LearnArchitecture.Core.Entities;
using LearnArchitecture.Core.Helper.Attributes;
using LearnArchitecture.Core.Helper.Constants;
using LearnArchitecture.Core.Models.RequestModels;
using LearnArchitecture.Services.IServices;
using LearnArchitecture.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LearnArchitecture.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly ILogger<RoleController> _logger;
        public RoleController(IRoleService roleService, ILogger<RoleController> logger)
        {
            this._roleService = roleService;
            this._logger = logger;
        }

        [HttpGet("GetAllRoles")]
        [Authorize]
        [HasPermission(PermissionConstants.RoleView)]
        public async Task<IActionResult> GetAllRoles()
        {
            const string methodName  = nameof(GetAllRoles);
            try
            {

                _logger.LogInformation($"{methodName} called from role controller");
                var JWTAuthClaim = HttpContext.Items["AuthClaim"] as AuthClaim;
                var data = await _roleService.GetAllRole(JWTAuthClaim);
                return Ok(data);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred in {methodName} called from role controller");
                throw;
            }
        }

        
        [HttpGet("GetRoleById")]
        [Authorize]
        [HasPermission(PermissionConstants.RoleView)]
        public async Task<IActionResult> GetRoleById(int roleId)
        {
            const string methodName = nameof(GetRoleById);
            try
            {
                _logger.LogInformation($"{methodName} called with roleId:{roleId} from role controller");
                var data = await _roleService.GetRoleById(roleId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred in {methodName} called with roleId:{roleId} from role controller");
                throw;
            }
        }


        [HttpPost("CreateRole")]
        [Authorize]
        [HasPermission(PermissionConstants.RoleCreate)]
        public async Task<IActionResult> CreateRole(CreateRoleRequestModel roleModel)
        {
            const string methodName = nameof(CreateRole);
            try
            {
                _logger.LogInformation($"{methodName} called from role controller");
                var JWTAuthClaim = HttpContext.Items["AuthClaim"] as AuthClaim;
                var data = await _roleService.CreateRole(roleModel,JWTAuthClaim);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred in {methodName} called from role controller");
                throw;
            }
        }
        [HttpPost("UpdateRole")]
        [Authorize]
        [HasPermission(PermissionConstants.RoleUpdate)]
        public async Task<IActionResult> UpdateRole(Role roleModel)
        {
            const string methodName = nameof(UpdateRole);
            try
            {
                _logger.LogInformation($"{methodName} called from role controller");
                var JWTAuthClaim = HttpContext.Items["AuthClaim"] as AuthClaim;
                var data = await _roleService.UpdateRole(roleModel,JWTAuthClaim);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred in {methodName} called from role controller");
                throw;
            }
        }

        [HttpDelete("DeleteRole")]
        [Authorize]
        [HasPermission(PermissionConstants.RoleDelete)]
        public async Task<IActionResult> DeletRole(int roleId)
        {
            const string methodName = nameof(DeletRole);
            try
            {
                _logger.LogInformation($"{methodName} called from role controller");
                var JWTAuthClaim = HttpContext.Items["AuthClaim"] as AuthClaim;
                var data = await _roleService.DeleteRole(roleId, JWTAuthClaim);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred in {methodName} called from role controller");
                throw;
            }
        }
    }
}
