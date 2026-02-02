using Marten;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults (health checks, telemetry, etc.)
builder.AddServiceDefaults();

// Add Marten document store
builder.Services.AddMarten()
.UseLightweightSessions()
.UseNpgsqlDataSource();

// Add services to the container
builder.Services.AddOpenApi();

var app = builder.Build();

// Map default endpoints (health, alive)
app.MapDefaultEndpoints();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Health check endpoint
app.MapGet("/", () => Results.Ok("SimpleCRM API"));

app.Run();
