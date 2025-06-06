using LearnArchitecture.Core.Entities;
using LearnArchitecture.Core.Helper;
using LearnArchitecture.Core.Helper.Constants;
using LearnArchitecture.Core.Helper.Enums;
using LearnArchitecture.Core.Models.RequestModels;
using LearnArchitecture.Core.Models.ResponseModel;
using LearnArchitecture.Data.IRepository;
using LearnArchitecture.Data.Repository;
using LearnArchitecture.Services.IServices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static LearnArchitecture.Core.Helper.Enums.Enums;

namespace LearnArchitecture.Services.Services
{ 
    
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<RoleService> _logger;
        public RoleService(IRoleRepository roleRepository, ILogger<RoleService> logger)
        {
            this._roleRepository = roleRepository;
            this._logger = logger;
        }

        public async Task<ApiResponse<List<Role>>> GetAllRole(AuthClaim authClaim)
        {
            const string methodName = nameof(GetAllRole);
            try
            {
                _logger.LogInformation($"{methodName} called");

                var roles = await _roleRepository.GetAllRole(authClaim);

                if (roles == null || !roles.Any())
                    return ResponseBuilder.Fail<List<Role>>("No roles found",HttpStatusCode.NotFound);

                return ResponseBuilder.Success(roles, "Roles retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName}");
                return ResponseBuilder.Fail<List<Role>>("An error occurred while retrieving roles");
            }
        }

        public async Task<ApiResponse<Role>> GetRoleById(int roleId)
        {
            const string methodName = nameof(GetRoleById);
            try
            {
                _logger.LogInformation($"{methodName} called with roleId: {roleId}");

                if (roleId <= 0)
                    return ResponseBuilder.Fail<Role>("Invalid role ID");

                var role = await _roleRepository.GetRoleById(roleId);

                if (role == null)
                    return ResponseBuilder.Fail<Role>("Role not found", HttpStatusCode.NotFound);

                return ResponseBuilder.Success(role, "Role retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName}");
                return ResponseBuilder.Fail<Role>("An error occurred while retrieving the role");
            }
        }
        public async Task<ApiResponse<bool>> CreateRole(CreateRoleRequestModel roleModel, AuthClaim authClaim)
        {
            const string methodName = nameof(CreateRole);
            try
            {
                _logger.LogInformation($"{methodName} called");

                if (roleModel == null)
                    return ResponseBuilder.Fail<bool>("Invalid role data");

                var userRole = await _roleRepository.GetRoleByAuthClaim(authClaim);
                if (userRole != null && !userRole.roleName.Equals(RoleConstants.SuperAdmin, StringComparison.OrdinalIgnoreCase))
                {
                    return ResponseBuilder.Fail<bool>("Not Authorize for create new Role", HttpStatusCode.Unauthorized);
                }
                Role role = new Role()
                {
                    roleName = roleModel.roleName,
                    description = roleModel.description,
                };

                role = GenericCommonField.UpdateCommonField(role, Enums.EnumOperationType.Add, authClaim);

                var lstRolePermission = roleModel.pemissionids
                  .Select(id =>
                  {
                      var permission = new RolePermission { PermissionsId = id };
                      return GenericCommonField.UpdateCommonField(permission, Enums.EnumOperationType.Add, authClaim);
                  })
                  .ToList();


                bool isCreated = await _roleRepository.CreateRole(role,lstRolePermission);

                if (!isCreated)
                    return ResponseBuilder.Fail<bool>("Failed to create role");

                return ResponseBuilder.Success(true, "Role created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName}");
                return ResponseBuilder.Fail<bool>("An error occurred while creating the role");
            }
        }
        public async Task<ApiResponse<bool>> UpdateRole(Role roleModel, AuthClaim authClaim)
        {
            const string methodName = nameof(UpdateRole);
            try
            {
                _logger.LogInformation($"{methodName} called with roleId: {roleModel?.roleId}");

                if (roleModel == null)
                    return ResponseBuilder.Fail<bool>("Invalid role data");

                var existingRole = await _roleRepository.GetRoleById(roleModel.roleId);
                if (existingRole == null)
                    return ResponseBuilder.Fail<bool>("Role not found", HttpStatusCode.NotFound);

                // Update necessary fields
                existingRole.roleName = roleModel.roleName;
                existingRole.description = roleModel.description;
                existingRole.isActive = roleModel.isActive;
                existingRole.isDelete = roleModel.isDelete;
                existingRole.updatedBy = roleModel.updatedBy;
                existingRole.updatedOn = DateTime.UtcNow;

                bool isUpdated = await _roleRepository.UpdateRole(existingRole);
                if (isUpdated)
                {
                    return ResponseBuilder.Success(true, "Role updated successfully");
                }
                return ResponseBuilder.Fail<bool>("Failed to update role");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName}");
                return ResponseBuilder.Fail<bool>("An error occurred while updating the role");
            }
        }

        public async Task<ApiResponse<bool>> DeleteRole (int roleId, AuthClaim authClaim)
        {
            const string methodName = nameof(DeleteRole);
            try
            {
                _logger.LogInformation($"{methodName} called with roleId: {roleId}");


                var existingRole = await _roleRepository.GetRoleById(roleId);
                if (existingRole == null)
                    return ResponseBuilder.Fail<bool>("Role not found");

                existingRole = GenericCommonField.UpdateCommonField(existingRole, EnumOperationType.Delete, authClaim);


                bool isDeleted = await _roleRepository.DeleteRole(existingRole);

                return isDeleted
                    ? ResponseBuilder.Success(true, "Role Deleted successfully")
                    : ResponseBuilder.Fail<bool>("Failed to Delete user");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in {methodName}");
                return ResponseBuilder.Fail<bool>("An error occurred while deleting the role");
            }
        }
    }
}
