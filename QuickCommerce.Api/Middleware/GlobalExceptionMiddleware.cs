using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QuickCommerce.Core.Common;
using QuickCommerce.Core.Exceptions;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuickCommerce.Api.Middleware
{
    /// <summary>
    /// Enterprise Global Exception Middleware
    /// Handles all exceptions consistently across the API.
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger,
            IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(
            HttpContext context,
            Exception exception)
        {
            context.Response.ContentType = "application/json";

            ApiResponse<object> response;

            switch (exception)
            {
                case ValidationException validationException:

                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                    response = ApiResponse<object>.FailureResponse(
                        validationException.Message,
                        validationException.Errors);

                    break;

                case BusinessException businessException:

                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                    response = ApiResponse<object>.FailureResponse(
                        businessException.Message);

                    break;

                case NotFoundException notFoundException:

                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;

                    response = ApiResponse<object>.FailureResponse(
                        notFoundException.Message);

                    break;

                case UnauthorizedAccessException unauthorized:

                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;

                    response = ApiResponse<object>.FailureResponse(
                        unauthorized.Message);

                    break;

                default:

                    context.Response.StatusCode =
                        (int)HttpStatusCode.InternalServerError;

                    if (_environment.IsDevelopment())
                    {
                        response = ApiResponse<object>.FailureResponse(
                            exception.Message,
                            new List<string>
                            {
                                exception.InnerException?.Message ?? "",
                                exception.StackTrace ?? ""
                            });
                    }
                    else
                    {
                        response = ApiResponse<object>.FailureResponse(
                            "An unexpected error occurred.");
                    }

                    break;
            }

            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }
    }
}