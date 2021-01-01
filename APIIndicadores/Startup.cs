using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.ApplicationInsights.DependencyCollector;
using FluentMigrator.Runner;

namespace APIIndicadores
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFluentMigratorCore()
                .ConfigureRunner(cfg => cfg
                    .AddSqlServer()
                    .WithGlobalConnectionString(Configuration.GetConnectionString("BaseIndicadores"))
                    .ScanIn(typeof(Startup).Assembly).For.Migrations()
                )
                .AddLogging(cfg => cfg.AddFluentMigratorConsole());
            
            services.AddControllers();
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "APIIndicadores", Version = "v1" });
            });


            if (!String.IsNullOrWhiteSpace(Configuration["ApplicationInsights:InstrumentationKey"]))
            {
                services.AddApplicationInsightsTelemetry(Configuration);
                services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>(
                    (module, o) =>
                    {
                        module.EnableSqlCommandTextInstrumentation = true;
                    });
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IMigrationRunner migrationRunner)
        {
            migrationRunner.MigrateUp();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
            }
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "APIIndicadores v1"));

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