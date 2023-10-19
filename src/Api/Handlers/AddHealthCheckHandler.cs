// -----------------------------------------------------------------------------------
// AddHealthCheckHandler.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using HealthChecks.Network;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetCa.Application.Common.Interfaces;
using NetCa.Application.Common.Models;

namespace NetCa.Api.Handlers;

/// <summary>
/// AddHealthCheckHandler
/// </summary>
public static class AddHealthCheckHandler
{
    private const string _healthQuery = Constants.DefaultHealthCheckQuery;

    /// <summary>
    /// AddHealthCheck
    /// </summary>
    /// <param name="services"></param>
    /// <param name="appSetting"></param>
    public static void AddHealthCheck(this IServiceCollection services, AppSetting appSetting)
    {
        services.AddHealthChecks().AddCheck<SystemCpuHealthCheck>(Constants.DefaultHealthCheckCpuUsage);
        services.AddHealthChecks().AddCheck<SystemMemoryHealthCheck>(Constants.DefaultHealthCheckMemoryUsage);
        services.AddHealthChecks().AddCheck<UmsHealthCheck>(Constants.DefaultHealthCheckUmsName);

        services.AddHealthChecks()
            .AddNpgSql(
                appSetting.ConnectionStrings.DefaultConnection,
                name: Constants.DefaultHealthCheckDatabaseName,
                healthQuery: _healthQuery,
                failureStatus: HealthStatus.Degraded,
                timeout: TimeSpan.FromSeconds(Constants.DefaultHealthCheckTimeoutInSeconds));

        foreach (var eventHub in appSetting.Messaging.AzureEventHubs)
        {
            foreach (var topic in eventHub.Topics)
            {
                services.AddHealthChecks().AddAzureEventHub(
                    eventHub.ConnectionString,
                    topic.Value,
                    $"{Constants.DefaultHealthCheckEventHub}_{eventHub.Name}_{topic.Name}",
                    HealthStatus.Degraded,
                    timeout: TimeSpan.FromSeconds(Constants.DefaultHealthCheckTimeoutInSeconds));
            }
        }

        services.AddHealthChecks()
            .AddRedis(
                appSetting.Redis.Server,
                Constants.DefaultHealthCheckRedisName,
                HealthStatus.Degraded,
                timeout: TimeSpan.FromSeconds(Constants.DefaultHealthCheckTimeoutInSeconds));

        var gateWay = new Uri(appSetting.AuthorizationServer.Gateway);
        void GateWaySetup(TcpHealthCheckOptions x) => x.AddHost(gateWay.Host, 443);
        services.AddHealthChecks()
            .AddTcpHealthCheck(
                GateWaySetup,
                Constants.DefaultHealthCheckGateWayName,
                HealthStatus.Degraded,
                timeout: TimeSpan.FromSeconds(Constants.DefaultHealthCheckTimeoutInSeconds));
    }

    /// <summary>
    /// UseHealthCheck
    /// </summary>
    /// <param name="app"></param>
    public static void UseHealthCheck(this IApplicationBuilder app)
    {
        app.UseHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UiResponseWriter.WriteHealthCheckUiResponse
        });
    }
}

/// <summary>
/// SystemCpuHealthCheck
/// </summary>
public class SystemCpuHealthCheck : IHealthCheck
{
    /// <summary>
    /// CheckHealthAsync
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;
        var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;

        await Task.Delay(500, cancellationToken);

        var endTime = DateTime.UtcNow;
        var endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;

        var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
        var totalMsPassed = (endTime - startTime).TotalMilliseconds;
        var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);

        var data = new Dictionary<string, object> { { "Cpu_Usage", cpuUsageTotal } };

        var result = new HealthCheckResult(HealthStatus.Healthy, null, null, data);

        return await Task.FromResult(result);
    }
}

/// <summary>
/// SystemMemoryHealthCheck
/// </summary>
public class SystemMemoryHealthCheck : IHealthCheck
{
    /// <summary>
    /// CheckHealthAsync
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var metrics = MemoryMetricsClient.GetMetrics();
        var percentUsed = 100 * metrics.Used / metrics.Total;

        var status = HealthStatus.Healthy;
        if (percentUsed > Constants.DefaultHealthCheckPercentageUsedDegraded)
            status = HealthStatus.Degraded;

        var data = new Dictionary<string, object>
        {
            { "Total_MB", metrics.Total },
            { "Used_MB", metrics.Used },
            { "Free_MB", metrics.Free }
        };

