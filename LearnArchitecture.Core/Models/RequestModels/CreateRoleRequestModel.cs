using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Core.Models.RequestModels
{
        public class CreateRoleRequestModel
        {
            public int roleId { get; set; }
            public string roleName { get; set; }
            public string description { get; set; }
            public int[] pemissionids { get; set; }
        }
}
