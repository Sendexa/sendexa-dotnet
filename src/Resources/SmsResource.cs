using Sendexa.Internal;
using System.Text.Json.Serialization;

namespace Sendexa.Resources;

public sealed class SmsResource
{
    private readonly SendexaHttpClient _client;
    internal SmsResource(SendexaHttpClient client) => _client = client;

    public Task<SendSmsResponse> SendAsync(SendSmsRequest request, CancellationToken ct = default)
        => _client.PostAsync<SendSmsResponse>("/v1/sms/send", request, ct);

    public Task<BulkSmsResponse> SendBulkAsync(SendBulkSmsRequest request, CancellationToken ct = default)
        => _client.PostAsync<BulkSmsResponse>("/v1/sms/bulk", request, ct);

    public Task<SendSmsResponse> GetStatusAsync(string messageId, CancellationToken ct = default)
        => _client.GetAsync<SendSmsResponse>($"/v1/sms/status/{messageId}", ct);

    public Task<SendSmsResponse> ResendAsync(string messageId, CancellationToken ct = default)
        => _client.PostAsync<SendSmsResponse>($"/v1/sms/resend/{messageId}", null, ct);
}

// ─── Requests ───────────────────────────────────────────────────────────────

public sealed record SendSmsRequest
{
    [JsonPropertyName("to")]          public required string  To          { get; init; }
    [JsonPropertyName("from")]        public required string  From        { get; init; }
    [JsonPropertyName("message")]     public required string  Message     { get; init; }
    [JsonPropertyName("callbackUrl")] public          string? CallbackUrl { get; init; }
}

public sealed record BulkSmsMessage
{
    [JsonPropertyName("to")]      public required string  To      { get; init; }
    [JsonPropertyName("message")] public          string? Message { get; init; }
}

public sealed record SendBulkSmsRequest
{
    [JsonPropertyName("from")]        public required string                    From     { get; init; }
    [JsonPropertyName("message")]     public          string?                   Message  { get; init; }
    [JsonPropertyName("messages")]    public required IReadOnlyList<BulkSmsMessage> Messages { get; init; }
    [JsonPropertyName("callbackUrl")] public          string?                   CallbackUrl { get; init; }
}

// ─── Responses ──────────────────────────────────────────────────────────────

public sealed record SendSmsResponse
{
    [JsonPropertyName("success")] public bool     Success { get; init; }
    [JsonPropertyName("message")] public string?  Message { get; init; }
    [JsonPropertyName("data")]    public SmsData? Data    { get; init; }
}

public sealed record BulkSmsResponse
{
    [JsonPropertyName("success")] public bool         Success { get; init; }
    [JsonPropertyName("message")] public string?      Message { get; init; }
    [JsonPropertyName("data")]    public BulkSmsData? Data    { get; init; }
}

public sealed record SmsData
{
    [JsonPropertyName("messageId")] public string? MessageId { get; init; }
    [JsonPropertyName("status")]    public string? Status    { get; init; }
    [JsonPropertyName("to")]        public string? To        { get; init; }
    [JsonPropertyName("from")]      public string? From      { get; init; }
    [JsonPropertyName("createdAt")] public string? CreatedAt { get; init; }
}

public sealed record BulkSmsData
{
    [JsonPropertyName("batchId")] public string? BatchId { get; init; }
    [JsonPropertyName("total")]   public int     Total   { get; init; }
    [JsonPropertyName("queued")]  public int     Queued  { get; init; }
    [JsonPropertyName("failed")]  public int     Failed  { get; init; }
}
