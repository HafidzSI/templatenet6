using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCa.Api;
using NetCa.Api.Handlers;
using NetCa.Api.Middlewares;
using NetCa.Application;
using NetCa.Application.Common.Models;
using NetCa.Infrastructure;
using NSwag;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration).WriteTo
        .Console();
});

builder.Configuration.AddJsonFile("appsettings.json", optional: true, true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, false);
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, true);
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddCommandLine(args);

var appSetting = builder.Configuration.Get<AppSetting>();

builder.Services.AddApiServices(builder.Environment, builder.WebHost, appSetting);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Environment, appSetting);
builder.Services.AddMvc(options => options.EnableEndpointRouting = false);

var app = builder.Build();

app.UseCorsOriginHandler(appSetting);

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsHandler(app.Environment, appSetting);
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
    RegisteredServicesPage(app, builder.Services);

    var option = new RewriteOptions();

    option.AddRedirect("^$", "swagger");
    app.UseRewriter(option);
}
else
{
    app.UseHsts();
}

app.UseHealthCheck();
app.UseAuthentication();
app.UseAuthorization();
app.UseResponseCompression();

if (!appSetting.IsEnableDetailError)
{
    Log.Debug("Activate exception middleware");
    app.UseCustomExceptionHandler();
}
else
{
    Log.Warning("enable detail error response");
}

if (appSetting.IsEnableAuth)
{
    Log.Information("Activate auth middleware");

    if (builder.Environment.EnvironmentName == Constants.EnvironmentNameTest)
    {
        app.UseLocalAuthHandler();
    }
    else
    {
        app.UseAuthHandler();
    }
}
else
{
    Log.Warning("Disable Auth middleware");
}

app.UseOverrideRequestHandler();

app.UseOverrideResponseHandler();

app.UseOpenApi(x =>
    x.PostProcess = (document, _) =>
    {
        document.Schemes = new[]
        {
            OpenApiSchema.Https, OpenApiSchema.Http
        };
    });

app.UseSwaggerUi3(settings =>
{
    settings.Path = "/swagger";
    settings.EnableTryItOut = true;
});

app.UseMvc();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});

Log.Information("Starting host");

app.Run();

void RegisteredServicesPage(IApplicationBuilder appBuilder, IServiceCollection services)
{
    appBuilder.Map("/services", build => build.Run(async context =>
    {
        var sb = new StringBuilder();
        sb.Append("<h1>Registered Services</h1>");
        sb.Append("<table><thead>");
        sb.Append("<tr><th>Type</th><th>Lifetime</th><th>Instance</th></tr>");
        sb.Append("</thead><tbody>");
        foreach (var svc in services)
        {
            sb.Append("<tr>");
            sb.Append($"<td>{svc.ServiceType.FullName}</td>");
            sb.Append($"<td>{svc.Lifetime}</td>");
            sb.Append($"<td>{svc.ImplementationType?.FullName}</td>");
            sb.Append("</tr>");
        }

        sb.Append("</tbody></table>");
        await context.Response.WriteAsync(sb.ToString());
    }));
}

/// <summary>
/// Program
/// </summary>
public partial class Program
{
}