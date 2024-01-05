using Microsoft.Net.Http.Headers;

namespace mvp.tickets.web.Middlewares
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value.Trim('/').ToLower();
            if (string.Equals(context.Request.Method, "get", StringComparison.OrdinalIgnoreCase)
                && !path.StartsWith("api")
                && !path.StartsWith("login")
                && !context.Request.Query.ContainsKey("token")
                && !context.User.Identity.IsAuthenticated)
            {
                context.Response.StatusCode = StatusCodes.Status302Found;
                context.Response.Headers[HeaderNames.Location] = "/login/";

                await Task.CompletedTask;
                return;
            }
            await _next(context);
        }
    }
}
