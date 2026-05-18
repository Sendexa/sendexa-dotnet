using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sendexa.Internal;

/// <summary>Internal HTTP client — not part of the public API.</summary>
internal sealed class SendexaHttpClient
{
    private readonly HttpClient _http;

    internal static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy        = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition      = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true,
    };

    public SendexaHttpClient(SendexaConfig config, HttpClient? httpClient = null)
    {
        string auth;
        if (config.Token is { Length: > 0 })
        {
            auth = $"Basic {config.Token}";
        }
        else if (config.ApiKey is { Length: > 0 } && config.ApiSecret is { Length: > 0 })
        {
            var creds = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{config.ApiKey}:{config.ApiSecret}"));
            auth = $"Basic {creds}";
        }
        else
        {
            throw new InvalidOperationException("Provide Token or both ApiKey and ApiSecret.");
        }

        _http = httpClient ?? new HttpClient();
        _http.BaseAddress = new Uri(config.BaseUrl.TrimEnd('/') + "/");
        _http.Timeout     = config.Timeout;
        _http.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(auth);
        _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _http.DefaultRequestHeaders.Add("User-Agent", "sendexa-dotnet/0.1.0");
    }

    // -------------------------------------------------------------------------
    // Public helpers
    // -------------------------------------------------------------------------

    public Task<T> GetAsync<T>(string path, CancellationToken ct = default)
        => SendAsync<T>(new HttpRequestMessage(HttpMethod.Get, path), ct);

    public Task<T> PostAsync<T>(string path, object? body = null, CancellationToken ct = default)
    {
        var req = new HttpRequestMessage(HttpMethod.Post, path);
        if (body is not null)
        {
            req.Content = new StringContent(
                JsonSerializer.Serialize(body, JsonOptions),
                Encoding.UTF8,
                "application/json");
        }
        return SendAsync<T>(req, ct);
    }

    // -------------------------------------------------------------------------
    // Core
    // -------------------------------------------------------------------------

    private async Task<T> SendAsync<T>(HttpRequestMessage req, CancellationToken ct)
    {
        HttpResponseMessage resp;
        try
        {
            resp = await _http.SendAsync(req, ct).ConfigureAwait(false);
        }
        catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
        {
            throw new SendexaException("Request timed out", 408, "REQUEST_TIMEOUT", raw: ex.Message);
        }

        var body = await resp.Content.ReadAsStringAsync(ct).ConfigureAwait(false);

        if (!resp.IsSuccessStatusCode)
        {
            Dictionary<string, JsonElement>? err = null;
            try { err = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(body, JsonOptions); }
            catch { /* swallow parse failures */ }

            var message   = err?.GetValueOrDefault("message").GetString() ?? $"HTTP {(int)resp.StatusCode}";
            var code      = err?.GetValueOrDefault("code").GetString()    ?? "UNKNOWN_ERROR";
            var requestId = err?.GetValueOrDefault("requestId").GetString();
            throw new SendexaException(message, (int)resp.StatusCode, code, requestId, err);
        }

        return JsonSerializer.Deserialize<T>(body, JsonOptions)
               ?? throw new SendexaException("Empty response body", 0, "EMPTY_RESPONSE");
    }
}
