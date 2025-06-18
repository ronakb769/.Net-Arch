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
using Azure.Core;
using System.Reflection;
using System.Security.Cryptography;
using Microsoft.Extensions.ObjectPool;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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

        public async Task<ApiResponse<LoginResponseModel>> Login(LoginRequestModel loginModel, string? clientIp)
        {
            try
            {
                _logger.LogInformation("Calling Login Service");
                var result = new ApiResponse<LoginResponseModel>();
                if (loginModel == null || string.IsNullOrEmpty(loginModel.email) || string.IsNullOrEmpty(loginModel.password))
                {
                     result.Message = "User Name/ Password is required";
                    return result;
                }
                var user = await _loginRepository.Login(loginModel);
                
                

                if (user != null)
                {
                    var userRole = await _loginRepository.HasAnyRoleAsync(user.userId);
                    //if (userRole == null || !allowedRoles.Contains(userRole.roleName))
                    //{
                    //    return ResponseBuilder.Fail<LoginResponseModel>("User is Not Authorize!",HttpStatusCode.Unauthorized);
                    //}

                    if (user.isActive == false || user.isDelete == true)
                        return ResponseBuilder.Fail<LoginResponseModel>("Inactive User!");

                    

                    // 👉 Log login history record (but wait until token is ready)
                    if(clientIp == null)
                    {
                        clientIp = "Unknown"; // Fallback if client IP is not provided
                    }
                    var loginHistory = new LoginHistory
                    {
                        userId = user.userId,
                        loginTime = DateTime.Now,
                        loginDate = DateOnly.FromDateTime(DateTime.Now),
                        ipAddress = clientIp
                    };
                    int loginHistoryId = await _loginRepository.SaveLoginHistory(loginHistory); // returns inserted ID


                    //Generate Access token
                    var jwtToken = await GenerateAccessToken(user, userRole.roleId, loginHistoryId); // Pass the login history ID to the token generation method

                    //Generate Refresh token
                    string refreshToken = GenerateRefreshToken();
                    DateTime refreshExpiry = DateTime.UtcNow.AddDays(7); // Or your desired expiry

                    // Save the refresh token in the database
                    await _loginRepository.SaveRefreshToken(user.userId, refreshToken, refreshExpiry);

                    UserByIdResponseModel userReponse = new UserByIdResponseModel
                    {
                        userId = user.userId,
                        firstName = user.firstName,
                        lastName = user.lastName,
                        email = user.email, 
                        password = AesEncryption.Decrypt(user.password),
                        phone = user.phone,
                        profileUrl = user.profileUrl,
                        roleId = userRole.roleId
                    };

                    var login = new LoginResponseModel();
                    login.Token = jwtToken;
                    login.userData = userReponse;
                    login.RefreshToken = refreshToken;
                    login.RefreshTokenExpiryTime = refreshExpiry;

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
                    result = ResponseBuilder.Fail<LoginResponseModel>("Invalid Credential",HttpStatusCode.BadRequest);
                }
                return result;
            }
            catch (Exception)
            {
                _logger.LogWarning("Error While Login Service");
                throw;
            }
        }


        private async Task<string> GenerateAccessToken(Users user, int roleId, int loginHistoryId)
        {
            var issuer = _configuration.GetSection("Jwt").GetSection("ValidAudience").Value;
            var audience = _configuration.GetSection("Jwt").GetSection("ValidIssuer").Value;
            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt").GetSection("SecretKey").Value);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                          {
                            new Claim("userId", user.userId.ToString()),
                            new Claim("userEmail", user.email.ToString()),
                            new Claim("userName", (user.firstName.ToString()+" "+user.lastName.ToString())),
                            new Claim("userRoleId", roleId.ToString()),
                            new Claim("loginHistoryId",loginHistoryId.ToString()) // Include login history ID in the token

                        }),
                Expires =  DateTime.UtcNow.AddMinutes(2),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                                               new SymmetricSecurityKey(key),
                                               SecurityAlgorithms.HmacSha256)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);

            return jwtToken;

        }
        public async Task<ApiResponse<LoginResponseModel>> RefreshToken(TokenApiModel tokenModel)
        {
            try
            {
                var principal = GetPrincipalFromExpiredToken(tokenModel.AccessToken);
                var userIdClaim = principal?.FindFirst("userId")?.Value;
                var roleIdClaim = principal?.FindFirst("userRoleId")?.Value;
                var loginHistoryIdClaim = principal?.FindFirst("loginHistoryId")?.Value;

                if (!int.TryParse(userIdClaim, out int userId) ||
                    !int.TryParse(roleIdClaim, out int roleId) ||
                    !int.TryParse(loginHistoryIdClaim, out int loginHistoryId))
                {
                    return ResponseBuilder.Fail<LoginResponseModel>("Invalid Token");
                }

                // 🔍 Fetch the refresh token entry from your custom RefreshToken table
                var refreshTokenEntry = await _loginRepository.GetRefreshToken(tokenModel.RefreshToken);

                if (refreshTokenEntry == null ||
                    refreshTokenEntry.userId != userId ||
                    refreshTokenEntry.expiryTime <= DateTime.UtcNow)
                {
                    return ResponseBuilder.Fail<LoginResponseModel>("Invalid Refresh Token");
                }

                // 👤 Fetch user details
                var user = await _loginRepository.GetUserById(userId);
                if (user == null)
                {
                    return ResponseBuilder.Fail<LoginResponseModel>("User not found");
                }

                // 🔐 Generate new tokens
                var newAccessToken = await GenerateAccessToken(user, roleId, loginHistoryId);
                var newRefreshToken = GenerateRefreshToken();
                var newRefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

                // 🔄 Update the existing refresh token entry
                refreshTokenEntry.refreshToken = newRefreshToken;
                refreshTokenEntry.expiryTime = newRefreshTokenExpiry;
                await _loginRepository.UpdateRefreshToken(refreshTokenEntry);

                UserByIdResponseModel userResponse = new UserByIdResponseModel
                {
                    userId = user.userId,
                    firstName = user.firstName,
                    lastName = user.lastName,
                    email = user.email,
                    password = AesEncryption.Decrypt(user.password),
                    phone = user.phone,
                    profileUrl = user.profileUrl,
                    roleId = roleId,
                };

                return ResponseBuilder.Success(new LoginResponseModel
                {
                    Token = newAccessToken,
                    RefreshToken = newRefreshToken,
                    RefreshTokenExpiryTime = newRefreshTokenExpiry,
                    userData = userResponse
                });
            }
            catch (Exception ex)
            {
                throw;
            }
        }
       


        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false, // 🔴 Ignore expiration here
                ValidIssuer = _configuration["JWT:ValidIssuer"],
                ValidAudience = _configuration["JWT:ValidAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            return principal;
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
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
                  return   ResponseBuilder.Fail<bool>("User not found with this Email", HttpStatusCode.NotFound);
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
                    return ResponseBuilder.Fail<bool>("User not found or error occurred", HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName}");
                return ResponseBuilder.Fail<bool>("An error occurred while reset password user");
            }
        }

        public async Task<ApiResponse<bool>> LogoutAsync(int loginHistoryId)
        {
            const string methodName = nameof(LogoutAsync);
            try
            {

                _logger.LogInformation($"Logging out session with LoginHistoryId: {loginHistoryId}");
                var isLogout =  await _loginRepository.UpdateLogoutTimeAsync(loginHistoryId);
                if (isLogout)
                {
                    return ResponseBuilder.Success(isLogout, "Logout Successfully");
                }
                else
                {
                    return ResponseBuilder.Fail<bool>("No matching login history found", HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, $"Exception in {methodName}");
                return ResponseBuilder.Fail<bool>("An error occurred while logging out the user.");
            }
          
        }
    }
}
