using OutreachFlow.Application.Common;

namespace OutreachFlow.Api.Errors;

public sealed record ApiError(string ErrorCode, string Message);

public static class ApiEndpoint
{
    public static IResult NotFound(string message)
    {
        return Results.NotFound(new ApiError("NOT_FOUND", message));
    }

    public static async Task<IResult> HandleAsync(Func<Task<IResult>> action)
    {
        try
        {
            return await action();
        }
        catch (ApplicationValidationException exception)
        {
            return Results.BadRequest(new ApiError(exception.ErrorCode, exception.Message));
        }
        catch (ApplicationConflictException exception)
        {
            return Results.Conflict(new ApiError(exception.ErrorCode, exception.Message));
        }
        catch (ApplicationNotFoundException exception)
        {
            return Results.NotFound(new ApiError(exception.ErrorCode, exception.Message));
        }
    }
}
