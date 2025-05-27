using LearnArchitecture.Core.Entities;
using LearnArchitecture.Core.Helper.Constants;
using LearnArchitecture.Data.Context;
using LearnArchitecture.Data.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
        public async Task<List<Users>> GetAllUsers(AuthClaim authClaim)
        {
            const string methodName = nameof(GetAllUsers);
            try
            {
                _logger.LogInformation($"{methodName} called from UserRepository");
                return await _dbContext.Users
                            .Where(x => x.isActive && !x.isDelete && x.userId != authClaim.userId)
                            .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName}");
                throw;
            }
        }

        public async Task<Users> GetUserById(int userId)
        {
            const string methodName = nameof(GetUserById);
            try
            {
                _logger.LogInformation($"{methodName} called with userId: {userId}");
                return await _dbContext.Users
                            .FirstOrDefaultAsync(x => x.userId == userId && x.isActive && !x.isDelete);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName} for userId: {userId}");
                throw;
            }
        }

        public async Task<bool> SaveUserWithRoleAsync(Users userModel, UserRoleMapping userRoleMapping, bool isUpdate)
        {
            const string methodName = nameof(SaveUserWithRoleAsync);

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation($"{methodName} started with isUpdate: {isUpdate}");

                if (isUpdate)
                {
                    _dbContext.Users.Update(userModel);
                    _dbContext.UserRoleMapping.Update(userRoleMapping);
                }
                else
                {
                    await _dbContext.Users.AddAsync(userModel);
                    await _dbContext.SaveChangesAsync(); // Save first to get userId

                    userRoleMapping.userId = userModel.userId;
                    await _dbContext.UserRoleMapping.AddAsync(userRoleMapping);
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName}, rolling back transaction");
                await transaction.RollbackAsync();
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
                            .FirstOrDefaultAsync(x => x.userId == userId && x.isActive && !x.isDelete);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName} for userId: {userId}");
                throw;
            }
        }
    }
}
