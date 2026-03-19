using System.Text;
using System.Text.Json;
using UTP.Payload;

namespace UtpTypes;

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

    //public Stream GetStream()
    //{
    //    string json = JsonSerializer.Serialize(this, new JsonSerializerOptions
    //    {
    //        WriteIndented = false,
    //        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    //    });

    //    byte[] stateInBytes = Encoding.UTF8.GetBytes(json);

    //    MemoryStream memStream = UtpMemoryManager.Pool.GetStream();
    //    memStream.Write(stateInBytes, 0, stateInBytes.Length);
    //    memStream.Position = 0;

    //    return memStream;
    //}
}
