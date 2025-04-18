using Autodesk.AutoCAD.Runtime;
using AutoCadMcp.Tcp;
using AutoCADMcp.Tcp;

[assembly: CommandClass(typeof(AutoCADMcpPlugin.McpCommand))]
[assembly: ExtensionApplication(typeof(AutoCADMcpPlugin.PluginExtension))]

namespace AutoCADMcpPlugin;

public class McpCommand
{
    private SocketServer _socketServer = new(SocketConfig.Default);

    [CommandMethod("StartMcp", CommandFlags.Modal)]
    public void StartMcp()
    {
        _socketServer.Start();
    }

    [CommandMethod("StopMcp", CommandFlags.Modal)]
    public void StopMcp()
    {
        _socketServer.Stop();
    }
}
