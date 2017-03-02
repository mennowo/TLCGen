using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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

namespace TLCGen.Controls
{
    public class IntValidate : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            bool ok = false;
            int i;
            if(value != null)
                ok = Int32.TryParse((string)value, out i);
            return ok ? new ValidationResult(ok, "") : new ValidationResult(ok, "not int");
        }
    }

    public class IntNullableValidate : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            bool ok = false;
            int i;
            if (string.IsNullOrEmpty((string)value))
                ok = true;
            else
                ok = Int32.TryParse((string)value, out i);
            return ok ? new ValidationResult(ok, "") : new ValidationResult(ok, "not int");
        }
    }

    public enum PropertyDescriptionPlacementEnum { ToTheLeft, Above }

    /// <summary>
    /// Interaction logic for SimplePropertyEditor.xaml
    /// </summary>
    public partial class SimplePropertyEditor : UserControl
    {

        public string NoObjectFoundDescription
        {
            get { return (string)GetValue(NoObjectFoundDescriptionProperty); }
            set { SetValue(NoObjectFoundDescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NoObjectFoundDescription.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NoObjectFoundDescriptionProperty =
            DependencyProperty.Register("NoObjectFoundDescription", typeof(string), typeof(SimplePropertyEditor), new PropertyMetadata("Geen instellingen beschikbaar."));

        public object BoundObject
        {
            get { return (object)GetValue(BoundObjectProperty); }
            set { SetValue(BoundObjectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BoundObject.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BoundObjectProperty =
            DependencyProperty.Register("BoundObject", typeof(object), typeof(SimplePropertyEditor), new PropertyMetadata(null, new PropertyChangedCallback(OnBoundObjectChanged)));

        private static void OnBoundObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var o = (SimplePropertyEditor)d;
            if (o.BoundObject == null)
            {
                o.MainGrid.Children.Clear();
                o.MainGrid.RowDefinitions.Clear();
                o.MainGrid.ColumnDefinitions.Clear();
                Label label = new Label();
                label.Content = o.NoObjectFoundDescription;
                o.MainGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                Grid.SetRow(label, 0); Grid.SetColumn(label, 0);
                o.MainGrid.Children.Add(label);
                return;
            }

            var props = o.BoundObject.GetType().GetProperties();
            int row = 0;
            o.MainGrid.Children.Clear();
            o.MainGrid.RowDefinitions.Clear();
            o.MainGrid.ColumnDefinitions.Clear();
            if(o.DescriptionPlacement == PropertyDescriptionPlacementEnum.ToTheLeft)
            {
                o.MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                o.MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }
            else
            {
                o.MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }
            foreach (var prop in props)
            {
                if (prop.PropertyType.IsValueType)
                {
                    var attr = prop.GetCustomAttributes(typeof(BrowsableAttribute), true);
                    if (attr != null && attr.Count() == 1)
                    {
                        if(!((BrowsableAttribute)attr.First()).Browsable)
                        {
                            continue;
                        }
                    }

                    attr = prop.GetCustomAttributes(typeof(CategoryAttribute), true);
                    if (attr != null && attr.Count() == 1)
                    {
                        if (!(string.IsNullOrEmpty(((CategoryAttribute)attr.First()).Category)))
                        {
                            o.MainGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                            TextBlock _l = new TextBlock();
                            _l.Text = ((CategoryAttribute)attr.First()).Category;
                            _l.HorizontalAlignment = o.HorizontalDescriptionPlacement;
                            _l.TextDecorations.Add(TextDecorations.Underline);
                            _l.Padding = new Thickness(5);
                            Grid.SetRow(_l, row);
                            Grid.SetColumnSpan(_l, 2);
                            o.MainGrid.Children.Add(_l);
                            row++;
                        }
                    }

                    var label = new Label();
                    attr = prop.GetCustomAttributes(typeof(DescriptionAttribute), true);
                    if (attr != null && attr.Count() == 1)
                    {
                        label.Content = ((DescriptionAttribute)attr.First()).Description;
                    }
                    else
                    {
                        label.Content = prop.Name;
                    }
                    label.HorizontalAlignment = o.HorizontalDescriptionPlacement;

                    UIElement editor = null;
                    
                    // edit string, int and int?
                    if(prop.PropertyType == typeof(string) || prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
                    {
                        editor = new TextBox() { Margin = new Thickness(2) };
                        Binding binding = new Binding();
                        binding.Path = new PropertyPath(prop.Name);
                        binding.Source = o.BoundObject;
                        binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                        binding.ValidatesOnDataErrors = true;
                        if (!(prop.PropertyType == typeof(int)))
                        {
                            binding.TargetNullValue = string.Empty;
                            var role = new IntNullableValidate();
                            binding.ValidationRules.Add(role);
                        }
                        else
                        {
                            var role = new IntValidate();
                            binding.ValidationRules.Add(role);
                        }
                        BindingOperations.SetBinding(editor, TextBox.TextProperty, binding);
                    }

                    // edit bool
                    if(prop.PropertyType == typeof(bool))
                    {
                        editor = new CheckBox() { Margin = new Thickness(5) };
                        Binding binding = new Binding();
                        binding.Path = new PropertyPath(prop.Name);
                        binding.Source = o.BoundObject;
                        binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                        BindingOperations.SetBinding(editor, CheckBox.IsCheckedProperty, binding);
                    }

                    // edit enums
                    if (prop.PropertyType.IsEnum)
                    {
                        editor = new ComboBox() { Margin = new Thickness(2) };
                        List<object> items = new List<object>();
                        var _items = prop.PropertyType.GetEnumValues();
                        foreach(var s in _items)
                        {
                            items.Add(s);
                        }
                        ((ComboBox)editor).ItemsSource = items;
                        Binding binding = new Binding();
                        binding.Path = new PropertyPath(prop.Name);
                        binding.Source = o.BoundObject;
                        binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                        BindingOperations.SetBinding(editor, ComboBox.SelectedItemProperty, binding);
                    }

                    if (editor != null)
                    {
                        o.MainGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                        Grid.SetRow(label, row); Grid.SetColumn(label, 0);
                        if(o.DescriptionPlacement == PropertyDescriptionPlacementEnum.ToTheLeft)
                        {
                            Grid.SetRow(editor, row); Grid.SetColumn(editor, 1);
                        }
                        else
                        {
                            o.MainGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                            row++;
                            Grid.SetRow(editor, row); Grid.SetColumn(editor, 0);
                        }
                        o.MainGrid.Children.Add(label);
                        o.MainGrid.Children.Add(editor);
                        row++;
                    }
                }
            }
        }

        public PropertyDescriptionPlacementEnum DescriptionPlacement
        {
            get { return (PropertyDescriptionPlacementEnum)GetValue(DescriptionPlacementProperty); }
            set { SetValue(DescriptionPlacementProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DescriptionPlacement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionPlacementProperty =
            DependencyProperty.Register("DescriptionPlacement", typeof(PropertyDescriptionPlacementEnum), typeof(SimplePropertyEditor), new PropertyMetadata(PropertyDescriptionPlacementEnum.ToTheLeft));



        public HorizontalAlignment HorizontalDescriptionPlacement
        {
            get { return (HorizontalAlignment)GetValue(HorizontalDescriptionPlacementProperty); }
            set { SetValue(HorizontalDescriptionPlacementProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HorizontalDescriptionPlacement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalDescriptionPlacementProperty =
            DependencyProperty.Register("HorizontalDescriptionPlacement", typeof(HorizontalAlignment), typeof(SimplePropertyEditor), new PropertyMetadata(HorizontalAlignment.Left));



        public SimplePropertyEditor()
        {
            InitializeComponent();
        }
    }
}
