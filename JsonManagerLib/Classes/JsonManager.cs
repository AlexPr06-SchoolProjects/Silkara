using System.Text.Json;
using JsonManagerLib.Interfaces;

namespace JsonManagerLib.Classes;

public static class JsonManager
{
    public static async Task SendMessageAsync<TMessage, TEnum>(TMessage message, StreamWriter writer)
        where TMessage : IMessage<TEnum>
        where TEnum : Enum
    {
        string json = JsonSerializer.Serialize(message);
        await writer.WriteLineAsync(json).ConfigureAwait(false);
    }

    public static void SendMessage<TMessage, TEnum>(TMessage message, StreamWriter writer)
        where TMessage : IMessage<TEnum>
        where TEnum : Enum
    {
        string json = JsonSerializer.Serialize(message);
        writer.WriteLine(json);
    }

    public static async Task<TMessage?> ReadMessageAsync<TMessage, TEnum>(StreamReader reader)
        where TMessage : IMessage<TEnum>
        where TEnum : Enum
    {
        string? json = await reader.ReadLineAsync().ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(json))
            return default;

        try
        {
            return JsonSerializer.Deserialize<TMessage>(json);
        }
        catch (JsonException)
        {
            return default;
        }
    }

    public static TMessage? ReadMessage<TMessage, TEnum>(StreamReader reader)
        where TMessage : IMessage<TEnum>
        where TEnum : Enum
    {
        string? json = reader.ReadLine();

        if (string.IsNullOrWhiteSpace(json))
            return default;

        try
        {
            return JsonSerializer.Deserialize<TMessage>(json);
        }
        catch (JsonException)
        {
            return default;
        }
    }
}