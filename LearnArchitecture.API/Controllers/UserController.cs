using LearnArchitecture.Core.Entities;
using LearnArchitecture.Core.Helper.Attributes;
using LearnArchitecture.Core.Helper.Constants;
using LearnArchitecture.Core.Models.RequestModels;
using LearnArchitecture.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Threading.Tasks;

namespace LearnArchitecture.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            this._userService = userService;
            this._logger = logger;
        }

        [HttpGet("GetAllUser")]
        [Authorize]
        [HasPermission(PermissionConstants.UserView)]
        public async Task<IActionResult> GetAllUser()
        {
            const string methodName = nameof(GetAllUser);
            try
            {
                _logger.LogInformation($"{methodName} called user controller");
                var JWTAuthClaim = HttpContext.Items["AuthClaim"] as AuthClaim;
                var data = await _userService.GetAllUsers(JWTAuthClaim);
                return Ok(data);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, $"Exception occurred in {methodName} called from user controller");
                throw;
            }
        }
                                                            
        [HttpGet("GetUserById")]
        [Authorize]
        [HasPermission(PermissionConstants.UserView)]
        public async Task<IActionResult> GetUserById(int userId)
        {
            const string methodName = nameof(GetUserById);
            try
            {
                _logger.LogInformation($"{methodName} called with userId: {userId} from user controller");
                var data = await _userService.GetUserById(userId);
                return Ok(data);
            }
            
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred in {methodName} for userId: {userId} from user controller");
                throw;
            }
        }
            
        [HttpPost("CreateUser")]
        [Authorize]
        [HasPermission(PermissionConstants.UserCreate)]
        public async Task<IActionResult> CreateUser([FromForm] CreateUserRoleRequestModel userModel)
        {
            const string methodName = nameof(CreateUser);
            try
            {
                _logger.LogInformation($"{methodName} called from user controller");
                var JWTAuthClaim = HttpContext.Items["AuthClaim"] as AuthClaim;
                var data = await _userService.CreateUser(userModel,JWTAuthClaim);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred in {methodName} called from user controller");
                throw;
            }
        }

        [HttpPost("UpdateUser")]
        [Authorize]
        [HasPermission(PermissionConstants.UserUpdate)]
        public async Task<IActionResult> UpdateUser([FromForm] CreateUserRoleRequestModel userModel)
        {
            const string methodName = nameof(UpdateUser);
            try
            {
                _logger.LogInformation($"{methodName} called from user controller");
                var JWTAuthClaim = HttpContext.Items["AuthClaim"] as AuthClaim;
                var data = await _userService.UpdateUser(userModel, JWTAuthClaim);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred in {methodName} called from user controller");
                throw;
            }
        }


       
    }
}
