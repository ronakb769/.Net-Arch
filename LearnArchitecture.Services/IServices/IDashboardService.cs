using LearnArchitecture.Core.Entities;
using LearnArchitecture.Core.Helper.Constants;
using LearnArchitecture.Core.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearnArchitecture.Core.Models.ResponseModel;

namespace LearnArchitecture.Services.IServices
{
    public interface IDashboardService
    {
        public Task<ApiResponse<DashboardResponseModel>> GetUserCount(AuthClaim authClaim);
    }
}
