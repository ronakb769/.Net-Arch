using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Core.Entities
{
    public class Permissions:CommonField
    {
        public int permissionsId { get; set; }
        public string permissionName { get; set; }
    }
}
