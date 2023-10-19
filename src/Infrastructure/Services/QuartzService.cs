// -----------------------------------------------------------------------------------
// QuartzService.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using NetCa.Application.Common.Models;
using Quartz;

namespace NetCa.Infrastructure.Services;

/// <summary>
/// QuartzService
/// </summary>
public static class QuartzService
{
    /// <summary>
    /// AddJobAndTrigger
    /// </summary>
    /// <param name="quartz"></param>
    /// <param name="jobName"></param>
    /// <param name="appSetting"></param>
    /// <exception cref="Exception">Exception</exception>
    public static void AddJobAndTrigger(this IServiceCollectionQuartzConfigurator quartz, string jobName, AppSetting appSetting)
    {
        var job = appSetting.BackgroundJob.Jobs.Find(x => x.Name.Equals(jobName));
        var type = Type.GetType($"{appSetting.App.Namespace}.Infrastructure.Jobs.{jobName}");

        if (job == null || type == null)
            throw new ArgumentNullException($"No Quartz.NET Cron schedule found for {jobName} in configuration");

        if (job is { IsEnable: false })
            return;

        if ((job.Parameters?.Count ?? 0) > 0)
        {
            for (byte i = 0; i < job.Parameters.Count; i++)
                quartz.AddJob(type, job, appSetting, i, job.Parameters[i]);
        }
        else
        {
            quartz.AddJob(type, job, appSetting);
        }
    }

    /// <summary>
    /// AddJob
    /// </summary>
    /// <param name="quartz"></param>
    /// <param name="type"></param>
    /// <param name="job"></param>
    /// <param name="appSetting"></param>
    /// <param name="index"></param>
    /// <param name="parameter"></param>
    public static void AddJob(
        this IServiceCollectionQuartzConfigurator quartz,
        Type type,
        Job job,
        AppSetting appSetting,
        byte? index = null,
        object parameter = null)
    {
        var jobName = job.Name;
        var jobNameWithIndex = index != null ? $"{jobName}{index}" : jobName;
        var group = $"{jobName}_Group";

        var jobKey = new JobKey(jobNameWithIndex, group);

        if (parameter != null)
        {
            var jobDataMap = new JobDataMap
            {
                { "parameter", parameter }
            };

            quartz.AddJob(type, jobKey, configure => configure.SetJobData(jobDataMap));
        }
        else
        {
            quartz.AddJob(type, jobKey);
        }

        var trigger = job.IsParallel && appSetting.BackgroundJob.UsePersistentStore ?
            $"{jobNameWithIndex}:{appSetting.BackgroundJob.HostName}" :
            jobNameWithIndex;
        trigger = $"{trigger}_Trigger";

        quartz.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithDescription(job.Description)
            .WithIdentity(trigger, group)
            .WithCronSchedule(job.Schedule, x =>
            {
                if (job.IgnoreMisfire)
                    x.WithMisfireHandlingInstructionDoNothing();
            }));
    }
}