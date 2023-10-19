// -----------------------------------------------------------------------------------
// AppSetting.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Collections.Generic;

namespace NetCa.Application.Common.Models;

/// <summary>
/// AppSetting
/// </summary>
public record AppSetting
{
    /// <summary>
    /// Gets or sets app
    /// </summary>
    /// <returns></returns>
    public App App { get; set; } = new ();

    /// <summary>
    /// Gets or sets a value indicating whether isEnableAuth
    /// </summary>
    /// <value></value>
    public bool IsEnableAuth { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether isEnableDetailError
    /// </summary>
    /// <value></value>
    public bool IsEnableDetailError { get; set; }

    /// <summary>
    /// Gets or sets requestPerformanceInMs
    /// </summary>
    /// <value></value>
    public int RequestPerformanceInMs { get; set; } = 60000;

    /// <summary>
    /// Gets or sets corsOrigin
    /// </summary>
    /// <value></value>
    public string CorsOrigin { get; set; } = "https://*.unitedtractors.com";

    /// <summary>
    /// Gets or sets kestrel
    /// </summary>
    /// <returns></returns>
    public Kestrel Kestrel { get; set; } = new ();

    /// <summary>
    /// Gets or sets resiliency policy
    /// </summary>
    /// <returns></returns>
    public ResiliencyPolicy ResiliencyPolicy { get; set; } = new ();

    /// <summary>
    /// Gets or sets connectionStrings
    /// </summary>
    /// <returns></returns>
    public ConnectionStrings ConnectionStrings { get; set; } = new ();

    /// <summary>
    /// Gets or sets databaseSettings
    /// </summary>
    /// <returns></returns>
    public DatabaseSettings DatabaseSettings { get; set; } = new ();

    /// <summary>
    /// Gets or sets messaging
    /// </summary>
    /// <returns></returns>
    public Messaging Messaging { get; set; } = new ();

    /// <summary>
    /// Gets or sets redis
    /// </summary>
    /// <returns></returns>
    public Redis Redis { get; set; } = new ();

    /// <summary>
    /// Gets or sets authorizationServer
    /// </summary>
    /// <returns></returns>
    public AuthorizationServer AuthorizationServer { get; set; } = new ();

    /// <summary>
    /// Gets or sets backgroundJob
    /// </summary>
    /// <returns></returns>
    public BackgroundJob BackgroundJob { get; set; } = new ();

    /// <summary>
    /// Gets or sets dataLifetime
    /// </summary>
    /// <returns></returns>
    public DataLifetime DataLifetime { get; set; } = new ();

    /// <summary>
    /// Gets or sets bot
    /// </summary>
    /// <returns></returns>
    public Bot Bot { get; set; } = new ();

    /// <summary>
    /// Gets or sets serverApi
    /// </summary>
    /// <returns></returns>
    public ServerApi ServerApi { get; set; } = new ();
}

/// <summary>
/// App
/// </summary>
public record App
{
    /// <summary>
    /// Gets or sets title
    /// </summary>
    /// <value></value>
    public string Title { get; set; } = "NetCa";

    /// <summary>
    /// Gets or sets description
    /// </summary>
    /// <value></value>
    public string Description { get; set; } = "DAD .NET Clean Architecture Web Api Solution Template";

    /// <summary>
    /// Gets or sets version
    /// </summary>
    /// <value></value>
    public string Version { get; set; } = "2.0.0";

    /// <summary>
    /// Gets or sets urlWeb
    /// </summary>
    /// <value></value>
    public string UrlWeb { get; set; } = "https://app-dev.unitedtractors.com";

    /// <summary>
    /// Gets or sets appContact
    /// </summary>
    /// <value></value>
    public AppContact AppContact { get; set; } = new ();

    /// <summary>
    /// Gets or sets namespace
    /// </summary>
    /// <value></value>
    public string Namespace { get; set; } = "NetCa";
}

/// <summary>
/// AppContact
/// </summary>
public record AppContact
{
    /// <summary>
    /// Gets or sets company
    /// </summary>
    /// <value></value>
    public string Company { get; set; } = "PT United Tractors Tbk";

    /// <summary>
    /// Gets or sets email
    /// </summary>
    /// <value></value>
    public string Email { get; set; } = "helpdesk.service@unitedtractors.com";

