// -----------------------------------------------------------------------------------
// ICurrentUserService.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NetCa.Application.Common.Models;
using NetCa.Application.Dtos;

namespace NetCa.Application.Common.Interfaces;

/// <summary>
/// IUserAuthorizationService
/// </summary>
public interface IUserAuthorizationService
{
    /// <summary>
    /// GetUserId
    /// </summary>
    /// <returns></returns>
    Guid GetUserId();

    /// <summary>
    /// GetUserName
    /// </summary>
    /// <returns></returns>
    string GetUserName();

    /// <summary>
    /// GetUserNameSystem
    /// </summary>
    /// <returns></returns>
    string GetUserNameSystem();

    /// <summary>
    /// GetCustomerCode
    /// </summary>
    /// <returns></returns>
    string GetCustomerCode();

    /// <summary>
    /// GetClientId
    /// </summary>
    /// <returns></returns>
    string GetClientId();

    /// <summary>
    /// GetAuthorizedUser
    /// </summary>
    /// <returns></returns>
    AuthorizedUser GetAuthorizedUser();

    /// <summary>
    /// GetAuthorizedUserSystem
    /// </summary>
    /// <returns></returns>
    AuthorizedUser GetAuthorizedUserSystem();

    /// <summary>
    /// GetUserAttributesAsync
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Dictionary<string, List<string>>> GetUserAttributesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// GetUsersByAttributes
    /// </summary>
    /// <param name="serviceName"></param>
    /// <param name="attributes"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<UserClientIdInfo>> GetUsersByAttributesAsync(
        string serviceName, Dictionary<string, IList<string>> attributes, CancellationToken cancellationToken);

    /// <summary>
    /// GetNotifiedUsersAsync
    /// </summary>
    /// <param name="serviceName"></param>
    /// <param name="attributes"></param>
    /// <param name="permission"></param>
    /// <param name="clientIds"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<UserClientIdInfo>> GetNotifiedUsersAsync(
        string serviceName,
        Dictionary<string, List<string>> attributes,
        string permission,
        IEnumerable<string> clientIds,
        CancellationToken cancellationToken);

    /// <summary>
    /// GetDevicesIdByUserId
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="clientId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<UserDeviceInfo>> GetDevicesIdByUserIdAsync(
        Guid userId,
        string clientId,
        CancellationToken cancellationToken);

    /// <summary>
    /// GetEmailByUserIdAsync
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<UserEmailInfo> GetEmailByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// GetGeneralParameterAsync
    /// </summary>
    /// <param name="generalParameterCode"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<GeneralParameterInfo> GetGeneralParameterAsync(
        string generalParameterCode, CancellationToken cancellationToken);

    /// <summary>
    /// GetUserServicesAsync
    /// </summary>
    /// <param name="generalParameterCode"></param>
    /// <returns></returns>
    Task<List<string>> GetUserServicesAsync(string generalParameterCode);

    /// <summary>
    /// GetUserAttributesAndServicesAsync
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Dictionary<string, List<string>>> GetUserAttributesAndServicesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// GetUserListAsync
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<UserManagementUser>> GetUserListAsync(CancellationToken cancellationToken);

    /// <summary>
    /// DeleteDeviceIdAsync
    /// </summary>
    /// <param name="deviceId"></param>
    /// <returns></returns>
    Task DeleteDeviceIdAsync(string deviceId);

    /// <summary>
    /// GetPermissionListAsync
    /// </summary>
    /// <param name="applicationId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<PermissionUms>> GetPermissionListAsync(Guid applicationId, CancellationToken cancellationToken);

    /// <summary>
    /// CreatePermissionsAsync
    /// </summary>
    /// <param name="applicationId"></param>
    /// <param name="permissions"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<ResponsePermissionUmsDto>> CreatePermissionsAsync(Guid applicationId, List<PermissionDto> permissions, CancellationToken cancellationToken);

    /// <summary>
    /// GetGroupListAsync
    /// </summary>
    /// <param name="applicationId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<GroupUms>> GetGroupListAsync(Guid applicationId, CancellationToken cancellationToken);

    /// <summary>
    /// CreateGroupAsync
    /// </summary>
    /// <param name="applicationId"></param>
    /// <param name="groupCode"></param>
    /// <param name="groupId"></param>
    /// <param name="permissionIds"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ResponseGroupRoleUmsDto> CreateGroupAsync(
        Guid applicationId, string groupCode, Guid? groupId, List<Guid> permissionIds, CancellationToken cancellationToken);

    /// <summary>
    /// GetRoleListAsync
    /// </summary>
    /// <param name="applicationId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<RoleUms>> GetRoleListAsync(Guid applicationId, CancellationToken cancellationToken);

    /// <summary>
    /// CreateRoleAsync
    /// </summary>
    /// <param name="applicationId"></param>
    /// <param name="roleCode"></param>
    /// <param name="roleId"></param>
    /// <param name="groupIds"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ResponseGroupRoleUmsDto> CreateRoleAsync(
        Guid applicationId, string roleCode, Guid? roleId, List<Guid> groupIds, CancellationToken cancellationToken);

    /// <summary>
    /// Health Check
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<HttpStatusCode> CheckHealthAsync(CancellationToken cancellationToken);
}

#pragma warning disable
public record PermissionDto
{
    public Guid ServiceId { get; set; }
    public string PermissionCode { get; set; }
    public string Path { get; set; }
    public string PostDescription { get; set; } = string.Empty;
    public string GetDescription { get; set; } = string.Empty;
    public string PutDescription { get; set; } = string.Empty;
    public string PatchDescription { get; set; } = string.Empty;
    public string DeleteDescription { get; set; } = string.Empty;
    public bool? PostStatus { get; set; }
    public bool? GetStatus { get; set; }
    public bool? PutStatus { get; set; }
    public bool? PatchStatus { get; set; }
    public bool? DeleteStatus { get; set; }
}

public record PermissionUms
{
    public Guid PermissionId { get; set; }
    public string PermissionCode { get; set; }
    public string RequestType { get; set; }
    public string Path { get; set; }
    public string Description { get; set; }
    public string Authorization { get; set; }
    public bool Status { get; set; }
    public Guid ServiceId { get; set; }
    public ServiceUms Service { get; set; }
}

public record ServiceUms
{
    public Guid ServiceId { get; set; }
    public string ServiceCode { get; set; }
    public string HostName { get; set; }
    public string ApiScope { get; set; }
    public bool Status { get; set; }
}

public record GroupUms
{
    public Guid? GroupId { get; set; }
    public Guid? ApplicationId { get; set; }
    public string GroupCode { get; set; }
    public bool? Status { get; set; }
    public List<Guid> PermissionIds { get; set; }
}

public record RoleUms
{
    public Guid? RoleId { get; set; }
    public Guid? ApplicationId { get; set; }
    public string RoleCode { get; set; }
    public bool PublicInformation { get; set; }
    public int RoleLevel { get; set; }
    public string RoleType { get; set; }
    public List<Guid> GroupIds { get; set; } = new List<Guid>();
    public List<Guid> AttributeIds { get; set; } = new List<Guid>();
    public bool Status { get; set; }
    public DateTime UpdatedDate { get; set; }
}
