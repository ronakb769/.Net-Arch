using LearnArchitecture.Core.Helper.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LearnArchitecture.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        private AuthClaim jwtAuthClaim;
        
        public AuthClaim JWTAuthClaim
        {
            get
            {
                if (jwtAuthClaim != null)
                    return jwtAuthClaim;

                var identity = HttpContext?.User?.Identity as ClaimsIdentity;
                if (identity == null)
                    return null;

                var userIdStr = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                ?? identity.FindFirst("sub")?.Value;
                var email = identity.FindFirst("userEmail")?.Value;
                var userName = identity.FindFirst("userName")?.Value;
                var roleIdStr = identity.FindFirst("userRoleId")?.Value;

                jwtAuthClaim = new AuthClaim
                {
                    userId = int.TryParse(userIdStr, out var uid) ? uid : 0,
                    email = email ?? string.Empty,
                    userName = userName ?? string.Empty,
                    roleId = int.TryParse(roleIdStr, out var rid) ? rid : 0
                };

                return jwtAuthClaim;
            }
        }
    }
}
