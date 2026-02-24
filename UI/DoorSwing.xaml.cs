using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using PyRevitChallenge.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PyRevitChallenge.UI
{
    /// <summary>
    /// Interaction logic for DoorSwing.xaml
    /// </summary>
    public partial class DoorSwing : Window
    {
        private Document doc;
        private UIDocument uidoc;

        public DoorSwing(Document doc, UIDocument uidoc)
        {
            InitializeComponent();
            this.doc = doc;
            this.uidoc = uidoc;
            info();
        }

        public void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (doc == null)
            {
                TaskDialog.Show("Message", "Document is null");
                return;
            }

            var selectedIds = new List<ElementId>();

            // Right doors
            if (RightDoorSwing.SelectedItems.Count > 0)
            {
                var rids = RightDoorSwing.SelectedItems
                    .Cast<FamilyInstance>()
                    .Select(x => x.Id);

                selectedIds.AddRange(rids);
            }

            // Left doors
            if (LeftDoorSwing.SelectedItems.Count > 0)
            {
                var lids = LeftDoorSwing.SelectedItems
                    .Cast<FamilyInstance>()
                    .Select(x => x.Id);

                selectedIds.AddRange(lids);
            }

            if (selectedIds.Count == 0)
            {
                TaskDialog.Show("Message", "Please select at least one door.");
                return;
            }

            uidoc.ShowElements(selectedIds);
            uidoc.Selection.SetElementIds(selectedIds);
        }

        public void info()
        {
            var doors = doc
                .GetFamilyInstances()
                .Where(x => x.Category != null && x.Category.Id.Value == (int)BuiltInCategory.OST_Doors)
                .ToList();

            List<FamilyInstance> leftDoors = new List<FamilyInstance>();
            List<FamilyInstance> rightDoors = new List<FamilyInstance>();

            foreach (var door in doors)
            {
                var handorientation = door.HandOrientation;
                var faceorientation = door.FacingOrientation;

                if (Math.Round(handorientation.X, 3) == -1)
                {
                    leftDoors.Add(door);
                }
                else
                {
                    rightDoors.Add(door);
                }
            }

            // Bind to UI
            LeftDoorSwing.ItemsSource = leftDoors;
            RightDoorSwing.ItemsSource = rightDoors;
        }
    }

}
