using AutoCadMcp.Model;
using AutoCadMcp.Model.Event;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace AutoCADMcpPlugin.Event;

public class DrawLineEventHandler : IEventHandler<DrawLineEvent>
{
    public Task<EventResult> HandleAsync(DrawLineEvent @event)
    {
        Document doc = Application.DocumentManager.MdiActiveDocument;
        Database db = doc.Database;

        using (Transaction tr = db.TransactionManager.StartTransaction())
        {
            var bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
            if (bt == null)
                return Task.FromResult(new EventResult("BlockTable could not be retrieved", null));

            var btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
            if (btr == null)
                return Task.FromResult(new EventResult("BlockTableRecord could not be retrieved", null));

            Point3d startPoint = new Point3d(@event.StartX, @event.StartY, 0);
            Point3d endPoint = new Point3d(@event.EndX, @event.EndY, 0);
            
            using (Line line = new Line(startPoint, endPoint))
            {
                btr.AppendEntity(line);
                tr.AddNewlyCreatedDBObject(line, true);
            }

            tr.Commit();
        }

        return Task.FromResult(new EventResult("Line drawn successfully", null));
    }
}