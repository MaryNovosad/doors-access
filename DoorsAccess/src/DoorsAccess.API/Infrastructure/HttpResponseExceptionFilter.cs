using System;
using System.Net;
using DoorsAccess.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DoorsAccess.API.Infrastructure;

public class HttpResponseExceptionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception != null)
        {
            var statusCode = HttpStatusCode.InternalServerError;

            if (context.Exception is ArgumentException)
            {
                statusCode = HttpStatusCode.BadRequest;
            }
            else if (context.Exception is DomainException e)
            {
                statusCode = e.ErrorType is DomainErrorType.NotFound or DomainErrorType.AccessDenied
                    ? HttpStatusCode.NotFound
                    : HttpStatusCode.BadRequest;
            }

            context.Result = new ObjectResult(context.Exception.Message)
            {
                StatusCode = (int?)statusCode
            };

            context.ExceptionHandled = true;
        }
    }
}