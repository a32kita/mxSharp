using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MxSharp;

public sealed class MxSharpHttpTransport
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = false,
    };

    private readonly HttpClient _httpClient;

    public MxSharpHttpTransport(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<T> GetJsonAsync<T>(string requestUri, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
        return await ReadAsJsonAsync<T>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<T> PostFormAsync<T>(string requestUri, IEnumerable<KeyValuePair<string, string>> formValues, CancellationToken cancellationToken = default)
    {
        using var content = new FormUrlEncodedContent(formValues);
        using var response = await _httpClient.PostAsync(requestUri, content, cancellationToken).ConfigureAwait(false);
        return await ReadAsJsonAsync<T>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<TResponse> PostJsonAsync<TRequest, TResponse>(string requestUri, TRequest request, CancellationToken cancellationToken = default)
    {
        using var content = new StringContent(JsonSerializer.Serialize(request, JsonSerializerOptions), Encoding.UTF8, "application/json");
        using var response = await _httpClient.PostAsync(requestUri, content, cancellationToken).ConfigureAwait(false);
        return await ReadAsJsonAsync<TResponse>(response, cancellationToken).ConfigureAwait(false);
    }

    public static AuthenticationHeaderValue CreateBearerHeader(string accessToken)
    {
        return new AuthenticationHeaderValue("Bearer", accessToken);
    }

    private static async Task<T> ReadAsJsonAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var payload = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            throw new MixiException(string.IsNullOrWhiteSpace(payload) ? response.ReasonPhrase ?? "Unknown error." : payload);
        }

        if (typeof(T) == typeof(string))
        {
            return (T)(object)payload;
        }

        var value = JsonSerializer.Deserialize<T>(payload, JsonSerializerOptions);
        if (value is null)
        {
            throw new MixiException("Failed to deserialize API response.");
        }

        return value;
    }
}