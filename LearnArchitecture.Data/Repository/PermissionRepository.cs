using LearnArchitecture.Core.Entities;
using LearnArchitecture.Core.Helper.Constants;
using LearnArchitecture.Data.Context;
using LearnArchitecture.Data.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Data.Repository
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly LearnArchitectureDbContext _dbContext;
        private readonly ILogger<PermissionRepository> _logger;
        public PermissionRepository(LearnArchitectureDbContext dbContext, ILogger<PermissionRepository> _logger)
        {
            this._dbContext = dbContext;  
            this._logger = _logger; 
        }

        public async Task<List<Permissions>> GetAllPermission(AuthClaim authClaim)
        {
            const string methodName = nameof(GetAllPermission);
            try
            {
                _logger.LogInformation($"{methodName} called from permission Repository");

                var roles = _dbContext.Permissions
                            .Where(x => x.isActive && !x.isDelete);

                return await roles.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName} from role Repository");
                throw;
            }
        }
        public async Task<Permissions> GetByNameAsync(string permissionName)
        {
            const string methodName = nameof(GetByNameAsync);
            try
            {
                _logger.LogInformation($"{methodName} called from permission repository");
                return await _dbContext.Permissions.FirstOrDefaultAsync(p => p.permissionName == permissionName && p.isActive && !p.isDelete);

            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, $"Exception in {methodName} from user Repository");
                throw;
            }
        }

        public async Task<IEnumerable<Role>> GetRolesByUserId(int userId)
        {
            const string methodName = nameof(GetRolesByUserId);
            try
            {
                _logger.LogInformation($"{methodName} called from permission repository");
                var roles = await(from urm in _dbContext.UserRoleMapping
                                  join r in _dbContext.Role on urm.roleId equals r.roleId
                                  where urm.userId == userId && urm.isActive && !urm.isDelete
                                        && r.isActive && !r.isDelete
                                  select r).ToListAsync();

                return roles;
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, $"Exception in {methodName} from user Repository");
                throw;
            }
        }

        public async Task<bool> HasPermissionAsync(int roleId, int permissionId)
        {
            const string methodName = nameof(HasPermissionAsync);
            try
            {
                return await _dbContext.RolePermission.AnyAsync(rp => rp.RoleId == roleId
                                && rp.PermissionsId == permissionId
                                && rp.isActive && !rp.isDelete);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, $"Exception in {methodName} from user Repository");
                throw;
            }
        }

        public async Task<List<string>> GetPermissionsByRoleId(int roleId)
        {
            return await _dbContext.RolePermission
                .Where(rp => rp.RoleId == roleId && rp.isActive && !rp.isDelete)
                .Join(_dbContext.Permissions,
                      rp => rp.PermissionsId,
                      p => p.permissionsId,
                      (rp, p) => new { p.permissionName, p.isActive, p.isDelete })
                .Where(x => x.isActive && !x.isDelete)
                .Select(x => x.permissionName)
                .Distinct()
                .ToListAsync();
        }
    }
}
