using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using UTP.Connection;
using UTP.Constants;
using UTP.Payload;
using UTP.UtpMessage;
using UTP.UtpMessage.Interfaces;
using UtpTypes.PayloadTypes;
using UtpTypes.UtpClientType;

namespace Benchmarking.Tests;

[MemoryDiagnoser]
[ThreadingDiagnoser]
//[DisassemblyDiagnoser(printSource: true, printInstructionAddresses: true)]
//[HardwareCounters(HardwareCounter.CacheMisses)]
[ExceptionDiagnoser]
[Config(typeof(Config))]
public class UtpClientReceiveMessageAsyncBenchmark
{
    private class Config : ManualConfig
    {
        public Config()
        {
            AddJob(Job.Default.WithId("WorkstationGC"));
            //AddJob(Job.Default.WithGcServer(true).WithId("ServerGC"));
        }
    }

    [Params(50)] 
    public int MegabytesAmount;

    private UtpClient _serverUtp = null!;
    private UtpClient _clientUtp = null!;
    private TcpListener _listener = null!;
    private TcpClient _serverSideClient = null!;
    private TcpClient _clientSideClient = null!;
    private CancellationTokenSource? _cts;

    private byte[] _preSerializedMessage = null!;

    [GlobalSetup]
    public void Setup()
    {

        // JSON PAYLAOD 
        //var payload = new JsonPayload(1, new string('A', 1024 * 1024 * MegabytesAmount));
        //var message = new UtpMessage<JsonPayload>(1, new Dictionary<string, string>(), payload);

        // EMPTY PAYLAOD
        var emptyPaylaod = new EmptyPayload();
        var message = new UtpMessage<EmptyPayload>(1, new Dictionary<string, string>(), emptyPaylaod);

        _preSerializedMessage = SerializeToBytes(message);

        // Raise connection
        _listener = new TcpListener(IPAddress.Loopback, 0);
        _listener.Start();
        var port = ((IPEndPoint)_listener.LocalEndpoint).Port;

        _clientSideClient = new TcpClient();
        _clientSideClient.Connect(IPAddress.Loopback, port);

        _serverSideClient = _listener.AcceptTcpClient();

        _serverUtp = new UtpClient(new UtpConnection(_serverSideClient.Client));
        _clientUtp = new UtpClient(new UtpConnection(_clientSideClient.Client));


        _cts  = new CancellationTokenSource();
        _ = Task.Run(async () =>
        {
            while (!_cts.Token.IsCancellationRequested && _clientSideClient.Connected) 
            {
                await _clientSideClient.GetStream().WriteAsync(_preSerializedMessage);
                // On my PC I found dependence of time required to process receiving message on the amount of megabytes message encompasses.
                // It is 1 : 3 , but I preferred 2 in sake of tests' accuracy
                await Task.Delay(MegabytesAmount * 2); 
            }
        }, _cts.Token);
    }

    [Benchmark]
    public async Task ReceiveMessageAsync()
    {
        await _serverUtp.ReceiveMessageAsync();
    }

    [GlobalCleanup]
    public async Task Cleanup()
    {
        _cts?.Cancel();

        await Task.Delay(50);
        try
        {
            _clientSideClient?.Dispose();
            _serverSideClient?.Dispose();
            _listener?.Stop();

            if (_serverUtp != null) await _serverUtp.DisposeAsync();
            if (_clientUtp != null) await _clientUtp.DisposeAsync();
        }
        catch (ObjectDisposedException) { /* It is OKAY */ }
        finally
        {
            _cts?.Dispose();
        }

    }

    private byte[] SerializeToBytes<TPayload>(UtpMessage<TPayload> utpMessage)
        where TPayload : IPayload
    {
        byte[] serializedHeaders = JsonSerializer.SerializeToUtf8Bytes(utpMessage.Headers);

        long payloadLen = 0;
        if (utpMessage.PayloadStream != null)
        {
            if (utpMessage.PayloadStream.CanSeek)
                utpMessage.PayloadStream.Position = 0;
            payloadLen = utpMessage.PayloadStream.Length;
        }

        int dataSizeAfterLengthField = UtpConstants.Sizes.ActionCodeLen +
                                       UtpConstants.Sizes.HeadersLen +
                                       serializedHeaders.Length +
                                       (int)payloadLen;

        int totalSize = UtpConstants.Sizes.MessageLen + dataSizeAfterLengthField;
        byte[] result = new byte[totalSize];
        Span<byte> span = result;

        int offset = 0;

        BinaryPrimitives.WriteInt32BigEndian(span.Slice(offset), dataSizeAfterLengthField);
        offset += UtpConstants.Sizes.MessageLen;

        BinaryPrimitives.WriteInt16BigEndian(span.Slice(offset), utpMessage.ActionCode);
        offset += UtpConstants.Sizes.ActionCodeLen;

        BinaryPrimitives.WriteInt32BigEndian(span.Slice(offset), serializedHeaders.Length);
        offset += UtpConstants.Sizes.HeadersLen;

        serializedHeaders.CopyTo(span.Slice(offset));
        offset += serializedHeaders.Length;

        if (utpMessage.PayloadStream != null && utpMessage.PayloadStream != Stream.Null)
        {
            using (var ms = new MemoryStream(result, offset, (int)payloadLen))
            {
                utpMessage.PayloadStream.CopyTo(ms);
            }
        }

        return result;
    }
}
