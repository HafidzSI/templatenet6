// -----------------------------------------------------------------------------------
// StringExtensions.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NetCa.Application.Common.Extensions;

/// <summary>
/// StringExtensions
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// AsRelativeResource
    /// </summary>
    /// <param name="resourcePath"></param>
    /// <returns></returns>
    public static string AsRelativeResource(this string resourcePath)
    {
        return resourcePath.StartsWith('/') ? resourcePath[1..] : resourcePath;
    }

    /// <summary>
    /// Truncate
    /// </summary>
    /// <param name="s"></param>
    /// <param name="maxChars"></param>
    /// <returns></returns>
    public static string Truncate(string s, int maxChars)
    {
        if (s == null)
            return null;

        return s.Length <= maxChars ? s : s[..maxChars];
    }

    /// <summary>
    /// SplitArrayString
    /// </summary>
    /// <param name="arrayString"></param>
    /// <returns></returns>
    public static string SplitArrayString(string arrayString)
    {
        var result = new StringBuilder();
        var splitString = arrayString.Split(' ');

        if (splitString.Length > 1)
        {
            var first = true;
            foreach (var category in splitString)
            {
                if (first)
                {
                    first = false;
                    continue;
                }

                result.Append(category + " ");
            }

            return result.ToString();
        }

        return arrayString;
    }

    /// <summary>
    /// JsonRepair
    /// </summary>
    /// <param name="value"></param>
    /// <param name="regex"></param>
    /// <returns></returns>
    public static string JsonRepair(string value, string regex)
    {
        var regexs = regex != null ? regex.Split(",").ToList() : new List<string>();

        return regexs
            .Select(item => Regex.Escape($@"{item}"))
            .Aggregate(value, (current, regexReplace) => Regex.Replace(current, regexReplace, ""));
    }

    /// <summary>
    /// ReplaceLast
    /// </summary>
    /// <param name="find"></param>
    /// <param name="replace"></param>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ReplaceLast(string find, string replace, string str)
    {
        var lastIndex = str.LastIndexOf(find, StringComparison.Ordinal);

        if (lastIndex == -1)
            return str;

        var beginString = str[..lastIndex];
        var endString = str[(lastIndex + find.Length) ..];

        return beginString + replace + endString;
    }

    /// <summary>
    /// ToCamelCase
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToCamelCase(this string value)
    {
        return char.ToLowerInvariant(value[0]) + value[1..];
    }

    /// <summary>
    /// GetQueryString
    /// </summary>
    /// <param name="url"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string GetQueryString(string url, object obj)
    {
        var properties = obj.GetType().GetProperties()
            .Where(x => x.CanRead && x.GetValue(obj, null) != null)
            .ToDictionary(x => x.Name, x => x.GetValue(obj, null));

        var propertyNames = properties
            .Where(x => x.Value is not string && x.Value is IEnumerable)
            .Select(x => x.Key)
            .ToList();

        var p = new List<string>();

        foreach (var key in propertyNames)
        {
            var valueType = properties[key]?.GetType();
            if (valueType != null)
            {
                var valueElemType = valueType is { IsGenericType: true } ?
                    valueType.GetGenericArguments()[0] :
                    valueType.GetElementType();

                if (valueElemType != null && (valueElemType.IsPrimitive || valueElemType == typeof(string)))
                {
                    var enumerable = properties[key] as IEnumerable;
                    p.AddRange(from object item in enumerable select key + "=" + item);
                }
            }

            properties.Remove(key);
        }

        var result = new StringBuilder(url);
        result.Append("?");
        result.Append(string.Join("&", properties
            .Select(x =>
            {
                var (key, value) = x;

                if (value != null)
                {
                    return string.Concat(
                        Uri.EscapeDataString(key),
                        "=",
                        Uri.EscapeDataString(value.ToString() ?? string.Empty));
                }

                return null;
            })));

        result.Append(string.Join("&", p));

        return result.ToString();
    }

    /// <summary>
    /// Get Attribute List
    /// </summary>
    /// <param name="dict"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static List<string> GetAttribute(this Dictionary<string, List<string>> dict, string key)
    {
        if (dict == null || !dict.ContainsKey(key))
            return new List<string>();

        return dict[key].Distinct().ToList();
    }

    /// <summary>
    /// NullSafeToLower
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string NullSafeToLower(this string value)
    {
        if (value == null)
            return null;

        return value.ToLower();
    }
}
