using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCa.Application.Common.Extensions;
using NetCa.Application.Common.Interfaces;
using NetCa.Application.Common.Models;
using NetCa.Domain.Entities;
using NetCa.Domain.Enums;
using Polly.Bulkhead;
using Polly.CircuitBreaker;
using Polly.RateLimit;
using Quartz;
using Z.EntityFramework.Plus;

namespace NetCa.Infrastructure.Jobs;

/// <summary>
/// ConsumerDbJob
/// </summary>
public class ConsumerDbJob : BaseJob<ConsumerDbJob>
{
    private readonly AppSetting _appSetting;
    private readonly int _numberOfMessage;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsumerDbJob"/> class.
    /// </summary>
    /// <param name="serviceScopeFactory"></param>
    /// <param name="appSetting"></param>
    /// <param name="logger"></param>
    public ConsumerDbJob(
        IServiceScopeFactory serviceScopeFactory,
        AppSetting appSetting,
        ILogger<ConsumerDbJob> logger)
        : base(logger, serviceScopeFactory)
    {
        _appSetting = appSetting;

        _numberOfMessage = _appSetting.Messaging.Configuration.MessageToProcess;
    }

    /// <summary>
    /// Execute
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task Execute(IJobExecutionContext context)
    {
        var dataMap = context.JobDetail.JobDataMap;
        var topicName = !dataMap.IsEmpty ? dataMap["parameter"] as string : string.Empty;

        if (!Constants.ConsumerJobList.ContainsKey(topicName))
            return;

        var topicData = Constants.ConsumerJobList[topicName];
        var topic = _appSetting.GetTopicValue(topicData.Item1, topicName);

        Logger.LogInformation("Process consume message from database with topic '{Topic}'", topic);

        try
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

            var tasks = new List<Task>();
            byte[] status = { (byte)ProcessStatus.Incoming, (byte)ProcessStatus.Process };

            var messages = await db.ReceivedMessageBroker
                .Where(x => x.Topic.ToLower().Equals(topic.NullSafeToLower()) &&
                    status.Contains((byte)x.Status))
                .OrderByDescending(x => x.Status)
                .ThenBy(x => x.TimeIn)
                .Take(_numberOfMessage)
                .AsNoTracking()
                .ToListAsync(context.CancellationToken);

            if (messages.Count != 0)
            {
                if (topicData.Item2)
                {
                    tasks.AddRange(messages.Select(async message =>
                        await ProcessMessage(topicName, message, context.CancellationToken)));
                }
                else
                {
                    foreach (var message in messages)
                        await ProcessMessage(topicName, message, context.CancellationToken);
                }

                await Task.WhenAll(tasks);
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error when processing message from database with topic '{Topic}': {Message}", topic, e.Message);
        }

        Logger.LogInformation("Success Consume message from database with topic '{Topic}'", topic);
    }

    /// <summary>
    /// ProcessMessage
    /// </summary>
    /// <param name="topicName"></param>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task ProcessMessage(string topicName, ReceivedMessageBroker message, CancellationToken cancellationToken)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();

        Logger.LogDebug("Process message from database");

        using var scope = ServiceScopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        using var scopeMediator = ServiceScopeFactory.CreateScope();

        message.Status = (byte)ProcessStatus.Process;
        message.TimeProcess = DateTime.UtcNow;

        await db.ReceivedMessageBroker
            .Where(x => x.Id.Equals(message.Id))
            .UpdateAsync(
                x => new ReceivedMessageBroker
                {
                    Status = message.Status,
                    TimeProcess = message.TimeProcess
                },
                cancellationToken);

        try
        {
            switch (topicName)
            {
                default:
                    await Task.Delay(0, cancellationToken);
                    break;
            }

            message.Status = (byte)ProcessStatus.Succeed;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error when processing message from database: {Message}", e.Message);

            message.Error = $"{DateTime.UtcNow} | {e.Message}";
            message.InnerMessage = e.InnerException?.Message;
            message.StackTrace = e.StackTrace;

            switch (e)
            {
                case BrokenCircuitException:
                case BulkheadRejectedException:
                case RateLimitRejectedException:
                    break;
                default:
                    message.Status = (byte)ProcessStatus.Failed;
                    break;
            }
        }

        message.TimeFinish = message.Status == (byte)ProcessStatus.Process ? null : DateTime.UtcNow;

        await db.ReceivedMessageBroker
            .Where(x => x.Id.Equals(message.Id))
            .UpdateAsync(
                x => new ReceivedMessageBroker
                {
                    Status = message.Status,
                    Error = message.Error,
                    InnerMessage = message.InnerMessage,
                    StackTrace = message.StackTrace,
                    TimeFinish = message.TimeFinish
                },
                cancellationToken);

        Logger.LogInformation(
            "Process message from database success with topic '{TopicName}' ({Elapsed}ms)",
            topicName,
            stopWatch.Elapsed.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

        stopWatch.Stop();
    }
}