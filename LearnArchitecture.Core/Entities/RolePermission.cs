using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Core.Entities
{
    public class RolePermission:CommonField
    {
        public int RolePermissionId { get; set; }
        public int RoleId { get; set; }
        public int PermissionsId { get; set; }
    }
}
