using System.Text;
using System.Text.Json;
using UTP.Payload;
using UtpTypes.Managers;

namespace UtpTypes;

public class JsonPayload : IPayload
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public Stream GetStream()
    {
        string json = JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        byte[] stateInBytes = Encoding.UTF8.GetBytes(json);

        MemoryStream memStream = UtpMemoryManager.Pool.GetStream();
        memStream.Write(stateInBytes, 0, stateInBytes.Length);
        memStream.Position = 0;

        return memStream;
    }
}
