using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Backend.Configuration;
using Backend.Services;
using Backend.Validators;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Load first, before anything else reads from configuration — rules.json is
// a standalone file (not merged into appsettings.json), so it must be added
// as an explicit source before any GetSection() call depends on it.
builder.Configuration.AddJsonFile("Configurations/rules.json", optional: false, reloadOnChange: true);

// ---------------------------------------------------------------------------
// Services
// ---------------------------------------------------------------------------

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "RepairReplaceAPI",
        Version = "v1",
        Description = "Stateless repair-vs-replace decision API. No request or " +
                      "result data is persisted anywhere — everything lives only " +
                      "for the duration of a single call."
    });
});

// Binds rules.json's "CalculationOptions" section into CalculationOptions.
// This happens once, at startup.
builder.Services.Configure<CalculationOptions>(
    builder.Configuration.GetSection("CalculationOptions"));

// Both services hold no per-request mutable state, so Singleton is correct and
// avoids re-constructing them (and re-validating config, for ProductService) on
// every request. If either ever gains request-scoped state, switch to Scoped.
builder.Services.AddSingleton<IProductService, ProductService>();
builder.Services.AddSingleton<ICalculationService, CalculationService>();

builder.Services.AddValidatorsFromAssemblyContaining<RepairReplaceRequestValidator>();

// Frontend runs on a different origin — adjust the allowed origin(s) for your
// actual deployment. Kept permissive here for local development.
const string frontendCorsPolicy = "FrontendPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(frontendCorsPolicy, policy =>
    {
        policy
            .WithOrigins(
                builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
                ?? new[] { "http://localhost:5173" }) // Vite dev server default
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// ---------------------------------------------------------------------------
// Pipeline
// ---------------------------------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Global exception handler: turns any unhandled exception into a consistent
// ProblemDetails response instead of leaking a stack trace to the client.
// Nothing about the failing request body is logged here — only the path and
// exception type/message, consistent with not retaining user-submitted data.
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
        var exception = exceptionFeature?.Error;

        var logger = context.RequestServices
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("GlobalExceptionHandler");

        logger.LogError(exception, "Unhandled exception on {Path}", context.Request.Path);

        context.Response.ContentType = "application/problem+json";

        var (statusCode, title) = exception switch
        {
            ArgumentException => (StatusCodes.Status400BadRequest, "Invalid request."),
            InvalidOperationException => (StatusCodes.Status500InternalServerError, "Server configuration error."),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };

        context.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = app.Environment.IsDevelopment() ? exception?.Message : null,
            Instance = context.Request.Path
        };

        await context.Response.WriteAsJsonAsync(problemDetails);
    });
});

app.UseHttpsRedirection();

app.UseCors(frontendCorsPolicy);

app.UseAuthorization();

app.MapControllers();

app.Run();