using specterworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace particle_system
{
    public class ParticleTypeCustomEditor : ITypeEditor
    {
        public static Type[] DerivedTypes;
        public static IEnumerable<Type> GetTypes()
        {
            if (DerivedTypes != null)
                return DerivedTypes;
            var partType = typeof(Particle);
            var list = partType.Assembly.GetTypes().Where(t => partType.IsAssignableFrom(t) && !t.IsAbstract);
            DerivedTypes = list.ToArray();
            return DerivedTypes;
        }
        public IEnumerable<Particle> GetOptions()
        {
            return GetTypes().Select(t => Activator.CreateInstance(t)).Cast<Particle>();
        }
        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            ComboBox options = new ComboBox();
            var source = GetOptions().ToList();
            options.ItemsSource = source;
            options.SelectedItem = source.First();
            var a = options.SelectedItem;

            //create the binding from the bound property item to the editor
            var _binding = new Binding("Value"); //bind to the Value property of the PropertyItem
            _binding.Source = propertyItem;
            _binding.ValidatesOnExceptions = true;
            _binding.ValidatesOnDataErrors = true;
            _binding.Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
            BindingOperations.SetBinding(options, ComboBox.SelectedItemProperty, _binding);
            return options;
        }
    }
}
