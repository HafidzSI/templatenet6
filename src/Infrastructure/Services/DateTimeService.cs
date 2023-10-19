// -----------------------------------------------------------------------------------
// DateTimeService.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using NetCa.Application.Common.Interfaces;

namespace NetCa.Infrastructure.Services;

/// <summary>
/// DateTimeService
/// </summary>
public class DateTimeService : IDateTime
{
    /// <summary>
    /// Gets now
    /// </summary>
    public DateTime Now => DateTime.Now;

    /// <summary>
    /// Gets utcNow
    /// </summary>
    public DateTime UtcNow => DateTime.UtcNow;
}