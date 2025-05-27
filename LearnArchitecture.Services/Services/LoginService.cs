using LearnArchitecture.Core.Helper;
using LearnArchitecture.Core.Entities;
using LearnArchitecture.Core.Models.RequestModels;
using LearnArchitecture.Core.Models.ResponseModel;
using LearnArchitecture.Data.IRepository;
using LearnArchitecture.Services.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using LearnArchitecture.Core.Helper.Constants;
using Microsoft.Extensions.Caching.Distributed;
using LearnArchitecture.Data.Repository;
using System.Net.Mail;
using Microsoft.Extensions.Caching.Memory;

namespace LearnArchitecture.Services.Services
{
    public class LoginService:ILoginService
    {
        private readonly ILoginRepository _loginRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LoginService> _logger;
        private readonly IMemoryCache _cache;
        private readonly IPermissionRepository _permissionRepository;
        public LoginService(ILoginRepository loginRepository, IConfiguration configuration, ILogger<LoginService> logger, IMemoryCache cache,IPermissionRepository permissionRepository)
        {
            this._loginRepository = loginRepository;
            this._configuration = configuration;
            this._logger = logger;
            this._cache = cache;
            this._permissionRepository = permissionRepository;
        }

        public async Task<ApiResponse<LoginResponseModel>> Login(LoginRequestModel loginModel)
        {
            try
            {
                _logger.LogInformation("Calling Login Service");
                var result = new ApiResponse<LoginResponseModel>();
                if (loginModel == null || string.IsNullOrEmpty(loginModel.email) || string.IsNullOrEmpty(loginModel.password))
                {
                    result.Message = "User Name/ Password is required";
                }
                var user = await _loginRepository.Login(loginModel);
                // Define allowed roles
                var allowedRoles = new List<string>
                {
                    RoleConstants.SuperAdmin,
                    RoleConstants.Admin
                };

                
                if (user != null)
                {
                    var userRole = await _loginRepository.HasAnyRoleAsync(user.userId);
                    if (userRole == null || !allowedRoles.Contains(userRole.roleName))
                    {
                        return ResponseBuilder.Fail<LoginResponseModel>("User is Not Authorize!");
                    }

                    if (user.isActive == false || user.isDelete == true)
                        return ResponseBuilder.Fail<LoginResponseModel>("Inactive User!");

                    var issuer = _configuration.GetSection("Jwt").GetSection("ValidAudience").Value;
                    var audience = _configuration.GetSection("Jwt").GetSection("ValidIssuer").Value;
                    var key = Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt").GetSection("SecretKey").Value);

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.userId.ToString()),
                            new Claim("userEmail", user.email.ToString()),
                            new Claim("userName", (user.firstName.ToString()+" "+user.lastName.ToString())),
                            new Claim("userRoleId", userRole.roleId.ToString())

                        }),
                        Expires = DateTime.Now.AddHours(2),
                        Issuer = issuer,
                        Audience = audience,
                        SigningCredentials = new SigningCredentials(
                                             new SymmetricSecurityKey(key),
                                             SecurityAlgorithms.HmacSha256)
                    };
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var jwtToken = tokenHandler.WriteToken(token);
                    var login = new LoginResponseModel();
                    login.Token = jwtToken;
                    login.userData = user;

                    // Fetch and cache permissions for the user
                    var userRoles = await _permissionRepository.GetRolesByUserId(user.userId);
                    var permissionList = new List<string>();

                    foreach (var role in userRoles)
                    {
                        var permissions = await _permissionRepository.GetPermissionsByRoleId(role.roleId);
                        permissionList.AddRange(permissions.Select(p => p));
                    }

                    // Remove duplicates
                    permissionList = permissionList.Distinct().ToList();

                    //In-memory 
                    string cacheKey = $"permissions:{user.userId}";
                    _cache.Set(cacheKey, permissionList, TimeSpan.FromHours(2));

                    #region redis cache
                    //// Serialize and cache
                    //var serializedPermissions = System.Text.Json.JsonSerializer.Serialize(permissionList);
                    //string cacheKey = $"permissions:{user.userId}";

                    //await _cache.SetStringAsync(cacheKey, serializedPermissions, new DistributedCacheEntryOptions
                    //{
                    //    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
                    //});
                    #endregion
                    result = ResponseBuilder.Success(login, "Login Successfully");
                }
                else
                {
                    result = ResponseBuilder.Fail<LoginResponseModel>("Invalid Credential");
                }
                return result;
            }
            catch (Exception)
            {
                _logger.LogWarning("Error While Login Service");
                throw;
            }
        }
        public async Task<ApiResponse<bool>> CheckEmail(string emailAddress)
        {
            const string methodName = nameof(CheckEmail);
            try
            {
                _logger.LogInformation($"{methodName} called with emailAddress: {emailAddress}");
                var isExist = await _loginRepository.CheckEmail(emailAddress);

                if (isExist)
                {
                    return ResponseBuilder.Success(isExist, "Valid User");
                }
                else
                {
                  return   ResponseBuilder.Fail<bool>("User not found with this Email");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName}");
                return ResponseBuilder.Fail<bool>("An error occurred while Checking email address the user");
            }
        }

        public async Task<ApiResponse<bool>> ResetPassword(LoginRequestModel resetModel)
        {
            const string methodName = nameof(ResetPassword);
            try
            {
                _logger.LogInformation($"{methodName} called from login service");
                var isExist = await _loginRepository.ResetPassword(resetModel);

                if (isExist)
                {
                    return ResponseBuilder.Success(isExist, "Password reset successfuly");
                }
                else
                {
                    return ResponseBuilder.Fail<bool>("User not found or error occurred");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName}");
                return ResponseBuilder.Fail<bool>("An error occurred while reset password user");
            }
        }
    }
}
