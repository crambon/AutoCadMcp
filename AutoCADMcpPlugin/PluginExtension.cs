using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;

namespace AutoCADMcpPlugin;
public class PluginExtension : IExtensionApplication
{
    public void Initialize()
    {
        var doc = Application.DocumentManager.MdiActiveDocument;
        if (doc == null)
            return;
        doc.Editor.WriteMessage("\nPlugin initialized.");
    }

    public void Terminate()
    {
        // Add your termination code here
    }
}
