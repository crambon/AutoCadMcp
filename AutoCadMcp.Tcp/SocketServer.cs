namespace AutoCADMcp.Tcp;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using AutoCadMcp.Model;
using AutoCadMcp.Tcp;
using AutoCADMcpPlugin.Model;



public class SocketServer : ISocketServer
{
    private TcpListener? _listener;
    private CancellationTokenSource? _cts;
    private ServerStatus _status = ServerStatus.Stopped;

    private readonly SocketConfig _config;

    private readonly EventBus _eventBus;

    public SocketServer(SocketConfig config)
    {
        _config = config;
        _eventBus = new EventBus();
    }

    public ServerStatus Start()
    {
        if (_status != ServerStatus.Stopped)
            return _status;

        _cts = new CancellationTokenSource();
        _listener = new TcpListener(IPAddress.Any, _config.Port);
        _listener.Start();
        _status = ServerStatus.Started;

        _ = Task.Run(() => AcceptClientsAsync(_cts.Token));

        return _status;
    }

    public ServerStatus Stop()
    {
        if (_status == ServerStatus.Stopped)
            return _status;

        _cts?.Cancel();
        _listener?.Stop();
        _status = ServerStatus.Stopped;

        return _status;
    }

    private async Task AcceptClientsAsync(CancellationToken token)
    {
        if (_listener == null)
            return;

        _status = ServerStatus.Running;

        try
        {
            while (!token.IsCancellationRequested)
            {
                var client = await _listener.AcceptTcpClientAsync();
                _ = Task.Run(() => HandleClientAsync(client, token));
            }
        }
        catch (ObjectDisposedException)
        {
            // Listener was stopped, ignore this exception
        }
        finally
        {
            _status = ServerStatus.Stopped;
        }
    }

    private async Task HandleClientAsync(TcpClient client, CancellationToken token)
    {
        using (client)
        {
            var stream = client.GetStream();
            var buffer = new byte[1024];

            while (!token.IsCancellationRequested && client.Connected)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token);
                if (bytesRead > 0)
                {
                    string received = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    // json decode
                    var @event = JsonSerializer.Deserialize<IEvent>(received, _config.JsonSerializerOptions);
                    var result = @event is null ? "Cannot parse event message." : await _eventBus.DispatchAsync(@event);
                    var response = Encoding.UTF8.GetBytes(result ?? string.Empty);
                    await stream.WriteAsync(response, 0, response.Length, token);
                }
            }
        }
    }
}