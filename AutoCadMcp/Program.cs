using AutoCadMcp.Model;
using AutoCadMcp.Tcp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddConsole(consoleLogOptions =>
{
    // Configure all logs to go to stderr
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();
await builder.Build().RunAsync();

[McpServerToolType]
public static class AutoCadTool
{
    private static readonly SocketClient socketClient = new SocketClient(DefaultSocketConfig.Get());

    [McpServerTool, Description("Connect to the AutoCAD server")]
    public static string Connect()
    {
        try
        {
            socketClient.Connect();
            return "Connected to server successfully.";
        }
        catch (Exception ex)
        {
            return $"Error connecting to server: {ex.Message}";
        }
    }

    [McpServerTool, Description("Alert the message in AutoCAD Application")]
    public static string Alert(string message)
    {
        try
        {
            socketClient.Send(new AlertEvent(message));
            var response = socketClient.Receive();
            return response;
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }

}
