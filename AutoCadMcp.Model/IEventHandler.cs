namespace AutoCadMcp.Model;

public interface IEventHandler<in TEvent> where TEvent : IEvent
{
    Task<string> HandleAsync(TEvent @event);
}