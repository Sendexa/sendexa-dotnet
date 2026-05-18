using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Sendexa.Internal;

namespace Sendexa.Resources;

/// <summary>
/// Helpers for verifying and parsing Sendexa webhook events.
/// Pure methods — no HTTP client required.
/// </summary>
public sealed class WebhooksResource
{
    /// <summary>
    /// Verify the <c>X-Sendexa-Signature</c> header on an incoming webhook request.
    ///
    /// <para>Sendexa signs every payload with HMAC-SHA256. Always verify before processing.</para>
    /// </summary>
    /// <example>
    /// <code>
    /// [HttpPost("/webhooks/sendexa")]
    /// public IActionResult Handle(
    ///     [FromHeader(Name = "X-Sendexa-Signature")] string signature,
    ///     [FromBody] byte[] rawBody)
    /// {
    ///     if (!_client.Webhooks.Verify(signature, rawBody, _webhookSecret))
    ///         return Unauthorized();
    ///
    ///     var evt = _client.Webhooks.Parse(rawBody);
    ///     // handle evt...
    ///     return Ok();
    /// }
    /// </code>
    /// </example>
    public bool Verify(string signature, byte[] rawBody, string secret)
    {
        if (string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(secret))
            return false;

        var sigHex = signature.StartsWith("sha256=", StringComparison.OrdinalIgnoreCase)
            ? signature[7..]
            : signature;

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var expected   = Convert.ToHexString(hmac.ComputeHash(rawBody)).ToLowerInvariant();

        // CryptographicOperations.FixedTimeEquals prevents timing attacks
        return CryptographicOperations.FixedTimeEquals(
            Encoding.ASCII.GetBytes(expected),
            Encoding.ASCII.GetBytes(sigHex.ToLowerInvariant()));
    }

    /// <summary>
    /// Parse a raw webhook body into a <see cref="WebhookEvent"/>.
    /// Does NOT verify the signature — call <see cref="Verify"/> first.
    /// </summary>
    /// <exception cref="SendexaException">Thrown if the payload is not valid JSON or missing the <c>event</c> field.</exception>
    public WebhookEvent Parse(byte[] rawBody)
    {
        WebhookEvent? evt;
        try
        {
            evt = JsonSerializer.Deserialize<WebhookEvent>(rawBody, SendexaHttpClient.JsonOptions);
        }
        catch (Exception ex)
        {
            throw new SendexaException($"Webhook payload is not valid JSON: {ex.Message}", 400, "INVALID_WEBHOOK_PAYLOAD");
        }

        if (evt is null || string.IsNullOrEmpty(evt.Event))
            throw new SendexaException("Webhook payload is missing the \"event\" field", 400, "INVALID_WEBHOOK_PAYLOAD");

        return evt;
    }

    /// <summary>Returns <see langword="true"/> if the parsed event matches <paramref name="eventType"/>.</summary>
    public bool IsEvent(WebhookEvent evt, string eventType) =>
        string.Equals(evt.Event, eventType, StringComparison.Ordinal);
}

// ─── Model ──────────────────────────────────────────────────────────────────

public sealed record WebhookEvent
{
    [JsonPropertyName("event")]     public string?                       Event     { get; init; }
    [JsonPropertyName("data")]      public IDictionary<string, object>?  Data      { get; init; }
    [JsonPropertyName("timestamp")] public string?                       Timestamp { get; init; }
    [JsonPropertyName("version")]   public string?                       Version   { get; init; }
}
