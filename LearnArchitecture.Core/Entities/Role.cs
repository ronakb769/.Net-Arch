using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Core.Entities
{
    public class Role:CommonField
    {
        public int roleId { get; set; }
        public string roleName { get; set; }
        public string description { get; set; }

    }
}
