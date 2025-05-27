using LearnArchitecture.Core.Models.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Data.IRepository
{
    public interface IDashboardRepository
    {
        public Task<DashboardResponseModel> GetUserCount();
     }
}
