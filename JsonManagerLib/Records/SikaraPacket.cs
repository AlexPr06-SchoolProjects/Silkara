using JsonManagerLib.Interfaces;

namespace JsonManagerLib.Records;

public record SikaraPacket<TEnum>(TEnum Command, string Data) : IMessage<TEnum> where TEnum : Enum;
