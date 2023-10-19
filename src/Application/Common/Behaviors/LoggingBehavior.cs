// -----------------------------------------------------------------------------------
// LoggingBehavior.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using NetCa.Application.Common.Interfaces;
using NetCa.Application.Common.Models;

namespace NetCa.Application.Common.Behaviors;

/// <summary>
/// LoggingBehavior
/// </summary>
/// <typeparam name="TRequest"></typeparam>
public class LoggingBehavior<TRequest> : IRequestPreProcessor<TRequest>
    where TRequest : notnull
{
    private readonly ILogger _logger;
    private readonly IUserAuthorizationService _userAuthorizationService;
    private readonly AppSetting _appSetting;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingBehavior{TRequest}"/> class.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="currentUserService"></param>
    /// <param name="appSetting"></param>
    public LoggingBehavior(
        ILogger<TRequest> logger,
        IUserAuthorizationService currentUserService,
        AppSetting appSetting)
    {
        _logger = logger;
        _userAuthorizationService = currentUserService;
        _appSetting = appSetting;
    }

    /// <summary>
    /// Process
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var user = _userAuthorizationService.GetAuthorizedUser();
        await Task.Delay(0, cancellationToken);
        _logger.LogDebug(
            "{Namespace} Request: {Name} {@UserId} {@UserName} {@Request}",
            _appSetting.App.Namespace,
            requestName,
            user.UserId,
            user.UserName,
            request);
    }
}