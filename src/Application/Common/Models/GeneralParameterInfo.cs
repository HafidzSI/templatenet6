// -----------------------------------------------------------------------------------
// GeneralParameterInfo.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;

namespace NetCa.Application.Common.Models;

/// <summary>
/// GeneralParameterInfo
/// </summary>
public class GeneralParameterInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GeneralParameterInfo"/> class.
    /// </summary>
    public GeneralParameterInfo()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GeneralParameterInfo"/> class.
    /// GeneralParameterInfo
    /// </summary>
    /// <param name="generalParameterValue3"></param>
    public GeneralParameterInfo(string generalParameterValue3)
    {
        GeneralParameterValue3 = generalParameterValue3;
    }

    /// <summary>
    /// Gets or sets generalParameterId
    /// </summary>
    /// <value></value>
    public Guid GeneralParameterId { get; set; }

    /// <summary>
    /// Gets or sets generalParameterCode
    /// </summary>
    /// <value></value>
    public string GeneralParameterCode { get; set; }

    /// <summary>
    /// Gets or sets generalParameterValue1
    /// </summary>
    /// <value></value>
    public string GeneralParameterValue1 { get; set; }

    /// <summary>
    /// Gets or sets generalParameterValue2
    /// </summary>
    /// <value></value>
    public string GeneralParameterValue2 { get; set; }

    /// <summary>
    /// Gets or sets generalParameterValue3
    /// </summary>
    /// <value></value>
    public string GeneralParameterValue3 { get; set; }

    /// <summary>
    /// Gets or sets generalParameterValue4
    /// </summary>
    /// <value></value>
    public string GeneralParameterValue4 { get; set; }

    /// <summary>
    /// Gets or sets generalParameterValue5
    /// </summary>
    /// <value></value>
    public string GeneralParameterValue5 { get; set; }
}

/// <summary>
/// UserDeviceInfo
/// </summary>
public class UserDeviceInfo
{
    /// <summary>
    /// Gets or sets deviceId
    /// </summary>
    /// <value></value>
    public string DeviceId { get; set; }

    /// <summary>
    /// Gets or sets appVersion
    /// </summary>
    /// <value></value>
    public string AppVersion { get; set; }

    /// <summary>
    /// Gets or sets osVersion
    /// </summary>
    /// <value></value>
    public string OsVersion { get; set; }
}

/// <summary>
/// UserEmailInfo
/// </summary>
public class UserEmailInfo
{
    /// <summary>
    /// Gets or sets email
    /// </summary>
    /// <value></value>
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets firstName
    /// </summary>
    /// <value></value>
    public string FirstName { get; set; }

    /// <summary>
    /// Gets or sets lastName
    /// </summary>
    /// <value></value>
    public string LastName { get; set; }
}

/// <summary>
/// UserClientIdInfo
/// </summary>
public class UserClientIdInfo
{
    /// <summary>
    /// Gets or sets userClientIdInfo
    /// </summary>
    /// <value></value>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets clientId
    /// </summary>
    /// <value></value>
    public string ClientId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether isCustomer
    /// </summary>
    /// <value></value>
    public bool IsCustomer { get; set; }
}

/// <summary>
/// UserMangementUser
/// </summary>
public class UserManagementUser
{
    /// <summary>
    /// Gets or sets userId
    /// </summary>
    /// <value></value>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets userName
    /// </summary>
    /// <value></value>
    public string UserName { get; set; }

    /// <summary>
    /// Gets or sets email
    /// </summary>
    /// <value></value>
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets firstName
    /// </summary>
    /// <value></value>
    public string FirstName { get; set; }

    /// <summary>
    /// Gets or sets lastName
    /// </summary>
    /// <value></value>
    public string LastName { get; set; }

    /// <summary>
    /// Gets or sets customerCode
    /// </summary>
    /// <value></value>
    public string CustomerCode { get; set; }

    /// <summary>
    /// Gets or sets contactNumber
    /// </summary>
    /// <value></value>
    public string ContactNumber { get; set; }

    /// <summary>
    /// Gets or sets createdBy
    /// </summary>
    /// <value></value>
    public string CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets createdDate
    /// </summary>
    /// <value></value>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets updatedBy
    /// </summary>
    /// <value></value>
    public string UpdatedBy { get; set; }

    /// <summary>
    /// Gets or sets updatedDate
    /// </summary>
    /// <value></value>
    public DateTime UpdatedDate { get; set; }

    /// <summary>
    /// Gets or sets forgotPasswordToken
    /// </summary>
    /// <value></value>
    public string ForgotPasswordToken { get; set; }

    /// <summary>
    /// Gets or sets forgotPasswordTokenExpiryDate
    /// </summary>
    /// <value></value>
    public string ForgotPasswordTokenExpiryDate { get; set; }
}

/// <summary>
/// AuthorizedUser
/// </summary>
public class AuthorizedUser
{
    /// <summary>
    /// Gets or sets userId
    /// </summary>
    /// <value></value>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets userName
    /// </summary>
    /// <value></value>
    public string UserName { get; set; }

    /// <summary>
    /// Gets or sets UserFullName
    /// </summary>
    /// <value></value>
    public string UserFullName { get; set; }

    /// <summary>
    /// Gets or sets customerCode
    /// </summary>
    /// <value></value>
    public string CustomerCode { get; set; }

    /// <summary>
    /// Gets or sets clientId
    /// </summary>
    /// <value></value>
    public string ClientId { get; set; }

    /// <summary>
    /// Gets or sets roleLevel
    /// </summary>
    /// <value></value>
    public int RoleLevel { get; set; }
}