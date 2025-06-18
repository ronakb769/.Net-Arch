using LearnArchitecture.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Data.IRepository
{
    public interface IAppSettngsRepository
    {
        public Task<List<AppSettings>> GetSettingValue();
        public Task<AppSettings> GetAppSettingById(int id);
        public Task<bool> UpdateAppSettings(AppSettings existingAppSetting);
    }
}
