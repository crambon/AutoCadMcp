# AutoCAD MCP (Model Context Protocol) Plugin

This project integrates AutoCAD with modern AI systems using Model Context Protocol (MCP). It creates a bridge between AutoCAD and AI assistants, allowing AI tools to interact with AutoCAD programmatically.

## Project Architecture

The solution consists of four main projects:

### 1. AutoCadMcp.Model
Contains the core interfaces and models that define the event system:
- `IEvent`: Base interface for all events
- `IEventHandler<TEvent>`: Generic interface for event handlers
- `EventBus`: Central dispatcher that routes events to their handlers
- Event record classes (AlertEvent, AutoLispExecutionEvent, etc.)

### 2. AutoCadMcp.Tcp
Handles communication between the MCP server and the AutoCAD plugin:
- `SocketClient`: Client used by the MCP server to connect to the AutoCAD plugin
- `SocketServer`: TCP server in the AutoCAD plugin that listens for events
- `SocketConfig`: Configuration for the TCP communication
- `EventConverter`: Converts between JSON and event objects

### 3. AutoCADMcpPlugin
The actual AutoCAD plugin that runs within AutoCAD:
- `PluginExtension`: Initializes the plugin when AutoCAD starts
- `McpCommand`: Adds AutoCAD commands to start/stop the MCP server
- Event handlers (AlertEventHandler, AutoLispExecutionHandler, etc.)

### 4. AutoCadMcp
The MCP server that connects to AutoCAD:
- Implements the Model Context Protocol
- Provides tools that can be called by AI assistants
- Forwards requests to AutoCAD through the TCP connection

## Communication Flow

1. AI assistant calls a tool in the MCP server
2. MCP server creates an event object
3. MCP server sends the event via TCP to the AutoCAD plugin
4. AutoCAD plugin receives the event and routes it through the EventBus
5. The appropriate EventHandler processes the event within AutoCAD
6. Response is sent back to the MCP server and ultimately to the AI assistant

## How to Add New Events and Event Handlers

### Step 1: Define a New Event

Create a new record class in the `AutoCadMcp.Model/Event` folder:

```csharp
namespace AutoCadMcp.Model.Event;

public record NewFunctionEvent(string Parameter1, int Parameter2) : IEvent
{
    public string Type => nameof(NewFunctionEvent);
}
```

The event should:
- Be defined as a record class for immutability and automatic property generation
- Implement the `IEvent` interface
- Have a `Type` property that returns its class name
- Include any parameters needed for the operation

### Step 2: Create an Event Handler

Add a new handler class in the `AutoCADMcpPlugin/Event` folder:

```csharp
using AutoCadMcp.Model;
using AutoCadMcp.Model.Event;
using Autodesk.AutoCAD.ApplicationServices;

namespace AutoCADMcpPlugin.Event;

public class NewFunctionEventHandler : IEventHandler<NewFunctionEvent>
{
    public Task<string> HandleAsync(NewFunctionEvent @event)
    {
        // Implement the actual functionality in AutoCAD
        Document doc = Application.DocumentManager.MdiActiveDocument;
        
        // Do something with doc and @event.Parameter1, @event.Parameter2
        
        return Task.FromResult("Operation completed successfully");
    }
}
```

The handler should:
- Implement the `IEventHandler<TEvent>` interface for your specific event type
- Contain the logic to execute the operation within AutoCAD
- Return a Task<string> with a response message

### Step 3: Add a Tool Method to the MCP Server

Add a method to the `AutoCadTool` class in `AutoCadMcp/Program.cs`:

```csharp
[McpServerTool, Description("Description of what the new function does")]
public static string NewFunction(string parameter1, int parameter2) => 
    SendEvent(new NewFunctionEvent(parameter1, parameter2));
```

The tool method should:
- Have the `[McpServerTool]` attribute
- Include a description of what it does
- Accept the parameters needed for the event
- Call `SendEvent` with a new instance of your event

### Step 4: Build and Commit Changes

When making changes to the codebase, follow these steps:

1. Build the solution to verify your changes:
```bash
dotnet build
```

2. Fix any build errors if they occur and verify that the build succeeds

3. Make your changes following the steps above

4. Stage your changes:
```bash
git add .
```

5. Commit your changes with an emoji that represents the type of change:
```bash
git commit -m ":sparkles: Add new feature for XYZ"
```

Common emoji prefixes:
- :sparkles: (sparkles): New feature
- :bug: (bug): Bug fix
- :memo: (memo): Documentation changes
- :recycle: (recycle): Refactoring code
- :art: (art): Improving structure/format of the code
- :zap:️ (zap): Performance improvements
- :wrench: (wrench): Configuration changes
- :rotating_light: (rotating_light): Tests
- :fire: (fire): Removing code/files

6. Push your changes:
```bash
git push origin feature/your-feature-name
```

7. Create a pull request for review

## Event Registration

The system automatically discovers and registers events and handlers:

- `EventConverter` uses reflection to find all classes that implement `IEvent`
- `EventBus` uses reflection to find all classes that implement `IEventHandler<>`

As long as your events and handlers follow the naming conventions and implement the correct interfaces, they will be automatically registered.

## Debugging
Use MCP Tool to debug the AutoCAD plugin.

## Best Practices

1. Keep events small and focused on a single operation
2. Use descriptive names for events and handlers
3. Handle exceptions appropriately in handlers
4. Return meaningful response messages
5. Use AutoCAD's ApplicationServices, DatabaseServices, and other APIs as needed in handlers

This documentation should help you understand how to extend the system with new events and handlers.
