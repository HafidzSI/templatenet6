// -----------------------------------------------------------------------------------
// BaseJob.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

namespace NetCa.Infrastructure.Jobs;

/// <summary>
/// BaseJob
/// </summary>
/// <typeparam name="T"></typeparam>
[DisallowConcurrentExecution]
public abstract class BaseJob<T> : IJob
{
    /// <summary>
    /// Logger
    /// </summary>
    protected readonly ILogger<T> Logger;

    /// <summary>
    /// ServiceScopeFactory
    /// </summary>
    protected readonly IServiceScopeFactory ServiceScopeFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseJob{T}"/> class.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="serviceScopeFactory"></param>
    protected BaseJob(ILogger<T> logger, IServiceScopeFactory serviceScopeFactory)
    {
        Logger = logger;
        ServiceScopeFactory = serviceScopeFactory;
    }

    /// <summary>
    /// Execute
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public virtual Task Execute(IJobExecutionContext context)
    {
        Logger.LogDebug("Executing job...");
        return Task.CompletedTask;
    }
}