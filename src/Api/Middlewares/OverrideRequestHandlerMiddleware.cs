// -----------------------------------------------------------------------------------
// OverrideRequestHandlerMiddleware.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NetCa.Application.Common.Interfaces;
using NetCa.Application.Common.Models;

namespace NetCa.Api.Middlewares;

/// <summary>
/// OverrideRequestHandlerMiddleware
/// </summary>
public class OverrideRequestHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;
    private readonly IRedisService _redisService;

    /// <summary>
    /// Initializes a new instance of the <see cref="OverrideRequestHandlerMiddleware"/> class.
    /// </summary>
    /// <param name="next"></param>
    /// <param name="redisService"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public OverrideRequestHandlerMiddleware(
        RequestDelegate next,
        IRedisService redisService,
        ILogger<OverrideRequestHandlerMiddleware> logger)
    {
        _next = next;
        _redisService = redisService;
        _logger = logger;
    }

    /// <summary>
    /// InvokeAsync
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogDebug("Overriding Request");

        if (context.Request.Path.StartsWithSegments("/api"))
        {
            var requestIfNoneMatch = context.Request.Headers[Constants.HeaderIfNoneMatch].ToString() ?? "";

            if (!string.IsNullOrEmpty(requestIfNoneMatch))
            {
                var encodedEntity = await _redisService.GetAsync(requestIfNoneMatch);
                if (!string.IsNullOrEmpty(encodedEntity))
                {
                    const int code = (int)HttpStatusCode.NotModified;
                    context.Response.StatusCode = code;
                    return;
                }
            }
        }

        await _next(context);
    }
}

/// <summary>
/// OverrideRequestMiddlewareExtensions
/// </summary>
public static class OverrideRequestMiddlewareExtensions
{
    /// <summary>
    /// UseOverrideRequestHandler
    /// </summary>
    /// <param name="builder"></param>
    public static void UseOverrideRequestHandler(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<OverrideRequestHandlerMiddleware>();
    }
}
