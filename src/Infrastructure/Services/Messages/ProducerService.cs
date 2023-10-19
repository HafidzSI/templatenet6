// -----------------------------------------------------------------------------------
// ProducerService.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCa.Application.Common.Extensions;
using NetCa.Application.Common.Interfaces;
using NetCa.Application.Common.Models;
using NetCa.Application.Dtos;
using NetCa.Application.MessageLog.Create;
using NetCa.Domain.Entities;
using Newtonsoft.Json;

namespace NetCa.Infrastructure.Services.Messages;

/// <summary>
/// ProducerService
/// </summary>
public class ProducerService : IProducerService
{
    private readonly ILogger<ProducerService> _logger;
    private readonly AppSetting _appSetting;
    private readonly IRedisService _redisService;
    private readonly IMediator _mediator;
    private readonly bool _saveToDb;
    private readonly EventHubProducerClientOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProducerService"/> class.
    /// ProducerService
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="logger"></param>
    /// <param name="redisService"></param>
    /// <param name="appSetting"></param>
    public ProducerService(IServiceProvider serviceProvider, ILogger<ProducerService> logger, IRedisService redisService, AppSetting appSetting)
    {
        _logger = logger;
        _appSetting = appSetting;
        _redisService = redisService;

        var messaging = _appSetting.Messaging.Configuration;

        _options = new EventHubProducerClientOptions
        {
            RetryOptions = new ()
            {
                Mode = EventHubsRetryMode.Exponential,
                MaximumRetries = messaging.MaximumRetries,
                Delay = TimeSpan.FromMilliseconds(messaging.Delay),
                MaximumDelay = TimeSpan.FromSeconds(messaging.MaximumDelay),
                TryTimeout = TimeSpan.FromSeconds(messaging.TryTimeout)
            }
        };

        _mediator = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IMediator>();
        _saveToDb = _appSetting.Messaging.Configuration.SaveToDb;
    }

    /// <summary>
    /// SendAsync
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task<bool> SendAsync(string topic, string message)
    {
        var result = false;

        if (string.IsNullOrEmpty(topic))
        {
            _logger.LogError("Failed to produce message with topic empty");
            return false;
        }

        if (string.IsNullOrEmpty(message))
        {
            _logger.LogError("Failed to produce message with message empty");
            return false;
        }

        try
        {
            var connectionString = _appSetting.Messaging.AzureEventHubs
                .Find(x => x.Topics.Exists(y => y.Value.NullSafeToLower() == topic.NullSafeToLower()))?
                .ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                _logger.LogError("Failed to produce message with connection string empty or not found '{Topic}': ", topic);
                return false;
            }

            await using var producerClient = new EventHubProducerClient(connectionString, topic, _options);
            var data = new EventData(Encoding.UTF8.GetBytes(message));

            using var eventBatch = await producerClient.CreateBatchAsync();
            eventBatch.TryAdd(data);
            await producerClient.SendAsync(eventBatch);

            result = true;

            _logger.LogDebug("Sent to topic: {Topic}, partition: {Partition}, offset: {Offset}, message: \n {Message}", topic, data.PartitionKey, data.Offset, message);

            try
            {
                if (_saveToDb)
                    await SaveMessageToDb(topic, message, true);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to save produce message with topic '{Topic}': {Message}", topic, e.Message);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to produce message with topic '{Topic}': {Message}", topic, e.Message);

            try
            {
                var redisMessage = new MessageBrokerDto
                {
                    Name = topic,
                    Value = message
                };

                var value = JsonConvert.SerializeObject(redisMessage, JsonExtensions.SerializerSettings());
                await _redisService.ListRightPushAsync(Constants.RedisSubKeyProduceMessage, value);
            }
            catch (Exception ex1)
            {
                _logger.LogError(ex1, "Failed to save message with topic '{Topic}' to Redis: {Message}", topic, ex1.Message);

                try
                {
                    await SaveMessageToDb(topic, message);
                }
                catch (Exception ex2)
                {
                    _logger.LogError(ex2, "Failed to save message with topic '{Topic}' to database: {Message}", topic, ex2.Message);
                }
            }
        }

        return result;
    }

    private async Task SaveMessageToDb(string topic, string message, bool isSend = false)
    {
        var record = new MessageBroker
        {
            Id = Guid.NewGuid(),
            StoredDate = DateTime.UtcNow,
            Message = message,
            Topic = topic,
            IsSend = isSend
        };

        await _mediator.Send(new CreateSendMessageCommand { MessageBroker = record });
    }
}