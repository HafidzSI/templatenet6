// -----------------------------------------------------------------------------------
// DepedencyInjection.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCa.Application.Common.Interfaces;
using NetCa.Application.Common.Models;
using NetCa.Infrastructure.Persistence;
using NetCa.Infrastructure.Persistence.Interceptors;
using NetCa.Infrastructure.Services;
using NetCa.Infrastructure.Services.Cache;
using NetCa.Infrastructure.Services.Messages;
using Quartz;
using Quartz.AspNetCore;

namespace NetCa.Infrastructure;

/// <summary>
/// DependencyInjection
/// </summary>
public static class DepedencyInjection
{
    /// <summary>
    /// AddInfrastructureServices
    /// </summary>
    /// <param name="services"></param>
    /// <param name="environment"></param>
    /// <param name="appSetting"></param>
    public static void AddInfrastructureServices(
        this IServiceCollection services, IWebHostEnvironment environment, AppSetting appSetting)
    {
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        services.AddEntityFrameworkNpgsql()
            .AddDbContext<ApplicationDbContext>(
                (provider, options) =>
                    options
                        .UseInternalServiceProvider(provider)
                        .UseNpgsql(
                            appSetting.ConnectionStrings.DefaultConnection,
                            x =>
                            {
                                x.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                                x.CommandTimeout(appSetting.DatabaseSettings.CommandTimeout);
                                x.EnableRetryOnFailure(
                                    appSetting.DatabaseSettings.MaxRetryCount,
                                    TimeSpan.FromSeconds(appSetting.DatabaseSettings.MaxRetryDelay),
                                    null);

                                if (!environment.IsProduction())
                                {
                                    options.EnableSensitiveDataLogging();
                                    options.EnableDetailedErrors();
                                }
                            }), ServiceLifetime.Transient);

        services.AddTransient<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());
        services.AddScoped<ApplicationDbContextInitializer>();
        services.AddTransient<IDateTime, DateTimeService>();

        services.AddScoped<IUserAuthorizationService, UserAuthorizationService>();
        services.AddSingleton<IAuthorizationHandler, UserAuthorizationHandlerService>();

        services.AddSingleton<IRedisService, RedisService>();
        services.AddSingleton<IProducerService, ProducerService>();
        services.AddSingleton<ICache, CacheService>();
        services.AddHostedService<ConsumerService>();

        services.AddHostedService<LifetimeEventsHostedService>();

        if (!appSetting.BackgroundJob.IsEnable)
            return;

        services.Configure<QuartzOptions>(options =>
        {
            options.Scheduling.IgnoreDuplicates = appSetting.BackgroundJob.PersistentStore.IgnoreDuplicates;
            options.Scheduling.OverWriteExistingData = appSetting.BackgroundJob.PersistentStore.OverWriteExistingData;
            options.Scheduling.ScheduleTriggerRelativeToReplacedTrigger = appSetting.BackgroundJob.PersistentStore
                .ScheduleTriggerRelativeToReplacedTrigger;
        });

        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();
            q.UseJobAutoInterrupt(options =>
            {
                options.DefaultMaxRunTime = TimeSpan.FromMinutes(appSetting.BackgroundJob.DefaultMaxRunTime);
            });

            q.InterruptJobsOnShutdown = false;
            q.InterruptJobsOnShutdownWithWait = true;

            if (appSetting.BackgroundJob.UsePersistentStore)
            {
                q.SchedulerId = appSetting.App.Title;
                q.SchedulerName = $"{appSetting.App.Title} Scheduler";
                q.MaxBatchSize = 10;

                q.UsePersistentStore(s =>
                {
                    s.UsePostgres(options =>
                    {
                        options.ConnectionString = appSetting.BackgroundJob.PersistentStore.ConnectionString;
                        options.TablePrefix = appSetting.BackgroundJob.PersistentStore.TablePrefix;
                    });
                    s.RetryInterval = TimeSpan.FromSeconds(appSetting.BackgroundJob.PersistentStore.RetryInterval);
                    s.UseJsonSerializer();

                    if (appSetting.BackgroundJob.PersistentStore.UseCluster)
                    {
                        s.UseClustering(cfg =>
                        {
                            q.SchedulerId = appSetting.BackgroundJob.HostName;
                            cfg.CheckinInterval =
                                TimeSpan.FromMilliseconds(appSetting.BackgroundJob.PersistentStore.CheckInInterval);
                            cfg.CheckinMisfireThreshold =
                                TimeSpan.FromMilliseconds(appSetting.BackgroundJob.PersistentStore
                                    .CheckInMisfireThreshold);
                        });
                    }
                });
                q.MisfireThreshold =
                    TimeSpan.FromMilliseconds(appSetting.BackgroundJob.PersistentStore.MisfireThreshold);
                q.UseDefaultThreadPool(tp =>
                {
                    tp.MaxConcurrency = appSetting.BackgroundJob.PersistentStore.MaxConcurrency;
                });
            }

            var jobs = appSetting.BackgroundJob.Jobs
                .Select(x => x.Name)
                .ToList();

            foreach (var jobName in jobs)
                q.AddJobAndTrigger(jobName, appSetting);
        });

        services.AddQuartzServer(q => q.WaitForJobsToComplete = true);
    }
}