using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using PyRevitChallenge.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace PyRevitChallenge.UI
{
    public partial class BIMpressionist_Painter : Window
    {
        private Document doc;
        private UIDocument uidoc;
        private System.Windows.Media.Color _selectedColor = Colors.White;

        public BIMpressionist_Painter(Document doc, UIDocument uidoc)
        {
            InitializeComponent();
            this.doc = doc;
            this.uidoc = uidoc;
            LoadCategories();
        }

        // ================= LOAD CATEGORIES =================
        private void LoadCategories()
        {
            var cats = doc.GetCategories();
            CategoryList.ItemsSource = cats;
            CategoryList.DisplayMemberPath = "Name";
        }

        private void CategoryToElment(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selectedCategory = CategoryList.SelectedItem as Category;
            if (selectedCategory == null) return;

            var elements = new FilteredElementCollector(doc)
                .OfCategoryId(selectedCategory.Id)
                .WhereElementIsNotElementType()
                .ToList();

            SelectedElementList.ItemsSource = elements;
            SelectedElementList.DisplayMemberPath = "Name";
        }

        // ================= COLOR PICKER (FINAL FIXED) =================
        private void PickColor_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.ColorDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _selectedColor = System.Windows.Media.Color.FromArgb(
                    dialog.Color.A,
                    dialog.Color.R,
                    dialog.Color.G,
                    dialog.Color.B);

                ColorPreview.Background = new SolidColorBrush(_selectedColor);
            }
        }

        // ================= ASSIGN COLOR =================
        private void AssignColor_Click(object sender, RoutedEventArgs e)
        {
            var selectedElements = SelectedElementList.SelectedItems
                .Cast<Element>()
                .ToList();

            if (!selectedElements.Any()) return;

            var revitColor = new Autodesk.Revit.DB.Color(
                _selectedColor.R,
                _selectedColor.G,
                _selectedColor.B);

            // Get solid fill pattern (required for shaded views)
            var solidFill = new FilteredElementCollector(doc)
                .OfClass(typeof(FillPatternElement))
                .Cast<FillPatternElement>()
                .First(x => x.GetFillPattern().IsSolidFill);

            using (Transaction t = new Transaction(doc, "Assign Color"))
            {
                t.Start();

                foreach (var element in selectedElements)
                {
                    var ogs = new OverrideGraphicSettings();

                    ogs.SetProjectionLineColor(revitColor);
                    ogs.SetSurfaceForegroundPatternId(solidFill.Id);
                    ogs.SetSurfaceForegroundPatternColor(revitColor);

                    doc.ActiveView.SetElementOverrides(element.Id, ogs);
                }

                t.Commit();
            }
        }

        // ================= CLEAR OVERRIDE =================
        private void ClearOverride_Click(object sender, RoutedEventArgs e)
        {
            var selectedElements = SelectedElementList.SelectedItems
                .Cast<Element>()
                .ToList();

            if (!selectedElements.Any()) return;

            using (Transaction t = new Transaction(doc, "Clear Override"))
            {
                t.Start();

                foreach (var element in selectedElements)
                {
                    doc.ActiveView.SetElementOverrides(
                        element.Id,
                        new OverrideGraphicSettings());
                }

                t.Commit();
            }
        }

        // ================= SHOW IN REVIT =================
        private void ShowInRevit_Click(object sender, RoutedEventArgs e)
        {
            var ids = SelectedElementList.SelectedItems
                .Cast<Element>()
                .Select(x => x.Id)
                .ToList();

            if (!ids.Any()) return;

            var view = uidoc.ActiveView;

            using (Transaction t = new Transaction(doc, "Focus Elements"))
            {
                t.Start();

                if (view.IsTemporaryHideIsolateActive())
                    view.DisableTemporaryViewMode(TemporaryViewMode.TemporaryHideIsolate);
                else
                    view.IsolateElementsTemporary(ids);

                t.Commit();
            }

            uidoc.ShowElements(ids);
            uidoc.Selection.SetElementIds(ids);
        }

        // ================= CLEAN EXIT =================
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            var view = uidoc.ActiveView;

            // Reset temp isolate if active
            if (view.IsTemporaryHideIsolateActive())
            {
                using (var t = new Transaction(doc, "Reset Temp View"))
                {
                    t.Start();
                    view.DisableTemporaryViewMode(TemporaryViewMode.TemporaryHideIsolate);
                    t.Commit();
                }
            }

            // Clear selection
            uidoc.Selection.SetElementIds(new List<ElementId>());
        }
    }
}