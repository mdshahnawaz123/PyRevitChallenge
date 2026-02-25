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
                .Where(x=>!x.IsTemplate)
                .ToList();
        }

        public static List<SpatialElement> GetRooms(this Document doc)
        {
            return new FilteredElementCollector(doc)
                .OfClass(typeof(SpatialElement))
                .Cast<SpatialElement>()
                .Where(x => x.Category.Id.Value == (int)BuiltInCategory.OST_Rooms)
                .ToList();
        }

    }
}