using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NetCa.Application.Common.Extensions;
using NetCa.Application.Common.Interfaces;
using NetCa.Application.Dtos;

namespace NetCa.Application.Development.Commands;

/// <summary>
/// CreateGroupCommand
/// </summary>
public class CreateGroupCommand : IRequest<DocumentRootJson<ResponseGroupRoleUmsVm>>
{
    /// <summary>
    /// Gets or sets ServiceId
    /// </summary>
    [BindRequired]
    public Guid ServiceId { get; set; }

    /// <summary>
    /// Gets or sets ApplicationId
    /// </summary>
    [BindRequired]
    public Guid ApplicationId { get; set; }

    /// <summary>
    /// Gets or sets ControllerList
    /// </summary>
    [BindRequired]
    public List<ControllerListDto> ControllerList { get; set; }

    /// <summary>
    /// Handling CreateGroupCommand
    /// </summary>
    public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, DocumentRootJson<ResponseGroupRoleUmsVm>>
    {
        private readonly IUserAuthorizationService _userAuthorizationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateGroupCommandHandler"/> class.
        /// </summary>
        /// <param name="userAuthorizationService">Set userAuthorizationService to get User's Attributes</param>
        public CreateGroupCommandHandler(IUserAuthorizationService userAuthorizationService)
        {
            _userAuthorizationService = userAuthorizationService;
        }

        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="request">
        /// The encapsulated request body
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token to perform cancel the operation
        /// </param>
        /// <returns>Add permission Group to UMS</returns>
        public async Task<DocumentRootJson<ResponseGroupRoleUmsVm>> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
        {
            var permissionList = await _userAuthorizationService.GetPermissionListAsync(request.ApplicationId, cancellationToken);
            var groupList = await _userAuthorizationService.GetGroupListAsync(request.ApplicationId, cancellationToken);

            var groupNameList = request.ControllerList
                .SelectMany(x => x.Groups)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            var responseGroups = new List<ResponseGroupRoleUms>();

            foreach (var groupName in groupNameList)
            {
                var permissionIds = new List<Guid>();
                var controllerMethodList = request.ControllerList
                    .Where(x => x.Groups.Contains(groupName))
                    .ToList();

                foreach (var item in controllerMethodList)
                {
                    var permissionId = permissionList
                        .Where(x => x.PermissionCode == $"{item.Controller}_{item.Action}" &&
                            x.RequestType.ToLower().Equals(item.Method.ToLower()) &&
                            x.Service.ServiceId.Equals(request.ServiceId))
                        .Select(x => x.PermissionId)
                        .ToList();

                    permissionIds.AddRange(permissionId);
                }

                var groupId = groupList.Where(x => x.GroupCode.ToLower().Contains(groupName.ToLower()))
                    .Select(x => x.GroupId)
                    .FirstOrDefault();

                var response = await _userAuthorizationService.CreateGroupAsync(
                    request.ApplicationId,
                    groupName,
                    groupId,
                    permissionIds,
                    cancellationToken);

                var actionList = controllerMethodList
                    .Select(x => x.Action)
                    .ToList();

                responseGroups.Add(new ResponseGroupRoleUms
                {
                    Name = groupName,
                    ItemList = actionList,
                    Response = response
                });
            }

            var result = new ResponseGroupRoleUmsVm { ResponseGroups = responseGroups };

            return JsonApiExtensions.ToJsonApi(result);
        }
    }
}