using LearnArchitecture.Core.Entities;
using LearnArchitecture.Core.Models.RequestModels;
using LearnArchitecture.Core.Models.ResponseModel;
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

        public async Task<List<AppSettings>> GetCaptchaStatus()
        {
            try
            {
                var settings = await _dbContext.AppSettings.ToListAsync();
                return settings;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<PagingResponseModel<AppSettings>> GetSettingValue(AppSettingPagingRequestModel request)
        {
            try
            {
                var query = _dbContext.AppSettings.AsQueryable();

                // Search filter (on Key or Value)
                if (!string.IsNullOrWhiteSpace(request.searchText))
                {
                    string search = request.searchText.ToLower();
                    query = query.Where(x =>
                        x.Key.ToLower().Contains(search) ||
                        x.Value.ToLower().Contains(search)||
                        x.description.ToLower().Contains(search));
                }

                // Sorting 
                if (request.SortDirection?.ToLower() == "desc")
                {
                    query = query.OrderByDescending(x => x.Key);
                }
                else
                {
                    query = query.OrderBy(x => x.Key);
                }

                // Total count before paging
                int totalRecords = await query.CountAsync();


                // Paging
                var settings = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                return new PagingResponseModel<AppSettings>
                {
                    Data = settings,
                    TotalRecords = totalRecords
                };
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
