using LearnArchitecture.Core.Entities;
using LearnArchitecture.Data.Context;
using LearnArchitecture.Data.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Data.Repository
{
    public class AppSettingsRepository:IAppSettngsRepository
    {
        private readonly LearnArchitectureDbContext _dbContext;
        private readonly ILogger<AppSettingsRepository> _logger;
        public AppSettingsRepository(LearnArchitectureDbContext dbContext, ILogger<AppSettingsRepository> logger)
        {
            this._dbContext = dbContext;
            this._logger = logger;
        }

        public async Task<List<AppSettings>> GetSettingValue()
        {
            try
            {
                var setting = await _dbContext.AppSettings.ToListAsync();
                return setting;
            }
            catch (Exception ex) 
            {
                throw;
            }
            
        }

        public async Task<AppSettings> GetAppSettingById(int id)
        {
            try
            {
                var setting = await _dbContext.AppSettings.Where(x => x.Id == id).FirstOrDefaultAsync();
                return setting;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> UpdateAppSettings(AppSettings existingAppSetting)
        {
            try
            {
                 _dbContext.AppSettings.Update(existingAppSetting);
                int id  = await _dbContext.SaveChangesAsync();
                return id > 0 ? true : false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
