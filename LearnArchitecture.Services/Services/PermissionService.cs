using LearnArchitecture.Core.Entities;
using LearnArchitecture.Core.Helper;
using LearnArchitecture.Core.Helper.Constants;
using LearnArchitecture.Data.IRepository;
using LearnArchitecture.Data.Repository;
using LearnArchitecture.Services.IServices;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Services.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly ILogger<PermissionService> _logger;
        public PermissionService(IPermissionRepository permissionRepository, ILogger<PermissionService> logger)
        {
            this._permissionRepository = permissionRepository;
            this._logger = logger;
        }
        public async Task<ApiResponse<List<Permissions>>> GetAllPermission(AuthClaim authClaim)
        {
            const string methodName = nameof(GetAllPermission);
            try
            {
                _logger.LogInformation($"{methodName} from Permission service");

                var permissions = await _permissionRepository.GetAllPermission(authClaim);

                if (permissions == null || !permissions.Any())
                    return ResponseBuilder.Fail<List<Permissions>>("No permissions found");

                return ResponseBuilder.Success(permissions, "Permissions retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName} from permission service");
                return ResponseBuilder.Fail<List<Permissions>>("An error occurred while retrieving permissions");
            }
        }
    }
}
