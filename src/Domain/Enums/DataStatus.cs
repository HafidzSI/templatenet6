// -----------------------------------------------------------------------------------
// DataStatus.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

namespace NetCa.Domain.Enums;

/// <summary>
/// DataStatus
/// </summary>
public enum DataStatus
{
    /// <summary>
    /// Blank
    /// </summary>
    Blank,

    /// <summary>
    /// Need_Approval
    /// </summary>
    Need_Approval,

    /// <summary>
    /// Approved
    /// </summary>
    Approved,

    /// <summary>
    /// Cancelled
    /// </summary>
    Cancelled,

    /// <summary>
    /// Rejected
    /// </summary>
    Rejected,

    /// <summary>
    /// Request_Edit
    /// </summary>
    Request_Edit
}