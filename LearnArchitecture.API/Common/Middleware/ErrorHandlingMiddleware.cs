using LearnArchitecture.Core.Entities;
using LearnArchitecture.Data.IRepository;
using Serilog;

namespace LearnArchitecture.API.Common.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IErrorLogRepository logRepo)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var userId = context.User?.FindFirst("userId")?.Value ?? "Anonymous";
                var log = new ErrorLog
                {
                    UserId = userId,
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    Source = ex.Source,
                    Path = context.Request.Path,
                    Timestamp = DateTime.UtcNow
                };

                await logRepo.LogAsync(log);

                // Optional: Log to file via Serilog
                Log.Error(ex, $"Exception for user {userId} at {context.Request.Path}");

                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("An unexpected error occurred.");
            }
        }
    }
}
