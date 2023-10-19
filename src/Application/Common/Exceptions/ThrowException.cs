// -----------------------------------------------------------------------------------
// ThrowException.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace NetCa.Application.Common.Exceptions;

/// <summary>
/// ThrowException
/// </summary>
[Serializable]
public class ThrowException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ThrowException"/> class.
    /// </summary>
    /// <param name="message"></param>
    public ThrowException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ThrowException"/> class.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    protected ThrowException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}