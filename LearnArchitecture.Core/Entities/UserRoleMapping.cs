using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Core.Entities
{
    public class UserRoleMapping:CommonField
    {
        public int userRoleMappingId { get; set; }
        public int userId { get; set; }
        public int roleId { get; set; }
    }
}
