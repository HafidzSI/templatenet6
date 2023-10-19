// -----------------------------------------------------------------------------------
// CustomValidator.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCa.Application.Common.Models;

/// <summary>
/// CustomValidator
/// </summary>
public static class CustomValidator
{
    internal static bool IsValidGuid(Guid? unValidatedGuid, bool ignoreNull = false)
    {
        try
        {
            if (unValidatedGuid != Guid.Empty && unValidatedGuid != null)
            {
                return Guid.TryParse(unValidatedGuid.ToString(), out Guid v);
            }

            return ignoreNull;
        }
        catch (Exception)
        {
            return false;
        }
    }

    internal static bool MustNullString(string arg)
    {
        return arg is not null;
    }

    internal static bool MustNullDate(DateTime? arg)
    {
        return arg is not null;
    }

    internal static bool IsValidDate(DateTime? arg)
    {
        return !arg.Equals(default(DateTime));
    }

    internal static bool MaximumLengthBase64(string arg, int maxLength = Constants.MaximumLengthBase64)
    {
        var length = Encoding.UTF8.GetByteCount(arg);

        return length <= maxLength;
    }

    internal static bool MaximumFileSize(long size, int maxLength = Constants.MaximumFileSize)
    {
        return size <= maxLength;
    }

    internal static bool IsValidFileName(string fileName, List<string> fileNameList)
    {
        return fileNameList.Contains(fileName);
    }

    internal static bool IsValidFileType(string fileType, List<string> fileTypeList)
    {
        return fileTypeList.Contains(fileType);
    }
}