namespace AutoCadMcp.Model;

public interface IEventSubscriber
{
    Task<string> Subscribe(IEventMessage eventMessage);
}