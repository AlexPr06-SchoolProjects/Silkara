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

    private IDictionary<string, string> _headers = new Dictionary<string, string>();

    public short ActionCode { get; set; }

    public int HeadersLen { get; private set; }

    public IDictionary<string, string> Headers { 
        get => _headers; 
        private set
        {
            _headers = value;
            UpdateHeadersLen();
        }
    }

    public TPayload? Payload { get; private set; }

    public MemoryStream? PayloadStream { get; private set; } = null;

    public UtpMessage(
        short actionCode,
        Dictionary<string, string> headers
        )
    {
        ActionCode = actionCode;
        SetHeaders(headers);
        UpdateHeadersLen();
    }

    public UtpMessage(
        short actionCode, 
        Dictionary<string, string> headers,
        TPayload payload) : this(actionCode, headers)
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
            _headers[kv.Key] = kv.Value;

        UpdateHeadersLen();
    }

    // ---------------------- TODO -------------------
    public void SetPayload(TPayload? payload)
    {
        Payload = payload;

        if (payload is not null)
        {
            PayloadStream = payload.GetStateStream();
            _headers[HEADER_PAYLOAD_TYPE_KEY] = payload.GetType().Name;
            _headers[HEADER_PAYLOAD_LEN_KEY] = PayloadStream?.Length.ToString() ?? "0";
        }
        else
        {
            PayloadStream = null;
            _headers.Remove(HEADER_PAYLOAD_TYPE_KEY);
            _headers.Remove(HEADER_PAYLOAD_LEN_KEY);
        }
        UpdateHeadersLen();
    }
    // ---------------------- TODO -------------------

    private void UpdateHeadersLen()
        => HeadersLen = JsonSerializer.SerializeToUtf8Bytes(_headers).Length;

    private string[] GetChunks(string headerLine)
       => headerLine.Split(
               HEADER_SEPARATOR,
               StringSplitOptions.RemoveEmptyEntries |
               StringSplitOptions.TrimEntries);

    public void Clear()
    {
        Headers.Clear();
    }
}
