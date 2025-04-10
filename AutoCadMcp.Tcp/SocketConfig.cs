namespace AutoCadMcp.Tcp;

using System.Text.Json;

public class SocketConfig(int port)
{
    public int Port { get; } = port;

    public JsonSerializerOptions JsonSerializerOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new EventConverter()
        }
    };

    public static SocketConfig Default => new(9887);
}
