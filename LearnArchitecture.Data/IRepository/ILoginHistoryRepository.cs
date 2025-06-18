using LearnArchitecture.Core.Entities;
using LearnArchitecture.Core.Models.RequestModels;
using LearnArchitecture.Core.Models.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Data.IRepository
{
    public interface ILoginHistoryRepository
    {
        public Task<PagingResponseModel<LoginHistoryResponseModel>> GetLoginHistory(LoginHistoryPagingRequestModel request);
        public Task<List<string>> GetAllUserNames();
    }
}
