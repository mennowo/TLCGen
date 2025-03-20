using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 6, type: TabItemTypeEnum.PrioriteitTab)]
    public class PrioriteitSpecialsTabViewModel : TLCGenTabItemViewModel
    {
        
        #region Fields

        private RelayCommand _addNevenMeldingCommand;
        private RelayCommand _removeNevenMeldingCommand;
        private ObservableCollectionAroundList<NevenMeldingViewModel, NevenMeldingModel> _nevenMeldingen;
        private NevenMeldingViewModel _selectedNevenMelding;
        private ObservableCollection<string> _selectableFasen = new ObservableCollection<string>();
        private ObservableCollection<string> _selectableFasen1 = new ObservableCollection<string>();

        #endregion // Fields

        #region Properties

        public ObservableCollectionAroundList<NevenMeldingViewModel, NevenMeldingModel> NevenMeldingen
        {
            get => _nevenMeldingen;
            private set
            {
                _nevenMeldingen = value;
                OnPropertyChanged();
            }
        }

        public NevenMeldingViewModel SelectedNevenMelding
        {
            get => _selectedNevenMelding;
            set
            {
                _selectedNevenMelding = value; 
                OnPropertyChanged();
                UpdateSelectables();
            }
        }
        
        public ObservableCollection<string> SelectableFasen1
        {
            get => _selectableFasen1;
            private set
            {
                _selectableFasen1 = value; 
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> SelectableFasen
        {
            get => _selectableFasen;
            private set
            {
                _selectableFasen = value; 
                OnPropertyChanged();
            }
        }

        #endregion // Properties

        #region TabItem Overrides

        public override string DisplayName => "Specials";

        public override bool CanBeEnabled()
        {
            return _Controller?.PrioData?.PrioIngreepType != Models.Enumerations.PrioIngreepTypeEnum.Geen;
        }

        public override void OnSelected()
        {
            UpdateSelectables();
        }

        public override ControllerModel Controller
        {
            get => _Controller;
            set
            {
                _Controller = value;
                if(_Controller != null)
                {
                    UpdateSelectables();
                    NevenMeldingen = new ObservableCollectionAroundList<NevenMeldingViewModel, NevenMeldingModel>(_Controller.PrioData.NevenMeldingen);
                }
                else
                {
                }
                OnPropertyChanged("");
            }
        }

        #endregion // TabItem Overrides

        #region Commands
        
        public ICommand AddNevenMeldingCommand => _addNevenMeldingCommand ?? (_addNevenMeldingCommand = new RelayCommand(() =>
        {
            var nm = new NevenMeldingViewModel(new NevenMeldingModel
            {
                FaseCyclus3 = "NG"
            });
            NevenMeldingen.Add(nm);
            SelectedNevenMelding = nm;
        }));
        
        public ICommand RemoveNevenMeldingCommand => _removeNevenMeldingCommand ?? (_removeNevenMeldingCommand = new RelayCommand(() =>
        {
            NevenMeldingen.Remove(SelectedNevenMelding);
            SelectedNevenMelding = NevenMeldingen.FirstOrDefault();
        },
        () => SelectedNevenMelding != null));

        #endregion // Commands

        #region Private methods

        private void UpdateSelectables()
        {
            var fasen = new List<string> {"NG"};
            var fasen1 = _Controller.Fasen.Where(x => x.NameAsInt >= 40 && x.NameAsInt < 60).Select(x => x.Naam).ToList();
            fasen.AddRange(fasen1);
            SelectableFasen.Clear();
            SelectableFasen1.Clear();
            SelectableFasen.AddRange(fasen);
            SelectableFasen1.AddRange(fasen1);
        }

        #endregion // Private methods

        #region Public methods

        #endregion // Public methods

        #region TLCGen events

        public void OnPrioIngrepenChangedMessage(object sender, PrioIngrepenChangedMessage message)
        {
            // TODO
        }

        private void OnFasenChanged(object sender, FasenChangedMessage obj)
        {
            UpdateSelectables();
        }

        #endregion TLCGen events

        #region Constructor

        public PrioriteitSpecialsTabViewModel()
        {
            WeakReferenceMessengerEx.Default.Register<PrioIngrepenChangedMessage>(this, OnPrioIngrepenChangedMessage);
            WeakReferenceMessengerEx.Default.Register<FasenChangedMessage>(this, OnFasenChanged);
        }

        #endregion // Constructor
    }
}
