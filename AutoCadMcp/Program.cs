using AutoCadMcp.Model;
using AutoCadMcp.Model.Event;
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
    private static readonly SocketClient socketClient = new SocketClient(SocketConfig.Default);

    [McpServerTool, Description("Connect to the AutoCAD server. Must be called before any other commands.")]
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

    private static string SendEvent(IEvent @event)
    {
        try
        {
            socketClient.Send(@event);
            var response = socketClient.Receive();
            return response;
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    [McpServerTool, Description("Alert the message in AutoCAD Application")]
    public static string Alert(string message) => SendEvent(new AlertEvent(message));

    [McpServerTool, Description("Execute AutoLISP code in AutoCAD")]
    public static string ExecuteAutoLisp(string code) => SendEvent(new AutoLispExecutionEvent(code));
}
