namespace UTP.Constants;

internal static class UtpConstants
{
    public static class Sizes
    {
        public const int MessageLen = 4;       // MESSAGE_LEN_LABEL_SIZE
        public const int ActionCodeLen = 2;  // MESSAGE_LEN_ACTION_CODE
        public const int HeadersLen = 4;
    }

    public static class Delimiters
    {
        public const char HeaderKeyValue = ':'; 
    }

    public static class Headers
    {
        public const string PayloadTypeKey = "pType";
        public const string PayloadLenKey = "pLen";
    }

    public static class UtpConnectionConstants
    {
        public const int  MINIMUM_BUFFER_SIZE = 1024;
    }
}
