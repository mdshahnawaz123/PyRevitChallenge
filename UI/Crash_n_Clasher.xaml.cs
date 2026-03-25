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
    /// Interaction logic for Crash_n_Clasher.xaml
    /// </summary>
    public partial class Crash_n_Clasher : Window
    {
        private Document doc;
        private UIDocument uidoc;

        private ExternalEvent externalEvent;
        private CrashExternal crashExternal;
        public Crash_n_Clasher(Document doc,UIDocument uidoc)
        {
            InitializeComponent();
            this.doc = doc;
            this.uidoc = uidoc;

            crashExternal = new CrashExternal();
            externalEvent = ExternalEvent.Create(crashExternal);
            


            info();
        }

        public void info()
        {
            var cat = doc.GetCategories();
            Category01.ItemsSource = cat;
            Category02.ItemsSource = cat;

            var revitLinked = doc.GetRevitLinkInstance();



        }

        private void Btn_Crash(object sender, RoutedEventArgs e)
        {


            externalEvent.Raise();

        }

        private void Btn_Export(object sender, RoutedEventArgs e)
        {

        }
    }
}
