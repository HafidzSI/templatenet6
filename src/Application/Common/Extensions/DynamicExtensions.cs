// -----------------------------------------------------------------------------------
// DynamicExtensions.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Dynamic;

namespace NetCa.Application.Common.Extensions;

/// <summary>
/// ClassExtensions
/// </summary>
public static class DynamicExtensions
{
    /// <summary>
    /// AddProperty
    /// </summary>
    /// <param name="expando"></param>
    /// <param name="propertyName"></param>
    /// <param name="propertyValue"></param>
    /// <param name="replace"></param>
    public static void AddProperty(
        ExpandoObject expando, string propertyName, object propertyValue, bool replace = true)
    {
        var exDict = expando as IDictionary<string, object>;
        if (exDict.ContainsKey(propertyName))
        {
            if (replace)
                exDict[propertyName] = propertyValue;
        }
        else
        {
            exDict.Add(propertyName, propertyValue);
        }
    }
}