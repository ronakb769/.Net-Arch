using LearnArchitecture.Core.Entities;
using LearnArchitecture.Core.Helper;
using LearnArchitecture.Core.Helper.Enums;
using LearnArchitecture.Core.Models.RequestModels;
using LearnArchitecture.Data.Context;
using LearnArchitecture.Data.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Data.Repository
{
    public class LoginRepository:ILoginRepository
    {
        private readonly LearnArchitectureDbContext _dbContext;
        public LoginRepository(LearnArchitectureDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<Users> Login(LoginRequestModel loginModel)
        {
            try
            {
                string encryptPass = AesEncryption.Encrypt(loginModel.password);
                var userData = await _dbContext.Users.Where(x => x.email.ToLower().Equals(loginModel.email) && x.password.Equals(encryptPass)).FirstOrDefaultAsync();
                return userData;   
            }
            catch (Exception) 
            {
                throw;
            }
        }
        public async Task<Role> HasAnyRoleAsync(int userId)
        {
            try
            {
                var userRole = await (from r in _dbContext.Role
                                      join ur in _dbContext.UserRoleMapping on r.roleId equals ur.roleId
                                      where ur.userId == userId && !r.isDelete && r.isActive
                                      select r).FirstOrDefaultAsync();

                return userRole;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> CheckEmail(string emailAddress)
        {
            try
            {
                bool isExist = await _dbContext.Users
                                .AnyAsync(x => x.email.ToLower() == emailAddress.ToLower() && x.isActive && !x.isDelete); // Assumes your user entity has an 'Email' property

                return isExist;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> ResetPassword(LoginRequestModel resetModel)
        {
            try
            {
                var userData = await _dbContext.Users
                                .Where(x => x.email.ToLower() == resetModel.email.ToLower() && x.isActive && !x.isDelete).FirstOrDefaultAsync(); // Assumes your user entity has an 'Email' property

                userData.password = AesEncryption.Encrypt(resetModel.password);
                _dbContext.Users.Update(userData);
                int id = await _dbContext.SaveChangesAsync();

                return id>0?true:false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> SaveLoginHistory(LoginHistory loginHistory)
        {
            try
            {
                await _dbContext.LoginHistory.AddAsync(loginHistory);
                await _dbContext.SaveChangesAsync();
                return loginHistory.loginHistoryId; // Return the ID of the newly inserted record
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> SaveRefreshToken(int userId, string refreshToken, DateTime expiryTime)
        {
            var refreshTokendata = new RefreshToken
            {
                userId = userId,
                refreshToken = refreshToken,
                expiryTime = expiryTime
            };
                await _dbContext.RefreshToken.AddAsync(refreshTokendata);
                int id  = await _dbContext.SaveChangesAsync();
            return id > 0 ? true : false;   
        }

        public async Task<bool> UpdateLogoutTimeAsync(int loginHistoryId)
        {
            try
            {
                var record = await _dbContext.LoginHistory.FindAsync(loginHistoryId);
                if (record == null)
                    return false;

                record.logoutTime = DateTime.Now;

                _dbContext.LoginHistory.Update(record);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Users> GetUserById(int userId)
        {
            try
            {
                var data = await _dbContext.Users.Where(x => x.userId == userId && x.isActive && !x.isDelete)
                                .FirstOrDefaultAsync();
                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<RefreshToken> GetRefreshToken(string refreshToken)
        {
            try
            {
                var data = await _dbContext.RefreshToken.Where(x => x.refreshToken == refreshToken)
                                .FirstOrDefaultAsync(); 
                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> UpdateRefreshToken(RefreshToken tokenEntity)
        {
             _dbContext.RefreshToken.Update(tokenEntity);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
