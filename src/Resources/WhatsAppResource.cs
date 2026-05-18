using Sendexa.Internal;
using System.Text.Json.Serialization;

namespace Sendexa.Resources;

public sealed class WhatsAppResource
{
    private readonly SendexaHttpClient _client;
    internal WhatsAppResource(SendexaHttpClient client) => _client = client;

    public Task<WhatsAppResponse> SendAsync(SendWhatsAppRequest request, CancellationToken ct = default)
        => _client.PostAsync<WhatsAppResponse>("/v1/whatsapp/send", request, ct);

    public Task<WhatsAppResponse> SendTextAsync(string to, string text, bool previewUrl = false, CancellationToken ct = default)
        => SendAsync(new SendWhatsAppRequest
        {
            To   = to,
            Type = "text",
            Text = new Dictionary<string, object> { ["body"] = text, ["preview_url"] = previewUrl },
        }, ct);

    public Task<WhatsAppResponse> SendImageAsync(string to, string imageUrl, string? caption = null, CancellationToken ct = default)
    {
        var image = new Dictionary<string, object> { ["link"] = imageUrl };
        if (caption is not null) image["caption"] = caption;
        return SendAsync(new SendWhatsAppRequest { To = to, Type = "image", Image = image }, ct);
    }

    public Task<WhatsAppResponse> SendDocumentAsync(string to, string docUrl, string? caption = null, string? filename = null, CancellationToken ct = default)
    {
        var doc = new Dictionary<string, object> { ["link"] = docUrl };
        if (caption  is not null) doc["caption"]  = caption;
        if (filename is not null) doc["filename"] = filename;
        return SendAsync(new SendWhatsAppRequest { To = to, Type = "document", Document = doc }, ct);
    }

    public Task<WhatsAppResponse> SendInteractiveAsync(string to, IDictionary<string, object> interactive, CancellationToken ct = default)
        => SendAsync(new SendWhatsAppRequest { To = to, Type = "interactive", Interactive = interactive }, ct);

    public Task<WhatsAppResponse> SendTemplateAsync(string to, IDictionary<string, object> template, CancellationToken ct = default)
        => SendAsync(new SendWhatsAppRequest { To = to, Type = "template", Template = template }, ct);

    public Task<WhatsAppResponse> GetStatusAsync(string messageId, CancellationToken ct = default)
        => _client.GetAsync<WhatsAppResponse>($"/v1/whatsapp/status/{messageId}", ct);

    public Task<WhatsAppResponse> ResendAsync(string messageId, CancellationToken ct = default)
        => _client.PostAsync<WhatsAppResponse>($"/v1/whatsapp/resend/{messageId}", null, ct);
}

// ─── Requests ───────────────────────────────────────────────────────────────

public sealed record SendWhatsAppRequest
{
    [JsonPropertyName("to")]          public required string                       To          { get; init; }
    [JsonPropertyName("type")]        public required string                       Type        { get; init; }
    [JsonPropertyName("text")]        public IDictionary<string, object>?          Text        { get; init; }
    [JsonPropertyName("image")]       public IDictionary<string, object>?          Image       { get; init; }
    [JsonPropertyName("video")]       public IDictionary<string, object>?          Video       { get; init; }
    [JsonPropertyName("audio")]       public IDictionary<string, object>?          Audio       { get; init; }
    [JsonPropertyName("document")]    public IDictionary<string, object>?          Document    { get; init; }
    [JsonPropertyName("interactive")] public IDictionary<string, object>?          Interactive { get; init; }
    [JsonPropertyName("template")]    public IDictionary<string, object>?          Template    { get; init; }
}

// ─── Responses ──────────────────────────────────────────────────────────────

public sealed record WhatsAppResponse
{
    [JsonPropertyName("success")] public bool            Success { get; init; }
    [JsonPropertyName("message")] public string?         Message { get; init; }
    [JsonPropertyName("data")]    public WhatsAppData?   Data    { get; init; }
}

public sealed record WhatsAppData
{
    [JsonPropertyName("messageId")] public string? MessageId { get; init; }
    [JsonPropertyName("status")]    public string? Status    { get; init; }
    [JsonPropertyName("to")]        public string? To        { get; init; }
}
