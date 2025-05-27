using LearnArchitecture.Core.Helper.Attributes;
using LearnArchitecture.Data.IRepository;
using LearnArchitecture.Services.IServices;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Services.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IPermissionRepository _permissionRepository;

        public AuthorizationService(IMemoryCache memoryCache, IPermissionRepository permissionRepository)
        {
            _memoryCache = memoryCache;
            _permissionRepository = permissionRepository;
        }
        //public async Task<bool> HasPermissionAsync(int userId, string permissionName)
        //{
        //    try
        //    {
        //        var userRoles = await _permissionRepository.GetRolesByUserId(userId);
        //        if (userRoles == null || !userRoles.Any())
        //            return false;

        //        var permission = await _permissionRepository.GetByNameAsync(permissionName);
        //        if (permission == null)
        //            return false;

        //        foreach (var role in userRoles)
        //        {
        //            var hasPermission = await _permissionRepository.HasPermissionAsync(role.roleId, permission.permissionsId);
        //            if (hasPermission)
        //                return true;
        //        }

        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }

        //}
        public async Task<bool> HasPermissionAsync(int userId, string permissionName)
        {
            try
            {
                string cacheKey = $"permissions:{userId}";

                if (!_memoryCache.TryGetValue(cacheKey, out List<string>? permissionList))
                {
                    // Cache miss: load from DB
                    var userRoles = await _permissionRepository.GetRolesByUserId(userId);
                    if (userRoles == null || !userRoles.Any())
                        return false;

                    var allPermissions = new List<string>();

                    foreach (var role in userRoles)
                    {
                        var permissions = await _permissionRepository.GetPermissionsByRoleId(role.roleId);
                        allPermissions.AddRange(permissions.Select(p => p));
                    }

                    permissionList = allPermissions.Distinct().ToList();

                    // Cache for 2 hours
                    _memoryCache.Set(cacheKey, permissionList, TimeSpan.FromHours(2));
                }

                if (permissionList.Contains(permissionName))
                    return true;

                // Optional: check database fallback (e.g. if permissions changed and not yet cached)
                var permission = await _permissionRepository.GetByNameAsync(permissionName);
                if (permission == null)
                    return false;

                var roles = await _permissionRepository.GetRolesByUserId(userId);
                foreach (var role in roles)
                {
                    var hasPermission = await _permissionRepository.HasPermissionAsync(role.roleId, permission.permissionsId);
                    if (hasPermission)
                    {
                        // Update the in-memory cache
                        permissionList.Add(permissionName);
                        permissionList = permissionList.Distinct().ToList();
                        _memoryCache.Set(cacheKey, permissionList, TimeSpan.FromHours(2));
                        return true;
                    }
                }

                return false;

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
