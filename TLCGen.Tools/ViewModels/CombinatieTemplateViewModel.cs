using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TLCGen.Plugins.Tools
{
    public class CombinatieTemplateViewModel : ViewModelBase
    {
        #region Fields

        private ObservableCollection<string> _fasen;
        private CombinatieTemplateOptieViewModel _selectedOptie;
        private CombinatieTemplateItemViewModel _selectedItem;

        #endregion // Fields

        #region Properties

        public CombinatieTemplateModel Template { get; }

        public string Name => Template.Name;

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
        }

        #endregion // Constructor
    }
}
