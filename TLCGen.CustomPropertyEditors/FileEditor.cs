using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace TLCGen.CustomPropertyEditors
{
    public class XMLFileNameEditor : Xceed.Wpf.Toolkit.PropertyGrid.Editors.ITypeEditor
    {
        TextBox textBox = new TextBox();
        Binding _binding;

        public FrameworkElement ResolveEditor(Xceed.Wpf.Toolkit.PropertyGrid.PropertyItem propertyItem)
        {
            Grid g = new Grid();
            ColumnDefinition cd1 = new ColumnDefinition();
            ColumnDefinition cd2 = new ColumnDefinition();
            cd1.Width = new GridLength(1, GridUnitType.Star);
            cd2.Width = GridLength.Auto;
            g.ColumnDefinitions.Add(cd1);
            g.ColumnDefinitions.Add(cd2);
            Button b = new Button();
            b.Click += B_Click;
            b.Margin = new Thickness(2);
            b.Content = "...";
            Grid.SetColumn(textBox, 0);
            Grid.SetColumn(b, 1);
            g.Children.Add(textBox);
            g.Children.Add(b);

            //create the binding from the bound property item to the editor
            _binding = new Binding("Value"); //bind to the Value property of the PropertyItem
            _binding.Source = propertyItem;
            _binding.ValidatesOnExceptions = true;
            _binding.ValidatesOnDataErrors = true;
            _binding.Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
            BindingOperations.SetBinding(textBox, TextBox.TextProperty, _binding);
            return g;
        }

        private void B_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.SaveFileDialog();
            dialog.CheckFileExists = false;
            dialog.Filter = "XML files|*.xml";
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                textBox.Text = dialog.FileName;
                BindingOperations.GetBindingExpression(textBox, TextBox.TextProperty).UpdateSource();
            }
        }
    }
}
