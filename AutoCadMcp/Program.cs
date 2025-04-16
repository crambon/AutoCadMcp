using AutoCadMcp.Model;
using AutoCadMcp.Model.Event;
using AutoCadMcp.Tcp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Protocol.Types;
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

    [McpServerTool, Description("Connect to the AutoCAD server.")]
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

    private static IEnumerable<Content> SendEvent(IEvent @event)
    {
        var contents = new List<Content>();
        if (!socketClient.IsConnected)
        {
            var result = Connect();
            contents.Add(new Content
            {
                Type = "text",
                Text = result
            });
        }
        try
        {
            socketClient.Send(@event);
            var response = socketClient.Receive();
            if (response == null)
            {
                contents.Add(new Content
                {
                    Type = "text",
                    Text = "No response from server."
                });
                return contents;
            }
            contents.Add(new Content
            {
                Type = "text",
                Text = response.Text,
            });
            if (response.ImageData != null)
            {
                contents.Add(new Content
                {
                    Type = "image",
                    Data = response.ImageData.Data,
                    MimeType = response.ImageData.MimeType
                });
            }
            return contents;
        }
        catch (Exception ex)
        {
            contents.Add(new Content
            {
                Type = "text",
                Text = $"Error sending event: {ex.Message}"
            });
            return contents;
        }
    }

    [McpServerTool, Description("Execute AutoLISP code in AutoCAD")]
    public static IEnumerable<Content> ExecuteAutoLisp(string code) => SendEvent(new AutoLispExecutionEvent(code));
}
