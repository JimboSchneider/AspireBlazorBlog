var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

// Add SQL Server for local development
var sqlServer = builder.AddSqlServer("sql")
    .WithDataVolume();
var sqlDatabase = sqlServer.AddDatabase("blogdb");

// Add Cosmos DB for production (will be configured in Azure)
// Note: Cosmos DB emulator support is limited on macOS/Linux
var cosmosDb = builder.AddAzureCosmosDB("cosmos");
var cosmosDatabase = cosmosDb.AddCosmosDatabase("blogdb");

var apiService = builder.AddProject<Projects.AspireBlazorBlog_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithReference(sqlDatabase)
    .WithReference(cosmosDatabase);

builder.AddProject<Projects.AspireBlazorBlog_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();