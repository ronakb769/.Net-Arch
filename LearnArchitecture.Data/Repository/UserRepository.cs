using LearnArchitecture.Core.Entities;
using LearnArchitecture.Core.Helper.Constants;
using LearnArchitecture.Core.Models.RequestModels;
using LearnArchitecture.Core.Models.ResponseModel;
using LearnArchitecture.Data.Context;
using LearnArchitecture.Data.IRepository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Data.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly LearnArchitectureDbContext _dbContext;
        private readonly ILogger<UserRepository> _logger;
        public UserRepository(LearnArchitectureDbContext dbContext, ILogger<UserRepository> _logger)
        {
            this._dbContext = dbContext;
            this._logger = _logger;
        }

        #region GetAllUsers using Entity Framework
        //public async Task<List<Users>> GetAllUsers(AuthClaim authClaim)
        //{
        //    const string methodName = nameof(GetAllUsers);
        //    try
        //    {
        //        _logger.LogInformation($"{methodName} called from UserRepository");
        //        return await _dbContext.Users
        //                    .Where(x => x.isActive && !x.isDelete && x.userId != authClaim.userId)
        //                    .ToListAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Exception in {methodName}");
        //        throw;
        //    }
        //}
        #endregion


        #region GetAllUsers using Stored Procedure
        public async Task<PagingResponseModel<UserResponseModel>> GetAllUsers(UserPagingRequestModel request, AuthClaim authClaim)
        {
            const string methodName = nameof(GetAllUsers);
            try
            {
                _logger.LogInformation($"{methodName} called from UserRepository");

                SqlParameter userId = new("@userId", authClaim.userId);
                var users = await _dbContext.Users
                    .FromSqlRaw("EXEC GetAllUser @userId", userId)
                    .ToListAsync();

                // Projection to response model (for better control and DTO separation)
                var query = users.Select(u => new UserResponseModel
                {
                    userId = u.userId,
                    userName = u.firstName + " " + u.lastName,
                    email = u.email,
                    phone = u.phone,
                    createdOn = u.createdOn,
                    isActive = u.isActive,
                    isDelete = u.isDelete,
                }).AsQueryable();

                // Filtering
                if (!string.IsNullOrWhiteSpace(request.searchText))
                {
                    string lowerSearch = request.searchText.ToLower();
                    query = query.Where(x =>
                        x.userName.ToLower().Contains(lowerSearch) ||
                        x.email.ToLower().Contains(lowerSearch) ||
                        x.phone.ToLower().Contains(lowerSearch));
                }

                // Apply isActive filter
                if (request.isActive == true)
                {
                    // Active users → isActive: true, isDelete: false
                    query = query.Where(x => x.isActive == true && x.isDelete == false);
                }
                else if (request.isActive == false)
                {
                    // Inactive users → isActive: true, isDelete: true
                    query = query.Where(x => x.isActive == false && x.isDelete == false);
                }
                else
                {
                    // All users with isActive: true (both deleted and not deleted)
                    query = query.Where(x => x.isDelete == false);
                }

                int totalRecords = query.Count();

                // Sorting
                query = request.SortColumn?.ToLower() switch
                {
                    "username" => request.SortDirection == "desc"
                        ? query.OrderByDescending(x => x.userName)
                        : query.OrderBy(x => x.userName),

                    "email" => request.SortDirection == "desc"
                        ? query.OrderByDescending(x => x.email)
                        : query.OrderBy(x => x.email),

                    "phone" => request.SortDirection == "desc"
                       ? query.OrderByDescending(x => x.phone)
                       : query.OrderBy(x => x.phone),

                    "createdOn" => request.SortDirection == "desc"
                        ? query.OrderByDescending(x => x.createdOn)
                        : query.OrderBy(x => x.createdOn),

					"status" => request.SortDirection == "desc"
		                ? query.OrderByDescending(x => x.isActive)
		                : query.OrderBy(x => x.isActive),

					_ => query.OrderByDescending(x => x.createdOn)
                };

                // Paging
                var data = query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                return new PagingResponseModel<UserResponseModel>
                {
                    Data = data,
                    TotalRecords = totalRecords
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName}");
                throw;
            }
        }

        #endregion
        public async Task<UserByIdResponseModel> GetUserById(int userId)
        {
            const string methodName = nameof(GetUserById);
            try
            {
                _logger.LogInformation($"{methodName} called with userId: {userId}");
                return await (from u in _dbContext.Users
                              join urm in _dbContext.UserRoleMapping
                              on u.userId equals urm.userId
                              where u.userId == userId && !u.isDelete
                              select new UserByIdResponseModel
                              {
                                  userId = u.userId,
                                  firstName = u.firstName,
                                  lastName = u.lastName,
                                  email = u.email,
                                  password = u.password,
                                  phone = u.phone,
                                  profileUrl = u.profileUrl,
                                  roleId = urm.roleId
                              }).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName} for userId: {userId}");
                throw;
            }
        }

        public async Task<Users> GetUserByIdForUpdate(int userId)
        {
            const string methodName = nameof(GetUserByIdForUpdate);
            try
            {
                _logger.LogInformation($"{methodName} called with userId: {userId}");
                return await _dbContext.Users.Where(u => u.userId == userId && !u.isDelete).FirstOrDefaultAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName} for userId: {userId}");
                throw;
            }
        }
        public async Task<bool> SaveUserWithRoleAsync(Users userModel, UserRoleMapping userRoleMapping, bool isUpdate)
        {
            const string methodName = nameof(SaveUserWithRoleAsync);

            try
            {
                _logger.LogInformation($"{methodName} started with isUpdate: {isUpdate}");

                    var result = new ResultModel();
                    SqlParameter userId = new("@userId", userModel.userId);
                    SqlParameter firstName = new("@firstName", userModel.firstName);
                    SqlParameter lastName = new("@lastName", userModel.lastName);
                    SqlParameter email = new("@email", userModel.email);
                    SqlParameter password = new("@password", userModel.password);
                    SqlParameter phone = new("@phone", userModel.phone);
                    SqlParameter profileUrl = new("@profileUrl", userModel.profileUrl);
                    SqlParameter roleId = new("@roleId", userRoleMapping.roleId);
                    SqlParameter isActive = new("@isActive", userModel.isActive);
                    SqlParameter isDelete = new("@isDelete", userModel.isDelete);
                    SqlParameter createdOn  = new("@createdOn", userModel.createdOn);
                    SqlParameter createdBy = new("@createdBy", userModel.createdBy);
                    SqlParameter updatedOn = new("@updatedOn", userModel.updatedOn);
                    SqlParameter updatedBy = new("@updatedBy", userModel.updatedBy);

                if (userModel.userId > 0)
                {
                    result = _dbContext.resultModels
                                        .FromSqlRaw("EXEC Sp_UpdateUser @userid, @firstName, @lastName, @email, @password, @phone, @profileUrl, @roleId, @isActive, @isDelete, @createdOn, @createdBy, @updatedOn, @updatedBy",
                                           userId, firstName, lastName, email, password, phone, profileUrl, roleId, isActive, isDelete, createdOn, createdBy, updatedOn, updatedBy)
                                        .AsEnumerable() // Switch to in-memory operations
                                        .FirstOrDefault();


                }

                else
                {


                    result = _dbContext.resultModels
                                        .FromSqlRaw("EXEC CreateUser @firstName, @lastName, @email, @password, @phone, @profileUrl, @roleId, @isActive, @isDelete, @createdOn, @createdBy",
                                            firstName, lastName, email, password, phone, profileUrl, roleId, isActive, isDelete, createdOn, createdBy)
                                        .AsEnumerable() // Switch to in-memory operations
                                        .FirstOrDefault();
                }
                if (result != null && result.flag)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName}, rolling back transaction");
                throw;
            }
        }

        public async Task<UserRoleMapping> GetRoleByUserId(int userId)
        {
            const string methodName = nameof(GetRoleByUserId);
            try
            {
                _logger.LogInformation($"{methodName} called for userId: {userId}");
                return await _dbContext.UserRoleMapping
                            .FirstOrDefaultAsync(x => x.userId == userId  && !x.isDelete);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName} for userId: {userId}");
                throw;
            }
        }
        public async Task<bool> DeleteUser(Users userModel)
        {
            const string methodName = nameof(DeleteUser);
            try
            {
                _logger.LogInformation($"{methodName} called for userId: {userModel.userId}");

                var result = new ResultModel();
                SqlParameter userId = new("@userId", userModel.userId);
                SqlParameter isActive = new("@isActive", userModel.isActive);
                SqlParameter isDelete = new("@isDelete", userModel.isDelete);
                SqlParameter updatedOn = new("@updatedOn", userModel.updatedOn);
                SqlParameter updatedBy = new("@updatedBy", userModel.updatedBy);

                result =   _dbContext.resultModels
                                        .FromSqlRaw("EXEC Sp_DeleteUser @userId,@isActive,@isDelete,@updatedOn,@updatedBy", 
                                            userId, isActive, isDelete, updatedOn, updatedBy)
                                        .AsEnumerable() // Switch to in-memory operations
                                        .FirstOrDefault();

                if (result != null && result.flag)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName} for userId: {userModel.userId}");
                throw;
            }
        }
    }
}