        var result = new HealthCheckResult(status, null, null, data);

        return await Task.FromResult(result);
    }
}

/// <summary>
/// UmsHealthCheck
/// </summary>
public class UmsHealthCheck : IHealthCheck
{
    private readonly IUserAuthorizationService _userAuthorizationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UmsHealthCheck"/> class.
    /// </summary>
    /// <param name="userAuthorizationService">Set userAuthorizationService to get User's Attributes</param>
    public UmsHealthCheck(IUserAuthorizationService userAuthorizationService)
    {
        _userAuthorizationService = userAuthorizationService;
    }

    /// <summary>
    /// CheckHealthAsync
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var status = HealthStatus.Healthy;

        try
        {
            var statusCode = await _userAuthorizationService.CheckHealthAsync(cancellationToken);

            if (statusCode != HttpStatusCode.OK)
                status = HealthStatus.Degraded;
        }
        catch
        {
            status = HealthStatus.Degraded;
        }

        var result = new HealthCheckResult(status, null, null);

        return await Task.FromResult(result);
    }
}

/// <summary>
/// MemoryMetrics
/// </summary>
public class MemoryMetrics
{
    /// <summary>
    /// Gets or sets total memory
    /// </summary>
    public double Total { get; set; }

    /// <summary>
    /// Gets or sets used memory
    /// </summary>
    public double Used { get; set; }

    /// <summary>
    /// Gets or sets free memory
    /// </summary>
    public double Free { get; set; }
}

/// <summary>
/// MemoryMetricsClient
/// </summary>
public static class MemoryMetricsClient
{
    /// <summary>
    /// GetMetrics
    /// </summary>
    /// <returns></returns>
    public static MemoryMetrics GetMetrics()
    {
        var metrics = IsUnix() ? GetUnixMetrics() : GetWindowsMetrics();

        return metrics;
    }

    private static bool IsUnix() => RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
                                    RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

    private static MemoryMetrics GetWindowsMetrics()
    {
        var info = new ProcessStartInfo
        {
            FileName = "wmic",
            Arguments = "OS get FreePhysicalMemory,TotalVisibleMemorySize /Value",
            RedirectStandardOutput = true
        };

        using var process = Process.Start(info);
        var output = process.StandardOutput.ReadToEnd();

        var lines = output.Trim().Split("\n");
        var freeMemoryParts = lines[0].Split("=", StringSplitOptions.RemoveEmptyEntries);
        var totalMemoryParts = lines[1].Split("=", StringSplitOptions.RemoveEmptyEntries);

        var metrics = new MemoryMetrics
        {
            Total = Math.Round(double.Parse(totalMemoryParts[1]) / 1024, 0),
            Free = Math.Round(double.Parse(freeMemoryParts[1]) / 1024, 0)
        };
        metrics.Used = metrics.Total - metrics.Free;

        return metrics;
    }

    private static MemoryMetrics GetUnixMetrics()
    {
        var info = new ProcessStartInfo("free -m")
        {
            FileName = "/bin/bash",
            Arguments = "-c \"free -m\"",
            RedirectStandardOutput = true
        };

        using var process = Process.Start(info);
        var output = process.StandardOutput.ReadToEnd();

        var lines = output.Split("\n");
        var memory = lines[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);

        var metrics = new MemoryMetrics
        {
            Total = double.Parse(memory[1]),
            Used = double.Parse(memory[2]),
            Free = double.Parse(memory[3])
        };

        return metrics;
    }
}

/// <summary>
/// UiHealthReport
/// </summary>
public class UiHealthReport
{
    /// <summary>
    /// Gets or sets status
    /// </summary>
    public UiHealthStatus Status { get; set; }

    /// <summary>
    /// Gets or sets totalDuration
    /// </summary>
    public TimeSpan TotalDuration { get; set; }

