// -----------------------------------------------------------------------------------
// CorsOriginHandler.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using Microsoft.AspNetCore.Builder;
using NetCa.Application.Common.Models;

namespace NetCa.Api.Handlers;

/// <summary>
/// CorsOriginHandler
/// </summary>
public static class CorsOriginHandler
{
    /// <summary>
    /// ApplyCorsOrigin
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="appSetting"></param>
    public static void ApplyCorsOrigin(IApplicationBuilder builder, AppSetting appSetting)
    {
        var origin = appSetting.CorsOrigin;
        builder.UseCors(
            options => options.WithOrigins(origin)
                .SetIsOriginAllowedToAllowWildcardSubdomains()
                .AllowAnyMethod()
                .AllowAnyHeader());
    }
}

/// <summary>
/// UseCorsOriginHandlerExtension
/// </summary>
public static class UseCorsOriginHandlerExtension
{
    /// <summary>
    /// UseCorsOriginHandler
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="appSetting"></param>
    public static void UseCorsOriginHandler(this IApplicationBuilder builder, AppSetting appSetting)
    {
        CorsOriginHandler.ApplyCorsOrigin(builder, appSetting);
    }
}
