using LearnArchitecture.Core.Helper;
using LearnArchitecture.Core.Helper.Constants;
using LearnArchitecture.Core.Models.RequestModels;
using LearnArchitecture.Services.IServices;
using LearnArchitecture.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

namespace LearnArchitecture.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly ILogger<LoginController> _logger;
        public LoginController(ILoginService loginService, ILogger<LoginController> logger)
        {
            this._loginService = loginService;
            _logger = logger;
        }

        [HttpPost("Token")]
        public async Task<IActionResult> Login(LoginRequestModel loginModel)
        {
            try
            {
                var content = await _loginService.Login(loginModel);
                return Ok(content);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("CheckEmail")]
        public async Task<IActionResult> CheckEmail(string emailAddress)
        {
            const string methodName = nameof(CheckEmail);
            try
            {
                _logger.LogInformation($"{methodName} called from user controller");
                var JWTAuthClaim = HttpContext.Items["AuthClaim"] as AuthClaim;
                var data = await _loginService.CheckEmail(emailAddress);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred in {methodName} called from user controller");
                throw;
            }
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(LoginRequestModel resetModel)
        {
            const string methodName = nameof(ResetPassword);
            try
            {
                _logger.LogInformation($"{methodName} called from user controller");
                var JWTAuthClaim = HttpContext.Items["AuthClaim"] as AuthClaim;
                var data = await _loginService.ResetPassword(resetModel);
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
