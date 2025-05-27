using LearnArchitecture.Core.Entities;
using LearnArchitecture.Core.Helper.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Data.IRepository
{
    public interface IRoleRepository
    {
        public Task<List<Role>> GetAllRole(AuthClaim authClaim);
        public Task<Role> GetRoleById(int roleId);
        public Task<Role> GetRoleByAuthClaim(AuthClaim authClaim);
        public Task<bool> CreateRole(Role role, List<RolePermission> lstRolePermissions);
        public Task<bool> UpdateRole(Role role);
        public Task<bool> DeleteRole(int roleId);
    }
}
