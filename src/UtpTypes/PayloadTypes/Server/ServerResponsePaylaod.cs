using UTP.Payload;
using System.Text.Json;
using System.Text;

namespace UtpTypes.PayloadTypes.Server;

public class ServerResponsePaylaod : IPayload
{
    public string Message { get; set; } = string.Empty;
    public ServerResponsePaylaod() { }
    public ServerResponsePaylaod(string message) => Message = message;
    public virtual Stream GetStream()
    {
        var json = JsonSerializer.Serialize(this);
        return new MemoryStream(Encoding.UTF8.GetBytes(json));
    }
}
