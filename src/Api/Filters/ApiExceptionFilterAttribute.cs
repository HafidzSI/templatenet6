// -----------------------------------------------------------------------------------
// ApiExceptionFilterAttribute.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using NetCa.Application.Common.Exceptions;
using NotFoundException = NetCa.Application.Common.Exceptions.NotFoundException;
using ValidationException = NetCa.Application.Common.Exceptions.ValidationException;

namespace NetCa.Api.Filters;

/// <summary>
/// ApiExceptionFilterAttribute
/// </summary>
public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
{
    private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiExceptionFilterAttribute"/> class.
    /// </summary>
    /// <param name="loggerFactory"></param>
    public ApiExceptionFilterAttribute(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<ApiExceptionFilterAttribute>();
        _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
        {
            { typeof(OperationCanceledException), HandleOperationCancelledException },
            { typeof(ValidationException), HandleValidationException },
            { typeof(NotFoundException), HandleNotFoundException },
            { typeof(UnauthorizedAccessException), HandleUnauthorizedAccessException },
        };
    }

    /// <summary>
    /// OnException
    /// </summary>
    /// <param name="context"></param>
    public override void OnException(ExceptionContext context)
    {
        HandleException(context);

        base.OnException(context);
    }

    private void HandleException(ExceptionContext context)
    {
        var type = context.Exception.GetType();

        if (_exceptionHandlers.TryGetValue(type, out var handler))
        {
            handler.Invoke(context);
        }
    }

    private void HandleOperationCancelledException(ExceptionContext context)
    {
        var details = new ProblemDetails
        {
            Status = 499,
            Title = "Client Closed Request",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3"
        };

        _logger.LogWarning("Client Closed Request");

        context.Result = new ObjectResult(details)
        {
            StatusCode = StatusCodes.Status400BadRequest
        };

        context.ExceptionHandled = true;
    }

    private void HandleValidationException(ExceptionContext context)
    {
        var exception = (ValidationException)context.Exception;

        var details = new ValidationProblemDetails(exception.Errors)
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        };

        _logger.LogWarning("Validation Exception");

        context.Result = new ObjectResult(details)
        {
            StatusCode = StatusCodes.Status400BadRequest
        };

        context.ExceptionHandled = true;
    }

    private void HandleNotFoundException(ExceptionContext context)
    {
        var exception = (NotFoundException)context.Exception;

        var details = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Title = "The specified resource was not found.",
            Detail = exception.Message
        };

        _logger.LogWarning("Not Found Exception");

        context.Result = new ObjectResult(details)
        {
            StatusCode = StatusCodes.Status404NotFound
        };

        context.ExceptionHandled = true;
    }

    private void HandleUnauthorizedAccessException(ExceptionContext context)
    {
        var details = new ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Unauthorized",
            Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
        };

        _logger.LogWarning("Unauthorized Access Exception");

        context.Result = new ObjectResult(details)
        {
            StatusCode = StatusCodes.Status401Unauthorized
        };

        context.ExceptionHandled = true;
    }
}