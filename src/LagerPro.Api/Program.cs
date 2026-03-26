using LagerPro.Application.DependencyInjection;
using LagerPro.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplication();
builder.Services.AddControllers();

var useDatabase = builder.Configuration.GetValue<bool>("UseDatabase", false);
if (useDatabase)
{
    builder.Services.AddInfrastructure(builder.Configuration);
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok", database = useDatabase ? "enabled" : "disabled" }));

app.Run();
