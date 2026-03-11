using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PyRevitChallenge.UI
{
    public partial class WorksetGrabber : Window
    {
        private Document doc;
        private UIDocument uidoc;

        public WorksetGrabber(Document doc, UIDocument uidoc)
        {
            InitializeComponent();

            this.doc = doc;
            this.uidoc = uidoc;

            LoadWorksets();
        }

        private void LoadWorksets()
        {
            var worksets = new FilteredWorksetCollector(doc)
                .OfKind(WorksetKind.UserWorkset)
                .ToList();

            WorksetList.ItemsSource = worksets;
            WorksetList.DisplayMemberPath = "Name";
        }

        private void WorksetList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WorksetList.SelectedItem is Workset selectedWorkset)
            {
                var worksetId = selectedWorkset.Id;

                var elements = new FilteredElementCollector(doc)
                    .WherePasses(new ElementWorksetFilter(worksetId))
                    .WhereElementIsElementType()
                    .Where(x => x.Category != null && x.Category.CategoryType == CategoryType.Model)
                    .ToList();

                EleWorksetSel.ItemsSource = elements;

                TotalCount.Text = elements.Count.ToString();
                SelectedCount.Text = "0";
            }
        }

        private void EleWorksetSel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedElements = EleWorksetSel
                .SelectedItems
                .Cast<Element>()
                .ToList();

            SelectedCount.Text = selectedElements.Count.ToString();
        }

        private void SelectElements_Click(object sender, RoutedEventArgs e)
        {
            var selectedIds = EleWorksetSel
                .SelectedItems
                .Cast<Element>()
                .Select(x => x.Id)
                .ToList();

            if (selectedIds.Count == 0)
            {
                MessageBox.Show("No elements selected.");
                return;
            }

            uidoc.Selection.SetElementIds(selectedIds);
            uidoc.ShowElements(selectedIds);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}