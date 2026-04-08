using System.Text;
using System.Text.Json;
using UTP.Payload;

namespace UtpTypes.PayloadTypes.Server;

public class ClientUnauthorizedPayload : IPayload
{
    public string Message { get; set; } = "Client not authorized";

    public Stream GetStream()
    {
        var json = JsonSerializer.Serialize(this);
        return new MemoryStream(Encoding.UTF8.GetBytes(json));
    }
}