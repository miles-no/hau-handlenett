using HandlenettAPI.Services;

namespace HandlenettAPI.Middleware
{
    public class UserInitializationMiddleware
    {
        private readonly RequestDelegate _next;

        public UserInitializationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            // Create a scope for resolving scoped services
            using (var scope = serviceProvider.CreateScope())
            {
                // Resolve the scoped UserInitializationService
                var userInitializationService = scope.ServiceProvider.GetRequiredService<UserInitializationService>();

                // Ensure the user exists using the service
                await userInitializationService.EnsureUserExistsAsync();
            }

            // Call the next middleware in the pipeline
            await _next(context);
        }
    }
}
