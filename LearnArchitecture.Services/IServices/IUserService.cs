using LearnArchitecture.Core.Entities;
using LearnArchitecture.Core.Helper;
using LearnArchitecture.Core.Helper.Constants;
using LearnArchitecture.Core.Models.RequestModels;
using LearnArchitecture.Core.Models.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Services.IServices
{
    public interface IUserService
    {
        public Task<ApiResponse<List<Users>>> GetAllUsers(AuthClaim authClaim);
        public Task<ApiResponse<UserByIdResponseModel>> GetUserById(int userId);
        public Task<ApiResponse<bool>> CreateUser(CreateUserRoleRequestModel userModel, AuthClaim authClaim);
        public Task<ApiResponse<bool>> UpdateUser(CreateUserRoleRequestModel userModel, AuthClaim authClaim);
        public Task<ApiResponse<bool>> DeleteUser(int userId, AuthClaim authClaim);
        public Task<ApiResponse<bool>> UpdateUserStatus(int userId, bool isActive, AuthClaim authClaim);
    }
}
