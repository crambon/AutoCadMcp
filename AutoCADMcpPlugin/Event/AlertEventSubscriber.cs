using AutoCadMcp.Model;
using Acad = Autodesk.AutoCAD.ApplicationServices.Application;

namespace AutoCADMcpPlugin.Event;

public class AlertEventSubscriber
{
    public async Task<string> Subscribe(AlertEvent eventMessage)
    {
        await Task.Yield();
        Acad.ShowAlertDialog(eventMessage.Message);
        return "Success Alert Dialog";
    }
}