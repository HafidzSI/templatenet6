// -----------------------------------------------------------------------------------
// MsTeamTemplate.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace NetCa.Application.Common.Models;

/// <summary>
/// MsTeamTemplate
/// </summary>
public record MsTeamTemplate
{
    /// <summary>
    /// Gets or sets summary
    /// </summary>
    public string Summary { get; set; } = Constants.MsTeamsSummaryError;

    /// <summary>
    /// Gets or sets themeColor
    /// </summary>
    public string ThemeColor { get; set; } = Constants.MsTeamsThemeColorError;

    /// <summary>
    /// Gets or sets sections
    /// </summary>
    public List<Section> Sections { get; set; }
}

/// <summary>
/// Section
/// </summary>
public record Section
{
    /// <summary>
    /// Gets or sets activityTitle
    /// </summary>
    /// <value></value>
    public string ActivityTitle { get; set; }

    /// <summary>
    /// Gets or sets activitySubtitle
    /// </summary>
    /// <value></value>
    public string ActivitySubtitle { get; set; }

    /// <summary>
    /// Gets or sets activityImage
    /// </summary>
    /// <value></value>
    public string ActivityImage { get; set; }

    /// <summary>
    /// Gets or sets facts
    /// </summary>
    /// <value></value>
    public List<Fact> Facts { get; set; }
}

/// <summary>
/// Fact
/// </summary>
public record Fact
{
    /// <summary>
    /// Gets or sets name
    /// </summary>
    /// <value></value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets value
    /// </summary>
    /// <value></value>
    public string Value { get; set; }
}

/// <summary>
/// CacheMsTeam
/// </summary>
public record CacheMsTeam
{
    /// <summary>
    /// Gets or sets counter
    /// </summary>
    public int Counter { get; set; } = 100;

    /// <summary>
    /// Gets hours
    /// </summary>
    public double Hours => 24;

    /// <summary>
    /// Gets or sets date
    /// </summary>
    public DateTime Date { get; set; } = DateTime.Now;
}