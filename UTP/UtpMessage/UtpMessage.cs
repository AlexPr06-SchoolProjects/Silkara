using System.Text.Json;
using UTP.Constants;
using UTP.Payload;
using UTP.UtpMessage.Interfaces;

namespace UTP.UtpMessage;

public class UtpMessage<TPayload> : IUtpMessage<TPayload>
    where TPayload : IPayload
{
    private const char HEADER_SEPARATOR = UtpConstants.Delimiters.HeaderKeyValue;
    private const string HEADER_PAYLOAD_TYPE_KEY = UtpConstants.Headers.PayloadTypeKey;
    private const string HEADER_PAYLOAD_LEN_KEY = UtpConstants.Headers.PayloadLenKey;

    private IDictionary<string, string>? _headers;

    public short ActionCode { get; set; }

    public int HeadersLen { get; private set; }

    public IDictionary<string, string> Headers { 
        get => _headers ??= new Dictionary<string, string>();
        private set
        {
            _headers = value;
            UpdateHeadersLen();
        }
    }

    public Stream? PayloadStream { get; private set; }

    public UtpMessage(
        short actionCode,
        Dictionary<string, string>? headers
        )
    {
        ActionCode = actionCode;
        SetHeaders(headers);
        UpdateHeadersLen();
    }

    public UtpMessage(
        short actionCode, 
        Dictionary<string, string>? headers,
        TPayload? payload) : this(actionCode, headers)
    {
        SetPayload(payload);
    }

    public UtpMessage() { }

    public void SetHeader(string key, string value)
    {
        Headers[key] = value;
        UpdateHeadersLen();
    }

    public void SetHeader(string? headerLine)
    {
        if (headerLine is null)
            return;

        string[] chunks = GetChunks(headerLine);
        if (chunks.Length >= 2)
            SetHeader(chunks[0], chunks[1]);
    }

    public void SetHeaders(IDictionary<string, string>? headers)
    {
        if (headers is null || headers.Count == 0)
            return;

        foreach (var kv in headers)
            Headers[kv.Key] = kv.Value;

        UpdateHeadersLen();
    }

    public void SetPayload(TPayload? payload)
    {
        if (payload is not null)
        {
            PayloadStream = payload.GetStream();
            Headers[HEADER_PAYLOAD_TYPE_KEY] = payload.GetType().Name;
            Headers[HEADER_PAYLOAD_LEN_KEY] = PayloadStream?.Length.ToString() ?? "0";
        }
        else
        {
            PayloadStream = null;
            Headers.Remove(HEADER_PAYLOAD_TYPE_KEY);
            Headers.Remove(HEADER_PAYLOAD_LEN_KEY);
        }
        UpdateHeadersLen();
    }

    private void UpdateHeadersLen()
    {
        if (_headers == null)
        {
            HeadersLen = 0;
            return;
        }
        HeadersLen = JsonSerializer.SerializeToUtf8Bytes(_headers).Length;
    }

    private string[] GetChunks(string headerLine)
       => headerLine.Split(
               HEADER_SEPARATOR,
               StringSplitOptions.RemoveEmptyEntries |
               StringSplitOptions.TrimEntries);

    public void Clear()
    {
        ActionCode = default;
        HeadersLen = default;
        _headers = null;
        PayloadStream?.Dispose();
        PayloadStream = default;
    }
}
