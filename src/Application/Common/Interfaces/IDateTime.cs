// -----------------------------------------------------------------------------------
// IDateTime.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;

namespace NetCa.Application.Common.Interfaces;

/// <summary>
/// IDateTime
/// </summary>
public interface IDateTime
{
    /// <summary>
    /// Gets now
    /// </summary>
    /// <value></value>
    DateTime Now { get; }

    /// <summary>
    /// Gets utcNow
    /// </summary>
    /// <value></value>
    DateTime UtcNow { get; }
}