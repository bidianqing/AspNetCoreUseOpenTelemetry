using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;
using System.Diagnostics;
using System.Net.Mime;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddHttpClient();

var connection = ConnectionMultiplexer.Connect("localhost:6379");
builder.Services.AddSingleton(sp =>
{
    return connection;
});

// https://devblogs.microsoft.com/dotnet/opentelemetry-net-reaches-v1-0/
// https://github.com/open-telemetry/opentelemetry-dotnet
// https://github.com/open-telemetry/opentelemetry-dotnet-contrib
builder.Services.AddOpenTelemetry()
    .WithTracing(traceProviderBuilder =>
    {
        traceProviderBuilder.AddSource(DiagnosticsConfig.ActivitySource.Name)
                            .ConfigureResource(resource => resource
                            .AddService(DiagnosticsConfig.ServiceName))
                            .AddAspNetCoreInstrumentation()
                            .AddHttpClientInstrumentation()
                            .AddConsoleExporter();

    });

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthorization();

app.MapControllers();

app.Run();

public static class DiagnosticsConfig
{
    public const string ServiceName = "MyService";
    public static ActivitySource ActivitySource = new ActivitySource(ServiceName);
}