    /// <summary>
    /// Gets entries
    /// </summary>
    public Dictionary<string, UiHealthReportEntry> Entries { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UiHealthReport"/> class.
    /// </summary>
    /// <param name="entries"></param>
    /// <param name="totalDuration"></param>
    public UiHealthReport(Dictionary<string, UiHealthReportEntry> entries, TimeSpan totalDuration)
    {
        Entries = entries;
        TotalDuration = totalDuration;
    }

    /// <summary>
    /// CreateFrom
    /// </summary>
    /// <param name="report"></param>
    /// <returns></returns>
    public static UiHealthReport CreateFrom(HealthReport report)
    {
        var uiReport = new UiHealthReport(new Dictionary<string, UiHealthReportEntry>(), report.TotalDuration)
        {
            Status = (UiHealthStatus)report.Status,
        };

        foreach (var (key, value) in report.Entries)
        {
            var entry = new UiHealthReportEntry
            {
                Data = value.Data,
                Description = value.Description,
                Duration = value.Duration,
                Status = (UiHealthStatus)value.Status
            };

            if (value.Exception != null)
            {
                var message = value.Exception?.Message;

                entry.Exception = message;
                entry.Description = value.Description ?? message;
            }

            uiReport.Entries.Add(key, entry);
        }

        return uiReport;
    }

    /// <summary>
    /// CreateFrom
    /// </summary>
    /// <param name="exception"></param>
    /// <param name="entryName"></param>
    /// <returns></returns>
    public static UiHealthReport CreateFrom(Exception exception, string entryName = "Endpoint")
    {
        var uiReport = new UiHealthReport(new Dictionary<string, UiHealthReportEntry>(), TimeSpan.FromSeconds(0))
        {
            Status = UiHealthStatus.Unhealthy,
        };

        uiReport.Entries.Add(entryName, new UiHealthReportEntry
        {
            Exception = exception.Message,
            Description = exception.Message,
            Duration = TimeSpan.FromSeconds(0),
            Status = UiHealthStatus.Unhealthy
        });

        return uiReport;
    }
}

/// <summary>
/// UiHealthStatus
/// </summary>
public enum UiHealthStatus
{
    /// <summary>
    /// Unhealthy
    /// </summary>
    Unhealthy = 0,

    /// <summary>
    /// Degraded
    /// </summary>
    Degraded = 1,

    /// <summary>
    /// Healthy
    /// </summary>
    Healthy = 2
}

/// <summary>
/// UiHealthReportEntry
/// </summary>
public class UiHealthReportEntry
{
    /// <summary>
    /// Gets or Sets Data
    /// </summary>
    /// <value></value>
    public IReadOnlyDictionary<string, object> Data { get; set; }

    /// <summary>
    /// Gets or Sets Description
    /// </summary>
    /// <value></value>
    public string Description { get; set; }

    /// <summary>
    /// Gets or Sets Duration
    /// </summary>
    /// <value></value>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Gets or Sets Exception
    /// </summary>
    /// <value></value>
    public string Exception { get; set; }

    /// <summary>
    /// Gets or Sets Status
    /// </summary>
    /// <value></value>
    public UiHealthStatus Status { get; set; }
}

/// <summary>
/// UiResponseWriter
/// </summary>
public static class UiResponseWriter
{
    private static readonly byte[] EmptyResponse = { (byte)'{', (byte)'}' };
    private static readonly Lazy<JsonSerializerOptions> Options = new (CreateJsonOptions);

    /// <summary>
    /// WriteHealthCheckUIResponse
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="report"></param>
    /// <returns></returns>
    public static async Task WriteHealthCheckUiResponse(HttpContext httpContext, HealthReport report)
    {
        if (report != null)
        {
            httpContext.Response.ContentType = Constants.HeaderJson;

            var uiReport = UiHealthReport
                .CreateFrom(report);

            await using var responseStream = new MemoryStream();

            if (uiReport.Status != UiHealthStatus.Healthy)
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            await JsonSerializer.SerializeAsync(responseStream, uiReport, Options.Value);
            await httpContext.Response.BodyWriter.WriteAsync(responseStream.ToArray());
        }
        else
        {
            await httpContext.Response.BodyWriter.WriteAsync(EmptyResponse);
        }
    }

    private static JsonSerializerOptions CreateJsonOptions()
    {
        var jsonSerializeOptions = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        jsonSerializeOptions.Converters.Add(new JsonStringEnumConverter());

        jsonSerializeOptions.Converters.Add(new TimeSpanConverter());

        return jsonSerializeOptions;
    }
}

/// <summary>
/// TimeSpanConverter
/// </summary>
internal class TimeSpanConverter : JsonConverter<TimeSpan>
{
    /// <summary>
    /// Read
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="typeToConvert"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return default;
    }

    /// <summary>
    /// Write
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="value"></param>
    /// <param name="options"></param>
    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
