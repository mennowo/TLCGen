using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

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
            get => (TLCGenDefaultModel)GetValue(SelectedDefaultProperty);
            set => SetValue(SelectedDefaultProperty, value);
        }

        // Using a DependencyProperty as the backing store for SelectedDefault.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedDefaultProperty =
            DependencyProperty.Register("SelectedDefault", typeof(TLCGenDefaultModel), typeof(DefaultsListView), new PropertyMetadata(null));



        public ObservableCollection<TLCGenDefaultModel> Defaults
        {
            get => (ObservableCollection<TLCGenDefaultModel>)GetValue(DefaultsProperty);
            set => SetValue(DefaultsProperty, value);
        }

        // Using a DependencyProperty as the backing store for Defaults.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DefaultsProperty =
            DependencyProperty.Register("Defaults", typeof(ObservableCollection<TLCGenDefaultModel>), typeof(DefaultsListView), new PropertyMetadata(null));


    }
}
