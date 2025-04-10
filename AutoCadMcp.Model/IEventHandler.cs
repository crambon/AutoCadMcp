namespace AutoCadMcp.Model;

public record ImageData(string Data, string MimeType);

public record EventResult(string Text, ImageData? ImageData);

public interface IEventHandler<in TEvent> where TEvent : IEvent
{
    Task<EventResult> HandleAsync(TEvent @event);
}