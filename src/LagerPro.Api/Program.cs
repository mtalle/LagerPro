using LagerPro.Api.Attributes;
using LagerPro.Api.Authorization;
using LagerPro.Api.Middleware;
using LagerPro.Api.Services;
using LagerPro.Application.DependencyInjection;
using LagerPro.Infrastructure.Data;
using LagerPro.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// CORS — origins read from config, with fallback defaults
var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:3000", "http://localhost:3001" };

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(corsOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers(options =>
{
    options.Filters.Add<TilgangAuthorizationFilter>();
})
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

builder.Services.AddScoped<ITilgangService, TilgangService>();

var app = builder.Build();

// Global exception handler — must be first in the pipeline
app.UseGlobalExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

// Seed database on startup (idempotent)
try
{
    await DbSeeder.SeedAsync(app.Services);
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogWarning(ex, "Database seeding failed — continuing anyway (database may not be available)");
}

app.UseHttpsRedirection();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok", timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss \"UTC\"") }));

app.Run();
