using FrontDoorAndCaching.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using FrontDoorAndCaching.Extensions;
using Microsoft.FeatureManagement;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureDefaultAppConfiguration();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAzureAppConfiguration();

builder.Services.AddHealthChecks()
    .AddCheck<FeatureEnabledHealthCheck>("IsEnabled");

builder.Services.AddFeatureManagement();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = HealthCheckHelper.WriteResponse
});

app.Run();
