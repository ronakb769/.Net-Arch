using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Core.Helper.Constants
{
    public class AuthClaim
    {
        public int userId {get; set;}
        public string email { get; set; }
        public string userName { get; set; }
        public int roleId { get; set; }
        public int loginHistoryId { get; set; }
    }
}
