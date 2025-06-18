using LearnArchitecture.Core.Entities;
using LearnArchitecture.Core.Helper;
using LearnArchitecture.Core.Helper.Constants;
using LearnArchitecture.Core.Models.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Services.IServices
{
    public interface IRoleService
    {
        public Task<ApiResponse<List<Role>>> GetAllRole(AuthClaim authClaim);
        public Task<ApiResponse<Role>> GetRoleById(int roleId);
        //public Task<ApiResponse<bool>> CreateRole(CreateRoleRequestModel role,AuthClaim authClaim);
        public Task<ApiResponse<bool>> CreateRole(Role role, AuthClaim authClaim);
        public Task<ApiResponse<bool>> UpdateRole(Role role, AuthClaim authClaim);
        public Task<ApiResponse<bool>> DeleteRole(int roleId, AuthClaim authClaim);
    }
}
