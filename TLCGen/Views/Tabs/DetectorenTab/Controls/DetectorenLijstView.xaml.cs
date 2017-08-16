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

namespace TLCGen.Views
{
    /// <summary>
    /// Interaction logic for DetectorenLijstView.xaml
    /// </summary>
    public partial class DetectorenLijstView : UserControl
    {
        public bool ShowVeiligheidsGroen
        {
            get { return (bool)GetValue(ShowVeiligheidsGroenProperty); }
            set { SetValue(ShowVeiligheidsGroenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowFaseCyclus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowVeiligheidsGroenProperty =
            DependencyProperty.Register("ShowVeiligheidsGroen", typeof(bool), typeof(DetectorenLijstView), new PropertyMetadata(false));


        public bool ShowStoring
        {
            get { return (bool)GetValue(ShowStoringProperty); }
            set { SetValue(ShowStoringProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowFaseCyclus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowStoringProperty =
            DependencyProperty.Register("ShowStoring", typeof(bool), typeof(DetectorenLijstView), new PropertyMetadata(false));

        public bool ShowRijstrook
        {
            get { return (bool)GetValue(ShowRijstrookProperty); }
            set { SetValue(ShowRijstrookProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowFaseCyclus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowRijstrookProperty =
            DependencyProperty.Register("ShowRijstrook", typeof(bool), typeof(DetectorenLijstView), new PropertyMetadata(false));

        public bool ShowFaseCyclus
        {
            get { return (bool)GetValue(ShowFaseCyclusProperty); }
            set { SetValue(ShowFaseCyclusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowFaseCyclus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowFaseCyclusProperty =
            DependencyProperty.Register("ShowFaseCyclus", typeof(bool), typeof(DetectorenLijstView), new PropertyMetadata(false));

        public bool ShowAanvraagVerlengen
        {
            get { return (bool)GetValue(ShowAanvraagVerlengenProperty); }
            set { SetValue(ShowAanvraagVerlengenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowFaseCyclus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowAanvraagVerlengenProperty =
            DependencyProperty.Register("ShowAanvraagVerlengen", typeof(bool), typeof(DetectorenLijstView), new PropertyMetadata(true));

        public DetectorenLijstView()
        {
            InitializeComponent();
        }

        #region Quick edit cells
        
        /* See XAML: 
                 here, we need to redo what the specialdatagrid has, 
                 cause the style for datagridcell is overwritten
                 not sweet, but so far, found no better way
        */

        public void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGridCell cell = sender as DataGridCell;
            GridColumnFastEdit(cell, e);
        }

        public void DataGridCell_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            DataGridCell cell = sender as DataGridCell;
            GridColumnFastEdit(cell, e);
        }

        private static void GridColumnFastEdit(DataGridCell cell, RoutedEventArgs e)
        {
            if (cell == null || cell.IsEditing || cell.IsReadOnly)
                return;

            DataGrid dataGrid = FindVisualParent<DataGrid>(cell);
            if (dataGrid == null)
                return;

            if (!cell.IsFocused)
            {
                cell.Focus();
            }

            if (cell.Content is CheckBox)
            {
                if (dataGrid.SelectionUnit != DataGridSelectionUnit.FullRow)
                {
                    if (!cell.IsSelected)
                        cell.IsSelected = true;
                }
                else
                {
                    DataGridRow row = FindVisualParent<DataGridRow>(cell);
                    if (row != null && !row.IsSelected)
                    {
                        row.IsSelected = true;
                    }
                }
            }
            else
            {
                ComboBox cb = cell.Content as ComboBox;
                if (cb != null)
                {
                    //DataGrid dataGrid = FindVisualParent<DataGrid>(cell);
                    dataGrid.BeginEdit(e);
                    //cell.Dispatcher.Invoke(
                    // DispatcherPriority.Background,
                    // new Action(delegate { }));
                    //cb.IsDropDownOpen = false;
                }
            }
        }


        private static T FindVisualParent<T>(UIElement element) where T : UIElement
        {
            UIElement parent = element;
            while (parent != null)
            {
                T correctlyTyped = parent as T;
                if (correctlyTyped != null)
                {
                    return correctlyTyped;
                }

                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }
            return null;
        }
        
        #endregion // Quick edit cells

    }
}
