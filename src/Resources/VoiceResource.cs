using Sendexa.Internal;
using System.Text.Json.Serialization;

namespace Sendexa.Resources;

public sealed class VoiceResource
{
    private readonly SendexaHttpClient _client;
    internal VoiceResource(SendexaHttpClient client) => _client = client;

    public Task<CallResponse> CallAsync(MakeCallRequest request, CancellationToken ct = default)
        => _client.PostAsync<CallResponse>("/v1/voice/call", request, ct);

    public Task<CallResponse> TtsAsync(TextToSpeechRequest request, CancellationToken ct = default)
        => _client.PostAsync<CallResponse>("/v1/voice/tts", request, ct);

    public Task<CallResponse> PlayAsync(PlayAudioRequest request, CancellationToken ct = default)
        => _client.PostAsync<CallResponse>("/v1/voice/play", request, ct);

    public Task<CallResponse> GetStatusAsync(string callId, CancellationToken ct = default)
        => _client.GetAsync<CallResponse>($"/v1/voice/status/{callId}", ct);
}

// ─── Requests ───────────────────────────────────────────────────────────────

public sealed record MakeCallRequest
{
    [JsonPropertyName("to")]                public required string  To                { get; init; }
    [JsonPropertyName("from")]              public required string  From              { get; init; }
    [JsonPropertyName("twiml")]             public          string? TwiML             { get; init; }
    [JsonPropertyName("url")]               public          string? Url               { get; init; }
    [JsonPropertyName("record")]            public          bool?   Record            { get; init; }
    [JsonPropertyName("statusCallbackUrl")] public          string? StatusCallbackUrl { get; init; }
    [JsonPropertyName("machineDetection")]  public          bool?   MachineDetection  { get; init; }
}

public sealed record TextToSpeechRequest
{
    [JsonPropertyName("to")]       public required string  To       { get; init; }
    [JsonPropertyName("from")]     public required string  From     { get; init; }
    [JsonPropertyName("text")]     public required string  Text     { get; init; }
    [JsonPropertyName("language")] public          string? Language { get; init; }
    [JsonPropertyName("voice")]    public          string? Voice    { get; init; }
    [JsonPropertyName("loop")]     public          int?    Loop     { get; init; }
}

public sealed record PlayAudioRequest
{
    [JsonPropertyName("to")]       public required string  To       { get; init; }
    [JsonPropertyName("from")]     public required string  From     { get; init; }
    [JsonPropertyName("audioUrl")] public required string  AudioUrl { get; init; }
    [JsonPropertyName("loop")]     public          int?    Loop     { get; init; }
}

// ─── Responses ──────────────────────────────────────────────────────────────

public sealed record CallResponse
{
    [JsonPropertyName("success")] public bool       Success { get; init; }
    [JsonPropertyName("message")] public string?    Message { get; init; }
    [JsonPropertyName("data")]    public CallData?  Data    { get; init; }
}

public sealed record CallData
{
    [JsonPropertyName("callId")]    public string? CallId    { get; init; }
    [JsonPropertyName("status")]    public string? Status    { get; init; }
    [JsonPropertyName("to")]        public string? To        { get; init; }
    [JsonPropertyName("from")]      public string? From      { get; init; }
    [JsonPropertyName("duration")]  public int     Duration  { get; init; }
    [JsonPropertyName("createdAt")] public string? CreatedAt { get; init; }
}
