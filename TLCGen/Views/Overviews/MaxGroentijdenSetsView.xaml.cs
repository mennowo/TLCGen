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
using TLCGen.Extensions;

namespace TLCGen.Views
{
    /// <summary>
    /// Interaction logic for MaxGroentijdenSetsView.xaml
    /// </summary>
    public partial class MaxGroentijdenSetsView : UserControl
    {
        private DataGridRow _PreviousRow = null;
        private DataGridColumn _PreviousCol = null;

        public MaxGroentijdenSetsView()
        {
            InitializeComponent();
        }

        bool editing = false;
        private void DataGrid2D_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            editing = false;
        }
        private void DataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            editing = true;
        }

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var dg = sender as DataGrid;

            if (dg != null && !dg.IsReadOnly && !editing &&
                !(e.Key == Key.Up ||
                e.Key == Key.Down ||
                e.Key == Key.Left ||
                e.Key == Key.Right ||
                e.Key == Key.Tab ||
                e.Key == Key.Enter ||
                e.Key == Key.Escape
                ))
            {
                if (dg.SelectedCells?.Count > 0)
                {
                    var row = (DataGridRow)dg.ItemContainerGenerator.ContainerFromItem(dg.SelectedCells[0].Item);
                    _PreviousRow = row;
                    var col = dg.SelectedCells[0].Column as DataGridColumn;
                    _PreviousCol = col;
                    DataGridCell cell = dg.GetCell(row.GetIndex(), col.DisplayIndex);
                    dg.BeginEdit();
                }
            }
        }
    }
}
