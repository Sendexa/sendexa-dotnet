using Sendexa.Internal;
using Sendexa.Resources;

namespace Sendexa;

/// <summary>
/// The official Sendexa .NET SDK client.
///
/// <para>Create one instance per application (e.g. as a singleton in your DI container)
/// and reuse it — it is thread-safe and manages its own HTTP connection pool.</para>
///
/// <para>Authentication — provide one of:
/// <list type="bullet">
///   <item><see cref="SendexaConfig.ApiKey"/> + <see cref="SendexaConfig.ApiSecret"/></item>
///   <item><see cref="SendexaConfig.Token"/> (pre-computed Base64 string)</item>
/// </list>
/// </para>
/// </summary>
/// <example>
/// <code>
/// var client = new SendexaClient(new SendexaConfig
/// {
///     ApiKey    = Environment.GetEnvironmentVariable("SENDEXA_API_KEY"),
///     ApiSecret = Environment.GetEnvironmentVariable("SENDEXA_API_SECRET"),
/// });
///
/// var resp = await client.Sms.SendAsync(new SendSmsRequest
/// {
///     To      = "0244123456",
///     From    = "MyBrand",
///     Message = "Hello from Sendexa!",
/// });
/// </code>
/// </example>
public sealed class SendexaClient
{
    /// <summary>SMS messaging methods.</summary>
    public SmsResource      Sms      { get; }

    /// <summary>One-time password methods.</summary>
    public OtpResource      Otp      { get; }

    /// <summary>WhatsApp Business messaging methods.</summary>
    public WhatsAppResource WhatsApp { get; }

    /// <summary>Email messaging methods.</summary>
    public EmailResource    Email    { get; }

    /// <summary>Voice call methods.</summary>
    public VoiceResource    Voice    { get; }

    /// <summary>Webhook verification and parsing helpers.</summary>
    public WebhooksResource Webhooks { get; }

    /// <param name="config">SDK configuration. Must have credentials set.</param>
    /// <param name="httpClient">Optional <see cref="HttpClient"/> to use (e.g. from <c>IHttpClientFactory</c>).</param>
    public SendexaClient(SendexaConfig config, HttpClient? httpClient = null)
    {
        var http = new SendexaHttpClient(config, httpClient);

        Sms      = new SmsResource(http);
        Otp      = new OtpResource(http);
        WhatsApp = new WhatsAppResource(http);
        Email    = new EmailResource(http);
        Voice    = new VoiceResource(http);
        Webhooks = new WebhooksResource();
    }
}
