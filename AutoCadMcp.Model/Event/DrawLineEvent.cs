namespace AutoCadMcp.Model.Event;

public record DrawLineEvent(double StartX, double StartY, double EndX, double EndY) : IEvent
{
    public string Type => nameof(DrawLineEvent);
}