    /// <summary>
    /// Gets or sets uri
    /// </summary>
    /// <value></value>
    public string Uri { get; set; } = "https://www.unitedtractors.com";
}

/// <summary>
/// Kestrel
/// </summary>
public record Kestrel
{
    /// <summary>
    /// Gets or sets keepAliveTimeoutInM
    /// </summary>
    /// <value></value>
    public int KeepAliveTimeoutInM { get; set; } = 2;

    /// <summary>
    /// Gets or sets minRequestBodyDataRate
    /// </summary>
    /// <value></value>
    public MinRequestBodyDataRate MinRequestBodyDataRate { get; set; } = new ();

    /// <summary>
    /// Gets or sets minResponseDataRate
    /// </summary>
    /// <value></value>
    public MinResponseDataRate MinResponseDataRate { get; set; } = new ();
}

/// <summary>
/// MinRequestBodyDataRate
/// </summary>
public record MinRequestBodyDataRate
{
    /// <summary>
    /// Gets or sets bytesPerSecond
    /// </summary>
    /// <value></value>
    public double BytesPerSecond { get; set; } = 100;

    /// <summary>
    /// Gets or sets gracePeriod
    /// </summary>
    /// <value></value>
    public int GracePeriod { get; set; } = 10;
}

/// <summary>
/// MinResponseDataRate
/// </summary>
public record MinResponseDataRate
{
    /// <summary>
    /// Gets or sets bytesPerSecond
    /// </summary>
    /// <value></value>
    public double BytesPerSecond { get; set; } = 100;

    /// <summary>
    /// Gets or sets gracePeriod
    /// </summary>
    /// <value></value>
    public int GracePeriod { get; set; } = 10;
}

/// <summary>
/// ResiliencyPolicy
/// </summary>
public record ResiliencyPolicy
{
    /// <summary>
    /// Gets or sets Bulkhead
    /// </summary>
    /// <value></value>
    public Bulkhead Bulkhead { get; set; } = new ();

    /// <summary>
    /// Gets or sets CircuitBreaker
    /// </summary>
    /// <value></value>
    public CircuitBreaker CircuitBreaker { get; set; } = new ();

    /// <summary>
    /// Gets or sets RateLimit
    /// </summary>
    /// <value></value>
    public RateLimit RateLimit { get; set; } = new ();

    /// <summary>
    /// Gets or sets Timeout
    /// </summary>
    /// <value></value>
    public Timeout Timeout { get; set; } = new ();
}

/// <summary>
/// Bulkhead
/// </summary>
public record Bulkhead
{
    /// <summary>
    /// Gets or sets a value indicating whether is enabled
    /// </summary>
    /// <value></value>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets MaxParallelization
    /// </summary>
    /// <value></value>
    public int MaxParallelization { get; set; } = 120;

    /// <summary>
    /// Gets or sets MaxQueuingActions
    /// </summary>
    /// <value></value>
    public int MaxQueuingActions { get; set; } = 60;
}

/// <summary>
/// Bulkhead
/// </summary>
public record CircuitBreaker
{
    /// <summary>
    /// Gets or sets a value indicating whether is enabled
    /// </summary>
    /// <value></value>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets ExceptionAllowed
    /// </summary>
    /// <value></value>
    public int ExceptionAllowed { get; set; } = 10;

    /// <summary>
    /// Gets or sets DurationOfBreak
    /// </summary>
    /// <value></value>
    public int DurationOfBreak { get; set; } = 30;
}

/// <summary>
/// RateLimit
/// </summary>
public record RateLimit
{
    /// <summary>
    /// Gets or sets a value indicating whether is enabled
    /// </summary>
    /// <value></value>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets NumberOfAllowedExecutions
    /// </summary>
    /// <value></value>
    public int NumberOfAllowedExecutions { get; set; } = 100;

    /// <summary>
    /// Gets or sets DurationLimit
    /// </summary>
    /// <value></value>
    public int DurationLimit { get; set; } = 1;

