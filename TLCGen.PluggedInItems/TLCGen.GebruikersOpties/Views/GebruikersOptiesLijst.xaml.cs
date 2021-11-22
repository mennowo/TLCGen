using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace TLCGen.GebruikersOpties
{
    /// <summary>
    /// Interaction logic for DetectorenLijstView.xaml
    /// </summary>
    public partial class GebruikersOptiesLijst : UserControl
    {
        public bool ShowNaam
        {
            get => (bool)GetValue(ShowNaamProperty);
            set => SetValue(ShowNaamProperty, value);
        }

        public static readonly DependencyProperty ShowNaamProperty =
            DependencyProperty.Register("ShowNaam", typeof(bool), typeof(GebruikersOptiesLijst), new PropertyMetadata(true));
        
        public bool ShowInstelling
        {
            get => (bool)GetValue(ShowInstellingProperty);
            set => SetValue(ShowInstellingProperty, value);
        }

        public static readonly DependencyProperty ShowInstellingProperty =
            DependencyProperty.Register("ShowInstelling", typeof(bool), typeof(GebruikersOptiesLijst), new PropertyMetadata(true));

        public bool ShowMultivalent
        {
            get => (bool)GetValue(ShowMultivalentProperty);
            set => SetValue(ShowMultivalentProperty, value);
        }

        public static readonly DependencyProperty ShowMultivalentProperty =
            DependencyProperty.Register("ShowMultivalent", typeof(bool), typeof(GebruikersOptiesLijst), new PropertyMetadata(true));
        
        public bool ShowDummy
        {
            get => (bool)GetValue(ShowDummyProperty);
            set => SetValue(ShowDummyProperty, value);
        }

        public static readonly DependencyProperty ShowDummyProperty =
            DependencyProperty.Register("ShowDummy", typeof(bool), typeof(GebruikersOptiesLijst), new PropertyMetadata(true));

        public bool ShowType
        {
            get => (bool)GetValue(ShowTypeProperty);
            set => SetValue(ShowTypeProperty, value);
        }

        public static readonly DependencyProperty ShowTypeProperty =
            DependencyProperty.Register("ShowType", typeof(bool), typeof(GebruikersOptiesLijst), new PropertyMetadata(true));

        public ICollection GridItemsSource
        {
            get => (Array)GetValue(ConflictMatrixProperty);
            set => SetValue(ConflictMatrixProperty, value);
        }

        public static readonly DependencyProperty ConflictMatrixProperty =
            DependencyProperty.Register("GridItemsSource", typeof(ICollection), typeof(GebruikersOptiesLijst), new PropertyMetadata(default(ICollection)));

        public object GridSelectedItem
        {
            get => (object)GetValue(GridSelectedItemProperty);
            set => SetValue(GridSelectedItemProperty, value);
        }

        public static readonly DependencyProperty GridSelectedItemProperty =
            DependencyProperty.Register("GridSelectedItem", typeof(object), typeof(GebruikersOptiesLijst), new PropertyMetadata(null));

        public IList GridSelectedItems
        {
            get => (IList)GetValue(GridSelectedItemsProperty);
            set => SetValue(GridSelectedItemsProperty, value);
        }
        
        public static readonly DependencyProperty GridSelectedItemsProperty =
            DependencyProperty.Register("GridSelectedItems", typeof(IList), typeof(GebruikersOptiesLijst), new PropertyMetadata(null));

        public GebruikersOptiesLijst()
        {
            InitializeComponent();
        }
    }
}
