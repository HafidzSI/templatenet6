// -----------------------------------------------------------------------------------
// IFallbackHandler.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

namespace NetCa.Application.Common.Interfaces;

/// <summary>
/// Interface of fallback policy implementation.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public interface IFallbackHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// HandleFallback
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<TResponse> HandleFallback(TRequest request, CancellationToken cancellationToken);
}
