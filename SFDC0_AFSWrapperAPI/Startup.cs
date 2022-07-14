using WrapperAPI.Factories;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.StaticFiles;
using System.Diagnostics.CodeAnalysis;

namespace WrapperAPI
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            const string apiTitle = "Wrapper to AFS API";
            const string apiSummary = "RESTful API Wrapper for connecting to AFS via ConnectWare";

            services.AddAppSettingsConfiguration(Configuration)
                .AddAuthConfiguration()
                .AddCaching(Configuration)
                .AddControllerConfiguration()
                .AddFactories()
                .AddFluentValidators()
                .AddForwardedHeaders()
                .AddHttpClients(Configuration)
                .AddHttpContextAccessors()
                .AddLogging()
                .AddMicrosoftHealthChecks(Configuration)
                .AddRepositoryServices()
                .AddSwaggerSupportWithVersioning(apiTitle, apiSummary);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UsePathBase("/api");

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            if (!env.IsDevelopment())
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions()
                {
                    AllowCachingResponses = false,
                    ResponseWriter = HealthCheckResponseFactory.WriteHealthCheckResponse
                });
            });

            app.UseSwagger();

            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".yaml"] = "application/yaml";

            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider
            });

        }
    }
}
