namespace AutoCadMcp.Model.Event;

public record AutoLispExecutionEvent(string Code) : IEvent
{
    public string Type => nameof(AutoLispExecutionEvent);
}