using Sendexa.Internal;
using System.Text.Json.Serialization;

namespace Sendexa.Resources;

public sealed class EmailResource
{
    private readonly SendexaHttpClient _client;
    internal EmailResource(SendexaHttpClient client) => _client = client;

    public Task<SendEmailResponse> SendAsync(SendEmailRequest request, CancellationToken ct = default)
        => _client.PostAsync<SendEmailResponse>("/v1/email/send", request, ct);

    public Task<SendEmailResponse> SendBulkAsync(SendBulkEmailRequest request, CancellationToken ct = default)
        => _client.PostAsync<SendEmailResponse>("/v1/email/bulk", request, ct);

    /// <summary>Set <see cref="SendEmailRequest.TemplateId"/> and <see cref="SendEmailRequest.Variables"/>; HTML/Text are ignored.</summary>
    public Task<SendEmailResponse> SendWithTemplateAsync(SendEmailRequest request, CancellationToken ct = default)
        => _client.PostAsync<SendEmailResponse>("/v1/email/send", request, ct);

    public Task<SendEmailResponse> GetStatusAsync(string messageId, CancellationToken ct = default)
        => _client.GetAsync<SendEmailResponse>($"/v1/email/status/{messageId}", ct);
}

// ─── Requests ───────────────────────────────────────────────────────────────

public sealed record EmailAttachment
{
    [JsonPropertyName("filename")]    public required string  Filename    { get; init; }
    [JsonPropertyName("content")]     public required string  Content     { get; init; }
    [JsonPropertyName("contentType")] public          string? ContentType { get; init; }
}

public sealed record SendEmailRequest
{
    [JsonPropertyName("to")]          public required string                        To          { get; init; }
    [JsonPropertyName("from")]        public required string                        From        { get; init; }
    [JsonPropertyName("subject")]     public required string                        Subject     { get; init; }
    [JsonPropertyName("html")]        public          string?                       Html        { get; init; }
    [JsonPropertyName("text")]        public          string?                       Text        { get; init; }
    [JsonPropertyName("replyTo")]     public          string?                       ReplyTo     { get; init; }
    [JsonPropertyName("templateId")]  public          string?                       TemplateId  { get; init; }
    [JsonPropertyName("variables")]   public          IDictionary<string, object>?  Variables   { get; init; }
    [JsonPropertyName("attachments")] public          IReadOnlyList<EmailAttachment>? Attachments { get; init; }
    [JsonPropertyName("metadata")]    public          IDictionary<string, object>?  Metadata    { get; init; }
}

public sealed record BulkEmailMessage
{
    [JsonPropertyName("to")]        public required string                       To        { get; init; }
    [JsonPropertyName("subject")]   public          string?                      Subject   { get; init; }
    [JsonPropertyName("variables")] public          IDictionary<string, object>? Variables { get; init; }
}

public sealed record SendBulkEmailRequest
{
    [JsonPropertyName("from")]     public required string                          From     { get; init; }
    [JsonPropertyName("subject")]  public required string                          Subject  { get; init; }
    [JsonPropertyName("html")]     public          string?                         Html     { get; init; }
    [JsonPropertyName("text")]     public          string?                         Text     { get; init; }
    [JsonPropertyName("messages")] public required IReadOnlyList<BulkEmailMessage> Messages { get; init; }
}

// ─── Responses ──────────────────────────────────────────────────────────────

public sealed record SendEmailResponse
{
    [JsonPropertyName("success")] public bool        Success { get; init; }
    [JsonPropertyName("message")] public string?     Message { get; init; }
    [JsonPropertyName("data")]    public EmailData?  Data    { get; init; }
}

public sealed record EmailData
{
    [JsonPropertyName("messageId")] public string? MessageId { get; init; }
    [JsonPropertyName("status")]    public string? Status    { get; init; }
    [JsonPropertyName("to")]        public string? To        { get; init; }
}
