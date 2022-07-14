using WrapperAPI.Attributes;
using WrapperAPI.Classes;
using WrapperAPI.Clients;
using WrapperAPI.Models;
using WrapperAPI.Models.Configuration;
using WrapperAPI.Models.Interfaces;
using WrapperAPI.Policies;
using WrapperAPI.Repositories;
using WrapperAPI.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Diagnostics.CodeAnalysis;

namespace WrapperAPI
{
    [ExcludeFromCodeCoverage]
    internal static class StartupExtensions
    {
        internal static IServiceCollection AddAppSettingsConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddConfigurationWithValidation<HealthChecksCommonConfiguration,
                HealthChecksCommonConfigurationValidator>(configuration.GetSection("Diagnostices:HealthChecks"));

            services.AddConfigurationWithValidation<HealthChecksConfiguration,
                HealthChecksConfigurationValidator>(configuration.GetSection("Diagnostics:HealthChecks"));

            services.AddConfigurationWithValidation<CachingConfiguration,
                CachingConfigurationValidator>(configuration.GetSection("Caching"));

            services.AddConfigurationWithValidation<NLogConfiguration,
                NLogConfigurationValidator>(configuration.GetSection("NLog"));

            services.AddConfigurationWithValidation<SiteConfiguration,
                SiteConfigurationValidator>(configuration.GetSection("Site"));

            return services;
        }

        internal static IServiceCollection AddAuthConfiguration(this IServiceCollection services)
        {
            services.AddAuthentication();
            services.AddAuthorization();

            return services;
        }
        
        internal static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();

            return services;
        }

        internal static IServiceCollection AddControllerConfiguration(this IServiceCollection services)
        {
            services.AddControllers(mvcOptions => 
            {
                mvcOptions.Filters.Add(typeof(ModelStateValidationAttribute));
                mvcOptions.Filters.Add(typeof(UncaughtExceptionHandlerAttribute));

                mvcOptions.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status200OK));
                mvcOptions.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status201Created));
                mvcOptions.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status202Accepted));
                mvcOptions.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status204NoContent));
                mvcOptions.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));
                mvcOptions.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status401Unauthorized));
                mvcOptions.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status403Forbidden));
                mvcOptions.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status404NotFound));
                mvcOptions.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status422UnprocessableEntity));
                mvcOptions.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));

                var newtonSoftJsonOutputFormatter = mvcOptions.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>().FirstOrDefault();
                var systemTextJsonOutputFormatter = mvcOptions.OutputFormatters.OfType<SystemTextJsonOutputFormatter>().FirstOrDefault();

                if (newtonSoftJsonOutputFormatter != null && newtonSoftJsonOutputFormatter.SupportedMediaTypes.Contains("text/json"))
                {
                    newtonSoftJsonOutputFormatter.SupportedMediaTypes.Remove("text/json");
                }

                if (systemTextJsonOutputFormatter != null && systemTextJsonOutputFormatter.SupportedMediaTypes.Contains("text/json"))
                {
                    systemTextJsonOutputFormatter.SupportedMediaTypes.Remove("text/json");
                }

            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            })
            .AddFluentValidation();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            return services;
        }

        internal static IServiceCollection AddFactories(this IServiceCollection services)
        {
            services.TryAddSingleton<ICancellationTokenSourceFactory, CancellationTokenSourceFactory>();
            return services;
        }

        internal static IServiceCollection AddFluentValidators(this IServiceCollection services)
        {
            services.AddFluentValidator<Request, RequestValidator>();

            return services;
        }

        internal static IServiceCollection AddForwardedHeaders(this IServiceCollection services)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.All;
            });

            return services;
        }

        internal static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.TryAddTransient<CompressionPolicy>();
            services.TryAddTransient<CorrelationHeaderPolicy>();
            services.TryAddTransient<ExceptionHandlingPolicy>();

            services.AddHttpClient<IHealthCheckClient, HealthCheckClient>()
                .AddHttpMessageHandler<CompressionPolicy>()
                .AddHttpMessageHandler<CorrelationHeaderPolicy>()
                .AddHttpMessageHandler<ExceptionHandlingPolicy>()
                .ConfigurePrimaryHttpMessageHandler(handler =>
                new HttpClientHandler()
                {
                    AutomaticDecompression = System.Net.DecompressionMethods.GZip
                });

            services.AddHttpClient<ISoapClient, SoapClient>()
                .AddHttpMessageHandler<CompressionPolicy>()
                .AddHttpMessageHandler<CorrelationHeaderPolicy>()
                .AddHttpMessageHandler<ExceptionHandlingPolicy>()
                .ConfigurePrimaryHttpMessageHandler(handler =>
                new HttpClientHandler()
                {
                    AutomaticDecompression = System.Net.DecompressionMethods.GZip
                });

            return services;
        }

        internal static IServiceCollection AddHttpContextAccessors(this IServiceCollection services)
        {

            return services;
        }

        internal static IServiceCollection AddMicrosoftHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            var afsEndpointUri = configuration.GetSection("HealthChecks").Get<HealthChecksConfiguration>().AfsEndpointUri;

            services.AddHealthChecks()
                .AddUrlGroup(uri: new Uri(afsEndpointUri), "AFS Soap API Service");

            return services;
        }

        internal static IServiceCollection AddRepositoryServices(this IServiceCollection services)
        {
            services.AddTransient<IService, Service>();
            services.AddTransient<IRepository, Repository>();

            return services;
        }

        internal static IServiceCollection AddSwaggerSupportWithVersioning(this IServiceCollection services, string apiTitle, string apiSummary)
        {
            services.AddEndpointsApiExplorer();

            services.AddApiVersioning(setupAction =>
            {
                setupAction.AssumeDefaultVersionWhenUnspecified = true;
                setupAction.DefaultApiVersion = new ApiVersion(1, 0);
                setupAction.ReportApiVersions = true;
            });

            var apiVersionDescriptionProvider = services.BuildServiceProvider().GetService<IApiVersionDescriptionProvider>();

            if (apiVersionDescriptionProvider == null) return services;

            services.AddSwaggerGen(setupAction =>
            {
                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    setupAction.SwaggerDoc(description.GroupName,
                        new OpenApiInfo()
                        {
                            Title = apiTitle,
                            Version = description.ApiVersion.ToString(),
                            Description = apiSummary
                        });
                }

            });

            return services;
        }

        #region Private Methods

        private static IServiceCollection AddConfigurationWithValidation<TConfiguration, TValidator>(this IServiceCollection services, IConfigurationSection configurationSection)
            where TConfiguration : class, new()
            where TValidator : class, IValidateOptions<TConfiguration>
        {
            services.Configure<TConfiguration>(configurationSection);

            services.TryAddEnumerable(ServiceDescriptor.Singleton<IValidateOptions<TConfiguration>, TValidator>());

            services.AddHostedService<ValidateOptionsService<TConfiguration>>();

            return services;
        }

        private static IServiceCollection AddFluentValidator<T, TValidator>(this IServiceCollection services)
            where T: class
            where TValidator : AbstractValidator<T>
        {
            services.TryAddTransient<IValidator<T>, TValidator> ();

            return services;
        }
        #endregion
    }
}
