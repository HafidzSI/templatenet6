// -----------------------------------------------------------------------------------
// Changelog.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;

namespace NetCa.Domain.Entities;

/// <summary>
/// Changelog
/// </summary>
public record Changelog
{
    /// <summary>
    /// Gets or sets id
    /// </summary>
    /// <value></value>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets method
    /// </summary>
    /// <value></value>
    public string Method { get; set; }

    /// <summary>
    /// Gets or sets tableName
    /// </summary>
    /// <value></value>
    public string TableName { get; set; }

    /// <summary>
    /// Gets or sets tableName
    /// </summary>
    /// <value></value>
    public string KeyValues { get; set; }

    /// <summary>
    /// Gets or sets newValues
    /// </summary>
    /// <value></value>
    public string NewValues { get; set; }

    /// <summary>
    /// Gets or sets oldValues
    /// </summary>
    /// <value></value>
    public string OldValues { get; set; }

    /// <summary>
    /// Gets or sets changeBy
    /// </summary>
    /// <value></value>
    public string ChangeBy { get; set; }

    /// <summary>
    /// Gets or sets changeDate
    /// </summary>
    /// <value></value>
    public DateTime ChangeDate { get; set; }
}