    /// <summary>
    /// Gets or sets Burst
    /// </summary>
    /// <value></value>
    public int Burst { get; set; } = 1;
}

/// <summary>
/// Timeout
/// </summary>
public record Timeout
{
    /// <summary>
    /// Gets or sets a value indicating whether is enabled
    /// </summary>
    /// <value></value>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets Duration
    /// </summary>
    /// <value></value>
    public int Duration { get; set; } = 180;
}

/// <summary>
/// ConnectionStrings
/// </summary>
public record ConnectionStrings
{
    /// <summary>
    /// Gets or sets defaultConnection
    /// </summary>
    /// <value></value>
    public string DefaultConnection { get; set; } = "NetCa.db";
}

/// <summary>
/// DatabaseSettings
/// </summary>
public record DatabaseSettings
{
    /// <summary>
    /// Gets or sets maxRetryDelay
    /// </summary>
    /// <value></value>
    public int MaxRetryDelay { get; set; } = 5;

    /// <summary>
    /// Gets or sets maxRetryCount
    /// </summary>
    /// <value></value>
    public int MaxRetryCount { get; set; } = 100;

    /// <summary>
    /// Gets or sets commandTimeout
    /// </summary>
    /// <value></value>
    public int CommandTimeout { get; set; } = 60;

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets enableAuditChangelog
    /// </summary>
    /// <value></value>
    public bool EnableAuditChangelog { get; set; } = true;

    /// <summary>
    /// Gets or sets a audit state
    /// </summary>
    /// <value></value>
    public List<string> AuditState { get; set; } = new ();

    /// <summary>
    /// Gets or sets a value indicating whether migrations
    /// </summary>
    /// <value></value>
    public bool Migrations { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether seedData
    /// </summary>
    /// <value></value>
    public bool SeedData { get; set; }
}

/// <summary>
/// Messaging
/// </summary>
public record Messaging
{
    /// <summary>
    /// Gets or sets azureEventHubs
    /// </summary>
    /// <value></value>
    public List<AzureEventHub> AzureEventHubs { get; set; } = new ();

    /// <summary>
    /// Gets or sets configuration
    /// </summary>
    /// <value></value>
    public Configuration Configuration { get; set; } = new ();
}

/// <summary>
/// AzureEventHub
/// </summary>
public record AzureEventHub
{
    /// <summary>
    /// Gets or sets name
    /// </summary>
    /// <value></value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets connectionString
    /// </summary>
    /// <value></value>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets storageAccount
    /// </summary>
    /// <value></value>
    public string StorageAccount { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets blobContainerName
    /// </summary>
    /// <value></value>
    public string BlobContainerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets topics
    /// </summary>
    /// <value></value>
    public List<EventHubEntity> Topics { get; set; } = new ();
}

/// <summary>
/// EventHubEntity
/// </summary>
public record EventHubEntity
{
    /// <summary>
    /// Gets or sets name
    /// </summary>
    /// <value></value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets groupName
    /// </summary>
    /// <value></value>
    public string GroupName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets value
    /// </summary>
    /// <value></value>
    public string Value { get; set; } = string.Empty;
}

/// <summary>
/// Configuration
/// </summary>
public record Configuration
{
    /// <summary>
    /// Gets or sets maximum retries
    /// </summary>
    /// <value></value>
    public int MaximumRetries { get; set; } = 4;

    /// <summary>
    /// Gets or sets delay in milliseconds
    /// </summary>
    /// <value></value>
    public int Delay { get; set; } = 1000;

    /// <summary>
    /// Gets or sets delay in seconds
    /// </summary>
    /// <value></value>
    public int MaximumDelay { get; set; } = 30;

    /// <summary>
    /// Gets or sets try timeout in seconds
    /// </summary>
    /// <value></value>
    public int TryTimeout { get; set; } = 60;

    /// <summary>
    /// Gets or sets a value indicating whether saveToDb For Debug Message
    /// </summary>
    /// <value></value>
    public bool SaveToDb { get; set; } = true;

    /// <summary>
    /// Gets or sets max data
    /// </summary>
    /// <value></value>
    public int MaxData { get; set; } = Constants.MaxData;

    /// <summary>
    /// Gets or sets message To Process
    /// </summary>
    /// <value></value>
    public int MessageToProcess { get; set; } = 25;

