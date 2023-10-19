// -----------------------------------------------------------------------------------
// LocalAuthenticationHandler.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NetCa.Api.Handlers;

/// <summary>
/// LocalAuthenticationHandler
/// </summary>
public class LocalAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    /// <summary>
    /// The default user id, injected into the claims for all requests.
    /// </summary>
    public static readonly Guid UserId = Guid.NewGuid();

    /// <summary>
    /// The name of the authorizaton scheme that this handler will respond to.
    /// </summary>
    public const string AuthScheme = "LocalAuth";

    private readonly Claim _defaultUserIdClaim = new (
        ClaimTypes.NameIdentifier, UserId.ToString());

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalAuthenticationHandler"/> class.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    /// <param name="encoder"></param>
    /// <param name="clock"></param>
    public LocalAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    /// <summary>
    /// Marks all authentication requests as successful, and injects the
    /// default company id into the user claims.
    /// </summary>
    /// <returns></returns>
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authenticationTicket = new AuthenticationTicket(
            new ClaimsPrincipal(new ClaimsIdentity(new[] { _defaultUserIdClaim }, AuthScheme)),
            new AuthenticationProperties(),
            AuthScheme);
        return Task.FromResult(AuthenticateResult.Success(authenticationTicket));
    }
}