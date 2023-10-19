// -----------------------------------------------------------------------------------
// UserAuthorizationHandlerService.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using NetCa.Application.Common.Models;

namespace NetCa.Infrastructure.Services;

/// <summary>
/// UserAuthorizationHandlerService
/// </summary>
public class UserAuthorizationHandlerService : AuthorizationHandler<Permission>
{
    private readonly ILogger<UserAuthorizationHandlerService> _logger;

    private static readonly HttpClient _httpClient = new (new HttpHandler(new HttpClientHandler())
    {
        UsingCircuitBreaker = true,
        UsingWaitRetry = true,
        RetryCount = 4,
        SleepDuration = 1000
    });

    private static readonly SemaphoreSlim SemaphoreSlim = new (1);

    /// <summary>
    /// Initializes a new instance of the <see cref="UserAuthorizationHandlerService"/> class.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="appSetting"></param>
    public UserAuthorizationHandlerService(ILogger<UserAuthorizationHandlerService> logger, AppSetting appSetting)
    {
        _logger = logger;

        _httpClient.BaseAddress ??= new Uri(appSetting.AuthorizationServer.Address);

        SemaphoreSlim.Wait();

        try
        {
            if (!_httpClient.DefaultRequestHeaders.Contains(appSetting.AuthorizationServer.Header))
                _httpClient.DefaultRequestHeaders.Add(appSetting.AuthorizationServer.Header, appSetting.AuthorizationServer.Secret);
        }
        catch
        {
            logger.LogDebug("Error when add request header");
        }

        SemaphoreSlim.Release();
    }

    /// <summary>
    /// HandleRequirementAsync
    /// </summary>
    /// <param name="context"></param>
    /// <param name="requirement"></param>
    /// <returns></returns>
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, Permission requirement)
    {
        var user = context.User;
        var userId = user.Claims.FirstOrDefault(i => i.Type == ClaimTypes.NameIdentifier)?.Value;
        var clientId = user.Claims.FirstOrDefault(i => i.Type == Constants.ClientId)?.Value;

        var url = $"api/authorize/{userId}/{clientId}/{requirement.PermissionName}";

        var response = await _httpClient.GetAsync(new Uri(_httpClient.BaseAddress + url));

        _logger.LogDebug("Response: \n {response}", response.ToString());

        if (response.IsSuccessStatusCode)
            context.Succeed(requirement);
    }
}

/// <summary>
/// Permission
/// </summary>
public class Permission : IAuthorizationRequirement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Permission"/> class.
    /// </summary>
    /// <param name="permissionName"></param>
    public Permission(string permissionName)
    {
        PermissionName = permissionName;
    }

    /// <summary>
    /// Gets or sets permissionName
    /// </summary>
    /// <value></value>
    public string PermissionName { get; set; }
}