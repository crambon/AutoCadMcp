namespace AutoCadMcp.Tcp;

using System.Text.Json;
using System.Text.Json.Serialization;
using AutoCadMcp.Model;

public class EventConverter : JsonConverter<IEvent>
{
    private static readonly Dictionary<string, Type> TypeMap;

    static EventConverter()
    {
        // IEvent を継承するすべての型を自動で登録
        TypeMap = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IEvent).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .ToDictionary(t => t.Name, t => t);
    }

    public override IEvent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (!root.TryGetProperty("type", out var typeProp))
            throw new JsonException("Missing 'type' property.");

        var typeName = typeProp.GetString();
        if (typeName == null || !TypeMap.TryGetValue(typeName, out var eventType))
            throw new JsonException($"Unknown event type: {typeName}");

        return (IEvent)JsonSerializer.Deserialize(root.GetRawText(), eventType, options)!;
    }

    public override void Write(Utf8JsonWriter writer, IEvent value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
    }
}
