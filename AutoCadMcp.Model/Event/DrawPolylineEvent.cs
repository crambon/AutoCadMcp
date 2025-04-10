namespace AutoCadMcp.Model.Event;

public record DrawPolylineEvent(Point[] Points) : IEvent
{
    public string Type => nameof(DrawPolylineEvent);
}

public record Point(double X, double Y);