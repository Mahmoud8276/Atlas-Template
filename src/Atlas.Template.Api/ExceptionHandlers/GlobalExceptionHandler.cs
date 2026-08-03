using Atlas.Template.Core.Exceptions;
using Atlas.Template.Services.Helpers;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Atlas.Template.Api.ExceptionHandlers
{
    public sealed class GlobalExceptionHandler 
        : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IProblemDetailsService _problemDetailsService;

        public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IProblemDetailsService problemDetailsService)
        {
            _logger = logger;
            _problemDetailsService = problemDetailsService;
        }



        public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception,
                "Unhandled exception occurred. TraceId: {TraceId}",
                httpContext.TraceIdentifier);

            var(statusCode, title) = MapExceptions(exception);

            httpContext.Response.StatusCode = statusCode;

            var problemDetails = ProblemDetailsHelper.Create(
                httpContext,
                statusCode,
                title,
                GetSafeErrorMessage(exception, httpContext));


            return _problemDetailsService.TryWriteAsync(new ProblemDetailsContext()
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails
            });
        }

        private (int statusCode, string title) MapExceptions(Exception exception) => exception switch
        {
            AppException AppEx => ((int)AppEx.StatusCode, AppEx.Message),
            ArgumentNullException => (StatusCodes.Status400BadRequest, "Invalid argument provided"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Invalid argument provided"),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            _ =>(StatusCodes.Status500InternalServerError, "An unexpected error occurred")
        };

        private static string? GetSafeErrorMessage(Exception exception, HttpContext context)
        {
            var env = context.RequestServices.GetRequiredService<IHostEnvironment>();
            if (env.IsDevelopment())
            {
                return exception.Message;
            }

            return exception is AppException ? exception.Message : null;
        }
    }
}
