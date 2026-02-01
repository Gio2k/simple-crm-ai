var builder = DistributedApplication.CreateBuilder(args);

// Infrastructure
var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin();

var crmDb = postgres.AddDatabase("crmdb");

var keycloak = builder.AddKeycloak("keycloak")
    .WithDataVolume();

// API
var api = builder.AddProject<Projects.SimpleCrm_Api>("api")
    .WithReference(crmDb)
    .WaitFor(crmDb)
    .WithReference(keycloak)
    .WaitFor(keycloak);

// Frontend
builder.AddNpmApp("web", "../SimpleCrm.Web")
    .WithReference(api)
    .WaitFor(api)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
