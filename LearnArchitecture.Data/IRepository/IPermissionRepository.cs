using LearnArchitecture.Core.Entities;
using LearnArchitecture.Core.Helper.Constants;
using LearnArchitecture.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Data.IRepository
{
    public interface IPermissionRepository
    {
        public Task<List<Permissions>> GetAllPermission(AuthClaim authClaim);
        public  Task<IEnumerable<Role>> GetRolesByUserId(int userId);
        public  Task<Permissions> GetByNameAsync(string permissionName);
        public  Task<bool> HasPermissionAsync(int roleId, int permissionId);
        public Task<List<string>> GetPermissionsByRoleId(int roleId);

    }
}
