using AutoCadMcp.Model;
using AutoCadMcp.Model.Event;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace AutoCADMcpPlugin.Event;

public class DrawPolylineEventHandler : IEventHandler<DrawPolylineEvent>
{
    public Task<EventResult> HandleAsync(DrawPolylineEvent @event)
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

            using (Polyline pline = new Polyline())
            {
                pline.SetDatabaseDefaults();
                
                for (int i = 0; i < @event.Points.Length; i++)
                {
                    var point = @event.Points[i];
                    pline.AddVertexAt(i, new Point2d(point.X, point.Y), 0, 0, 0);
                }

                btr.AppendEntity(pline);
                tr.AddNewlyCreatedDBObject(pline, true);
            }

            tr.Commit();
        }

        return Task.FromResult(new EventResult("Polyline drawn successfully", null));
    }
}