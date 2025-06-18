using Azure.Core;
using LearnArchitecture.Core.Entities;
using LearnArchitecture.Core.Helper.Constants;
using LearnArchitecture.Core.Models.RequestModels;
using LearnArchitecture.Core.Models.ResponseModel;
using LearnArchitecture.Data.Context;
using LearnArchitecture.Data.IRepository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LearnArchitecture.Data.Repository
{
    public class RoleRepository:IRoleRepository
    {
        private readonly LearnArchitectureDbContext _dbContext;
        private readonly ILogger<RoleRepository> _logger;
        public RoleRepository(LearnArchitectureDbContext dbContext, ILogger<RoleRepository> logger)
        {
            this._dbContext = dbContext;
            this._logger = logger;
        }

        public async Task<PagingResponseModel<Role>> GetAllRole(RolePagingRequestModel rolePagingRequestModel,AuthClaim authClaim)
        {
            const string methodName = nameof(GetAllRole);
            try
            {
                _logger.LogInformation($"{methodName} called from role Repository");

                var roles = _dbContext.Role.Where(x => x.isActive && !x.isDelete);

                var userRole = await GetRoleByAuthClaim(authClaim);

				// Admin Role
				#region If the user is not SuperAdmin, hide SuperAdmin and Admin roles
				if (userRole == null || !userRole.roleName.Equals(RoleConstants.SuperAdmin, StringComparison.OrdinalIgnoreCase))
                {
                    roles = roles.Where(x =>
                           x.roleName.ToLower() != RoleConstants.SuperAdmin.ToLower() &&
                           x.roleName.ToLower() != RoleConstants.Admin.ToLower());
                }
				#endregion
				#region Filtering
				if (!string.IsNullOrWhiteSpace(rolePagingRequestModel.searchText))
                {
                    string lowerSearch = rolePagingRequestModel.searchText.ToLower();
					roles = roles.Where(x =>
                        x.roleName.ToLower().Contains(lowerSearch) ||
                        x.description.ToLower().Contains(lowerSearch));
                }
				#endregion
				int totalRecords = roles.Count();

				#region Sorting
				roles = rolePagingRequestModel.SortColumn?.ToLower() switch
				{
					"rolename" => rolePagingRequestModel.SortDirection == "desc"
						? roles.OrderByDescending(x => x.roleName)
						: roles.OrderBy(x => x.roleName),

					"description" => rolePagingRequestModel.SortDirection == "desc"
						? roles.OrderByDescending(x => x.description)
						: roles.OrderBy(x => x.description),

					"createddate" => rolePagingRequestModel.SortDirection == "desc"
						? roles.OrderByDescending(x => x.createdOn)
						: roles.OrderBy(x => x.createdOn),

					_ => roles.OrderByDescending(x => x.createdOn) // Default case
				};
				#endregion

				#region Paging
				var data = roles
					.Skip((rolePagingRequestModel.PageNumber - 1) * rolePagingRequestModel.PageSize)
					.Take(rolePagingRequestModel.PageSize).ToList();
				#endregion
				return new PagingResponseModel<Role>
				{
					Data = data,
					TotalRecords = totalRecords
				};
			}
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName} from role Repository");
                throw;
            }
        }

        public async Task<Role> GetRoleByAuthClaim(AuthClaim authClaim)
        {
            const string methodName = nameof(GetRoleByAuthClaim);
            try
            {
                _logger.LogInformation($"{methodName} called from role Repository");
                var userRole = await _dbContext.Role.Where(x => x.roleId == authClaim.roleId && x.isActive && !x.isDelete).FirstOrDefaultAsync();
                return userRole;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName} from role Repository");
                throw;
            }
        }

        public async Task<Role> GetRoleById(int roleId)
        {
            const string methodName = nameof(GetRoleById);
            try
            {
                _logger.LogInformation($"{methodName} called with roleId: {roleId} from role Repository");
                var data = await _dbContext.Role
                       .Where(x => x.roleId == roleId && x.isDelete == false && x.isActive)
                       .FirstOrDefaultAsync();
                return data;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, $"Exception in {methodName} for roleId: {roleId} from role Repository");
                throw;
            }
            

        }
        //public async Task<bool> CreateRole(Role role, List<RolePermission> lstRolePermission)
        //{
        //    const string methodName = nameof(CreateRole);
        //    using var transaction = await _dbContext.Database.BeginTransactionAsync();

        //    try
        //    {
        //        _logger.LogInformation($"{methodName} called from role repository");

        //        // Add Role
        //        await _dbContext.Role.AddAsync(role);
        //        await _dbContext.SaveChangesAsync(); // This generates Role.Id

        //        // Set RoleId and bulk add RolePermissions using LINQ
        //        lstRolePermission.ForEach(p => p.RoleId = role.roleId);
        //        await _dbContext.RolePermission.AddRangeAsync(lstRolePermission);
        //        await _dbContext.SaveChangesAsync();

        //        // Commit transaction
        //        await transaction.CommitAsync();
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Rollback transaction
        //        await transaction.RollbackAsync();
        //        _logger.LogError(ex, $"Exception in {methodName} from role repository");
        //        throw;
        //    }
        //}

        //public Task<bool> CreateRole(Role role)
        //{
        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}


        public async Task<bool> UpdateRole(Role role)
        {
            const string methodName = nameof(UpdateRole);
            try
            {
                _logger.LogInformation($"{methodName} called from role repository");
                _dbContext.Role.Update(role);
                int id =  _dbContext.SaveChanges();
                return id > 0 ? true : false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName} from role repository");
                throw;
            }
        }
        public async Task<bool> DeleteRole(Role role)
        {
            const string methodName = nameof(DeleteRole);
            try
            {
                _logger.LogInformation($"{methodName} called for userId: {role.roleId}");

                var result = new ResultModel();
                SqlParameter roleId = new("@roleId", role.roleId);
                SqlParameter isActive = new("@isActive", role.isActive);
                SqlParameter isDelete = new("@isDelete", role.isDelete);
                SqlParameter updatedOn = new("@updatedOn", role.updatedOn);
                SqlParameter updatedBy = new("@updatedBy", role.updatedBy);

                result = _dbContext.resultModels
                                        .FromSqlRaw("EXEC Sp_DeleteRole @roleId,@isActive,@isDelete,@updatedOn,@updatedBy",
                                            roleId, isActive, isDelete, updatedOn, updatedBy)
                                        .AsEnumerable() // Switch to in-memory operations
                                        .FirstOrDefault();

                if (result != null && result.flag)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName} from role repository");
                throw;
            }
        }

        public async Task<bool> CreateRole(Role role)
        {
            const string methodName = nameof(CreateRole);
            try
            {
                _logger.LogInformation($"{methodName} called from role repository");

                //        // Add Role
                await _dbContext.Role.AddAsync(role);
                int id  =  await _dbContext.SaveChangesAsync();
                return id>0?true:false;
            }
            catch (Exception ex) 
            {
                throw;
            }
        }
    }
}
