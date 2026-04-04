using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using System.Net;
using System.Net.Sockets;
using UTP.Connection;
using UTP.UtpMessage;
using UtpTypes.PayloadTypes;
using UtpTypes.UtpClientType;


namespace Benchmarking.Tests;

[MemoryDiagnoser]
[ThreadingDiagnoser]
//[DisassemblyDiagnoser(printSource: true)]
//[HardwareCounters(HardwareCounter.CacheMisses)]
[ExceptionDiagnoser]
[Config(typeof(Config))]
public class UtpClientSendMessageAsyncBenchmark
{
    private class Config : ManualConfig
    {
        public Config()
        {
            AddJob(Job.Default.WithId("WorkstationGC"));
            //AddJob(Job.Default.WithGcServer(true).WithId("ServerGC"));
        }
    }

    private UtpMessage<JsonPayload>[] _messages = null!;
    private UtpMessage<EmptyPayload>[] _emptyMessages = null!;
    private UtpClient? _utpClient;
    private TcpListener? _server;
    private TcpClient? _client;
    private readonly string _serverIp = "127.0.0.1";
    
    [Params(10, 50)]
    public int MegabytesAmount;
    private const int MessageCount = 5;

    [GlobalSetup]
    public void Setup()
    {
        _messages = new UtpMessage<JsonPayload>[MessageCount];
        _emptyMessages = new UtpMessage<EmptyPayload>[MessageCount];
        int actualPayloadSize = 1024 * 1024 * MegabytesAmount;

        for (int i = 0; i < MessageCount; ++i)
        {
            string bigData = new string('A', actualPayloadSize);
            _messages[i] = new UtpMessage<JsonPayload>(
                actionCode: 1,
                headers: new Dictionary<string, string> { ["TraceId"] = Guid.NewGuid().ToString() },
                payload: new JsonPayload(i, bigData)
            );
        }

        for (int i = 0; i < MessageCount; ++i)
        {
            _emptyMessages[i] = new UtpMessage<EmptyPayload>(
                actionCode: 1,
                headers: new Dictionary<string, string> { ["TraceId"] = Guid.NewGuid().ToString() },
                payload: EmptyPayload.Instance
            );
        }

        _server = new TcpListener(IPAddress.Parse(_serverIp), 0);
        _server.Start();
        int port = ((IPEndPoint)_server.LocalEndpoint).Port;

        _client = new TcpClient();
        _client.Connect(IPAddress.Parse(_serverIp), port);

        _ = _server.AcceptSocketAsync().ContinueWith(t => {
            var s = t.Result;
            byte[] buffer = new byte[65536];
            while (s.Connected) s.Receive(buffer);
        });

        var utpConnection = new UtpConnection(_client.Client);
        _utpClient = new UtpClient(utpConnection);
    }

    [GlobalCleanup]
    public async Task Cleanup()
    {
        if (_utpClient != null)
            await _utpClient.DisposeAsync();

        _client?.Close();
        _server?.Stop();
    }

    [Benchmark]
    public async Task SendBigMessagesAsync()
    {
        foreach (var t in _messages)
        {
            if (_utpClient != null)
                await _utpClient.SendMessageAsync(t);
        }
    }

    [Benchmark]
    public async Task SendMessagesWithEmptyPayloads()
    {
        foreach (var t in _emptyMessages)
        {
            if (_utpClient != null)
                await _utpClient.SendMessageAsync(t);
        }
    }
}
