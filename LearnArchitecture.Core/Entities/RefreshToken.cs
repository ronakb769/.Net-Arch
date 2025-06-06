using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Core.Entities
{
    public class RefreshToken
    {
        public int refreshTokenId{ get; set; }
        public int userId { get; set; }
        public string refreshToken { get; set; }
        public DateTime expiryTime { get; set; }
    }
}
