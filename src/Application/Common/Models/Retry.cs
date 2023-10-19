// -----------------------------------------------------------------------------------
// Retry.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;

namespace NetCa.Application.Common.Models;

/// <summary>
/// Retry
/// </summary>
public static class Retry
{
    /// <summary>
    /// Do
    /// </summary>
    /// <param name="action"></param>
    /// <param name="appSetting"></param>
    /// <param name="tmpl"></param>
    /// <param name="retryInterval"></param>
    /// <param name="maxAttemptCount"></param>
    public static void Do(Action<AppSetting, MsTeamTemplate> action, AppSetting appSetting, MsTeamTemplate tmpl, TimeSpan retryInterval, int maxAttemptCount = 3)
    {
        Do<object>(
            () =>
            {
                action(appSetting, tmpl);
                return null;
            },
            retryInterval,
            maxAttemptCount);
    }

    private static void Do<T>(Func<T> action, TimeSpan retryInterval, int maxAttemptCount = 3)
    {
        var exceptions = new List<Exception>();

        for (var attempted = 0; attempted < maxAttemptCount; attempted++)
        {
            try
            {
                if (attempted > 0)
                {
                    Thread.Sleep(retryInterval);
                }

                action();
                return;
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        throw new AggregateException(exceptions);
    }
}