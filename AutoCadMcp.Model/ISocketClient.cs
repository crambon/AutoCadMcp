namespace AutoCadMcp.Model;

public interface ISocketClient
{
    void Connect();
    void Disconnect();
    void Send<T>(T eventMessage) where T : IEventMessage;
    string Receive();
    bool IsConnected { get; }
}