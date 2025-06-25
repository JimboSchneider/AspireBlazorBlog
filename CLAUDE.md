# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

This is a .NET Aspire distributed application with a Blazor Server front-end. Currently, it's based on the Aspire + Blazor template and doesn't yet implement blog functionality.

## Essential Commands

### Run the Application
```bash
# Start all services (AppHost orchestrates everything)
dotnet run --project AspireBlazorBlog.AppHost

# Access points:
# - Aspire Dashboard: https://localhost:17011
# - Web app and API URLs are dynamically assigned (check dashboard)
```

### Build and Test
```bash
# Build entire solution
dotnet build

# Run tests
dotnet test

# Run specific test project
dotnet test AspireBlazorBlog.Tests
```

## Architecture

### Project Structure
- **AspireBlazorBlog.AppHost** - Aspire orchestrator that manages all services and dependencies
- **AspireBlazorBlog.Web** - Blazor Server application (front-end)
- **AspireBlazorBlog.ApiService** - Backend API service
- **AspireBlazorBlog.ServiceDefaults** - Shared configurations for telemetry, health checks, and service discovery
- **AspireBlazorBlog.Tests** - Integration tests using WebApplicationFactory

### Key Architectural Patterns

1. **Service Discovery** - Services communicate using service names, not URLs:
   ```csharp
   // In Web project, API client uses "apiservice" name
   builder.Services.AddHttpClient<WeatherApiClient>(
       client => client.BaseAddress = new("http://apiservice"));
   ```

2. **Distributed Caching** - Redis is configured for output caching:
   ```csharp
   // In AppHost
   var cache = builder.AddRedis("cache");
   var apiService = builder.AddProject<Projects.AspireBlazorBlog_ApiService>("apiservice");
   builder.AddProject<Projects.AspireBlazorBlog_Web>("webfrontend")
       .WithReference(cache)
       .WithReference(apiService);
   ```

3. **Observability** - All services include OpenTelemetry for metrics, tracing, and logging
4. **Health Checks** - Services expose `/health` and `/alive` endpoints
5. **Resilience** - HTTP clients use standard resilience handlers

### Blazor Component Organization
- `/Components/Layout/` - Layout components (MainLayout.razor, NavMenu.razor)
- `/Components/Pages/` - Page components (Home.razor, Weather.razor, etc.)
- Interactive Server rendering mode with SignalR

## Current State

The application currently implements:
- Basic Blazor template pages (Home, Counter, Weather)
- Weather forecast API endpoint
- Redis caching integration
- Full Aspire infrastructure (telemetry, health checks, service discovery)

## Development Notes

- Uses .NET 9.0 and Aspire 9.3.0
- xUnit v3 for testing
- No blog functionality implemented yet - this is still the base template
- All HTTP clients should use service discovery names, not hardcoded URLs
- When adding new services, register them in the AppHost and add appropriate references