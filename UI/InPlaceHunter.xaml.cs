using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using PyRevitChallenge.Extension;

namespace PyRevitChallenge.UI
{
    public partial class InPlaceHunter : Window
    {
        private readonly Document doc;
        private readonly UIDocument uidoc;
        private List<FamilyInstance> _instances;

        public InPlaceHunter(Document doc, UIDocument uidoc)
        {
            InitializeComponent();
            this.doc = doc;
            this.uidoc = uidoc;

            Loaded += (s, e) => LoadData();
        }

        private void LoadData()
        {
            _instances = doc.GetInPlaceInstances();
            InPlaceList.ItemsSource = _instances;
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (doc == null || InPlaceList.SelectedItems.Count == 0)
            {
                TaskDialog.Show("Message", "Please Select the Item");
                return;
            }

            var ids = InPlaceList.
                SelectedItems.
                Cast<FamilyInstance>()
                .Select(x => x.Id)
                .ToList();
            uidoc.ShowElements(ids);
            uidoc.Selection.SetElementIds(ids);
        }
    }
}