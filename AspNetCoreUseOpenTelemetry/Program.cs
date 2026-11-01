using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddHttpClient();

// https://devblogs.microsoft.com/dotnet/opentelemetry-net-reaches-v1-0/
// https://github.com/open-telemetry/opentelemetry-dotnet
// https://github.com/open-telemetry/opentelemetry-dotnet-contrib
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(serviceName: builder.Environment.ApplicationName))
    .WithTracing(tracing =>
    {
        tracing.AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddOtlpExporter(options =>
            {
                // docker run -d -p 9411:9411 --name zipkin ghcr.io/openzipkin-contrib/zipkin-otel
                options.Endpoint = new Uri("https://a170d9ba36f79358-1-9411.spca.r.killercoda.com/v1/traces");
                options.Protocol = OtlpExportProtocol.HttpProtobuf;
            });
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapControllers();

app.Run();