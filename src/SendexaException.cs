namespace Sendexa;

/// <summary>
/// Thrown for every API-level error returned by the Sendexa platform.
/// </summary>
public sealed class SendexaException : Exception
{
    /// <summary>HTTP status code (e.g. 400, 401, 403, 429).</summary>
    public int Status { get; }

    /// <summary>Machine-readable error code (e.g. <c>"SENDER_ID_NOT_APPROVED"</c>).</summary>
    public string Code { get; }

    /// <summary>Sendexa trace ID. Include when contacting support. May be <see langword="null"/>.</summary>
    public string? RequestId { get; }

    /// <summary>Full raw error payload returned by the API.</summary>
    public object? Raw { get; }

    public SendexaException(string message, int status, string code, string? requestId = null, object? raw = null)
        : base(message)
    {
        Status    = status;
        Code      = code;
        RequestId = requestId;
        Raw       = raw;
    }

    public override string ToString() =>
        $"SendexaException(Status={Status}, Code={Code}, Message={Message})";
}
