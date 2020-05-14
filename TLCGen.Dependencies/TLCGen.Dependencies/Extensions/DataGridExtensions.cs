using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace TLCGen.Extensions
{
    public static class ListViewExtensions
    {
        #region SelectedItemsListProperty

        public static readonly DependencyProperty SelectedItemsListProperty = DependencyProperty.RegisterAttached(
            "SelectedItemsList",
            typeof(IList),
            typeof(ListViewExtensions),
            new PropertyMetadata(default(IList), OnSelectedItemsListChanged),
            OnValidateSelectedItemsList);

        public static void SetSelectedItemsList(this ListView element, IList value)
        {
            element.SetValue(SelectedItemsListProperty, value);
        }

        [AttachedPropertyBrowsableForChildren(IncludeDescendants = false)]
        [AttachedPropertyBrowsableForType(typeof(ListView))]
        public static Array GetSelectedItemsList(this ListView element)
        {
            return (Array)element.GetValue(SelectedItemsListProperty);
        }

        private static void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = (ListView)sender;
            listView.SetSelectedItemsList(listView.SelectedItems);
        }

        private static void OnSelectedItemsListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listView = (ListView)d;
            if (e.OldValue == null)
            {
                // Sign up for changes to the ListView’s selected items to enable a two-way binding effect
                listView.SelectionChanged += ListView_SelectionChanged;
            }
        }

        private static bool OnValidateSelectedItemsList(object value)
        {
            return true;
        }

        #endregion // SelectedItemsListProperty
    }

    public static class DataGridExtensions
    {

        #region GetCell Functions

        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            var child = default(T);
            var numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < numVisuals; i++)
            {
                var v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

        public static DataGridRow GetRow(this DataGrid grid, int index)
        {
            var row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                // May be virtualized, bring into view and try again.
                grid.UpdateLayout();
                grid.ScrollIntoView(grid.Items[index]);
                row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }

        public static DataGridCell GetCell(this DataGrid grid, DataGridRow row, int column)
        {
            if (row != null)
            {
                var presenter = GetVisualChild<DataGridCellsPresenter>(row);

                if (presenter == null)
                {
                    grid.ScrollIntoView(row, grid.Columns[column]);
                    presenter = GetVisualChild<DataGridCellsPresenter>(row);
                }

                var cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                return cell;
            }
            return null;
        }

        public static DataGridCell GetCell(this DataGrid grid, int row, int column)
        {
            var gridRow = GetRow(grid, row);
            return GetCell(grid, gridRow, column);
        }

        #endregion // GetCell Functions

        #region SelectedItemsListProperty

        public static readonly DependencyProperty SelectedItemsListProperty = DependencyProperty.RegisterAttached(
            "SelectedItemsList",
            typeof(IList),
            typeof(DataGridExtensions),
            new PropertyMetadata(default(IList), OnSelectedItemsListChanged),
            OnValidateSelectedItemsList);

        public static void SetSelectedItemsList(this DataGrid element, IList value)
        {
            element.SetValue(SelectedItemsListProperty, value);
        }

        [AttachedPropertyBrowsableForChildren(IncludeDescendants = false)]
        [AttachedPropertyBrowsableForType(typeof(DataGrid))]
        public static Array GetSelectedItemsList(this DataGrid element)
        {
            return (Array)element.GetValue(SelectedItemsListProperty);
        }

        private static void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dataGrid = (DataGrid)sender;
            dataGrid.SetSelectedItemsList(dataGrid.SelectedItems);
        }

        private static void OnSelectedItemsListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = (DataGrid)d;
            if (e.OldValue == null)
            {
                // Sign up for changes to the DataGrid’s selected items to enable a two-way binding effect
                dataGrid.SelectionChanged += DataGrid_SelectionChanged;
            }
        }

        private static bool OnValidateSelectedItemsList(object value)
        {
            return true;
        }

        #endregion // SelectedItemsListProperty

    }
}
