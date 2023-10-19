// -----------------------------------------------------------------------------------
// CustomAssemblyLoadContext.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Runtime.Loader;

namespace NetCa.Infrastructure.Files;

/// <summary>
/// CustomAssemblyLoadContext
/// </summary>
public class CustomAssemblyLoadContext : AssemblyLoadContext
{
    /// <summary>
    /// LoadUnmanagedLibrary
    /// </summary>
    /// <param name="absolutePath"></param>
    /// <returns></returns>
    public IntPtr LoadUnmanagedLibrary(string absolutePath)
    {
        return LoadUnmanagedDll(absolutePath);
    }

    /// <summary>
    /// LoadUnmanagedDll
    /// </summary>
    /// <param name="unmanagedDllName"></param>
    /// <returns></returns>
    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        return LoadUnmanagedDllFromPath(unmanagedDllName);
    }

    /// <summary>
    /// Load
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <returns></returns>
    protected override Assembly Load(AssemblyName assemblyName)
    {
        throw new NotImplementedException();
    }
}