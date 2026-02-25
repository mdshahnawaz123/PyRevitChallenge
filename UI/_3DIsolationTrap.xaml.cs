using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace PyRevitChallenge.UI
{
    public partial class _3DIsolationTrap : Window
    {
        private readonly Random _random = new Random();
        private Document doc;
        private UIDocument uidoc;

        private List<string> _facts = new List<string>()
        {
            "Revit was first released in 2000.",
            "BIM reduces coordination errors drastically.",
            "Clash detection saves millions in mega projects.",
            "Parametric modeling is the heart of BIM.",
            "Automation starts when repetition hurts enough.",
            "Most BIM pros learned by breaking models first 😄"
        };

        public _3DIsolationTrap(Document doc, UIDocument uidoc)
        {
            InitializeComponent();
            this.doc = doc;
            this.uidoc = uidoc;
        }

        // Window load
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadErikImage();
            await LoadOnlineFact();
            ShowRandomFact();
        }

        // Load Erik image safely for Revit
        private void LoadErikImage()
        {
            try
            {
                var asm = typeof(_3DIsolationTrap).Assembly.GetName().Name;

                var uri = new Uri(
                    $"pack://application:,,,/{asm};component/Image/erik.png",
                    UriKind.Absolute);

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = uri;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                ErikImageBrush.ImageSource = bitmap;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Image Error", ex.ToString());
            }
        }

        // FREE online fact (C# 7.3 safe)
        private async Task LoadOnlineFact()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string json = await client.GetStringAsync(
                        "https://uselessfacts.jsph.pl/api/v2/facts/random");

                    using (JsonDocument docJson = JsonDocument.Parse(json))
                    {
                        string fact = docJson.RootElement.GetProperty("text").GetString();

                        if (!string.IsNullOrWhiteSpace(fact))
                            _facts.Add(fact);
                    }
                }
            }
            catch
            {
                // offline fallback
            }
        }

        private void NextFact_Click(object sender, RoutedEventArgs e)
        {
            ShowRandomFact();
        }

        private void ShowRandomFact()
        {
            string fact = _facts[_random.Next(_facts.Count)];
            FactText.Text = fact;

            Storyboard sb = (Storyboard)FindResource("FadeInText");
            sb.Begin();
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            TaskDialog.Show("Revit", "Your logic here.");
        }
    }
}