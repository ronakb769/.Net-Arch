using LearnArchitecture.Core.Entities;
using LearnArchitecture.Core.Helper;
using LearnArchitecture.Core.Models.RequestModels;
using LearnArchitecture.Core.Models.ResponseModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Services.IServices
{
    public interface IAppSettingsService
    {
        public Task<ApiResponse<List<AppSettings>>>  GetCaptchaStatus();
        public Task<ApiResponse<PagingResponseModel<AppSettings>>> GetSettingValue(AppSettingPagingRequestModel request);
        public Task<ApiResponse<AppSettings>> GetAppSettingById(int id);
        public Task<ApiResponse<bool>> UpdateAppSettings(AppSettings appSettings);
    }
}
