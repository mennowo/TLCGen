using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TLCGen.Views.Resources
{
    public class SpecialDataGrid : DataGrid
    {
        public SpecialDataGrid() : base()
        {
            this.GotFocus += SpecialDataGrid_GotFocus;
        }

        private void SpecialDataGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            // Lookup for the source to be DataGridCell
            if (e.OriginalSource.GetType() == typeof(DataGridCell))
            {
                // Starts the Edit on the row;
                DataGrid grd = (DataGrid)sender;
                grd.BeginEdit(e);
            }
        }
    }
}
