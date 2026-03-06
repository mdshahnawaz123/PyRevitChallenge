using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace PyRevitChallenge.Command
{
    [Transaction(TransactionMode.Manual)]
    public class FloorifyMyRooms : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;
            try
            {
                var frm = new UI.FloorifyMyRooms(doc, uidoc);
                frm.ShowDialog();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return Result.Succeeded;
        }
    }
}
