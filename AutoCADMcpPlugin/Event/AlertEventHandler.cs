using AutoCadMcp.Model;
using AutoCadMcp.Model.Event;
using Autodesk.AutoCAD.ApplicationServices;

namespace AutoCADMcpPlugin.Event;

public class AlertEventHandler : IEventHandler<AlertEvent>
{
    public Task<EventResult> HandleAsync(AlertEvent @event)
    {
        Application.ShowAlertDialog(@event.Message);
        var result = new EventResult(@event.Message, null);
        return Task.FromResult(result);
    }
}