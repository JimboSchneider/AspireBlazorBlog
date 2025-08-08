using AspireBlazorBlog.ApiService.Data;
using AspireBlazorBlog.ApiService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register application services
builder.Services.AddScoped<IBlogPostService, BlogPostService>();

// Configure database based on environment
if (builder.Environment.IsProduction() || builder.Configuration.GetConnectionString("cosmos") != null)
{
    // Use Cosmos DB in production or when connection string is available
    builder.Services.AddDbContext<BlogDbContext>(options =>
    {
        var cosmosConnectionString = builder.Configuration.GetConnectionString("cosmos");
        if (!string.IsNullOrEmpty(cosmosConnectionString))
        {
            // Parse the connection string to extract endpoint and key
            var parts = cosmosConnectionString.Split(';')
                .Select(p => p.Split('='))
                .Where(p => p.Length == 2)
                .ToDictionary(p => p[0], p => p[1]);
            
            if (parts.TryGetValue("AccountEndpoint", out var endpoint) && 
                parts.TryGetValue("AccountKey", out var key))
            {
                options.UseCosmos(endpoint, key, "blogdb");
            }
        }
    });
}
else
{
    // Use SQL Server for local development
    builder.AddSqlServerDbContext<BlogDbContext>("blogdb");
}

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();
app.MapDefaultEndpoints();

// Ensure database is created and migrations applied (development only)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

app.Run();