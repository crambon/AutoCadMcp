namespace AutoCadMcp.Model;

public class EventBus
{
    private readonly Dictionary<Type, object> _handlers = new();

    public EventBus()
    {
        RegisterHandlers();
    }

    private void RegisterHandlers()
    {
        var handlerTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                .Select(i => new { HandlerType = t, MessageType = i.GetGenericArguments()[0] }));

        foreach (var h in handlerTypes)
        {
            var handlerInstance = Activator.CreateInstance(h.HandlerType);
            _handlers[h.MessageType] = handlerInstance!;
        }
    }

    public async Task<string> DispatchAsync(IEvent @event)
    {
        var eventType = @event.GetType();
        if (_handlers.TryGetValue(eventType, out var handler))
        {
            var method = handler.GetType().GetMethod("HandleAsync");
            if (method != null)
            {
                var task = (Task<string>)method.Invoke(handler, [@event])!;
                return await task.ConfigureAwait(false);
            }
        }

        throw new NotSupportedException($"No handler found for message type: {eventType.Name}");
    }
}
