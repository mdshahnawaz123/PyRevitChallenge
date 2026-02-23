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
    public class MasterCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;
            try
            {
                


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
