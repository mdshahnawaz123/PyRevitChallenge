using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using PyRevitChallenge.UI;

namespace PyRevitChallenge.Command
{
    [Transaction(TransactionMode.Manual)]
    public class DreamPicker2 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;
            try
            {
                
                
                var frm = new UI.DreamPicker2(doc,uidoc);
                frm.Show();


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
