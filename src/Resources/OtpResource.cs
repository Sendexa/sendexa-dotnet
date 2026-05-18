using Sendexa.Internal;
using System.Text.Json.Serialization;

namespace Sendexa.Resources;

public sealed class OtpResource
{
    private readonly SendexaHttpClient _client;
    internal OtpResource(SendexaHttpClient client) => _client = client;

    public Task<RequestOtpResponse> RequestAsync(RequestOtpRequest request, CancellationToken ct = default)
        => _client.PostAsync<RequestOtpResponse>("/v1/otp/request", request, ct);

    public Task<VerifyOtpResponse> VerifyAsync(string id, string pin, CancellationToken ct = default)
        => _client.PostAsync<VerifyOtpResponse>("/v1/otp/verify", new { id, pin }, ct);

    public Task<RequestOtpResponse> ResendAsync(string otpId, CancellationToken ct = default)
        => _client.PostAsync<RequestOtpResponse>($"/v1/otp/resend/{otpId}", null, ct);
}

// ─── Requests ───────────────────────────────────────────────────────────────

public sealed record OtpExpiry
{
    [JsonPropertyName("amount")]   public required int    Amount   { get; init; }
    [JsonPropertyName("duration")] public required string Duration { get; init; }
}

public sealed record RequestOtpRequest
{
    [JsonPropertyName("phone")]                        public required string              Phone      { get; init; }
    [JsonPropertyName("from")]                         public required string              From       { get; init; }
    [JsonPropertyName("message")]                      public          string?             Message    { get; init; }
    [JsonPropertyName("pinLength")]                    public          int?                PinLength  { get; init; }
    [JsonPropertyName("pinType")]                      public          string?             PinType    { get; init; }
    [JsonPropertyName("expiry")]                       public          OtpExpiry?          Expiry     { get; init; }
    [JsonPropertyName("maxAmountOfValidationRetries")] public          int?                MaxRetries { get; init; }
    [JsonPropertyName("metadata")]                     public          IDictionary<string, object>? Metadata { get; init; }
}

// ─── Responses ──────────────────────────────────────────────────────────────

public sealed record RequestOtpResponse
{
    [JsonPropertyName("success")] public bool     Success { get; init; }
    [JsonPropertyName("message")] public string?  Message { get; init; }
    [JsonPropertyName("data")]    public OtpData? Data    { get; init; }
}

public sealed record VerifyOtpResponse
{
    [JsonPropertyName("success")] public bool           Success { get; init; }
    [JsonPropertyName("message")] public string?        Message { get; init; }
    [JsonPropertyName("data")]    public VerifyOtpData? Data    { get; init; }
}

public sealed record OtpData
{
    [JsonPropertyName("id")]     public string?    Id     { get; init; }
    [JsonPropertyName("status")] public string?    Status { get; init; }
    [JsonPropertyName("phone")]  public string?    Phone  { get; init; }
    [JsonPropertyName("expiry")] public OtpExpiry? Expiry { get; init; }
}

public sealed record VerifyOtpData
{
    [JsonPropertyName("id")]                public string? Id                { get; init; }
    [JsonPropertyName("verified")]          public bool    Verified          { get; init; }
    [JsonPropertyName("attemptsRemaining")] public int     AttemptsRemaining { get; init; }
}
