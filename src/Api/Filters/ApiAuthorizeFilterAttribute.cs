// -----------------------------------------------------------------------------------
// ApiAuthorizeFilterAttribute.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetCa.Application.Common.Models;

namespace NetCa.Api.Filters;

/// <summary>
/// ApiAuthorizeFilterAttribute
/// </summary>
public class ApiAuthorizeFilterAttribute : ActionFilterAttribute
{
    /// <summary>
    /// OnActionExecutionAsync
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var environment = context.HttpContext.RequestServices.GetRequiredService<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>();
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<ApiAuthorizeFilterAttribute>>();
        var appSetting = context.HttpContext.RequestServices.GetRequiredService<AppSetting>();

        if (!(!environment.IsDevelopment() || appSetting.IsEnableAuth))
        {
            await next();
            return;
        }

        var actionDescriptor = context.ActionDescriptor;
        var ctrl = actionDescriptor.RouteValues["controller"].ToLower();
        var action = actionDescriptor.RouteValues["action"].ToLower();
        var permission = $"{appSetting.AuthorizationServer.Service}:{context.HttpContext.Request.Method.ToLower()}:{ctrl}_{action}";

        context.HttpContext.Items.Add("CurrentPolicyName", permission);

        try
        {
            var policy = GetPolicy(logger, appSetting, permission);

            if (policy is { IsCheck: true } or null)
            {
                var auth = context.HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();

                logger.LogDebug("Checking permission '{permission}'", permission);

                var permissionCheck = auth.AuthorizeAsync(context.HttpContext.User, null, permission).Result;

                if (!permissionCheck.Succeeded)
                    context.Result = new ForbidResult();
                else
                    await next();
            }
            else
            {
                await next();
            }
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Error when checking permission: {message}", e.Message);
            context.Result = new ForbidResult();
        }
    }

    private static Policy GetPolicy(ILogger logger, AppSetting appSetting, string policy)
    {
        logger.LogDebug("Get Policy '{policy}' if exists", policy);

        var policyList = appSetting.AuthorizationServer.Policy ?? new List<Policy>();
        return policyList.Count.Equals(0) ?
            null :
            policyList.FirstOrDefault(x => x.Name.ToLower().Equals(policy.ToLower()));
    }
}