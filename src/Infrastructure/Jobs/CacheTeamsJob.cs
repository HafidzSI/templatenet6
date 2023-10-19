// -----------------------------------------------------------------------------------
// CacheTeamsJob.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCa.Application.Common.Models;
using Quartz;

namespace NetCa.Infrastructure.Jobs;

/// <summary>
/// CacheTeamsJob
/// </summary>
public class CacheTeamsJob : BaseJob<CacheTeamsJob>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CacheTeamsJob"/> class.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="serviceScopeFactory"></param>
    public CacheTeamsJob(ILogger<CacheTeamsJob> logger, IServiceScopeFactory serviceScopeFactory)
        : base(logger, serviceScopeFactory)
    {
    }

    /// <summary>
    /// Execute
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override Task Execute(IJobExecutionContext context)
    {
        using var scope = ServiceScopeFactory.CreateScope();
        var ch = scope.ServiceProvider.GetRequiredService<IMemoryCache>();
        var appSet = scope.ServiceProvider.GetRequiredService<AppSetting>();

        var ctm = GetCounter(ch);
        var hours = (DateTime.UtcNow - ctm.Date).TotalHours;

        if (hours < appSet.Bot.CacheMsTeam.Hours)
            return Task.CompletedTask;

        Logger.LogWarning("Resetting CacheMSTeams");

        ctm.Counter = 0;
        ctm.Date = DateTime.UtcNow;

        SetCounter(ctm, ch);

        return Task.CompletedTask;
    }

    private static void SetCounter(CacheMsTeam cacheMsTeam, IMemoryCache mc)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromDays(2));
        mc.Set("CacheMSTeams", cacheMsTeam, cacheEntryOptions);
    }

    private static CacheMsTeam GetCounter(IMemoryCache mc)
    {
        var isExist = mc.TryGetValue("CacheMSTeams", out CacheMsTeam cacheMsTeam);

        if (isExist)
            return cacheMsTeam;

        cacheMsTeam = new CacheMsTeam { Counter = 0, Date = DateTime.UtcNow };

        return cacheMsTeam;
    }
}
