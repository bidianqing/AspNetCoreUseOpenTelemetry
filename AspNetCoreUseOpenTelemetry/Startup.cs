using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;
using System;
using System.Reflection;

namespace AspNetCoreUseOpenTelemetry
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

            services.AddHttpClient();

            var connection = ConnectionMultiplexer.Connect("localhost:6379");
            services.AddSingleton(sp =>
            {
                return connection;
            });

            // https://devblogs.microsoft.com/dotnet/opentelemetry-net-reaches-v1-0/
            // https://github.com/open-telemetry/opentelemetry-dotnet
            // https://github.com/open-telemetry/opentelemetry-dotnet-contrib
            services.AddOpenTelemetryTracing(builder =>
            {
                // OpenTelemetry
                var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown";

                var resourceBuilder = ResourceBuilder.CreateDefault().AddService("app1", serviceVersion: assemblyVersion, serviceInstanceId: Environment.MachineName);

                builder.SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRedisInstrumentation(connection)
                    .AddMySqlDataInstrumentation();
                    
                string tracingExporter = Configuration["UseTracingExporter"];

                switch (tracingExporter)
                {
                    // https://zipkin.io/
                    case "zipkin":
                        builder.AddZipkinExporter(configure =>
                        {
                            configure.Endpoint = new Uri("http://localhost:9411/api/v2/spans");
                        });
                        break;

                    // https://www.jaegertracing.io/
                    case "jaeger":
                        builder.AddJaegerExporter(configure =>
                        {
                            configure.Protocol = JaegerExportProtocol.UdpCompactThrift;
                        });
                        break;
                }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
