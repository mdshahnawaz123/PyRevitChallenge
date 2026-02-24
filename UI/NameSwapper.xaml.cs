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
    /// Interaction logic for NameSwapper.xaml
    /// </summary>
    public partial class NameSwapper : Window
    {
        private Document doc;
        private UIDocument uidoc;
        public NameSwapper(Document doc,UIDocument uidoc)
        {
            InitializeComponent();
            this.doc = doc;
            this.uidoc = uidoc;
            info();
        }

        public void info()
        {
            var viewList = doc.GetViews();

            ViewTree.ItemsSource = viewList;
            ViewTree.DisplayMemberPath = "Name";

            var prefix = Text_Prefix.Text;
            var find = Text_Find.Text;
            var replace = Text_RePlace;
            var sufix = Text_Sufix.Text;
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if(doc == null) return;

            var selectedView = ViewTree.SelectedItems.Cast<View>().ToList();

            foreach (var view in selectedView)
            {
                var viewName = view.Name;

                TaskDialog.Show("Message", $"view Name :- {viewName.ToString()}");
            }
            
        }

        private void ViewTre_SelectionChnaged(object sender, SelectionChangedEventArgs e)
        {
            if(doc == null) return;

            var selectedView = ViewTree.SelectedItems.Cast<View>().ToList();

            foreach(var view in selectedView)
            {

            }
            
        }
    }
}
