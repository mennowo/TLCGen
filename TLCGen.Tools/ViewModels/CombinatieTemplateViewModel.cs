using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace TLCGen.Plugins.Tools
{
    public class CombinatieTemplateViewModel : ViewModelBase
    {
        #region Fields

        private RelayCommand _addTemplateItemCommand;
        private RelayCommand _removeTemplateItemCommand;
        private RelayCommand _addTemplateOptieCommand;
        private RelayCommand _removeTemplateOptieCommand;
        private CombinatieTemplateOptieViewModel _selectedOptie;
        private CombinatieTemplateItemViewModel _selectedItem;

        #endregion // Fields

        #region Properties

        public CombinatieTemplateModel Template { get; }

        public string Name
        {
            get => Template.Name;
            set
            {
                Template.Name = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<CombinatieTemplateOptieViewModel> Opties { get; }

        public ObservableCollection<CombinatieTemplateItemViewModel> Items { get; }

        public CombinatieTemplateOptieViewModel SelectedOptie
        {
            get => _selectedOptie;
            set
            {
                _selectedOptie = value;
                RaisePropertyChanged();
            }
        }

        public CombinatieTemplateItemViewModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                value?.SetSelectableItems();
                RaisePropertyChanged();
            }
        }

        #endregion // Properties

        #region Commands

        public ICommand AddTemplateItemCommand => _addTemplateItemCommand ?? (_addTemplateItemCommand = new RelayCommand(() =>
        {
            Items.Add(new CombinatieTemplateItemViewModel(new CombinatieTemplateItemModel { Description = "Nieuw template item" }));
        }));

        public ICommand RemoveTemplateItemCommand => _removeTemplateItemCommand ?? (_removeTemplateItemCommand = new RelayCommand(() =>
        {
            var i = Items.IndexOf(SelectedItem);
            Items.Remove(SelectedItem);
            SelectedItem = null;
            if (Items.Any())
            {
                if (i >= Items.Count) SelectedItem = Items.Last();
                else if (i >= 0 && i < Items.Count) SelectedItem = Items[i];
            }
        },
        () => SelectedItem != null));

        public ICommand AddTemplateOptieCommand => _addTemplateOptieCommand ?? (_addTemplateOptieCommand = new RelayCommand(() =>
        {
            Opties.Add(new CombinatieTemplateOptieViewModel(new CombinatieTemplateOptieModel { Description = "Nieuwe template optie" }));
        }));

        public ICommand RemoveTemplateOptieCommand => _removeTemplateOptieCommand ?? (_removeTemplateOptieCommand = new RelayCommand(() =>
        {
            var i = Opties.IndexOf(SelectedOptie);
            Opties.Remove(SelectedOptie);
            SelectedOptie = null;
            if (Opties.Any())
            {
                if (i >= Opties.Count) SelectedOptie = Opties.Last();
                else if (i >= 0 && i < Opties.Count) SelectedOptie = Opties[i];
            }
        },
        () => SelectedOptie != null));

        #endregion // Commands

        #region Constructor

        public CombinatieTemplateViewModel(CombinatieTemplateModel template)
        {
            Template = template;
            Opties = new ObservableCollection<CombinatieTemplateOptieViewModel>();
            Items = new ObservableCollection<CombinatieTemplateItemViewModel>();
            foreach (var o in template.Opties)
            {
                Opties.Add(new CombinatieTemplateOptieViewModel(o));
            }
            foreach (var i in template.Items)
            {
                Items.Add(new CombinatieTemplateItemViewModel(i));
            }
            Opties.CollectionChanged += Opties_CollectionChanged;
            Items.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (CombinatieTemplateItemViewModel vvm in e.NewItems)
                {
                    Template.Items.Add(vvm.CombinatieTemplateItem);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (CombinatieTemplateItemViewModel vvm in e.OldItems)
                {
                    Template.Items.Remove(vvm.CombinatieTemplateItem);
                }
            }
        }

        private void Opties_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (CombinatieTemplateOptieViewModel vvm in e.NewItems)
                {
                    Template.Opties.Add(vvm.Optie);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (CombinatieTemplateOptieViewModel vvm in e.OldItems)
                {
                    Template.Opties.Remove(vvm.Optie);
                }
            }
        }

        #endregion // Constructor
    }
}
