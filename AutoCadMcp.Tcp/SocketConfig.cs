
using System.Net;

public record SocketConfig(IPAddress Address, int Port);

public static class DefaultSocketConfig
{
    public static SocketConfig Get() => new(IPAddress.Any, 5000);
}