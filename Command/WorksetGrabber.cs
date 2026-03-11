using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyRevitChallenge.Command
{
    [Transaction(TransactionMode.Manual)]
    public class WorksetGrabber : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;
            try
            {
                var frm = new UI.WorksetGrabber(doc, uidoc);
                frm.ShowDialog();
            }
            catch(Exception ex)
            {

            }
            return Result.Succeeded;
        }
    }
}
