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
using UtpTypes;
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

    [Params(5)] // Уменьшил до 50, так как 100МБ x 5 сообщ. быстро забьют RAM при прогреве
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
        // 1. Готовим данные заранее (чтобы не тратить время в фоне)
        var payload = new JsonPayload(1, new string('A', 1024 * 1024 * MegabytesAmount));
        var message = new UtpMessage<JsonPayload>(1, new Dictionary<string, string>(), payload);
        // Предположим, у тебя есть метод сериализации в байты, чтобы клиент слал "сырые" данные
        _preSerializedMessage = SerializeToBytes(message);

        // 2. Поднимаем соединение
        _listener = new TcpListener(IPAddress.Loopback, 0);
        _listener.Start();
        var port = ((IPEndPoint)_listener.LocalEndpoint).Port;

        _clientSideClient = new TcpClient();
        _clientSideClient.Connect(IPAddress.Loopback, port);

        _serverSideClient = _listener.AcceptTcpClient();

        _serverUtp = new UtpClient(new UtpConnection(_serverSideClient.Client));
        _clientUtp = new UtpClient(new UtpConnection(_clientSideClient.Client));


        _cts  = new CancellationTokenSource();
        // 3. Запускаем фоновый поток "спама" от клиента
        _ = Task.Run(async () =>
        {
            while (!_cts.Token.IsCancellationRequested && _clientSideClient.Connected) // Бесконечный поток данных для бенчмарка
            {
                await _clientSideClient.GetStream().WriteAsync(_preSerializedMessage);
                await Task.Delay(1);
            }
        }, _cts.Token);
    }

    [Benchmark]
    public async Task<IUtpMessage> ReceiveMessageAsync()
    {
        // Теперь измеряем только время вычитки и парсинга сообщения сервером
        return await _serverUtp.ReceiveMessageAsync();
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
        catch (ObjectDisposedException) { /* Это нормально */ }
        finally
        {
            _cts?.Dispose();
        }

    }

    // Вспомогательный метод (зависит от твоей реализации)
    private byte[] SerializeToBytes<TPayload>(UtpMessage<TPayload> utpMessage)
    where TPayload : IPayload
    {
        // 1. Сериализуем заголовки
        byte[] serializedHeaders = JsonSerializer.SerializeToUtf8Bytes(utpMessage.Headers);

        // 2. Считаем длину Payload (если есть стрим)
        long payloadLen = 0;
        if (utpMessage.PayloadStream != null)
        {
            if (utpMessage.PayloadStream.CanSeek)
                utpMessage.PayloadStream.Position = 0;
            payloadLen = utpMessage.PayloadStream.Length;
        }

        // 3. Вычисляем общий размер всего пакета
        // dataSizeAfterLengthField — это всё, что идет ПОСЛЕ первых 4 байт длины
        int dataSizeAfterLengthField = UtpConstants.Sizes.ActionCodeLen +
                                       UtpConstants.Sizes.HeadersLen +
                                       serializedHeaders.Length +
                                       (int)payloadLen;

        // Полный размер массива: 4 байта (Length) + всё остальное
        int totalSize = UtpConstants.Sizes.MessageLen + dataSizeAfterLengthField;
        byte[] result = new byte[totalSize];
        Span<byte> span = result;

        int offset = 0;

        // Записываем общую длину (Big Endian)
        BinaryPrimitives.WriteInt32BigEndian(span.Slice(offset), dataSizeAfterLengthField);
        offset += UtpConstants.Sizes.MessageLen;

        // Записываем ActionCode
        BinaryPrimitives.WriteInt16BigEndian(span.Slice(offset), utpMessage.ActionCode);
        offset += UtpConstants.Sizes.ActionCodeLen;

        // Записываем длину сериализованных заголовков
        BinaryPrimitives.WriteInt32BigEndian(span.Slice(offset), serializedHeaders.Length);
        offset += UtpConstants.Sizes.HeadersLen;

        // Копируем сами заголовки
        serializedHeaders.CopyTo(span.Slice(offset));
        offset += serializedHeaders.Length;

        // 4. Копируем Payload из стрима в итоговый массив
        if (utpMessage.PayloadStream != null)
        {
            // Используем MemoryStream, чтобы удобно вычитать всё в наш массив
            using (var ms = new MemoryStream(result, offset, (int)payloadLen))
            {
                utpMessage.PayloadStream.CopyTo(ms);
            }
        }

        return result;
    }
}
