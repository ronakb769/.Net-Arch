using LearnArchitecture.Core.Entities;
using LearnArchitecture.Core.Helper;
using LearnArchitecture.Core.Models.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Services.IServices
{
    public interface ILoginHistoryService
    {
        public Task<ApiResponse<List<LoginHistoryResponseModel>>> GetLoginHistory();
    }
}
