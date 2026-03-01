namespace UTP.Constants;

internal static class UtpMessageConstants
{
    internal const byte MESSAGE_LEN_LABEL_SIZE = 4;
    internal const byte MESSAGE_LEN_ACTION_CODE = 2;
    internal const char MESSAGE_HEADER_KEY_VALUE_SEPARATOR = ':';
    internal const byte MESSAGE_HEADERS_PAYLOAD_SEPARATOR = 0x0A;
    internal const byte MESSAGE_HEADERS_PAYLOAD_SEPARATOR_BYTES_LEN = sizeof(byte);
}
