using LearnArchitecture.Core.Entities;
using LearnArchitecture.Core.Helper;
using LearnArchitecture.Core.Models.RequestModels;
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
        public Task<ApiResponse<PagingResponseModel<LoginHistoryResponseModel>>> GetLoginHistory(LoginHistoryPagingRequestModel request);
        public Task<ApiResponse<List<string>>> GetAllUserNames();

    }
}