    /// <summary>
    /// Gets or sets process delay
    /// </summary>
    /// <value></value>
    public int ProcessDelay { get; set; } = 30;
}

/// <summary>
/// Redis
/// </summary>
public record Redis
{
    /// <summary>
    /// Gets or sets server
    /// </summary>
    /// <value></value>
    public string Server { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets instanceName
    /// </summary>
    /// <value></value>
    public string InstanceName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets databaseNumber
    /// </summary>
    /// <value></value>
    public int DatabaseNumber { get; set; }

    /// <summary>
    /// Gets or sets databaseNumber
    /// </summary>
    /// <value></value>
    public int? ConnectRetry { get; set; } = 4;

    /// <summary>
    /// Gets or sets databaseNumber
    /// </summary>
    /// <value></value>
    public int? ConnectTimeout { get; set; } = 60;

    /// <summary>
    /// Gets or sets databaseNumber
    /// </summary>
    /// <value></value>
    public int? OperationTimeout { get; set; } = 60;

    /// <summary>
    /// Gets or sets delta back off in milliseconds
    /// </summary>
    /// <value></value>
    public int DeltaBackOff { get; set; } = 1000;

    /// <summary>
    /// Gets or sets max delta back off in milliseconds
    /// </summary>
    /// <value></value>
    public int MaxDeltaBackOff { get; set; } = 30000;

    /// <summary>
    /// Gets or sets defaultExpiryInDays
    /// </summary>
    /// <value></value>
    public int DefaultExpiryInDays { get; set; } = 30;

    /// <summary>
    /// Gets or sets requestExpiryInMinutes
    /// </summary>
    /// <value></value>
    public int RequestExpiryInMinutes { get; set; } = 30;

    /// <summary>
    /// Gets or sets messageExpiryInDays
    /// </summary>
    /// <value></value>
    public int MessageExpiryInDays { get; set; } = 180;

    /// <summary>
    /// Gets or sets policy
    /// </summary>
    /// <value></value>
    public List<Policy> Policy { get; set; } = new ();
}

/// <summary>
/// Policy
/// </summary>
public record Policy
{
    /// <summary>
    /// Gets or sets name
    /// </summary>
    /// <value></value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether isCheck
    /// </summary>
    /// <value></value>
    public bool IsCheck { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether isCache
    /// </summary>
    /// <value></value>
    public bool IsCache { get; set; }
}

/// <summary>
/// Role
/// </summary>
public record Role
{
    /// <summary>
    /// Gets or sets name
    /// </summary>
    /// <value></value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets name
    /// </summary>
    /// <value></value>
    public List<string> Group { get; set; } = new ();
}

/// <summary>
/// AuthorizationServer
/// </summary>
public record AuthorizationServer
{
    /// <summary>
    /// Gets or sets gateway
    /// </summary>
    /// <value></value>
    public string Gateway { get; set; } = "https://apigateway-dev.unitedtractors.com/ucp/user-management";

    /// <summary>
    /// Gets or sets address
    /// </summary>
    /// <value></value>
    public string Address { get; set; } = "https://dev-aks.unitedtractors.com:31863";

    /// <summary>
    /// Gets or sets whiteListPathSegment
    /// </summary>
    /// <value></value>
    public string WhiteListPathSegment { get; set; } = "/swagger,/health";

    /// <summary>
    /// Gets or sets header
    /// </summary>
    /// <value></value>
    public string Header { get; set; } = "Ocp-Apim-Subscription-Key";

    /// <summary>
    /// Gets or sets secret
    /// </summary>
    /// <value></value>
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets service
    /// </summary>
    /// <value></value>
    public string Service { get; set; } = "netca";

    /// <summary>
    /// Gets or sets policy
    /// </summary>
    /// <value></value>
    public List<Policy> Policy { get; set; } = new ();

    /// <summary>
    /// Gets or sets role
    /// </summary>
    /// <value></value>
    public List<Role> Role { get; set; } = new ();
}

/// <summary>
/// BackgroundJob
/// </summary>
public record BackgroundJob
{
    /// <summary>
    /// Gets or sets a value indicating whether isEnable
    /// </summary>
    /// <value></value>
    public bool IsEnable { get; set; } = true;

    /// <summary>
    /// Gets or sets HostName
    /// </summary>
    /// <value></value>
    public string HostName { get; set; } = "local";

    /// <summary>
    /// Gets or sets DefaultMaxRunTime
    /// </summary>
    /// <value></value>
    public int DefaultMaxRunTime { get; set; } = 10;

    /// <summary>
    /// Gets or sets a value indicating whether usePersistentStore
    /// </summary>
    /// <value></value>
    public bool UsePersistentStore { get; set; }

