using Marten;
using SimpleCrm.Api.Data.Leads;
using SimpleCrm.Api.Features.Leads.CreateLead;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults (health checks, telemetry, etc.)
builder.AddServiceDefaults();

// Add Marten document store
builder.Services.AddMarten()
.UseLightweightSessions()
.UseNpgsqlDataSource();

// Register application services
builder.Services.AddScoped<ILeadRepository, MartenLeadRepository>();

// Add services to the container
builder.Services.AddOpenApi();

var app = builder.Build();

// Map default endpoints (health, alive)
app.MapDefaultEndpoints();

// Map application endpoints
app.MapCreateLeadEndpoint();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Health check endpoint
app.MapGet("/", () => Results.Ok("SimpleCRM API"));

app.Run();
