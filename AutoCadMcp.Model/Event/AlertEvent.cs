namespace AutoCadMcp.Model.Event;

public record AlertEvent(string Message) : IEvent
{
    public string Type => nameof(AlertEvent);
}