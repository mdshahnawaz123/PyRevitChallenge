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
    /// Interaction logic for WorksetGrabber.xaml
    /// </summary>
    public partial class WorksetGrabber : Window
    {
        private Document doc;
        private UIDocument uidoc;
        public WorksetGrabber(Document doc,UIDocument uidoc)
        {
            InitializeComponent();
            this.doc = doc;
            this.uidoc = uidoc;
            info();
        }

        public void info()
        {
            var worksetList = doc.GetWorksets();
            WorksetList.ItemsSource = worksetList;
            WorksetList.DisplayMemberPath = "Name";
            
        }
    }
}
