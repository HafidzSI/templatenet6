// -----------------------------------------------------------------------------------
// ApiUserGroupCustomAttribute.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;

namespace NetCa.Api.Filters;

/// <summary>
/// ApiUserGroupCustomAttribute
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public abstract class ApiUserGroupCustomAttribute : Attribute
{
    /// <summary>
    /// Gets or sets group
    /// </summary>
    public string[] Group { get; set; }
}
