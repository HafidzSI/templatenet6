// -----------------------------------------------------------------------------------
// LogEventSinkHandler.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using NetCa.Application.Common.Models;
using NetCa.Infrastructure.Apis;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Constants = NetCa.Application.Common.Models.Constants;

namespace NetCa.Api.Handlers;

/// <summary>
/// LogEventSinkHandler
/// </summary>
public class LogEventSinkHandler : ILogEventSink
{
    private readonly AppSetting _appSetting;
    private readonly IMemoryCache _memoryCache;
    private static readonly ILogger Logger = Log.ForContext(typeof(LogEventSinkHandler));

    /// <summary>
    /// Initializes a new instance of the <see cref="LogEventSinkHandler"/> class.
    /// </summary>
    /// <param name="appSetting"></param>
    /// <param name="memoryCache"></param>
    public LogEventSinkHandler(AppSetting appSetting, IMemoryCache memoryCache)
    {
        _appSetting = appSetting;
        _memoryCache = memoryCache;
    }

    /// <summary>
    /// Emit
    /// </summary>
    /// <param name="logEvent"></param>
    public void Emit(LogEvent logEvent)
    {
        if (!_appSetting.Bot.IsEnable)
            return;

        if (!logEvent.Level.Equals(LogEventLevel.Error))
            return;

        var cacheMsTeam = GetCounter();
        var hours = (DateTime.UtcNow - cacheMsTeam.Date).TotalHours;

        if (cacheMsTeam.Counter >= _appSetting.Bot.CacheMsTeam.Counter || hours >= _appSetting.Bot.CacheMsTeam.Hours)
            return;

        SetCounter(cacheMsTeam);

        var facts = new List<Fact>();
        var sections = new List<Section>();
        var serviceName = _appSetting.Bot.ServiceName;
        var serviceDomain = _appSetting.Bot.ServiceDomain;

        facts.Add(new Fact
        {
            Name = "Date",
            Value = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss zzz}"
        });

        var app = $"[{serviceName}](http://{serviceDomain})";

        sections.Add(new Section
        {
            ActivityTitle = app,
            ActivitySubtitle = "Internal Server Error",
            Facts = facts,
            ActivityImage = Constants.MsTeamsImageError
        });

        facts.Add(new Fact
        {
            Name = "Message",
            Value = logEvent.RenderMessage()
        });

        var tmpl = new MsTeamTemplate
        {
            Sections = sections,
            Summary = $"{Constants.MsTeamsSummaryError} with {app}"
        };

        Logger.Debug("Sending message to MsTeam with color {ThemeColor}", tmpl.ThemeColor);

        SendToMsTeams.Send(_appSetting, tmpl).ConfigureAwait(false);
    }

    private void SetCounter(CacheMsTeam cacheMsTeam)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromDays(2));
        _memoryCache.Set("CacheMsTeams", cacheMsTeam, cacheEntryOptions);
    }

    private CacheMsTeam GetCounter()
    {
        var isExist = _memoryCache.TryGetValue("CacheMsTeams", out CacheMsTeam cacheMsTeam);

        if (isExist)
            return cacheMsTeam;

        cacheMsTeam = new CacheMsTeam
        {
            Counter = 0,
            Date = DateTime.UtcNow
        };

        return cacheMsTeam;
    }
}
