using LearnArchitecture.Core.Entities;
using LearnArchitecture.Core.Helper;
using LearnArchitecture.Core.Helper.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Services.IServices
{
    public interface IPermissionService
    {
        public Task<ApiResponse<List<Permissions>>> GetAllPermission(AuthClaim authClaim); 
    }
}
