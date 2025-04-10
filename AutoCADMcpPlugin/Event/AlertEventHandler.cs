using AutoCadMcp.Model;
using AutoCadMcp.Model.Event;
using Autodesk.AutoCAD.ApplicationServices;

namespace AutoCADMcpPlugin.Event;

public class AlertEventHandler : IEventHandler<AlertEvent>
{
    public Task<string> HandleAsync(AlertEvent @event)
    {
        Application.ShowAlertDialog(@event.Message);
        return Task.FromResult($"Alert: {@event.Message}");
    }
}