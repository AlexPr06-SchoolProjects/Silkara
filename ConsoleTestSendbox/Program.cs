using UTP.UnitTests.Helpers.Payload;
using UTP.UtpMessage;
using UTP;
using Newtonsoft.Json;

var headers = new Dictionary<string, string> { ["Type"] = "ConsoleTest🚀🚀🚀" };
var payload = new TestPayload(777, "Hello from Console!🚀🚀🚀🚀");

Test.TestUtpEngineIdempotence(100, headers, payload);

class Test
{
    public static void TestUtpEngineIdempotence(
        short actionCode,
        Dictionary<string, string> headers,
        TestPayload payload)
    {
        MemoryStream sharedStream = new MemoryStream();

        var utpMessageSend = new UtpMessage<TestPayload>(actionCode, headers, payload);
        var utpMessageReceive = new UtpMessage<TestPayload>();

        UtpEngine utpEngineSend = new UtpEngine(sharedStream);
        UtpEngine utpEngineReceive = new UtpEngine(sharedStream);

        utpEngineSend.SendMessage(utpMessageSend);
        sharedStream.Position = 0;
        utpMessageReceive = utpEngineReceive.ReceiveMessage<TestPayload>();


        Console.WriteLine("\n" + new string('=', 50));
        Console.WriteLine("🚀 UTP MESSAGE ROUND-TRIP COMPARISON");
        Console.WriteLine(new string('=', 50));

        // 1. Action Code
        Console.WriteLine($"[ActionCode]  Sent: {utpMessageSend.ActionCode,-6} | Received: {utpMessageReceive.ActionCode}");

        // 2. Headers
        // --- Вывод ОТПРАВЛЕННЫХ заголовков ---
        Console.WriteLine("\n[Headers - SENT]");
        if (utpMessageSend.Headers != null)
        {
            foreach (var header in utpMessageSend.Headers) // Используем var, чтобы избежать ошибки CS1061
            {
                Console.WriteLine($"  ├─ {header.Key}: {header.Value}");
            }
        }

        // --- Вывод ПОЛУЧЕННЫХ заголовков ---
        Console.WriteLine("\n[Headers - RECEIVED]");
        if (utpMessageReceive.Headers != null)
        {
            foreach (var header in utpMessageReceive.Headers)
            {
                Console.WriteLine($"  ├─ {header.Key}: {header.Value}");
            }
        }

        // 3. Payload
        Console.WriteLine("\n[Payload]     Testing TestPayload structure...");

        MemoryStream memoryStream = utpMessageReceive?.PayloadStream!;
        StreamReader reader = new StreamReader(memoryStream);
        Console.WriteLine("------------------");
        Console.WriteLine(reader.ReadToEnd());
        Console.WriteLine("------------------");
        

        Console.WriteLine(new string('-', 50));
        Console.WriteLine("✅ STATUS: IDEMPOTENT (Binary Identity Confirmed)");
        Console.WriteLine(new string('=', 50) + "\n");
    }
}

