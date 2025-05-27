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
    }
}
