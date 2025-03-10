﻿using System.Text.Json.Serialization;
using CorrelationId;
using Lamar;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Core.Backend.Api.Configurations.Extensions;
using Core.Backend.Api.Middleware.ExceptionHandling;
using CorrelationId.DependencyInjection;
using CorrelationId.HttpClient;
using Core.Backend.Api.HealthChecks;
using Core.Backend.Api.Middleware.Logging;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Routing;

namespace Core.Backend.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureContainer(ServiceRegistry services)
        {
            services.AddDefaultCorrelationId();
            services.AddHttpContextAccessor();
            services.AddMvc();
            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.AssumeDefaultVersionWhenUnspecified = true;
            })
            .AddMvc()
            .AddApiExplorer(
            options =>
            {
                options.GroupNameFormat = "'v'VVV";
            });
            services.AddOptions();
            services.AddHttpClient(string.Empty)
                .AddCorrelationIdForwarding();

            services.AddSwagger();
            services.AddHealthChecks().AddCheck<ReadinessCheck>("Core.Backend readiness", tags: new[] {"readiness"});
            services.AddCustomizedLogging();
            services.AddDependencyInjection(Configuration);

            services.AddHealthChecks();
            services.AddControllers()
                .AddJsonOptions(options =>
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {
            app.UseCorrelationId();
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseSwaggerDocumentation(provider);
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHealthChecks("/health");
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
