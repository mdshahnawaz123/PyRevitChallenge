using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PyRevitChallenge.UI
{
    public partial class DoorSwing : Window
    {
        private Document doc;
        private UIDocument uidoc;

        public DoorSwing(Document doc, UIDocument uidoc)
        {
            InitializeComponent();
            this.doc = doc;
            this.uidoc = uidoc;
            LoadDoors();
        }

        public void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (doc == null)
            {
                TaskDialog.Show("Message", "Document is null");
                return;
            }

            var selectedIds = new HashSet<ElementId>();

            // Right doors
            foreach (FamilyInstance d in RightDoorSwing.SelectedItems)
                selectedIds.Add(d.Id);

            // Left doors
            foreach (FamilyInstance d in LeftDoorSwing.SelectedItems)
                selectedIds.Add(d.Id);

            if (selectedIds.Count == 0)
            {
                TaskDialog.Show("Message", "Please select at least one door.");
                return;
            }

            uidoc.ShowElements(selectedIds);
            uidoc.Selection.SetElementIds(selectedIds.ToList());
        }

        private void LoadDoors()
        {
            var doors = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Doors)
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .ToList();

            List<FamilyInstance> leftDoors = new List<FamilyInstance>();
            List<FamilyInstance> rightDoors = new List<FamilyInstance>();

            foreach (var door in doors)
            {
                XYZ hand = door.HandOrientation;
                XYZ facing = door.FacingOrientation;

               
                var cross = facing.CrossProduct(hand);

                if (cross.Z > 0)
                    leftDoors.Add(door);
                else
                    rightDoors.Add(door);
            }

            LeftDoorSwing.ItemsSource = leftDoors;
            RightDoorSwing.ItemsSource = rightDoors;

           
            this.Title = $"Door Swing  |  L:{leftDoors.Count}  R:{rightDoors.Count}";
        }
    }
}
