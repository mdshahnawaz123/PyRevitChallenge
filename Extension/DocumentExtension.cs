using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace PyRevitChallenge.Extension
{
    public static class DocumentExtension
    {
        public static List<FamilyInstance> GetInPlaceInstances(this Document doc)
        {
            return new FilteredElementCollector(doc)
                .OfClass(typeof(FamilyInstance))
                .Cast<FamilyInstance>()
                .Where(x => x.Symbol.Family.IsInPlace)
                .ToList();
        }

        public static List<FamilyInstance> GetFamilyInstances(this Document doc)
        {
            return new FilteredElementCollector(doc)
                .OfClass(typeof(FamilyInstance))
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .ToList();
        }

        public static List<View> GetViews(this Document doc)
        {
            return new FilteredElementCollector(doc)
                .OfClass(typeof(View))
                .Cast<View>()
                .ToList();
        }
    }
}