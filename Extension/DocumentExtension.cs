using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

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

        public static List<SpatialElement> GetRooms(this Document doc)
        {
            return new FilteredElementCollector(doc)
                .OfClass(typeof(SpatialElement))
                .Cast<SpatialElement>()
                .Where(x => x.Category.Id.Value == (int)BuiltInCategory.OST_Rooms)
                .ToList();
        }

        public static List<Category> GetCategories(this Document doc)
        {
            return doc.Settings.Categories
                .Cast<Category>()
                .Where(c =>
                    c.CategoryType == CategoryType.Model &&
                    !c.IsTagCategory &&
                    c.CanAddSubcategory)
                .OrderBy(c => c.Name)
                .ToList();
        }

        public class MasterSelection : ISelectionFilter
        {
            public IList<ElementId> ElementIds;

            public MasterSelection(IList<Element> elements)
            {
                ElementIds = elements
                    .Select(x=>x.Id)
                    .ToList();
            }
            public bool AllowElement(Element elem)
            {
                return ElementIds.Contains(elem.Id);
            }

            public bool AllowReference(Reference reference, XYZ position)
            {
                return true;
            }
        }

        public static IList<Workset> GetWorksets(this Document doc)
        {
            return new FilteredWorksetCollector(doc)
                .OfKind(WorksetKind.UserWorkset)
                .ToWorksets()
                .ToList();
        }

        public static IList<Element> GetElementBasedOnWorkSet(this Document doc, Workset workset)
        {
            return new FilteredElementCollector(doc)
                .WherePasses(new ElementWorksetFilter(workset.Id))
                .ToElements()
                .ToList();
        }
    }
}