using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace demo1
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
            services.AddApplicationInsightsTelemetry();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "demo1", Version = "v1" });
            });

            
            // services.AddOpenTelemetryTracing((builder) => builder
            //             .AddSource("Demo.DemoClient")
            //             .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Demo.DemoClient").AddTelemetrySdk())
            //             .AddSqlClientInstrumentation(s =>
            //             {
            //                 s.SetDbStatementForStoredProcedure = true;
            //                 s.SetDbStatementForText = true;
            //                 s.RecordException = true;
            //             })
            //             .AddAspNetCoreInstrumentation(options =>
            //             {
            //                 options.RecordException = true;
            //                 options.Filter = (req) => !req.Request.Path.ToUriComponent().Contains("swagger", StringComparison.OrdinalIgnoreCase);
            //             })
            //             .AddHttpClientInstrumentation((options) => options.RecordException = true)
            //             .AddConsoleExporter()
            //             .AddAzureMonitorTraceExporter(o =>
            //             {
            //                 o.ConnectionString = "InstrumentationKey=27b120b9-5e3a-4b6d-ae4e-7eb14fca678a";
            //             }));


            

            // services.AddOpenTelemetryTracing((builder) => builder
            // .AddSource("Demo.DemoClient")
            // .AddAzureMonitorTraceExporter(o =>
            // {
            //     o.ConnectionString = "InstrumentationKey=27b120b9-5e3a-4b6d-ae4e-7eb14fca678a";
            // }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "demo1 v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
