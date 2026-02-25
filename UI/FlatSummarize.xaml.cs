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
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using PyRevitChallenge.Extension;

namespace PyRevitChallenge.UI
{
    /// <summary>
    /// Interaction logic for FlatSummarize.xaml
    /// </summary>
    public partial class FlatSummarize : Window
    {
        private Document doc;
        private UIDocument uidoc;
        public FlatSummarize(Document doc,UIDocument uidoc)
        {
            InitializeComponent();
            this.doc = doc;
            this.uidoc = uidoc;
            info();
        }

        public void info()
        {
            var rooms = doc.GetRooms();
            RoomArea.ItemsSource = rooms;
            var totalArea = rooms.Sum(x => x.Area);

            // Let's Change the Unit to m^2
            var totalAreaM2 = UnitUtils.ConvertFromInternalUnits(totalArea, UnitTypeId.SquareMeters);
            TotalArea.Text = $"{totalAreaM2 :F1} sq.m.";
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();

            if (doc == null)
            {
                TaskDialog.Show("Message", "Document is null");
                return;
            }

            var roomId = RoomArea
                .SelectedItems
                .Cast<SpatialElement>()
                .Select(x => x.Id)
                .ToList();

            foreach (var elementId in roomId)
            {
                var element = doc.GetElement(elementId);
                var RoomName = element.get_Parameter(BuiltInParameter.ROOM_NAME).AsString();
                var RoomNumber = element.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString();
                sb.AppendLine($"Room Name: {RoomName}, Room Number: {RoomNumber}");
            }
            TaskDialog.Show("Selected Room Information", sb.ToString());
            uidoc.ShowElements(roomId);
            uidoc.Selection.SetElementIds(roomId);

        }

        private void RoomArea_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedRooms = RoomArea.SelectedItems
                .OfType<SpatialElement>()
                .ToList();

            if (!selectedRooms.Any())
            {
                SelectedArea.Text = "0 sq.m.";
                return;
            }

            // Sum internal areas (Revit internal units = ft²)
            double totalArea = selectedRooms.Sum(r => r.Area);

            // Convert to m²
            double totalAreaM2 = UnitUtils.ConvertFromInternalUnits(
                totalArea,
                UnitTypeId.SquareMeters);

            SelectedArea.Text = $"{totalAreaM2:F2} sq.m.";
        }
    }
}
