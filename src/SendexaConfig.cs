namespace Sendexa;

/// <summary>
/// Configuration options for <see cref="SendexaClient"/>.
/// Provide <see cref="Token"/> OR both <see cref="ApiKey"/> and <see cref="ApiSecret"/>.
/// </summary>
public sealed class SendexaConfig
{
    /// <summary>Your Sendexa API key from the dashboard.</summary>
    public string? ApiKey { get; set; }

    /// <summary>Your Sendexa API secret from the dashboard.</summary>
    public string? ApiSecret { get; set; }

    /// <summary>Pre-computed Base64 token (<c>base64("key:secret")</c>). Alternative to ApiKey + ApiSecret.</summary>
    public string? Token { get; set; }

    /// <summary>Override the default API base URL. Defaults to <c>https://api.sendexa.co</c>.</summary>
    public string BaseUrl { get; set; } = "https://api.sendexa.co";

    /// <summary>HTTP request timeout. Defaults to 30 seconds.</summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
}
