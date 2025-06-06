using LearnArchitecture.Core.Entities;
using LearnArchitecture.Core.Helper;
using LearnArchitecture.Core.Helper.Constants;
using LearnArchitecture.Core.Helper.Enums;
using LearnArchitecture.Core.Models.RequestModels;
using LearnArchitecture.Core.Models.ResponseModel;
using LearnArchitecture.Data.IRepository;
using LearnArchitecture.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using static LearnArchitecture.Core.Helper.Enums.Enums;

namespace LearnArchitecture.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            this._userRepository = userRepository;
            this._logger = logger;
        }

        public async Task<ApiResponse<List<Users>>> GetAllUsers(AuthClaim authClaim)
        {
            const string methodName = nameof(GetAllUsers);
            try
            {
                _logger.LogInformation($"{methodName} called from user service");

                var users = await _userRepository.GetAllUsers(authClaim);
                if (users == null || !users.Any())
                    return ResponseBuilder.Fail<List<Users>>("No users found",HttpStatusCode.NotFound);

                return ResponseBuilder.Success(users, "Users retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName} from user service");
                return ResponseBuilder.Fail<List<Users>>("An error occurred while retrieving users");
            }
        }

        public async Task<ApiResponse<UserByIdResponseModel>> GetUserById(int userId)
        {
            const string methodName = nameof(GetUserById);
            try
            {
                _logger.LogInformation($"{methodName} called with userId: {userId}");

                if (userId <= 0)
                    return ResponseBuilder.Fail<UserByIdResponseModel>("Invalid user ID");

                var user = await _userRepository.GetUserById(userId);
                if (user == null)
                    return ResponseBuilder.Fail<UserByIdResponseModel>("User not found",HttpStatusCode.NotFound);

                user.password = AesEncryption.Decrypt(user.password);
                return ResponseBuilder.Success(user, "User retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName} from user service");
                return ResponseBuilder.Fail<UserByIdResponseModel>("An error occurred while retrieving the user");
            }
        }

        #region Create user using Enity Framework
        //public async Task<ApiResponse<bool>> CreateUser(CreateUserRoleRequestModel userModel, AuthClaim authClaim)
        //{
        //    const string methodName = nameof(CreateUser);
        //    try
        //    {
        //        _logger.LogInformation($"{methodName} called");

        //        if (userModel == null)
        //            return ResponseBuilder.Fail<bool>("Invalid user data");

        //        userModel.password = AesEncryption.Encrypt(userModel.password);

        //        var user = new Users
        //        {
        //            userId = userModel.userId,
        //            firstName = userModel.firstName,
        //            lastName = userModel.lastName,
        //            email = userModel.email,
        //            password = userModel.password,
        //            phone = userModel.phone
        //        };

        //        user = GenericCommonField.UpdateCommonField(user, EnumOperationType.Add, authClaim);

        //        if (userModel.profile != null)
        //        {
        //            user.profileUrl = await SaveUserProfileImage(userModel.profile);
        //        }

        //        var userRoleModel = new UserRoleMapping
        //        {
        //            userId = user.userId,
        //            roleId = userModel.roleId
        //        };

        //        userRoleModel = GenericCommonField.UpdateCommonField(userRoleModel, EnumOperationType.Add, authClaim);

        //        bool isCreated = await _userRepository.SaveUserWithRoleAsync(user, userRoleModel, false);

        //        return isCreated
        //            ? ResponseBuilder.Success(true, "User created successfully")
        //            : ResponseBuilder.Fail<bool>("Failed to create user");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Exception in {methodName}");
        //        return ResponseBuilder.Fail<bool>("An error occurred while creating the user");
        //    }
        //}
        #endregion


        #region Create User using Stored procedure
        public async Task<ApiResponse<bool>> CreateUser(CreateUserRoleRequestModel userModel, AuthClaim authClaim)
        {
            const string methodName = nameof(CreateUser);
            try
            {
                _logger.LogInformation($"{methodName} called");

                if (userModel == null)
                    return ResponseBuilder.Fail<bool>("Invalid user data");

                userModel.password = AesEncryption.Encrypt(userModel.password);

                var user = new Users
                {
                    userId = userModel.userId,
                    firstName = userModel.firstName,
                    lastName = userModel.lastName,
                    email = userModel.email,
                    password = userModel.password,
                    phone = userModel.phone
                };

                user = GenericCommonField.UpdateCommonField(user, EnumOperationType.Add, authClaim);

                if (userModel.profile != null)
                {
                    user.profileUrl = await SaveUserProfileImage(userModel.profile);
                }

                var userRoleModel = new UserRoleMapping
                {
                    userId = user.userId,
                    roleId = userModel.roleId
                };

                userRoleModel = GenericCommonField.UpdateCommonField(userRoleModel, EnumOperationType.Add, authClaim);

                bool isCreated = await _userRepository.SaveUserWithRoleAsync(user, userRoleModel, false);

                return isCreated
                    ? ResponseBuilder.Success(true, "User created successfully")
                    : ResponseBuilder.Fail<bool>("Failed to create user");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName}");
                return ResponseBuilder.Fail<bool>("An error occurred while creating the user");
            }
        }
        #endregion



        public async Task<ApiResponse<bool>> UpdateUser(CreateUserRoleRequestModel userModel, AuthClaim authClaim)
        {
            const string methodName = nameof(UpdateUser);
            try
            {
                _logger.LogInformation($"{methodName} called with userId: {userModel?.userId}");

                if (userModel == null)
                    return ResponseBuilder.Fail<bool>("Invalid user data");

                var existingUser = await _userRepository.GetUserByIdForUpdate(userModel.userId);
                if (existingUser == null)
                    return ResponseBuilder.Fail<bool>("User not found");

                existingUser.firstName = userModel.firstName;
                existingUser.lastName = userModel.lastName;
                existingUser.email = userModel.email;
                existingUser.phone = userModel.phone;
                existingUser.password = AesEncryption.Encrypt(userModel.password);

                existingUser = GenericCommonField.UpdateCommonField(existingUser, EnumOperationType.Update, authClaim);

                if (userModel.profile != null && userModel.profile.Length > 0)
                {
                    var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(userModel.profile.FileName).ToLower();

                    if (!validExtensions.Contains(extension))
                        return ResponseBuilder.Fail<bool>("Invalid file format. Allowed formats are: .jpg, .jpeg, .png, .gif");

                  
                    if (!string.IsNullOrEmpty(existingUser.profileUrl))
                    {
                        var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), existingUser.profileUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                        if (File.Exists(oldFilePath))
                        {
                            File.Delete(oldFilePath);
                        }
                    }

                    existingUser.profileUrl = await SaveUserProfileImage(userModel.profile);
                }

                var userRoleMapping = await _userRepository.GetRoleByUserId(existingUser.userId);
                if (userRoleMapping != null)
                {
                    userRoleMapping.roleId = userModel.roleId;
                    userRoleMapping = GenericCommonField.UpdateCommonField(userRoleMapping, EnumOperationType.Update, authClaim);
                }

                bool isUpdated = await _userRepository.SaveUserWithRoleAsync(existingUser, userRoleMapping, true);

                return isUpdated
                    ? ResponseBuilder.Success(true, "User updated successfully")
                    : ResponseBuilder.Fail<bool>("Failed to update user");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName}");
                return ResponseBuilder.Fail<bool>("An error occurred while updating the user");
            }
        }

        private static string GetUniqueFileName(string fileName)
        {
            var guidPart = Guid.NewGuid().ToString("N").Substring(0, 10);
            return $"{Path.GetFileNameWithoutExtension(fileName)}_{guidPart}{Path.GetExtension(fileName)}";
        }

        private async Task<string> SaveUserProfileImage(IFormFile file)
        {
            try
            {
                var uniqueFileName = GetUniqueFileName(file.FileName);
                var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "Images");

                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);

                var filePath = Path.Combine(uploadDir, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                return $"/Images/{uniqueFileName}";
            }
            catch (Exception)
            {
                throw;
            }   
        }

        public async Task<ApiResponse<bool>> DeleteUser(int userId, AuthClaim authClaim)
        {
            const string methodName = nameof(DeleteUser);
            try
            {
                _logger.LogInformation($"{methodName} called with userId: {userId}");


                var existingUser = await _userRepository.GetUserByIdForUpdate(userId);
                if (existingUser == null)
                    return ResponseBuilder.Fail<bool>("User not found"); 

                existingUser = GenericCommonField.UpdateCommonField(existingUser, EnumOperationType.Delete, authClaim);


                //var userRoleMapping = await _userRepository.GetRoleByUserId(existingUser.userId);
                //if (userRoleMapping != null)
                //{
                //    userRoleMapping = GenericCommonField.UpdateCommonField(userRoleMapping, EnumOperationType.Delete, authClaim);
                //}

                bool isDeleted = await _userRepository.DeleteUser(existingUser);

                return isDeleted
                    ? ResponseBuilder.Success(true, "User Deleted successfully")
                    : ResponseBuilder.Fail<bool>("Failed to Delete user");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName}");
                return ResponseBuilder.Fail<bool>("An error occurred while deleting the user");
            }
        }

        public async Task<ApiResponse<bool>> UpdateUserStatus(int userId, bool isActive, AuthClaim authClaim)
        {
            const string methodName = nameof(UpdateUserStatus);
            try
            {
                _logger.LogInformation($"{methodName} called with userId: {userId}");


                var existingUser = await _userRepository.GetUserByIdForUpdate(userId);
                if (existingUser == null)
                    return ResponseBuilder.Fail<bool>("User not found");

                existingUser.isActive = isActive;

                existingUser = GenericCommonField.UpdateCommonField(existingUser, EnumOperationType.Update, authClaim);


                var userRoleMapping = await _userRepository.GetRoleByUserId(existingUser.userId);
                if (userRoleMapping != null)
                {
                    userRoleMapping.isActive = isActive;
                    userRoleMapping = GenericCommonField.UpdateCommonField(userRoleMapping, EnumOperationType.Update, authClaim);
                }

                bool isUpdated = await _userRepository.SaveUserWithRoleAsync(existingUser, userRoleMapping, true);

                return isUpdated
                    ? ResponseBuilder.Success(true, "User updated successfully")
                    : ResponseBuilder.Fail<bool>("Failed to update user");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName}");
                return ResponseBuilder.Fail<bool>("An error occurred while updating the user");
            }
        }
    }
}
