// -----------------------------------------------------------------------------------
// ConsumerService.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCa.Application.Common.Interfaces;
using NetCa.Application.Common.Models;
using NetCa.Application.MessageLog.Create;
using NetCa.Domain.Entities;
using NetCa.Domain.Enums;
using Timeout = System.Threading.Timeout;

namespace NetCa.Infrastructure.Services.Messages;

/// <summary>
/// ConsumerService
/// </summary>
public class ConsumerService : BaseWorkerService
{
    private readonly bool _saveToDb;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ConsumerService> _logger;
    private readonly AppSetting _appSetting;
    private readonly IRedisService _redisService;
    private readonly EventProcessorClientOptions _options;

    private static readonly List<string> _redirectRedis = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsumerService"/> class.
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="logger"></param>
    /// <param name="redisService"></param>
    /// <param name="appSetting"></param>
    public ConsumerService(IServiceProvider serviceProvider, ILogger<ConsumerService> logger,
        IRedisService redisService, AppSetting appSetting)
    {
        _appSetting = appSetting;
        _serviceProvider = serviceProvider;
        _logger = logger;

        var messaging = _appSetting.Messaging.Configuration;

        _options = new EventProcessorClientOptions
        {
            RetryOptions = new EventHubsRetryOptions
            {
                Mode = EventHubsRetryMode.Exponential,
                MaximumRetries = messaging.MaximumRetries,
                Delay = TimeSpan.FromMilliseconds(messaging.Delay),
                MaximumDelay = TimeSpan.FromSeconds(messaging.MaximumDelay),
                TryTimeout = TimeSpan.FromSeconds(messaging.TryTimeout)
            }
        };

        _saveToDb = _appSetting.Messaging.Configuration.SaveToDb;
        _redisService = redisService;
    }

    /// <summary>
    /// ConsumeAsync
    /// </summary>
    /// <param name="name"></param>
    /// <param name="topicName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task ConsumeAsync(string name, string topicName, CancellationToken cancellationToken)
    {
        var eventHub = _appSetting.Messaging.AzureEventHubs.Find(x => x.Name.Equals(name));
        var connection = eventHub?.ConnectionString;
        var storageAccountConnectionString = eventHub?.StorageAccount;
        var blobContainerName = eventHub?.BlobContainerName;

        var topicData = eventHub?.Topics.Find(x => x.Name.Equals(topicName));
        var topic = topicData?.Value;
        var consumerGroup = topicData?.GroupName ?? EventHubConsumerClient.DefaultConsumerGroupName;

        var storageClient = new BlobContainerClient(storageAccountConnectionString, blobContainerName);
        var processor = new EventProcessorClient(storageClient, consumerGroup, connection, topic, _options);

        async Task ProcessEventHandler(ProcessEventArgs eventArgs)
        {
            try
            {
                _logger.LogDebug("Process consume message with topic '{Topic}'", topic);

                var processed = false;

                var result = eventArgs.Data;
                var value = Encoding.UTF8.GetString(result.Body.ToArray());

                try
                {
                    var saved = false;

                    if (_redirectRedis.Contains(topicName))
                    {
                        await SaveMessageToRedis(topic, value);
                        processed = saved = true;
                    }

                    if (_saveToDb && !saved)
                    {
                        await SaveMessageToDb(
                            result,
                            topic,
                            eventArgs.Partition.PartitionId,
                            cancellationToken);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(
                        e,
                        "Error when processing message with topic '{Topic}': {Message}",
                        topic,
                        e.Message);

                    if (!processed)
                    {
                        try
                        {
                            if (_redirectRedis.Contains(topicName))
                            {
                                await SaveMessageToDb(
                                    result,
                                    topic,
                                    eventArgs.Partition.PartitionId,
                                    cancellationToken,
                                    e.Message);
                            }
                            else
                            {
                                await SaveMessageToRedis(topic, value);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(
                                ex,
                                "Failed to trying save message with topic '{Topic}' to database/redis: {Message}",
                                topic,
                                ex.Message);

                            if (!_redirectRedis.Contains(topicName))
                            {
                                await SaveMessageToDb(
                                    result,
                                    topic,
                                    eventArgs.Partition.PartitionId,
                                    cancellationToken,
                                    ex.Message);
                            }
                        }
                    }
                }

                if (processed)
                    _logger.LogDebug("Data processed with value: \n {Value}", value);

                _logger.LogDebug("Consume message done with topic '{Topic}'", topic);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error when consume message with topic '{Topic}': {Message}", topic, e.Message);
            }

            await eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
        }

        Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
        {
            _logger.LogError(
                "Partition '{PartitionId}': an unhandled exception was encountered. This was not expected to happen",
                eventArgs.PartitionId);
            _logger.LogError(eventArgs.Exception, "{Message}", eventArgs.Exception.Message);

            return Task.CompletedTask;
        }

        try
        {
            processor.ProcessEventAsync += ProcessEventHandler;
            processor.ProcessErrorAsync += ProcessErrorHandler;

            try
            {
                await processor.StartProcessingAsync(cancellationToken);
                await Task.Delay(Timeout.Infinite, cancellationToken);
            }
            catch (TaskCanceledException e)
            {
                _logger.LogWarning(e, "Consume message with topic '{Topic}' has been cancelled", topic);
            }
            finally
            {
                await processor.StopProcessingAsync(cancellationToken);
            }
        }
        catch (Exception e)
        {
            switch (e)
            {
                case TaskCanceledException:
                    _logger.LogWarning(e, "Consume message with topic '{Topic}' has been cancelled", topic);
                    break;
                default:
                    _logger.LogError(e, "Error when consume message with topic '{Topic}': {Message}", topic, e.Message);
                    break;
            }
        }
        finally
        {
            processor.ProcessEventAsync -= ProcessEventHandler;
            processor.ProcessErrorAsync -= ProcessErrorHandler;
        }
    }

    /// <summary>
    /// ExecuteAsync
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ConsumerService is running");
        await RunJob(stoppingToken);
    }

    private Task RunJob(CancellationToken cancellationToken)
    {
        var tasks = new List<Task>();

        foreach (var topicData in Constants.ConsumerJobList)
        {
            tasks.Add(Task.Run(
                async () =>
                {
                    try
                    {
                        await ConsumeAsync(topicData.Value.Item1, topicData.Key, cancellationToken);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error when consume message with topic '{Topic}': {Message}", topicData.Key,
                            e.Message);
                    }
                },
                cancellationToken));
        }

        return Task.WhenAll(tasks);
    }

    private async Task SaveMessageToRedis(string topic, string value)
    {
        await _redisService.ListRightPushAsync(Constants.RedisSubKeyConsumeMessage, value, topic);
    }

    private async Task SaveMessageToDb(EventData result, string topic, string partition,
        CancellationToken cancellationToken, string error = null)
    {
        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var record = new ReceivedMessageBroker
        {
            Id = Guid.NewGuid(),
            Message = Encoding.UTF8.GetString(result.Body.ToArray()),
            Offset = (int)result.Offset,
            Partition = int.Parse(partition),
            Topic = topic,
            TimeIn = DateTime.UtcNow,
            Error = error,
            Status = error == null ? (byte)ProcessStatus.Incoming : (byte)ProcessStatus.Initial
        };

        await mediator.Send(
            new CreateReceivedMessageCommand
            {
                Message = record
            },
            cancellationToken);
    }
}