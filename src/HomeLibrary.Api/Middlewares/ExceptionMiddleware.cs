using HomeLibrary.Application.Exceptions;
using HomeLibrary.Contracts.Responses;

namespace HomeLibrary.Api.Middlewares;

public sealed class ExceptionMiddleware(
    RequestDelegate next,
    ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (CsvValidationException exception)
        {
            await WriteResponse(
                httpContext,
                StatusCodes.Status400BadRequest,
                exception.Errors);
        }
        catch (DuplicateImportException exception)
        {
            await WriteResponse(
                httpContext,
                StatusCodes.Status409Conflict,
                new[] { exception.Message });
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "An unexpected error occurred while processing the request.");

            await WriteResponse(
                httpContext,
                StatusCodes.Status500InternalServerError,
                new[]
                {
                    "An unexpected error occurred. Please try again."
                });
        }
    }

    private static async Task WriteResponse(
        HttpContext httpContext,
        int statusCode,
        IEnumerable<string> errors)
    {
        if (httpContext.Response.HasStarted)
        {
            return;
        }

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        var response = ApiResponse<object>.Fail(
            errors.ToArray());

        await httpContext.Response.WriteAsJsonAsync(response);
    }
}