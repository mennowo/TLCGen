using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Plugins.AFM.Models;
using RelayCommand = GalaSoft.MvvmLight.CommandWpf.RelayCommand;
using System;
using TLCGen.Messaging.Messages;

namespace TLCGen.Plugins.AFM
{
    public class AFMTabViewModel : ViewModelBase
    {
        #region Fields

        private AFMPlugin _plugin;

        #endregion // Fields

        #region Properties

        public ObservableCollection<string> SelectableFasen { get; } = new ObservableCollection<string>();

        private string _selectedFaseToAdd;
        public string SelectedFaseToAdd
        {
            get => _selectedFaseToAdd;
            set
            {
                _selectedFaseToAdd = value;
                RaisePropertyChanged();
            }
        }

        private AFMFaseCyclusDataViewModel _selectedAFMFase;
        public AFMFaseCyclusDataViewModel SelectedAFMFase
        {
            get => _selectedAFMFase;
            set
            {
                _selectedAFMFase = value;
                RaisePropertyChanged();
            }
        }

        private AFMDataModel _afmModel;
        public AFMDataModel AfmModel
        {
            get => _afmModel;
            set
            {
                _afmModel = value;
                AFMFasen = new ObservableCollectionAroundList<AFMFaseCyclusDataViewModel, AFMFaseCyclusDataModel>(_afmModel.AFMFasen);
            }
        }

        public ObservableCollectionAroundList<AFMFaseCyclusDataViewModel, AFMFaseCyclusDataModel> AFMFasen { get; private set; }

        public bool AFMToepassen
        {
            get => _afmModel.AFMToepassen;
            set
            {
                _afmModel.AFMToepassen = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _addFaseCommand;
        public ICommand AddFaseCommand => _addFaseCommand ?? (_addFaseCommand = new RelayCommand(AddFaseCommand_executed, AddFaseCommand_canExecute));

        private bool AddFaseCommand_canExecute()
        {
            return !string.IsNullOrWhiteSpace(SelectedFaseToAdd);
        }

        private void AddFaseCommand_executed()
        {
            AFMFasen.Add(new AFMFaseCyclusDataViewModel(new AFMFaseCyclusDataModel
            {
                FaseCyclus = SelectedFaseToAdd
            }));
            MessengerInstance.Send(new ControllerDataChangedMessage());
        }

        RelayCommand _removeFaseCommand;
        public ICommand RemoveFaseCommand => _removeFaseCommand ?? (_removeFaseCommand = new RelayCommand(RemoveFaseCommand_executed, RemoveFaseCommand_canExecute));

        private bool RemoveFaseCommand_canExecute()
        {
            return SelectedAFMFase != null;
        }

        private void RemoveFaseCommand_executed()
        {
            AFMFasen.Remove(SelectedAFMFase);
            SelectedAFMFase = null;
            MessengerInstance.Send(new ControllerDataChangedMessage());
        }

        #endregion // Commands

        #region TLCGen messaging
        #endregion // TLCGen messaging

        #region Public Methods

        public void UpdateSelectableFasen(IEnumerable<string> fasen)
        {
            SelectableFasen.Clear();
            foreach(var f in fasen)
            {
                SelectableFasen.Add(f);
            }
        }

        #endregion // Public Methods

        #region Constructor

        public AFMTabViewModel(AFMPlugin plugin)
        {
            _plugin = plugin;
        }

        #endregion // Constructor
    }
}
