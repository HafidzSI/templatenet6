using System;
using System.Buffers;
using System.Linq;
using FluentValidation;
using FluentValidation.AspNetCore;
using JsonApiSerializer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetCa.Api.Filters;
using NetCa.Api.Handlers;
using NetCa.Api.Middlewares;
using NetCa.Api.Processors;
using NetCa.Application.Common.Extensions;
using NetCa.Application.Common.Interfaces;
using NetCa.Application.Common.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NJsonSchema.Generation;
using NSwag;
using NSwag.Generation.Processors.Security;
using Serilog;

namespace NetCa.Api;

/// <summary>
/// DepedencyInjection
/// </summary>
public static class DepedencyInjection
{
    /// <summary>
    /// AddApiServices
    /// </summary>
    /// <param name="services"></param>
    /// <param name="environment"></param>
    /// <param name="webHost"></param>
    /// <param name="appSetting"></param>
    public static void AddApiServices(
        this IServiceCollection services,
        IWebHostEnvironment environment,
        ConfigureWebHostBuilder webHost,
        AppSetting appSetting)
    {
        AppLoggingExtensions.LoggerFactory = services.BuildServiceProvider().GetService<ILoggerFactory>();

        webHost.UseKestrel(option =>
        {
            option.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(appSetting.Kestrel.KeepAliveTimeoutInM);
            option.Limits.MinRequestBodyDataRate = new MinDataRate(
                appSetting.Kestrel.MinRequestBodyDataRate.BytesPerSecond,
                TimeSpan.FromSeconds(appSetting.Kestrel.MinRequestBodyDataRate.GracePeriod));
            option.Limits.MinResponseDataRate = new MinDataRate(
                appSetting.Kestrel.MinResponseDataRate.BytesPerSecond,
                TimeSpan.FromSeconds(appSetting.Kestrel.MinResponseDataRate.GracePeriod));
            option.AddServerHeader = false;
        });

        services.AddApplicationInsightsTelemetry();
        services.AddDatabaseDeveloperPageExceptionFilter();
        services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
        services.AddSingleton(appSetting);
        services.AddDistributedMemoryCache();
        services.AddMemoryCache();
        services.AddHttpContextAccessor();
        services.AddScoped<ApiDevelopmentFilterAttribute>();
        services.AddScoped<ApiAuthenticationFilterAttribute>();
        services.AddScoped<ApiAuthorizeFilterAttribute>();

        if (environment.EnvironmentName == Constants.EnvironmentNameTest)
            services.AddLocalPermissions(appSetting);
        else
            services.AddPermissions(appSetting);

        services.AddMvcCore();

        services.AddRouting(options =>
        {
            options.LowercaseUrls = true;
            options.LowercaseQueryStrings = true;
        });

        services
            .AddControllers(options =>
            {
                var serializerSettings = new JsonApiSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Include,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                };

                var jsonApiFormatter =
                    new NewtonsoftJsonOutputFormatter(serializerSettings, ArrayPool<char>.Shared,
                        new MvcOptions(), new MvcNewtonsoftJsonOptions());

                options.OutputFormatters.RemoveType<NewtonsoftJsonOutputFormatter>();
                options.OutputFormatters.Insert(0, jsonApiFormatter);

                options.EnableEndpointRouting = false;

                options.Filters.Add<ApiExceptionFilterAttribute>();
                options.Filters.Add(new ProducesResponseTypeAttribute(
                    typeof(object),
                    (int)System.Net.HttpStatusCode.Unauthorized));
                options.Filters.Add(new ProducesResponseTypeAttribute(
                    typeof(object),
                    (int)System.Net.HttpStatusCode.Forbidden));
                options.Filters.Add(new ProducesResponseTypeAttribute(
                    typeof(object),
                    (int)System.Net.HttpStatusCode.InternalServerError));
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                };
            });

        services.AddFluentValidationClientsideAdapters()
            .AddValidatorsFromAssemblyContaining<IApplicationDbContext>()
            .AddFluentValidationClientsideAdapters();

        services.AddCors();

        services.AddOptions();

        services.AddCompressionHandler();

        services.AddHealthCheck(appSetting);

        services.AddApiVersioning(
            options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            });

        services.AddVersionedApiExplorer(
            options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
            });

        services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });

        services.AddOpenApiDocument(configure =>
        {
            if (environment.IsProduction())
                configure.OperationProcessors.Insert(0, new MyControllerProcessor());

            configure.Title = appSetting.App.Title;
            configure.Description = appSetting.App.Description;
            configure.Version = appSetting.App.Version;
            configure.AllowNullableBodyParameters = false;
            configure.AllowReferencesWithProperties = true;
            configure.IgnoreObsoleteProperties = true;
            configure.DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull;
            configure.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.ApiKey,
                Name = "Authorization",
                In = OpenApiSecurityApiKeyLocation.Header,
                Description = "Type into the text box: Bearer {your JWT token}."
            });

            configure.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
            configure.PostProcess = document =>
            {
                document.Info.Contact = new OpenApiContact
                {
                    Name = appSetting.App.AppContact.Company,
                    Email = appSetting.App.AppContact.Email,
                    Url = appSetting.App.AppContact.Uri
                };
            };
        });

        services.AddEndpointsApiExplorer();
    }
}