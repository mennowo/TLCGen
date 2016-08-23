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

namespace TLCGen.Views
{
    /// <summary>
    /// Interaction logic for TemplateManagerView.xaml
    /// </summary>
    public partial class TemplateManagerView : UserControl
    {


        public string ApplyToItem
        {
            get { return (string)GetValue(ApplyToItemProperty); }
            set { SetValue(ApplyToItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ApplyToItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ApplyToItemProperty =
            DependencyProperty.Register("ApplyToItem", typeof(string), typeof(TemplateManagerView), new PropertyMetadata(null));



        public TemplateManagerView()
        {
            InitializeComponent();
        }
    }
}
