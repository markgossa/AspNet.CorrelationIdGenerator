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

