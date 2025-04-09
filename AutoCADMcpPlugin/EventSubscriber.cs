using AutoCadMcp.Model;
using AutoCADMcpPlugin.Event;

namespace AutoCADMcpPlugin;

public class EventSubscriber : IEventSubscriber
{
    public Task<string> Subscribe(IEventMessage eventMessage)
    {
        switch (eventMessage)
        {
            case AlertEvent alertEvent:
                return new AlertEventSubscriber().Subscribe(alertEvent);
            default:
                throw new NotImplementedException($"No subscriber for event type {eventMessage.GetType()}");
        }
    }
}