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
    public interface IAppSettngsRepository
    {
        public Task<List<AppSettings>> GetCaptchaStatus();
        public Task<PagingResponseModel<AppSettings>> GetSettingValue(AppSettingPagingRequestModel request);
        public Task<AppSettings> GetAppSettingById(int id);
        public Task<bool> UpdateAppSettings(AppSettings existingAppSetting);
    }
}
