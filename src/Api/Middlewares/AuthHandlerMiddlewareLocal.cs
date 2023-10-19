// -----------------------------------------------------------------------------------
// AuthHandlerMiddlewareLocal.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NetCa.Api.Handlers;
using NetCa.Application.Common.Models;

namespace NetCa.Api.Middlewares;

/// <summary>
/// AuthHandlerMiddlewareLocal
/// </summary>
public class AuthHandlerMiddlewareLocal
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthHandlerMiddlewareLocal"/> class.
    /// </summary>
    /// <param name="next"></param>
    public AuthHandlerMiddlewareLocal(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Invoke
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task Invoke(HttpContext context)
    {
        await _next(context);
    }
}

/// <summary>
/// AuthHandlerMiddlewareLocalExtensions
/// </summary>
public static class AuthHandlerMiddlewareLocalExtensions
{
    /// <summary>
    /// UseAuthHandler
    /// </summary>
    /// <param name="builder"></param>
    public static void UseLocalAuthHandler(this IApplicationBuilder builder)
    {
        builder.UseAuthentication();
        builder.UseMiddleware<AuthHandlerMiddlewareLocal>();
    }

    /// <summary>
    /// AddPermissions
    /// </summary>
    /// <param name="services"></param>
    /// <param name="appSetting"></param>
    public static void AddLocalPermissions(this IServiceCollection services, AppSetting appSetting)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = LocalAuthenticationHandler.AuthScheme;
            options.DefaultAuthenticateScheme = LocalAuthenticationHandler.AuthScheme;
            options.DefaultChallengeScheme = LocalAuthenticationHandler.AuthScheme;
        }).AddScheme<AuthenticationSchemeOptions, LocalAuthenticationHandler>(
            LocalAuthenticationHandler.AuthScheme, _ => { });
    }
}