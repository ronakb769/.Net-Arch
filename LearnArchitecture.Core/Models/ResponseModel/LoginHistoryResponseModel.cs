using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Core.Models.ResponseModel
{
    public class LoginHistoryResponseModel
    {
        public int loginHistoryId { get; set; }
        public int userId { get; set; }
        public string userName { get; set; }
        public DateTime loginTime { get; set; }
        public DateTime? logoutTime { get; set; }
        public DateOnly loginDate { get; set; }
        public string ipAddress { get; set; }
    }
}
