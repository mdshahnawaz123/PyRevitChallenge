using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PyRevitChallenge.UI
{
    public partial class NameSwapper : Window
    {
        private readonly Document doc;
        private readonly UIDocument uidoc;

        public NameSwapper(Document doc, UIDocument uidoc)
        {
            InitializeComponent();
            this.doc = doc;
            this.uidoc = uidoc;
            LoadViews();
        }

        
        private void LoadViews()
        {
            var views = new FilteredElementCollector(doc)
                .OfClass(typeof(View))
                .Cast<View>()
                .Where(v => !v.IsTemplate)
                .OrderBy(v => v.Name)
                .ToList();

            ViewTree.ItemsSource = views;
            ViewTree.DisplayMemberPath = "Name";
        }

        
        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (doc == null) return;

            var selectedViews = ViewTree.SelectedItems
                .OfType<View>()
                .ToList();

            if (!selectedViews.Any())
            {
                TaskDialog.Show("Name Swapper", "No views selected.");
                return;
            }

            using (var tx = new Transaction(doc, "Rename Views"))
            {
                tx.Start();

                foreach (var view in selectedViews)
                {
                    var originalName = view.Name;

                    // Skip if find text not present
                    if (!string.IsNullOrEmpty(Text_Find.Text) &&
                        !originalName.Contains(Text_Find.Text))
                        continue;

                    var newName =
                        $"{Text_Prefix.Text}" +
                        $"{originalName.Replace(Text_Find.Text, Text_RePlace.Text)}" +
                        $"{Text_Sufix.Text}";

                    try
                    {
                        // 🔥 SPECIAL HANDLING FOR SHEETS
                        if (view.ViewType == ViewType.DrawingSheet)
                        {
                            var sheet = view as ViewSheet;
                            sheet.Name = newName; // rename sheet title
                        }
                        else
                        {
                            view.Name = newName;
                        }
                    }
                    catch (Exception ex)
                    {
                        TaskDialog.Show("Rename Failed",
                            $"{originalName}\n\n{ex.Message}");
                    }
                }

                tx.Commit();
            }

            // Refresh UI
            LoadViews();
        }

        private void ViewTree_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Optional: preview logic later
        }
    }
}