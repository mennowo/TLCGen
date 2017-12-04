using System;
using System.Collections.Generic;
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
using TLCGen.Extensions;
using TLCGen.ViewModels.Enums;

namespace TLCGen.Views
{
    /// <summary>
    /// Interaction logic for ConflictMatrixDataGrid.xaml
    /// </summary>
    public partial class ConflictMatrixDataGrid : UserControl
    {
        private DataGridRow _previousRow = null;
        private DataGridColumn _previousCol = null;
        private readonly Style _styleSelectedC;
        private readonly Style _styleSelectedR;
	    bool _editing = false;

		public static readonly DependencyProperty ConflictMatrixProperty =
            DependencyProperty.Register("ConflictMatrix", typeof(Array), typeof(ConflictMatrixDataGrid), new PropertyMetadata(default(Array)));

        public Array ConflictMatrix
        {
            get { return (Array)GetValue(ConflictMatrixProperty); }
            set { SetValue(ConflictMatrixProperty, value); }
        }

        public object GridSelectedItem
        {
            get { return (object)GetValue(GridSelectedItemProperty); }
            set { SetValue(GridSelectedItemProperty, value); }
        }
        
        public static readonly DependencyProperty GridSelectedItemProperty =
            DependencyProperty.Register("GridSelectedItem", typeof(object), typeof(ConflictMatrixDataGrid), new PropertyMetadata(null));

        public ConflictMatrixDataGrid()
        {
            InitializeComponent();

            if (!DesignMode.IsInDesignMode)
            {
                _styleSelectedC = new Style();
                _styleSelectedC.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(3, 3, 3, 3)));
                _styleSelectedC.Setters.Add(new Setter(Grid.BackgroundProperty, new SolidColorBrush(Colors.Beige)));
                _styleSelectedC.Setters.Add(new Setter(Grid.HorizontalAlignmentProperty, HorizontalAlignment.Stretch));
                _styleSelectedC.Setters.Add(new Setter(TextBox.ForegroundProperty, new SolidColorBrush(Colors.Blue)));
                _styleSelectedC.Setters.Add(new Setter(TextBox.PaddingProperty, new Thickness(5, 3, 5, 3)));
                _styleSelectedC.Setters.Add(new Setter(TextBox.HorizontalAlignmentProperty, HorizontalAlignment.Center));

                _styleSelectedR = new Style();
                _styleSelectedR.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(2, 3, 2, 3)));
                _styleSelectedR.Setters.Add(new Setter(Grid.BackgroundProperty, new SolidColorBrush(Colors.Beige)));
                _styleSelectedC.Setters.Add(new Setter(Grid.HorizontalAlignmentProperty, HorizontalAlignment.Stretch));
                _styleSelectedR.Setters.Add(new Setter(TextBox.ForegroundProperty, new SolidColorBrush(Colors.Blue)));
                _styleSelectedR.Setters.Add(new Setter(TextBox.PaddingProperty, new Thickness(3, 3, 3, 3)));
                _styleSelectedR.Setters.Add(new Setter(TextBox.HorizontalAlignmentProperty, HorizontalAlignment.Center));
            }
        }

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            if (dg != null)
            {
                if (_previousRow != null) _previousRow.HeaderStyle = null;
                if (_previousCol != null) _previousCol.HeaderStyle = null;

                if (e.AddedCells != null && e.AddedCells.Count > 0)
                {
                    var row = (DataGridRow)dg.ItemContainerGenerator.ContainerFromItem(e.AddedCells[0].Item);
                    _previousRow = row;
                    var col = e.AddedCells[0].Column as DataGridColumn;
                    _previousCol = col;
                    col.HeaderStyle = _styleSelectedC;
                    row.HeaderStyle = _styleSelectedR;
                }
            _editing = false;
            }
        }

        private void DataGrid2D_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            _editing = false;
        }

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
	        if (sender is DataGrid dg && !dg.IsReadOnly && !_editing && 
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
                    _previousRow = row;
                    var col = dg.SelectedCells[0].Column as DataGridColumn;
                    _previousCol = col;
                    DataGridCell cell = dg.GetCell(row.GetIndex(), col.DisplayIndex);
                    dg.BeginEdit();
                }
            }
        }

        private void DataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            _editing = true;
        }
    }

    public class SynchCheckBoxVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Count() != 2)
                throw new IndexOutOfRangeException();

            if (values[0] == DependencyProperty.UnsetValue ||
                values[1] == DependencyProperty.UnsetValue)
                return null;

            var displayType = (IntersignaalGroepTypeEnum)values[0];
            var isEnabled = (bool)values[1];

            switch(displayType)
            {
                case IntersignaalGroepTypeEnum.Conflict:
                case IntersignaalGroepTypeEnum.GarantieConflict:
                    return Visibility.Hidden;
                default:
                    return isEnabled ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }

    public class ViewModelValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
	        if (parameter != null)
	        {
		        IntersignaalGroepTypeEnum e = (IntersignaalGroepTypeEnum)parameter;
		        switch(e)
		        {
			        case IntersignaalGroepTypeEnum.Conflict:
			        case IntersignaalGroepTypeEnum.GarantieConflict:
				        if (value is string s)
					        return s;
				        else
					        return "";
			        case IntersignaalGroepTypeEnum.Gelijkstart:
			        case IntersignaalGroepTypeEnum.Voorstart:
			        case IntersignaalGroepTypeEnum.Naloop:
				        if (value is bool b)
					        return b;
				        else
					        return false;
			        default:
				        throw new ArgumentOutOfRangeException();
		        }
	        }
			throw new NullReferenceException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}
