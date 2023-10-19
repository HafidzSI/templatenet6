// -----------------------------------------------------------------------------------
// ControllerExtension.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using NetCa.Api.Filters;
using NetCa.Application.Dtos;
using NSwag.Annotations;

namespace NetCa.Api.Extensions;

/// <summary>
/// ControllerExtension
/// </summary>
public static class ControllerExtension
{
    /// <summary>
    /// GetControllerList
    /// </summary>
    /// <returns></returns>
    public static List<ControllerListDto> GetControllerList()
    {
        var controllerActionList = typeof(Program).Assembly.GetTypes()
            .Where(type => typeof(ControllerBase).IsAssignableFrom(type) &&
                type.GetCustomAttributes<ServiceFilterAttribute>().Any(x => x.ServiceType.Equals(typeof(ApiAuthorizeFilterAttribute))) &&
                !type.GetCustomAttributes<ServiceFilterAttribute>().Any(x => x.ServiceType.Equals(typeof(ApiDevelopmentFilterAttribute))))
            .SelectMany(type => type
                .GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public)
                .Where(x => !x.GetCustomAttributes<CompilerGeneratedAttribute>().Any() &&
                    x.GetCustomAttributes<SwaggerResponseAttribute>().Any())
                .Select(x =>
                {
                    var apiVersion = type.GetCustomAttribute<ApiVersionAttribute>().Versions[0]
                        .ToString()
                        .Replace(".0", string.Empty);
                    var urlTemplate = type.GetCustomAttribute<RouteAttribute>().Template;

                    var method = string.Empty;
                    var urlExt = string.Empty;
                    var groups = new List<string>();

                    if (x.GetCustomAttribute<HttpGetAttribute>() is var httpAttr && httpAttr != null)
                    {
                        method = "GET";
                        urlExt = httpAttr.Template;
                    }
                    else if (x.GetCustomAttribute<HttpPostAttribute>() is var httpAttr1 && httpAttr1 != null)
                    {
                        method = "POST";
                        urlExt = httpAttr1.Template;
                    }
                    else if (x.GetCustomAttribute<HttpPutAttribute>() is var httpAttr2 && httpAttr2 != null)
                    {
                        method = "PUT";
                        urlExt = httpAttr2.Template;
                    }
                    else if (x.GetCustomAttribute<HttpDeleteAttribute>() is var httpAttr3 && httpAttr3 != null)
                    {
                        method = "DELETE";
                        urlExt = httpAttr3.Template;
                    }
                    else if (x.GetCustomAttribute<HttpPatchAttribute>() is var httpAttr4 && httpAttr4 != null)
                    {
                        method = "PATCH";
                        urlExt = httpAttr4.Template;
                    }

                    if (x.GetCustomAttribute<ApiUserGroupCustomAttribute>() is var groupAttr && groupAttr != null)
                        groups = groupAttr.Group.ToList();

                    urlExt = string.IsNullOrEmpty(urlExt) ? string.Empty : $"/{urlExt}";

                    return new ControllerListDto
                    {
                        Controller = type.Name.Replace("Controller", string.Empty).ToLower(),
                        Action = x.Name.ToLower(),
                        Url = $"/{urlTemplate.Replace("{version:apiVersion}", apiVersion)}{urlExt}",
                        Method = method,
                        Groups = groups,
                    };
                }))
            .OrderBy(x => x.Controller)
            .ThenBy(x => x.Action)
            .ToList();

        return controllerActionList;
    }
}
