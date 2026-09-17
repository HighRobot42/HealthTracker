var builder = DistributedApplication.CreateBuilder(args);

// -- Infrastructure services ----------------------------------------------------
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .AddDatabase("HealthTrackerDb");

var redis = builder.AddRedis("redis");

// -- GraphQL API ----------------------------------------------------------------
var graphql = builder.AddProject<Projects.HealthTracker_Api_GraphQl>("graphql")
    .WithReference(postgres, connectionName: "DefaultConnection")
    .WithReference(redis, connectionName: "Redis");
 
var migration = builder.AddProject<Projects.HealthTracker_DatabaseMigration>("migration")
    .WithReference(postgres, connectionName: "DefaultConnection");

builder.AddNpmApp("mobileapp", "../../MobileApp", "web")
    .WithReference(graphql)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints();

builder.Build().Run();
