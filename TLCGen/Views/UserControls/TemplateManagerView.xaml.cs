using System.Windows;
using System.Windows.Controls;

namespace TLCGen.Views
{
    /// <summary>
    /// Interaction logic for TemplateManagerView.xaml
    /// </summary>
    public partial class TemplateManagerView : UserControl
    {
        public string ParentItemName
        {
            get => (string)GetValue(ParentItemNameProperty);
            set => SetValue(ParentItemNameProperty, value);
        }

        // Using a DependencyProperty as the backing store for ApplyToItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ParentItemNameProperty =
            DependencyProperty.Register("ParentItemName", typeof(string), typeof(TemplateManagerView), new PropertyMetadata(null));

        public TemplateManagerView()
        {
            InitializeComponent();
        }
    }
}
