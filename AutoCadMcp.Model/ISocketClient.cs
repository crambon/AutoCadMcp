namespace AutoCadMcp.Model;

public interface ISocketClient
{
    void Connect();
    void Disconnect();
    void Send<T>(T eventMessage) where T : IEvent;
    EventResult? Receive();
    bool IsConnected { get; }
}