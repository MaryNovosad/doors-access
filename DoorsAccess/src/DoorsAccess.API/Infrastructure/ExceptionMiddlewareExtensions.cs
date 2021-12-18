using System;
using System.Net;
using DoorsAccess.Domain.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace DoorsAccess.API.Infrastructure
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            var logger = app.ApplicationServices.GetService<ILogger<IApplicationBuilder>>();

            app.UseExceptionHandler(builder =>
            {
                builder.Run(async context =>
                {
                    var exception = context.Features
                        .Get<IExceptionHandlerPathFeature>()
                        ?.Error;

                    if (exception != null)
                    {
                        var statusCode = HttpStatusCode.InternalServerError;

                        if (exception is ArgumentException)
                        {
                            statusCode = HttpStatusCode.BadRequest;
                        }
                        else if (exception is DomainException e)
                        {
                            statusCode = e.ErrorType is DomainErrorType.NotFound or DomainErrorType.AccessDenied
                                ? HttpStatusCode.NotFound
                                : HttpStatusCode.BadRequest;
                        }

                        logger.LogError(exception.Message);

                        var shouldExposeExceptionMessage = WebApplication.CreateBuilder().Environment.IsDevelopment();
                        var errorResponse = new { Error = shouldExposeExceptionMessage ? exception.Message : "Something went wrong" };

                        context.Response.StatusCode = (int)statusCode;
                        await context.Response.WriteAsJsonAsync(errorResponse);
                    }
                });
            });
        }
    }
}
