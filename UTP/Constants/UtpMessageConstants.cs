namespace UTP.Constants;

internal static class UtpMessageConstants
{
    public static class Sizes
    {
        public const int Label = 4;       // MESSAGE_LEN_LABEL_SIZE
        public const int ActionCode = 2;  // MESSAGE_LEN_ACTION_CODE
        public const int HeaderPayloadSeparator = sizeof(byte);
    }

    public static class Delimiters
    {
        public const char HeaderKeyValue = ':';
        public const byte HeaderPayload = 0x0A; // \n
    }
}
