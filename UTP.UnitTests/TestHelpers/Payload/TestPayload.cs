using System.Text;
using System.Text.Json;
using UTP.Payload;

namespace UTP.UnitTests.Helpers.Payload;

public class TestPayload : IPayload
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public TestPayload() { }
    public TestPayload(int id, string name) { Id = id; Name = name; }

    public Stream GetStream()
    {
        var json = JsonSerializer.Serialize(this);
        return new MemoryStream(Encoding.UTF8.GetBytes(json));
    }
}
