namespace JsonManagerLib.Interfaces;

public interface IMessage<TEnum> where TEnum : Enum
{
    TEnum Command { get; }

    string Data { get; }
}
