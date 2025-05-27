using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Core.Models.ResponseModel
{
    public class DashboardResponseModel
    {
        public int ActiveUser { get; set; }
        public int SuperAdminUser   { get; set; }
        public int AdminUser { get; set; }
        public int NormalUser { get; set; }
        public int InActiveUser { get; set; }
    }
}
