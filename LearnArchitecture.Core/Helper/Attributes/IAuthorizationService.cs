using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Core.Helper.Attributes
{
    public interface IAuthorizationService
    {
        public Task<bool> HasPermissionAsync(int userId, string permissionName);
    }
}
