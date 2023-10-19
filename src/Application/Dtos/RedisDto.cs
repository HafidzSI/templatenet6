// -----------------------------------------------------------------------------------
// RedisDto.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

namespace NetCa.Application.Dtos;

/// <summary>
/// RedisDto
/// </summary>
public record RedisDto
{
    /// <summary>
    /// Gets or sets code
    /// </summary>
    /// <value></value>
    public string Key { get; set; }

    /// <summary>
    /// Gets or sets desc
    /// </summary>
    /// <value></value>
    public string Value { get; set; }
}