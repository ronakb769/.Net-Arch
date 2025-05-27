using LearnArchitecture.Core.Helper;
using LearnArchitecture.Core.Entities;
using LearnArchitecture.Core.Models.RequestModels;
using LearnArchitecture.Core.Models.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Services.IServices
{
    public interface ILoginService
    {
        public Task<ApiResponse<LoginResponseModel>> Login(LoginRequestModel loginModel);
        public Task<ApiResponse<bool>> CheckEmail(string emailAddress);
        public Task<ApiResponse<bool>> ResetPassword(LoginRequestModel resetModel);
    }
}
