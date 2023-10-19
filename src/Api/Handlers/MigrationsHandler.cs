// -----------------------------------------------------------------------------------
// MigrationsHandler.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NetCa.Application.Common.Models;
using NetCa.Infrastructure.Persistence;
using Serilog;

namespace NetCa.Api.Handlers;

/// <summary>
/// MigrationsHandler
/// </summary>
public static class MigrationsHandler
{
    /// <summary>
    /// ApplyMigration
    /// </summary>
    /// <param name="app"></param>
    /// <param name="environment"></param>
    /// <param name="appSetting"></param>
    /// <returns></returns>
    public static async Task ApplyMigration(
        IApplicationBuilder app, IWebHostEnvironment environment, AppSetting appSetting)
    {
        if (!appSetting.DatabaseSettings.Migrations && !appSetting.DatabaseSettings.SeedData)
            return;

        using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope();
        var logger = Log.ForContext(typeof(MigrationsHandler));

        try
        {
            if (serviceScope != null)
            {
                var initializer = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();

                if (appSetting.DatabaseSettings.Migrations)
                {
                    logger.Information("Migration Database");

                    await initializer.InitializeAsync();
                }

                if (appSetting.DatabaseSettings.SeedData)
                {
                    logger.Information("Seeding Database");

                    await initializer.SeedAsync();
                }
            }
        }
        catch (Exception ex)
        {
            logger.Error(ex, "An error occurred while migrating the database.");
        }
    }
}

/// <summary>
/// UseMigrationsHandlerExtensions
/// </summary>
public static class UseMigrationsHandlerExtensions
{
    /// <summary>
    /// UseMigrationsHandler
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="environment"></param>
    /// <param name="appSetting"></param>
    public static void UseMigrationsHandler(
        this IApplicationBuilder builder, IWebHostEnvironment environment, AppSetting appSetting)
    {
        MigrationsHandler.ApplyMigration(builder, environment, appSetting).ConfigureAwait(false);
    }
}
