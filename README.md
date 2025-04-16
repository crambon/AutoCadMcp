<h1 align="center">
  AutoCAD MCP Plugin
</h1>

<p align="center">
  <a href="README.md">English</a> •
  <a href="/docs/README-ja.md">日本語</a>
</p>

## Project Purpose

AutoCAD MCP Plugin is a plugin to connect AutoCAD and AI assistants (such as GitHub Copilot or Claude) via the Model Context Protocol (MCP). It enables AI tools to programmatically access AutoCAD, automate command execution, and retrieve information.

## How to Use the Plugin

1. In VS Code, select `AutoCAD Debug` from `Run and Debug` and start debugging.
2. Launch AutoCAD and load `AutoCADMcpPlugin.dll` using the `NETLOAD` command.
3. Type `STARTMCP` in the command line to start the MCP server.

## How to Connect via MCP

### Connecting with GitHub Copilot

1. Start the MCP server (AutoCadMcp) from `.github/mcp.json`. ![start-mcp](static/start-mcp.png)
2. Switch Copilot Chat to Agent mode and confirm that the AutoCADMcp server is added from the wrench icon at the top left. ![agent-mode](static/copilot-agent.png)
3. Give instructions such as "Draw a circle in AutoCAD" in Copilot Chat, and AutoCAD will be operated via the MCP server.

Reference: https://docs.github.com/en/copilot/customizing-copilot/extending-copilot-chat-with-mcp

### Connecting with Claude Desktop

1. Build the solution to create the `AutoCadMcp` executable.
2. Refer to [Claude Desktop MCP connection guide](https://modelcontextprotocol.io/quickstart/user) and open the MCP config file.
3. Specify the built executable for the server:
```yaml
{
  "mcpServers": {
    "AutoCADMcp": {
      "command": "\\path\\to\\AutoCadMcp.exe",
    }
  }
}
```
4. Restart Claude Desktop and confirm that the MCP server is added to the tools.
5. Give instructions such as "Draw a circle in AutoCAD" and AutoCAD will be operated via the MCP server.

## Notes

- Stop the server with the `StopMcp` command.
- Pay attention to the startup order of the plugin and MCP server.
- For details on extension and adding events, refer to the documentation in the project.