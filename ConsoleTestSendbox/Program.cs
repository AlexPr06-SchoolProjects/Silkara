
//using System.Net;
//using System.Net.Sockets;
//using System.Text;
//using UTP;
//using UTP.Connection;
//using UTP.UnitTests.Helpers.Payload;
//using UTP.UtpMessage;

//var headers = new Dictionary<string, string> { ["Type"] = "ConsoleTest🚀🚀🚀" };
//var payload = new TestPayload(777, "Hello from Console!🚀🚀🚀🚀");

//await Test.TestUtpEngineIdempotence(100, headers, payload);

//class Test
//{
//    public static async Task TestUtpEngineIdempotence(
//        short actionCode,
//        Dictionary<string, string> headers,
//        TestPayload payload)
//    {
//        using Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//        listener.Bind(new IPEndPoint(IPAddress.Loopback, 0));
//        listener.Listen(1);

//        var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//        var connectTask = clientSocket.ConnectAsync(listener.LocalEndPoint!);
//        var serverSocket = await listener.AcceptAsync();
//        await connectTask;

//        var connClient = new UtpConnection(clientSocket);
//        var connServer = new UtpConnection(serverSocket);

//        var utpMessageSend = new UtpMessage<TestPayload>(actionCode, headers, payload);
//        var utpMessageReceive = new UtpMessage<TestPayload>();

//        UtpEngine engineClient = new UtpEngine(connClient);
//        UtpEngine engineServer = new UtpEngine(connServer);


//        string bigData = new string('A', 100);
//        var bigPayload = new TestPayload(1, bigData);

//        var message = new UtpMessage<TestPayload>(
//            actionCode: 1,
//            headers: new Dictionary<string, string> { ["Test"] = "BigDataCheck" },
//            payload: bigPayload
//        );

//        var stream = message.PayloadStream;
//        if (stream is null)
//        {
//            Console.WriteLine("message.PayloadStream is null");
//            return;
//        }

//        using StreamReader reader1 = new StreamReader(stream);
//        Console.WriteLine(reader1.ReadToEnd());

//        Console.WriteLine("📤 Отправка сообщения...");
//        await engineClient.SendMessageAsync(message);
//        Console.WriteLine("✅ Сообщение ушло в PipeWriter");

//        Console.WriteLine("📥 Ожидание приема...");
//        // Установим таймаут, чтобы не ждать вечно
//        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
//        try
//        {
//            var received = await engineServer.ReceiveMessageAsync<TestPayload>(cts.Token);
//            Console.WriteLine("🎉 Сообщение получено!");

//            Console.WriteLine($"Результат: ActionCode: {received.ActionCode}");
//            foreach (var header in received.Headers)
//                Console.WriteLine($"{header.Key} : {header.Value}");


//            if (received.PayloadStream is not null)
//            {
//                if (received.PayloadStream.CanSeek)
//                    received.PayloadStream.Position = 0;

//                using var reader = new StreamReader(received.PayloadStream, Encoding.UTF8);
//                string payloadText = await reader.ReadToEndAsync();
//                Console.WriteLine($"{received.Headers["pType"]} - {payloadText}");
//            } else
//            {
//                Console.WriteLine("PayloadStream is null");
//            }
//        }
//        catch (OperationCanceledException)
//        {
//            Console.WriteLine("❌ ТАЙМАУТ: Сервер так и не получил полное сообщение!");
//        }

//        Console.WriteLine("Нажми Enter для выхода...");
//        Console.ReadLine();
//    }
//}

