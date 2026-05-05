using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OutreachFlow.Web.Common;

internal static class ApiClientJson
{
    public static readonly JsonSerializerOptions Options = CreateOptions();

    public static async Task<T> ReadRequiredAsync<T>(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            var result = await ReadSuccessPayloadAsync<T>(response, cancellationToken);
            return result ?? throw new InvalidOperationException("The API returned an empty response.");
        }

        throw new InvalidOperationException(await ReadErrorMessageAsync(response, cancellationToken));
    }

    public static async Task EnsureSuccessAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        throw new InvalidOperationException(await ReadErrorMessageAsync(response, cancellationToken));
    }

    private static async Task<T?> ReadSuccessPayloadAsync<T>(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        try
        {
            return await response.Content.ReadFromJsonAsync<T>(Options, cancellationToken);
        }
        catch (JsonException exception)
        {
            throw new InvalidOperationException(
                "The API returned a response that was not valid JSON.",
                exception);
        }
    }

    private static async Task<string> ReadErrorMessageAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var fallbackMessage = $"The API request failed with status code {(int)response.StatusCode} ({response.ReasonPhrase}).";
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(content))
        {
            return fallbackMessage;
        }

        try
        {
            var error = JsonSerializer.Deserialize<ApiErrorResponse>(content, Options);
            return string.IsNullOrWhiteSpace(error?.Message)
                ? fallbackMessage
                : error.Message;
        }
        catch (JsonException)
        {
            return $"{fallbackMessage} The response was not a valid OutreachFlow error payload. Check the API logs for details.";
        }
    }

    private static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }

    private sealed record ApiErrorResponse(string ErrorCode, string Message);
}
