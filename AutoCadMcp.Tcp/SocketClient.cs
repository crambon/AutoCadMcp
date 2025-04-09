using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using AutoCadMcp.Model;

namespace AutoCadMcp.Tcp;

public class SocketClient(SocketConfig config) : ISocketClient
{
    private TcpClient? _client;
    private NetworkStream? _stream;

    public bool IsConnected => _client?.Connected ?? false;

    public void Connect()
    {
        if (IsConnected)
            throw new InvalidOperationException("Already connected to a server.");

        _client = new TcpClient();
        _client.Connect(config.Address, config.Port);
        _stream = _client.GetStream();
    }

    public void Disconnect()
    {
        _stream?.Close();
        _client?.Close();
        _stream = null;
        _client = null;
    }

    public void Send<T>(T eventMessage) where T : IEventMessage
    {
        if (!IsConnected || _stream == null)
            throw new InvalidOperationException("Not connected to a server.");

        var message = JsonSerializer.Serialize(eventMessage);
        byte[] data = Encoding.UTF8.GetBytes(message);
        _stream.Write(data, 0, data.Length);
    }

    public string Receive()
    {
        if (!IsConnected || _stream == null)
            throw new InvalidOperationException("Not connected to a server.");

        byte[] buffer = new byte[1024];
        int bytesRead = _stream.Read(buffer, 0, buffer.Length);
        return Encoding.UTF8.GetString(buffer, 0, bytesRead);
    }
}