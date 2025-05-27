using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LearnArchitecture.Core.Helper.Attributes;
using Microsoft.AspNetCore.Http;

namespace LearnArchitecture.Core.Helper.Attributes
{
    public class HasPermissionAttribute:Attribute,IAsyncActionFilter
    {
            private readonly string _permission;

            public HasPermissionAttribute(string permission)
            {
                _permission = permission;
            }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userId = int.Parse(context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var authService = (IAuthorizationService)context.HttpContext.RequestServices.GetService(typeof(IAuthorizationService));

            if (!await authService.HasPermissionAsync(userId, _permission))
            {
                var response = ResponseBuilder.Fail<object>("User is not authorized to perform this action");
                context.Result = new JsonResult(response)
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return;
            }

            await next();
        }
    }
}
