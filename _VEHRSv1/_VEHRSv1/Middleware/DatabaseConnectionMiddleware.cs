using _VEHRSv1.Helper;

namespace _VEHRSv1.Middleware
{
    public class DatabaseConnectionMiddleware
    {
        private readonly RequestDelegate _next;

        public DatabaseConnectionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DatabaseConnectionException)
            {
                context.Response.Redirect("/Error/DatabaseConnection");
            }
        }
    }
}
