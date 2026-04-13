namespace UtpTypes.Actions;

public enum ServerCode : short
{
    Ok = 900,
    Error = 901,
    Unauthorized = 902,
    Forbidden = 903,
    NotFound = 904,
    RateLimitExceeded = 905,
    ClientIdNotFound = 906
}

