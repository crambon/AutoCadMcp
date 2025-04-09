namespace AutoCADMcpPlugin.Model;

public enum ServerStatus
{
    Stopped,
    Started,
    Running
}


public interface ISocketServer
{
    ServerStatus Start();

    ServerStatus Stop();
}