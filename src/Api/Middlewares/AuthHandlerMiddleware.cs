// -----------------------------------------------------------------------------------
// AuthHandlerMiddleware.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetCa.Application.Common.Models;
using NetCa.Infrastructure.Services;

namespace NetCa.Api.Middlewares;

/// <summary>
/// AuthHandlerMiddleware
/// </summary>
public class AuthHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AppSetting _appSetting;
    private readonly ILogger<AuthHandlerMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthHandlerMiddleware"/> class.
    /// </summary>
    /// <param name="next"></param>
    /// <param name="appSetting"></param>
    /// <param name="logger"></param>
    public AuthHandlerMiddleware(RequestDelegate next, AppSetting appSetting, ILogger<AuthHandlerMiddleware> logger)
    {
        _next = next;
        _appSetting = appSetting;
        _logger = logger;
    }

    /// <summary>
    /// Invoke
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task Invoke(HttpContext context)
    {
        var whitelistPathSegment = _appSetting
            .AuthorizationServer
            .WhiteListPathSegment?.Split(",").ToList() ?? new List<string>();
        var requiredCheck = !whitelistPathSegment.Any(item => context.Request.Path.StartsWithSegments(item));

        if (requiredCheck)
        {
            _logger.LogDebug("Authenticating");
            var auth = await CheckAuthAsync(context);
            if (!auth.Succeeded)
            {
                await context.ChallengeAsync();
                return;
            }
        }

        await _next(context);
    }

    private static async Task<AuthenticateResult> CheckAuthAsync(HttpContext context)
    {
        var auth = context.RequestServices.GetRequiredService<IAuthenticationService>();
        return await auth.AuthenticateAsync(context, scheme: JwtBearerDefaults.AuthenticationScheme);
    }
}

/// <summary>
/// AuthHandlerMiddlewareExtensions
/// </summary>
public static class AuthHandlerMiddlewareExtensions
{
    /// <summary>
    /// UseAuthHandler
    /// </summary>
    /// <param name="builder"></param>
    public static void UseAuthHandler(this IApplicationBuilder builder)
    {
        builder.UseAuthentication();
        builder.UseMiddleware<AuthHandlerMiddleware>();
    }

    /// <summary>
    /// AddPermissions
    /// </summary>
    /// <param name="services"></param>
    /// <param name="appSetting"></param>
    public static void AddPermissions(this IServiceCollection services, AppSetting appSetting)
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = appSetting.AuthorizationServer.Address;
                options.Audience = appSetting.AuthorizationServer.Service;
                options.RequireHttpsMetadata = false;
                options.BackchannelHttpHandler = new HttpHandler(new HttpClientHandler())
                {
                    UsingCircuitBreaker = true,
                    UsingWaitRetry = true,
                    RetryCount = 4,
                    SleepDuration = 1000
                };
            });

        services.AddAuthorization(options =>
        {
            var policy = appSetting.AuthorizationServer.Policy ?? new List<Policy>();
            policy.ForEach(p =>
            {
                if (p != null)
                    options.AddPolicy(p.Name, pol => pol.Requirements.Add(new Permission(p.Name)));
            });
        });

        services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
    }
}

/// <summary>
/// AuthorizationPolicyProvider
/// </summary>
public class AuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
{
    private readonly AuthorizationOptions _options;
    private static readonly SemaphoreSlim SemaphoreSlim = new (1);

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationPolicyProvider"/> class.
    /// </summary>
    /// <param name="options"></param>
    public AuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
        : base(options)
    {
        _options = options.Value;
    }

    /// <inheritdoc/>
    public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
        SemaphoreSlim.Wait();

        var policy = await base.GetPolicyAsync(policyName);

        if (policy == null)
        {
            policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new Permission(policyName))
                .Build();

            _options.AddPolicy(policyName, policy);
        }

        SemaphoreSlim.Release();

        return policy;
    }
}
