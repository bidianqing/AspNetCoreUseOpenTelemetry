using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddHttpClient();

var connection = ConnectionMultiplexer.Connect("localhost:25689");
builder.Services.AddSingleton(sp =>
{
    return connection;
});

// https://devblogs.microsoft.com/dotnet/opentelemetry-net-reaches-v1-0/
// https://github.com/open-telemetry/opentelemetry-dotnet
// https://github.com/open-telemetry/opentelemetry-dotnet-contrib
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(serviceName: builder.Environment.ApplicationName))
    .WithTracing(tracing =>
    {
        tracing.AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter()
            .AddOtlpExporter(options =>
            {
                // docker run --rm -d -p 9411:9411 --name zipkin ghcr.io/openzipkin-contrib/zipkin-otel
                options.Endpoint = new Uri("https://7005286a83df530c-1-9411.spca.r.killercoda.com/v1/traces");
                options.Protocol = OtlpExportProtocol.HttpProtobuf;
            });
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapControllers();

app.Run();