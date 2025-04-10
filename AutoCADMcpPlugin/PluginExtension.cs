using Autodesk.AutoCAD.Runtime;
using Acad = Autodesk.AutoCAD.ApplicationServices.Application;

namespace AutoCADMcpPlugin;
public class PluginExtension : IExtensionApplication
{
    public void Initialize()
    {
        var doc = Acad.DocumentManager.MdiActiveDocument;
        if (doc == null)
            return;
        doc.Editor.WriteMessage("\nPlugin initialized.");
    }

    public void Terminate()
    {
        // Add your termination code here
    }
}
