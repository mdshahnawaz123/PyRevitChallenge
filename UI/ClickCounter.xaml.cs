using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using PyRevitChallenge.Extension;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static PyRevitChallenge.Extension.DocumentExtension;

namespace PyRevitChallenge.UI
{
    public partial class ClickCounter : Window
    {
        private Document doc;
        private UIDocument uidoc;

        public ClickCounter(Document doc, UIDocument uidoc)
        {
            InitializeComponent();
            this.doc = doc;
            this.uidoc = uidoc;

            LoadCategories();
        }

        // Load categories into ComboBox
        private void LoadCategories()
        {
            CatComb.ItemsSource = doc.GetCategories()
                .OrderBy(x => x.Name)
                .ToList();
        }

        // Category selection changed
        private void categorySelection(object sender, SelectionChangedEventArgs e)
        {
            var selectedCategory = CatComb.SelectedItem as Category;

            if (selectedCategory == null) return;

            var element = new FilteredElementCollector(doc)
                .OfCategoryId(selectedCategory.Id)
                .WhereElementIsNotElementType()
                .FirstOrDefault();

            if (element == null) return;

            // Collect string parameters
            var parameters = element.Parameters
                .Cast<Parameter>()
                .Where(p => p.StorageType == StorageType.String)
                .Select(p => p.Definition.Name)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            ParamComb.ItemsSource = parameters;
        }

        // Button click event
        private void BtnClickCount(object sender, RoutedEventArgs e)
        {
            string prefix = TBPrefix.Text;

            if (string.IsNullOrWhiteSpace(prefix))
            {
                TaskDialog.Show("Message", "Enter Prefix First");
                return;
            }

            int startNumber;
            if (!int.TryParse(TBStart.Text, out startNumber))
                startNumber = 1;

            var selectedCategory = CatComb.SelectedItem as Category;
            var selectedParamName = ParamComb.SelectedItem as string;

            if (selectedCategory == null || selectedParamName == null)
            {
                TaskDialog.Show("Message", "Select Category and Parameter");
                return;
            }

            this.Hide();

            int counter = startNumber;

            // Collect elements from selected category
            var elements = new FilteredElementCollector(doc)
                .OfCategoryId(selectedCategory.Id)
                .WhereElementIsNotElementType()
                .ToList();

            var filter = new MasterSelection(elements);

            try
            {
                while (true)
                {
                    Reference pickedRef = uidoc.Selection.PickObject(
                        ObjectType.Element,
                        filter,
                        "Click elements to number. Press ESC to finish.");

                    Element el = doc.GetElement(pickedRef);

                    using (Transaction t = new Transaction(doc, "Click Counter"))
                    {
                        t.Start();

                        Parameter param = el.LookupParameter(selectedParamName);

                        // If parameter is not instance, check type parameter
                        if (param == null)
                        {
                            ElementId typeId = el.GetTypeId();
                            Element typeElem = doc.GetElement(typeId);

                            if (typeElem != null)
                                param = typeElem.LookupParameter(selectedParamName);
                        }

                        if (param != null && !param.IsReadOnly)
                        {
                            string value = $"{prefix}-{counter:00}";
                            param.Set(value);
                            counter++;
                        }

                        t.Commit();
                    }
                }
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                // User pressed ESC
            }

            this.Show();
        }
    }
}