using System.Text;
using System.Text.Json;
using UTP.Payload;

namespace UtpTypes.PayloadTypes;

public class JsonPayload : IPayload
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public JsonPayload() { }
    public JsonPayload(int id, string name) { Id = id; Name = name; }

    public Stream GetStream()
    {
        var json = JsonSerializer.Serialize(this);
        return new MemoryStream(Encoding.UTF8.GetBytes(json));
    }
}
