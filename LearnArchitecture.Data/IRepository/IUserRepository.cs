using LearnArchitecture.Core.Entities;
using LearnArchitecture.Core.Helper.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Data.IRepository
{
    public interface IUserRepository
    {
        public Task<List<Users>> GetAllUsers(AuthClaim authClaim);
        public Task<Users> GetUserById(int userId);
        public Task<bool> SaveUserWithRoleAsync(Users userModel, UserRoleMapping userRoleMapping, bool isUpdate);
        public Task<UserRoleMapping> GetRoleByUserId(int userId);
    }
}
