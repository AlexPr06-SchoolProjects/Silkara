using System.Text;
using System.Text.Json;
using UTP.Payload;

namespace UTP.UnitTests.Helpers.Payload;

public record TestPayload(int Id, string Name) : IPayload
{
    public MemoryStream GetStateStream()
    {
        string json = JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        byte[] stateInBytes = Encoding.UTF8.GetBytes(json);

        MemoryStream memStream = new MemoryStream(stateInBytes.Length);
        memStream.Write(stateInBytes, 0, stateInBytes.Length);

        return memStream;
    }
}
