using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Extensions;
using RelayCommandWpf = GalaSoft.MvvmLight.CommandWpf.RelayCommand;

namespace TLCGen.Helpers
{
    public class AddRemoveItemsManager<T1, T2, T3> : ViewModelBase where T1 : IViewModelWithItem
    {
        public ObservableCollectionAroundList<T1, T2> ItemsSource { get; }
        public ObservableCollection<T3> AllSelectableItems { get; } = new ObservableCollection<T3>();
        public ObservableCollection<T3> SelectableItems { get; } = new ObservableCollection<T3>();
        
        private T3 _selectedItemToAdd;
        public T3 SelectedItemToAdd
        {
            get => _selectedItemToAdd;
            set
            {
                _selectedItemToAdd = value;
                RaisePropertyChanged();
            }
        }

        private T1 _selectedItem;
        public T1 SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                RaisePropertyChanged();
            }
        }

        public Func<T3, T1> GetNewItem { get; }
        public Func<T1, T3, bool> SelectableEqualsItem { get; }

        RelayCommandWpf _addItemCommand;
        public ICommand AddItemCommand => _addItemCommand ?? (_addItemCommand = new RelayCommandWpf(AddItemCommand_executed, AddItemCommand_canExecute));

        private bool AddItemCommand_canExecute()
        {
            return SelectedItemToAdd != null || !AllSelectableItems.Any();
        }

        private void AddItemCommand_executed()
        {
            var i = SelectableItems.IndexOf(SelectedItemToAdd);
            ItemsSource.Add(GetNewItem(SelectedItemToAdd));
            ItemsSource.BubbleSort();
            UpdateSelectables(null);
            if (SelectableItems.Any())
            {
                if (i >= 0 && SelectableItems.Count > i) SelectedItemToAdd = SelectableItems[i];
                else SelectedItemToAdd = SelectableItems.Last();
            }
            else
            {
                SelectedItemToAdd = default(T3);
            }
        }

        RelayCommandWpf _removeItemCommand;
        public ICommand RemoveItemCommand => _removeItemCommand ?? (_removeItemCommand = new RelayCommandWpf(RemoveItemCommand_executed, RemoveItemCommand_canExecute));

        private bool RemoveItemCommand_canExecute()
        {
            return SelectedItem != null;
        }

        private void RemoveItemCommand_executed()
        {
            var iSelectedItem = ItemsSource.IndexOf(SelectedItem);
            var removedSelToAddItem = AllSelectableItems.FirstOrDefault(x => SelectableEqualsItem(SelectedItem, x));
            var iSelectedToAdd = SelectableItems.IndexOf(SelectedItemToAdd);
            ItemsSource.Remove(SelectedItem);
            if (ItemsSource.Count > 0)
            {
                if (iSelectedItem >= 0 && ItemsSource.Count > iSelectedItem) SelectedItem = ItemsSource[iSelectedItem];
                else SelectedItem = ItemsSource.Last();
            }
            else
            {
                SelectedItem = default(T1);
            }
            UpdateSelectables(null);
            if (SelectableItems.Any())
            {
                if(removedSelToAddItem != null && SelectableItems.Any(x => x.Equals(removedSelToAddItem)))
                {
                    SelectedItemToAdd = removedSelToAddItem;
                }
                else
                {
                    if (iSelectedToAdd >= 0 && iSelectedToAdd < SelectableItems.Count) SelectedItemToAdd = SelectableItems[iSelectedToAdd];
                    else SelectedItemToAdd = SelectableItems.Last();
                }
            }
            else
            {
                SelectedItemToAdd = default(T3);
            }
        }

        public void UpdateSelectables(IEnumerable<T3> allItems)
        {
            if (allItems != null)
            {
                AllSelectableItems.Clear();
                foreach (var i in allItems)
                {
                    AllSelectableItems.Add(i);
                }
            }
            SelectableItems.Clear();
            foreach (var i in AllSelectableItems)
            {
                if (!ItemsSource.Any(x => SelectableEqualsItem(x, i)))
                {
                    SelectableItems.Add(i);
                }
            }
            if (SelectableItems.Any()) SelectedItemToAdd = SelectableItems[0];
        }

        public AddRemoveItemsManager(ObservableCollectionAroundList<T1, T2> itemsSource, Func<T3, T1> getNewItem, Func<T1, T3, bool> selectableEqualsItem)
        {
            ItemsSource = itemsSource;
            GetNewItem = getNewItem;
            SelectableEqualsItem = selectableEqualsItem;
        }
    }
}
