using Newtonsoft.Json;
using specterworks;
using specterworks.ParticleTypes;
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
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace particle_system_ui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Particle first = new StartParticle();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var btn = (UIElement)sender;
            using (var game = new ParticleWindow(first))
            {
                var save = JsonConvert.SerializeObject(first, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                });
                btn.IsEnabled = false;
                game.Run(30);
                btn.IsEnabled = true;
                first = JsonConvert.DeserializeObject<StartParticle>(save, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                });

                properties.SelectedObject = first;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var start = (StartParticle)first;
            var ce = new Cloud1Emiter();
            ce.Location.X = 5;
            ce.Location.Y = 5;
            ce.Location.Z = 5;
            start.StartingParticles.Add(new StartingParticle { TemplateParticle = ce, });
            properties.SelectedObject = first;
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            var text = Clipboard.GetText();
            first = JsonConvert.DeserializeObject<StartParticle>(text, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                Formatting = Formatting.Indented
            });

            properties.SelectedObject = first;
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(JsonConvert.SerializeObject(first, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
            }));
        }
    }
}
