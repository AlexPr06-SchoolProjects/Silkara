using JsonManagerLib.Interfaces;

namespace JsonManagerLib.Records;

public record SilkaraPacket<TEnum>(TEnum Command, string Data) : IMessage<TEnum> where TEnum : Enum;
