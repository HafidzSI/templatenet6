// -----------------------------------------------------------------------------------
// IProducerService.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Threading.Tasks;

namespace NetCa.Application.Common.Interfaces;

/// <summary>
/// IProducerService
/// </summary>
public interface IProducerService
{
    /// <summary>
    /// SendAsync
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    Task<bool> SendAsync(string topic, string message);
}