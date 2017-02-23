using System;
using System.Collections;
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

namespace TLCGen.GebruikersOpties
{
    /// <summary>
    /// Interaction logic for DetectorenLijstView.xaml
    /// </summary>
    public partial class GebruikersOptiesLijst : UserControl
    {
        public bool ShowNaam
        {
            get { return (bool)GetValue(ShowNaamProperty); }
            set { SetValue(ShowNaamProperty, value); }
        }

        public static readonly DependencyProperty ShowNaamProperty =
            DependencyProperty.Register("ShowNaam", typeof(bool), typeof(GebruikersOptiesLijst), new PropertyMetadata(true));
        
        public bool ShowInstelling
        {
            get { return (bool)GetValue(ShowInstellingProperty); }
            set { SetValue(ShowInstellingProperty, value); }
        }

        public static readonly DependencyProperty ShowInstellingProperty =
            DependencyProperty.Register("ShowInstelling", typeof(bool), typeof(GebruikersOptiesLijst), new PropertyMetadata(true));

        public bool ShowType
        {
            get { return (bool)GetValue(ShowTypeProperty); }
            set { SetValue(ShowTypeProperty, value); }
        }

        public static readonly DependencyProperty ShowTypeProperty =
            DependencyProperty.Register("ShowType", typeof(bool), typeof(GebruikersOptiesLijst), new PropertyMetadata(true));

        public ICollection GridItemsSource
        {
            get { return (Array)GetValue(ConflictMatrixProperty); }
            set { SetValue(ConflictMatrixProperty, value); }
        }

        public static readonly DependencyProperty ConflictMatrixProperty =
            DependencyProperty.Register("GridItemsSource", typeof(ICollection), typeof(GebruikersOptiesLijst), new PropertyMetadata(default(ICollection)));

        public object GridSelectedItem
        {
            get { return (object)GetValue(GridSelectedItemProperty); }
            set { SetValue(GridSelectedItemProperty, value); }
        }

        public static readonly DependencyProperty GridSelectedItemProperty =
            DependencyProperty.Register("GridSelectedItem", typeof(object), typeof(GebruikersOptiesLijst), new PropertyMetadata(null));

        public IList GridSelectedItems
        {
            get { return (IList)GetValue(GridSelectedItemsProperty); }
            set { SetValue(GridSelectedItemsProperty, value); }
        }
        
        public static readonly DependencyProperty GridSelectedItemsProperty =
            DependencyProperty.Register("GridSelectedItems", typeof(IList), typeof(GebruikersOptiesLijst), new PropertyMetadata(null));

        public GebruikersOptiesLijst()
        {
            InitializeComponent();
        }
    }
}
