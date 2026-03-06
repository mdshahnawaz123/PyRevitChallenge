using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PyRevitChallenge.UI
{
    public partial class FloorifyMyRooms : Window
    {
        private Document doc;
        private UIDocument uidoc;

        private List<Room> selectedRooms = new List<Room>();

        public FloorifyMyRooms(Document doc, UIDocument uidoc)
        {
            InitializeComponent();
            this.doc = doc;
            this.uidoc = uidoc;
        }

        // SELECT ROOMS
        private void btnSelectRooms(object sender, RoutedEventArgs e)
        {
            this.Hide();
            var picked = uidoc.Selection
                .PickElementsByRectangle(new RoomFilter(), "Select Rooms");

            selectedRooms = picked
                .Select(x => x as Room)
                .Where(x => x != null)
                .ToList();

            MessageBox.Show(selectedRooms.Count + " Rooms Selected");
            this.Show();
        }

        // CREATE FLOORS
        private void btnFloorify(object sender, RoutedEventArgs e)
        {
            if (selectedRooms.Count == 0)
            {
                MessageBox.Show("Please select rooms first.");
                return;
            }

            using (Transaction t = new Transaction(doc, "Create Floors From Rooms"))
            {
                t.Start();

                foreach (Room room in selectedRooms)
                {
                    var boundary = room.GetBoundarySegments(new SpatialElementBoundaryOptions());

                    if (boundary == null) continue;

                    foreach (var loop in boundary)
                    {
                        List<Curve> curves = new List<Curve>();

                        foreach (var seg in loop)
                        {
                            curves.Add(seg.GetCurve());
                        }

                        var curveLoop= new CurveLoop();

                        foreach (var c in curves)
                        {
                            curveLoop.Append(c);
                        }
                        List<CurveLoop> loops = new List<CurveLoop>();
                        loops.Add(curveLoop);



                        FloorType floorType = new FilteredElementCollector(doc)
                            .OfClass(typeof(FloorType))
                            .Cast<FloorType>()
                            .First();

                        Level level = doc.GetElement(room.LevelId) as Level;

                        var lvlid = level.LevelId;

                        Floor.Create(doc, loops, floorType.Id, lvlid);
                    }
                }

                t.Commit();
            }

            MessageBox.Show("Floors Created Successfully");
        }

        // ROOM FILTER
        public class RoomFilter : ISelectionFilter
        {
            public bool AllowElement(Element elem)
            {
                return elem is Room;
            }

            public bool AllowReference(Reference reference, XYZ position)
            {
                return false;
            }
        }
    }
}