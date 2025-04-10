using AutoCadMcp.Model;
using AutoCadMcp.Model.Event;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;

namespace AutoCADMcpPlugin.Event
{
    public class AutoLispExecutionHandler : IEventHandler<AutoLispExecutionEvent>
    {
        public async Task<EventResult> HandleAsync(AutoLispExecutionEvent @event)
        {
            // Get the current document and editor
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            try
            {
                // Execute the AutoLISP code
                doc.SendStringToExecute(@event.Code + "\n", true, false, false);

                // Return success message
                var result = new EventResult("AutoLISP code executed successfully.", null);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                // Log and return error message
                ed.WriteMessage($"Error executing AutoLISP code: {ex.Message}\n");
                var result = new EventResult($"Error executing AutoLISP code: {ex.Message}", null);
                return await Task.FromResult(result);
            }
        }
    }
}