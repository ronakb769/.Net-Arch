using LearnArchitecture.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Core.Models.ResponseModel
{
    public class LoginResponseModel
    {
        public string Token { get; set;}
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public UserByIdResponseModel userData { get; set; }
    }
}
