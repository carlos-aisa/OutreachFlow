using OutreachFlow.Application.Common;
using Microsoft.AspNetCore.Http;

namespace OutreachFlow.Api.Errors;

public sealed record ApiError(string ErrorCode, string Message);

public static class ApiEndpoint
{
    private static IHttpContextAccessor? httpContextAccessor;

    public static void ConfigureHttpContextAccessor(IHttpContextAccessor accessor)
    {
        httpContextAccessor = accessor;
    }

    public static IResult NotFound(string message)
    {
        return Results.NotFound(new ApiError("NOT_FOUND", Localize(message)));
    }

    public static async Task<IResult> HandleAsync(Func<Task<IResult>> action)
    {
        try
        {
            return await action();
        }
        catch (ApplicationValidationException exception)
        {
            return Results.BadRequest(new ApiError(exception.ErrorCode, Localize(exception.Message)));
        }
        catch (ApplicationConflictException exception)
        {
            return Results.Conflict(new ApiError(exception.ErrorCode, Localize(exception.Message)));
        }
        catch (ApplicationNotFoundException exception)
        {
            return Results.NotFound(new ApiError(exception.ErrorCode, Localize(exception.Message)));
        }
    }

    private static string Localize(string message)
    {
        var localizer = httpContextAccessor?
            .HttpContext?
            .RequestServices
            .GetService(typeof(IApiErrorLocalizer)) as IApiErrorLocalizer;

        return localizer?.Localize(message) ?? message;
    }
}
