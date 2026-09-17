using Microsoft.AspNetCore.Mvc;

namespace Backend.Middleware
{
    /// <summary>
    /// Catches unhandled exceptions anywhere downstream in the pipeline and turns
    /// them into a consistent ProblemDetails response instead of a raw stack trace.
    /// Only the request path and exception type/message are logged — never the
    /// request body — consistent with not retaining anything user-submitted.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unhandled exception on {Path}", context.Request.Path);

                var (statusCode, title) = exception switch
                {
                    ArgumentException => (StatusCodes.Status400BadRequest, "Invalid request."),
                    InvalidOperationException => (StatusCodes.Status500InternalServerError, "Server configuration error."),
                    _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
                };

                context.Response.ContentType = "application/problem+json";
                context.Response.StatusCode = statusCode;

                var problemDetails = new ProblemDetails
                {
                    Status = statusCode,
                    Title = title,
                    // Stack-trace-adjacent detail only in Development — never leak
                    // internals to a client in Production.
                    Detail = _environment.IsDevelopment() ? exception.Message : null,
                    Instance = context.Request.Path
                };

                await context.Response.WriteAsJsonAsync(problemDetails);
            }
        }
    }

    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandling(this IApplicationBuilder app)
            => app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}