using AutoCadMcp.Model;
using AutoCadMcp.Model.Event;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;

namespace AutoCADMcpPlugin.Event
{
    public class AutoLispExecutionHandler : IEventHandler<AutoLispExecutionEvent>
    {
        public async Task<string> HandleAsync(AutoLispExecutionEvent @event)
        {
            // Get the current document and editor
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            try
            {
                // Execute the AutoLISP code
                doc.SendStringToExecute(@event.Code + "\n", true, false, false);

                // Return success message
                return await Task.FromResult("AutoLISP code executed successfully.");
            }
            catch (System.Exception ex)
            {
                // Log and return error message
                ed.WriteMessage($"Error executing AutoLISP code: {ex.Message}\n");
                return await Task.FromResult($"Error: {ex.Message}");
            }
        }
    }
}