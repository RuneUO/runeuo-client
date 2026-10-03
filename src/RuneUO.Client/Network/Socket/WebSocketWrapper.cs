using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using RuneUO.Utility.Logging;
using TcpSocket = System.Net.Sockets.Socket;
using static System.Buffers.ArrayPool<byte>;

namespace RuneUO.Network.Socket;

/// <summary>
/// Handles websocket connections to shards that support it. `ws(s)://[hostname]` as the ip in settings.json.
/// For testing see `tools/ws/README.md` 
/// </summary>
sealed class WebSocketWrapper : SocketWrapper
{
    private const int MAX_RECEIVE_BUFFER_SIZE = 1024 * 1024; // 1MB
    private const int WS_KEEP_ALIVE_INTERVAL = 5;            // seconds

    private ClientWebSocket _webSocket;
    private TcpSocket _rawSocket;

    public override bool IsConnected => _webSocket?.State is WebSocketState.Connecting or WebSocketState.Open;
    public override EndPoint LocalEndPoint => _rawSocket?.LocalEndPoint;
    public bool IsCanceled => _tokenSource.IsCancellationRequested;

    private CancellationTokenSource _tokenSource = new();
    private CircularBuffer _receiveStream;

    public override void Connect(Uri uri) => ConnectAsync(uri, _tokenSource).Wait();

    public override void Send(byte[] buffer, int offset, int count)
    {
        var copy = Shared.Rent(count);
        Buffer.BlockCopy(buffer, offset, copy, 0, count);
        SendCopyAsync(copy, count);
    }

    private async void SendCopyAsync(byte[] copy, int count)
    {
        try
        {
            await _webSocket.SendAsync(copy.AsMemory().Slice(0, count), WebSocketMessageType.Binary, true, _tokenSource.Token);
        }
        catch (Exception e)
        {
            // async void: an exception escaping here would end the process. The receive loop reports the disconnect.
            if (!IsCanceled)
                Log.Warn($"WebSocket send failed: {e.Message}");
        }
        finally
        {
            Shared.Return(copy);
        }
    }

    public override int Read(byte[] buffer)
    {
        lock (_receiveStream)
        {
            return _receiveStream.Dequeue(buffer, 0, buffer.Length);
        }
    }

    public async Task ConnectAsync(Uri uri, CancellationTokenSource tokenSource = null)
    {
        if (IsConnected)
            return;

        _tokenSource = tokenSource ?? new CancellationTokenSource();
        _receiveStream = new CircularBuffer();

        try
        {
            await ConnectWebSocketAsyncCore(uri);

            if (IsConnected)
                InvokeOnConnected();
            else
                InvokeOnError(SocketError.NotConnected);
        }
        catch (WebSocketException ex)
        {
            SocketError error = ex.InnerException?.InnerException switch
            {
                SocketException socketException => socketException.SocketErrorCode,
                _ => SocketError.SocketError
            };

            Log.Error($"Error {ex.GetType().Name} {error} while connecting to {uri} {ex}");
            InvokeOnError(error);
        }
        catch (Exception ex)
        {
            Log.Error($"Unknown Error {ex.GetType().Name} while connecting to {uri} {ex}");
            InvokeOnError(SocketError.SocketError);
        }
    }


    private async Task ConnectWebSocketAsyncCore(Uri uri)
    {
        // Take control of creating the raw socket, turn off Nagle, also lets us peek at `Available` bytes.
        _rawSocket = new TcpSocket(SocketType.Stream, ProtocolType.Tcp)
        {
            NoDelay = true
        };

        _webSocket = new ClientWebSocket();
        _webSocket.Options.KeepAliveInterval = TimeSpan.FromSeconds(WS_KEEP_ALIVE_INTERVAL); // ping/pong

        using var httpClient = new HttpClient
        (
            new SocketsHttpHandler
            {
                ConnectCallback = async (context, token) =>
                {
                    try
                    {
                        await _rawSocket.ConnectAsync(context.DnsEndPoint, token);

                        return new NetworkStream(_rawSocket, ownsSocket: true);
                    }
                    catch
                    {
                        _rawSocket?.Dispose();
                        _rawSocket = null;
                        _webSocket?.Dispose();
                        _webSocket = null;

                        throw;
                    }
                }
            }
        );


        await _webSocket.ConnectAsync(uri, httpClient, _tokenSource.Token);

        Log.Trace($"Connected WebSocket: {uri}");

        // Kicks off the async receiving loop 
        _ = StartReceiveAsync();
    }

