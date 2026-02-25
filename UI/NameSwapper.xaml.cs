using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using PyRevitChallenge.Extension;
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
            var viewList = doc.GetViews();
            ViewTree.ItemsSource = viewList;
            ViewTree.DisplayMemberPath = "Name";
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (doc == null) return;

            var allowedTypes = new HashSet<ViewType>
            {
                ViewType.FloorPlan,
                ViewType.CeilingPlan,
                ViewType.AreaPlan,
                ViewType.DrawingSheet,
                ViewType.Elevation,
                ViewType.DraftingView,
                ViewType.ThreeD,
                ViewType.Section
            };

            var selectedViews = ViewTree.SelectedItems
                .OfType<View>()
                .Where(v => allowedTypes.Contains(v.ViewType))
                .ToList();

            if (!selectedViews.Any())
            {
                TaskDialog.Show("Name Swapper", "No valid views selected.");
                return;
            }

            using (var tx = new Transaction(doc, "Rename Views"))
            {
                tx.Start();

                foreach (var view in selectedViews)
                {
                    var viewName = view.Name;

                    if (!string.IsNullOrEmpty(Text_Find.Text) &&
                        viewName.Contains(Text_Find.Text))
                    {
                        var newName =
                            $"{Text_Prefix.Text}" +
                            $"{viewName.Replace(Text_Find.Text, Text_RePlace.Text)}" +
                            $"{Text_Sufix.Text}";

                        try
                        {
                            view.Name = newName;
                        }
                        catch (Exception ex)
                        {
                            TaskDialog.Show("Rename Failed",
                                $"{viewName}\n\n{ex.Message}");
                        }
                    }
                }

                tx.Commit();
            }

            // Refresh UI
            LoadViews();
        }

        private void ViewTree_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}