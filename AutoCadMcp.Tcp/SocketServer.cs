using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using AutoCadMcp.Model;
using AutoCADMcpPlugin.Model;

namespace AutoCADMcp.Tcp;


public class SocketServer(SocketConfig config, IEventSubscriber eventSubscriber) : ISocketServer
{
    private TcpListener? _listener;
    private CancellationTokenSource? _cts;
    private ServerStatus _status = ServerStatus.Stopped;

    public ServerStatus Start()
    {
        if (_status != ServerStatus.Stopped)
            return _status;

        _cts = new CancellationTokenSource();
        _listener = new TcpListener(config.Address, config.Port);
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
                    var eventMessage = JsonSerializer.Deserialize<AlertEvent>(received);
                    var result = eventMessage is null ? "Cannot parse event message." : await eventSubscriber.Subscribe(eventMessage);
                    var response = Encoding.UTF8.GetBytes(result ?? string.Empty);
                    await stream.WriteAsync(response, 0, response.Length, token);
                }
            }
        }
    }
}