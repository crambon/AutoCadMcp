namespace AutoCadMcp.Model;

public enum EventType
{
    Alert,
}

public interface IEventMessage
{
    EventType type { get; }
}

public record AlertEvent(string Message) : IEventMessage
{
    public EventType type => EventType.Alert;
}
