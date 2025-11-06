using System.Net;
using System.Text.Json;
using IdentityServer.Common.Exceptions;
using IdentityServer.Common.Models;

namespace IdentityServer.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var response = new ApiResponse<object>();

        switch (exception)
        {
            case ValidationException validationEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = ApiResponse<object>.ErrorResponse(validationEx.Message);
                break;
            case UnauthorizedException unauthorizedEx:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response = ApiResponse<object>.ErrorResponse(unauthorizedEx.Message);
                break;
            case InvalidGrantException invalidGrantEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = ApiResponse<object>.ErrorResponse(invalidGrantEx.Message);
                break;
            case BusinessLogicException businessEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = ApiResponse<object>.ErrorResponse(businessEx.Message);
                break;
            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = ApiResponse<object>.ErrorResponse("An error occurred while processing your request");
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}