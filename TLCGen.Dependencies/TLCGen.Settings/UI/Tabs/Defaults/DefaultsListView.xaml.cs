using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace TLCGen.Settings.Views
{
    /// <summary>
    /// Interaction logic for DefaultsListView.xaml
    /// </summary>
    public partial class DefaultsListView : UserControl
    {
        public DefaultsListView()
        {
            InitializeComponent();
        }



        public TLCGenDefaultModel SelectedDefault
        {
            get { return (TLCGenDefaultModel)GetValue(SelectedDefaultProperty); }
            set { SetValue(SelectedDefaultProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedDefault.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedDefaultProperty =
            DependencyProperty.Register("SelectedDefault", typeof(TLCGenDefaultModel), typeof(DefaultsListView), new PropertyMetadata(null));



        public ObservableCollection<TLCGenDefaultModel> Defaults
        {
            get { return (ObservableCollection<TLCGenDefaultModel>)GetValue(DefaultsProperty); }
            set { SetValue(DefaultsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Defaults.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DefaultsProperty =
            DependencyProperty.Register("Defaults", typeof(ObservableCollection<TLCGenDefaultModel>), typeof(DefaultsListView), new PropertyMetadata(null));


    }
}