    /// <summary>
    /// Gets or sets persistentStore
    /// </summary>
    /// <returns></returns>
    public PersistentStore PersistentStore { get; set; } = new ();

    /// <summary>
    /// Gets or sets jobs
    /// </summary>
    public List<Job> Jobs { get; set; } = new ();
}

/// <summary>
/// PersistentStore
/// </summary>
public record PersistentStore
{
    /// <summary>
    /// Gets or sets connectionString
    /// </summary>
    /// <value></value>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether useCluster
    /// </summary>
    /// <value></value>
    public bool UseCluster { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether ignoreDuplicates
    /// </summary>
    /// <value></value>
    public bool IgnoreDuplicates { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether overWriteExistingData
    /// </summary>
    /// <value></value>
    public bool OverWriteExistingData { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether ScheduleTriggerRelativeToReplacedTrigger
    /// </summary>
    /// <value></value>
    public bool ScheduleTriggerRelativeToReplacedTrigger { get; set; } = true;

    /// <summary>
    /// Gets or sets maxConcurrency
    /// </summary>
    /// <value></value>
    public int MaxConcurrency { get; set; } = 10;

    /// <summary>
    /// Gets or sets retryInterval
    /// </summary>
    /// <value></value>
    public int RetryInterval { get; set; } = 15;

    /// <summary>
    /// Gets or sets checkInInterval
    /// </summary>
    /// <value></value>
    public int CheckInInterval { get; set; } = 15000;

    /// <summary>
    /// Gets or sets checkInMisfireThreshold
    /// </summary>
    /// <value></value>
    public int CheckInMisfireThreshold { get; set; } = 15000;

    /// <summary>
    /// Gets or sets misfireThreshold
    /// </summary>
    /// <value></value>
    public int MisfireThreshold { get; set; } = 15000;

    /// <summary>
    /// Gets or sets tablePrefix
    /// </summary>
    /// <value></value>
    public string TablePrefix { get; set; } = "QRTZ_";
}

/// <summary>
/// Job
/// </summary>
public record Job
{
    /// <summary>
    /// Gets or sets name
    /// </summary>
    /// <value></value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether isEnable
    /// </summary>
    /// <value></value>
    public bool IsEnable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether isParallel
    /// </summary>
    /// <value></value>
    public bool IsParallel { get; set; }

    /// <summary>
    /// Gets or sets schedule
    /// </summary>
    /// <value></value>
    public string Schedule { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether ignoreMisfire
    /// </summary>
    /// <value></value>
    public bool IgnoreMisfire { get; set; }

    /// <summary>
    /// Gets or sets description
    /// </summary>
    /// <value></value>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets parameters
    /// </summary>
    public List<object> Parameters { get; set; } = new ();
}

/// <summary>
/// DataLifetime
/// </summary>
public record DataLifetime
{
    /// <summary>
    /// Gets or sets changelog
    /// </summary>
    /// <value></value>
    public int Changelog { get; set; } = 180;
}

/// <summary>
/// Bot
/// </summary>
public record Bot
{
    /// <summary>
    /// Gets or sets a value indicating whether isEnable
    /// </summary>
    /// <value></value>
    public bool IsEnable { get; set; }

    /// <summary>
    /// Gets or sets serviceName
    /// </summary>
    /// <value></value>
    public string ServiceName { get; set; } = "DAD - NetCa";

    /// <summary>
    /// Gets or sets serviceDomain
    /// </summary>
    /// <value></value>
    public string ServiceDomain { get; set; } = "netca.dev-aks.unitedtractors.com";

    /// <summary>
    /// Gets or sets address
    /// </summary>
    /// <value></value>
    public string Address { get; set; } = "https://apigateway-dev.unitedtractors.com/dad/msteam";

    /// <summary>
    /// Gets or sets header
    /// </summary>
    /// <value></value>
    public string Header { get; set; } = "Ocp-Apim-Subscription-Key";

    /// <summary>
    /// Gets or sets secret
    /// </summary>
    /// <value></value>
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets cacheMsTeam
    /// </summary>
    /// <returns></returns>
    public CacheMsTeam CacheMsTeam { get; set; } = new ();
}

/// <summary>
/// ServerApi
/// </summary>
public record ServerApi;