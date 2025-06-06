using LearnArchitecture.Core.Helper.Constants;
using System.Security.Claims;

namespace LearnArchitecture.API.Common.Middleware
{
    public class JwtClaimsMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtClaimsMiddleware(RequestDelegate next)
        {
            _next = next;
        }


        public async Task InvokeAsync(HttpContext context)
        {
            var identity = context.User.Identity as ClaimsIdentity;
            if (identity != null && identity.IsAuthenticated)
            {
                var userIdStr = identity.FindFirst("userId")?.Value;
                var email = identity.FindFirst("userEmail")?.Value;
                var userName = identity.FindFirst("userName")?.Value;
                var roleIdStr = identity.FindFirst("userRoleId")?.Value;
                var loginHistoryId = identity.FindFirst("loginHistoryId")?.Value;

                var authClaim = new AuthClaim
                {
                    userId = int.TryParse(userIdStr, out var uid) ? uid : 0,
                    email = email ?? string.Empty,
                    userName = userName ?? string.Empty,
                    roleId = int.TryParse(roleIdStr, out var rid) ? rid : 0,
                    loginHistoryId = int.TryParse(loginHistoryId, out var lhid) ? lhid : 0
                };

                // Store in HttpContext.Items
                context.Items["AuthClaim"] = authClaim;
            }

            await _next(context);
        }
    }
}
