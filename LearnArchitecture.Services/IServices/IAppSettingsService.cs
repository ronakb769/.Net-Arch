using LearnArchitecture.Core.Entities;
using LearnArchitecture.Core.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Services.IServices
{
    public interface IAppSettingsService
    {
        public Task<ApiResponse<List<AppSettings>>> GetSettingValue();
        public Task<ApiResponse<AppSettings>> GetAppSettingById(int id);
        public Task<ApiResponse<bool>> UpdateAppSettings(AppSettings appSettings);
    }
}