    private async Task StartReceiveAsync()
    {
        var buffer = Shared.Rent(4096);
        var memory = buffer.AsMemory();
        var position = 0;
        var errorReported = false;

        try
        {
            while (IsConnected)
            {
                GrowReceiveBufferIfNeeded(ref buffer, ref memory, position);

                var receiveResult = await _webSocket.ReceiveAsync(memory.Slice(position), _tokenSource.Token);

                // Ignoring message types:
                // 1. WebSocketMessageType.Text: shouldn't be sent by the server, though might be useful for multiplexing commands
                // 2. WebSocketMessageType.Close: will be handled by IsConnected
                if (receiveResult.MessageType == WebSocketMessageType.Binary)
                    position += receiveResult.Count;

                if (!receiveResult.EndOfMessage)
                    continue;

                lock (_receiveStream)
                {
                    _receiveStream.Enqueue(buffer, 0, position);
                }

                position = 0;
            }
        }
        catch (OperationCanceledException)
        {
            Log.Trace("WebSocket OperationCanceledException on websocket " + (IsCanceled ? "(was requested)" : "(remote cancelled)"));
        }
        catch (Exception e) when (!IsCanceled)
        {
            Log.Trace($"WebSocket error in StartReceiveAsync {e}");
            InvokeOnError(SocketError.SocketError);
            errorReported = true;
        }
        catch (Exception)
        {
            // Disposed or disconnected on request, nothing to report.
        }
        finally
        {
            Shared.Return(buffer);
        }

        if (!IsCanceled && !errorReported)
            InvokeOnError(SocketError.ConnectionReset);
    }

    // This is probably unnecessary, but WebSocket frames can be up to 2^63 bytes so we put some cap on it, yet to see packets larger than 4KB come through.
    // We peek the raw tcp socket available bytes, grow if the frame is bigger, we're naively assuming no compression.
    // A message split over several frames keeps the bytes already received at the front of the buffer.
    private void GrowReceiveBufferIfNeeded(ref byte[] buffer, ref Memory<byte> memory, int position)
    {
        // Always leave room for at least one byte, or ReceiveAsync gets an empty buffer and spins.
        var needed = position + Math.Max(_rawSocket?.Available ?? 0, 1);

        if (needed <= buffer.Length)
            return;

        if (needed > MAX_RECEIVE_BUFFER_SIZE)
            throw new SocketException((int)SocketError.MessageSize, $"WebSocket message frame too large: {needed} > {MAX_RECEIVE_BUFFER_SIZE}");

        var size = Math.Min(MAX_RECEIVE_BUFFER_SIZE, Math.Max(needed, buffer.Length * 2));

        Log.Trace($"WebSocket growing receive buffer {buffer.Length} bytes to {size} bytes");

        var bigger = Shared.Rent(size);
        buffer.AsSpan(0, position).CopyTo(bigger);
        Shared.Return(buffer);
        buffer = bigger;
        memory = buffer.AsMemory();
    }

    public override void Disconnect()
    {
        if (!IsConnected)
            return;

        try
        {
            _webSocket?.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnect", CancellationToken.None)
                .ContinueWith(_ => _tokenSource?.Cancel());
        }
        catch
        {
            _tokenSource?.Cancel();
        }
    }

    public override void Dispose()
    {
        try
        {
            _tokenSource?.Cancel();
        }
        catch (ObjectDisposedException)
        {
        }

        _webSocket?.Dispose();
        _webSocket = null;
        _rawSocket?.Dispose();
        _rawSocket = null;
    }
}