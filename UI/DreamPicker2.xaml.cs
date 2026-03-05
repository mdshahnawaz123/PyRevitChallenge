using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using PyRevitChallenge.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PyRevitChallenge.UI
{
    public partial class DreamPicker2 : Window
    {
        private Document Document;
        private UIDocument UIDocument;

        public DreamPicker2(Document doc, UIDocument uidoc)
        {
            InitializeComponent();
            Document = doc;
            UIDocument = uidoc;
            LoadCategories();
        }

        private void LoadCategories()
        {
            var categories = Document.GetCategories();
            CatComboBox.ItemsSource = categories;
            CatComboBox.DisplayMemberPath = "Name";
        }

        private void CategorySelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var category = CatComboBox.SelectedItem as Category;

            if (category == null)
                return;

            var elements = new FilteredElementCollector(Document)
                .OfCategoryId(category.Id)
                .WhereElementIsNotElementType()
                .ToList();

            ElementListView.ItemsSource = elements;
            ElementListView.DisplayMemberPath = "Name";
        }

        private void SelectElemnt(object sender, RoutedEventArgs e)
        {
            var selectedElements = ElementListView.SelectedItems
                                   .Cast<Element>()
                                   .ToList();

            if (selectedElements.Count == 0)
            {
                TaskDialog.Show("Selection", "Please select elements from the list.");
                return;
            }

            var picked = UIDocument.Selection.PickElementsByRectangle(
                new MasterSelection(selectedElements),
                "Select elements in model");

            UIDocument.Selection.SetElementIds(picked.Select(x => x.Id).ToList());
        }

        private void CancelElement(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public class MasterSelection : ISelectionFilter
        {
            private IList<ElementId> AllowedIds;

            public MasterSelection(IList<Element> elements)
            {
                AllowedIds = elements.Select(x => x.Id).ToList();
            }

            public bool AllowElement(Element elem)
            {
                return AllowedIds.Contains(elem.Id);
            }

            public bool AllowReference(Reference reference, XYZ position)
            {
                return false;
            }
        }
    }
}