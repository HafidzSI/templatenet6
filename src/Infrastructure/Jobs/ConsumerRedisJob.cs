using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCa.Application.Common.Extensions;
using NetCa.Application.Common.Interfaces;
using NetCa.Application.Common.Models;
using NetCa.Application.MessageLog.Create;
using NetCa.Domain.Entities;
using NetCa.Domain.Enums;
using Polly.Bulkhead;
using Polly.CircuitBreaker;
using Polly.RateLimit;
using Quartz;

namespace NetCa.Infrastructure.Jobs;

/// <summary>
/// ConsumerRedisJob
/// </summary>
public class ConsumerRedisJob : BaseJob<ConsumerRedisJob>
{
    private readonly AppSetting _appSetting;
    private readonly IRedisService _redisService;
    private readonly int _numberOfMessage;
    private readonly bool _saveToDb;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsumerRedisJob"/> class.
    /// </summary>
    /// <param name="serviceScopeFactory"></param>
    /// <param name="logger"></param>
    /// <param name="appSetting"></param>
    /// <param name="redisService"></param>
    public ConsumerRedisJob(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<ConsumerRedisJob> logger,
        AppSetting appSetting,
        IRedisService redisService)
        : base(logger, serviceScopeFactory)
    {
        _appSetting = appSetting;
        _redisService = redisService;

        _numberOfMessage = _appSetting.Messaging.Configuration.MessageToProcess;
        _saveToDb = _appSetting.Messaging.Configuration.SaveToDb;
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

        Logger.LogInformation("Process consume message from Redis with topic '{Topic}'", topic);

        try
        {
            var tasks = new List<Task>();
            var count = 0;

            do
            {
                if (count >= _numberOfMessage)
                    break;

                try
                {
                    var value = await _redisService.ListLeftPopAsync(Constants.RedisSubKeyConsumeMessage, topic);

                    if (value == null)
                        break;

                    if (topicData.Item2)
                        tasks.Add(PreProcessMessage(topicName, topic, value, context.CancellationToken));
                    else
                        await PreProcessMessage(topicName, topic, value, context.CancellationToken);
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "Error when processing message from Redis with topic '{Topic}': {Message}", topic, e.Message);
                    break;
                }

                count++;
            }
            while (!context.CancellationToken.IsCancellationRequested);

            await Task.WhenAll(tasks);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error when processing message from Redis with topic '{Topic}': {Message}", topic, e.Message);
        }

        Logger.LogInformation("Success Consume message from Redis with topic '{Topic}'", topic);
    }

    private async Task PreProcessMessage(string topicName, string topic, string message, CancellationToken cancellationToken)
    {
        Exception exception = null;

        try
        {
            await ProcessMessage(topicName);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error when processing message from Redis with topic '{Topic}': {Message}", topic, e.Message);

            exception = e;
        }

        if (_saveToDb || exception != null)
            await SaveMessageToDb(topic, message, exception, cancellationToken);
    }

    private async Task ProcessMessage(string topicName)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();

        Logger.LogDebug("Process message from Redis");

        using var scope = ServiceScopeFactory.CreateScope();
        scope.ServiceProvider.GetRequiredService<IMediator>();

        try
        {
            switch (topicName)
            {
                default:
                    await Task.Delay(0);
                    break;
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error when processing message from Redis: {Message}", e.Message);

            stopWatch.Stop();

            return;
        }

        Logger.LogInformation("Process message from Redis success with topic '{TopicName}' ({Elapsed}ms)", topicName, stopWatch.Elapsed.TotalMilliseconds);

        stopWatch.Stop();
    }

    private async Task SaveMessageToDb(
        string topic, string message, Exception exception, CancellationToken cancellationToken = default)
    {
        try
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var record = new ReceivedMessageBroker
            {
                Id = Guid.NewGuid(),
                Message = message,
                Offset = 0,
                Partition = 0,
                Topic = topic,
                TimeIn = DateTime.UtcNow,
                Error = exception?.Message,
                InnerMessage = exception?.InnerException?.Message,
                StackTrace = exception?.StackTrace,
                Status = exception switch
                {
                    BrokenCircuitException or
                    BulkheadRejectedException or
                    RateLimitRejectedException => (int?)(byte)ProcessStatus.Process,
                    null => (byte)ProcessStatus.Succeed,
                    _ => (byte)ProcessStatus.Failed,
                }
            };

            record.TimeFinish = record.Status == (byte)ProcessStatus.Process ? null : DateTime.UtcNow;

            await mediator.Send(
                new CreateReceivedMessageCommand
                {
                    Message = record
                },
                cancellationToken);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error when save message from Redis to database: {Message}", e.Message);
        }
    }
}