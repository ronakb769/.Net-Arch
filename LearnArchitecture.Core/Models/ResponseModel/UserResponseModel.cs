using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Core.Models.ResponseModel
{
    public class UserResponseModel
    {
        public int userId {get; set;}
        public string userName {get; set;}
        public string email {get; set;}
        public string phone {get; set;}
        public DateTime createdOn {get; set;}
        public bool isActive {get; set;}
        public bool isDelete {get; set;}
    }
}
