using LearnArchitecture.Core.Entities;
using LearnArchitecture.Core.Helper;
using LearnArchitecture.Core.Models.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Data.IRepository
{
    public interface ILoginRepository
    {
        public Task<Users> Login(LoginRequestModel loginModel);
        public Task<Role> HasAnyRoleAsync(int userId);
        public Task<bool> CheckEmail(string emailAddress);
        public Task<bool> ResetPassword(LoginRequestModel resetModel);
        public Task<int> SaveLoginHistory(LoginHistory loginHistory);
        public Task<bool> UpdateLogoutTimeAsync(int loginHistoryId);
        public Task<bool> SaveRefreshToken(int userId, string refreshToken, DateTime expiryTime);
        public Task<Users> GetUserById(int userId);
        public Task<RefreshToken> GetRefreshToken(string refreshToken);
        public Task<bool> UpdateRefreshToken(RefreshToken tokenEntity);
    }
}
