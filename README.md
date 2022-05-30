# AspNet.CorrelationIdGenerator

- [AspNet.CorrelationIdGenerator](#aspnetcorrelationidgenerator)
  - [Overview](#overview)
  - [Features](#features)
  - [Setup](#setup)
  - [Custom logging](#custom-logging)

## Overview
CorrelationId generator with middleware for ASP.NET services. Uses middleware to capture an `X-Correlation-Id` header or generate one if not passed by the client. The `CorrelationId` is also kept for the duration of the request so you can use it for custom logging also.

A full working example can be found [here](https://github.com/markgossa/AspNet.CorrelationIdGenerator/tree/main/example/Example.Api).

## Features
Below is a list of features:

1. Receives the `X-Correlation-Id` header from the client request and generates a `CorrelationId` if one was not sent by the client
2. Returns the `X-Correlation-Id` header in the response
3. Enables access to the `CorrelationId` for the duration of the HTTP request so you can do custom logging
4. No need to add the header manually to each controller. It will be done centrally using ASP.NET Middleware.

## Setup
Follow these steps to set it up:

1. Install NuGet packages into your ASP.NET API project:

```PowerShell
Install-Package AspNet.CorrelationIdGenerator
```

2. Register `ICorrelationIdGenerator` in your IoC container:

```C#
builder.Services.AddCorrelationIdGenerator();
```

3. Add the CorelationIdGenerator middleware:

```C#
app.AddCorrelationIdMiddleware();
```

If you're using the new minimal hosting model in .NET 6, your Program.cs file should look like this:

```C#
using AspNet.CorrelationIdGenerator.ApplicationBuilderExtensions;
using AspNet.CorrelationIdGenerator.ServiceCollectionExtensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCorrelationIdGenerator();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.AddCorrelationIdMiddleware();

app.Run();
```

For .NET 5, your startup.cs should look like this:

```C#
using AspNet.CorrelationIdGenerator.ApplicationBuilderExtensions;
using AspNet.CorrelationIdGenerator.ServiceCollectionExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddCorrelationIdGenerator();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.AddCorrelationIdMiddleware();
        }
    }
}
```

## Custom logging
You can get access to the `CorrelationId` for the HTTP request by injecting the `ICorrelationIdGenerator` into your service and then use the `Get()` method to pull out the `CorrelationId`:

```C#
using AspNet.CorrelationIdGenerator;
using Microsoft.AspNetCore.Mvc;

namespace Example.Api.Controllers;
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] _summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ICorrelationIdGenerator _correlationIdGenerator;
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ICorrelationIdGenerator correlationIdGenerator, 
        ILogger<WeatherForecastController> logger)
    {
        _correlationIdGenerator = correlationIdGenerator;
        _logger = logger;
    }

    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
        _logger.LogInformation("CorrelationId {correlationId}: Processing weather forecast request",
            _correlationIdGenerator.Get());

        return Enumerable
                .Range(1, 5)
                .Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = _summaries[Random.Shared.Next(_summaries.Length)]
                }).ToArray();
    }
}
```