using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Plugins.AFM.Models;
using RelayCommand = GalaSoft.MvvmLight.CommandWpf.RelayCommand;
using System;
using TLCGen.Messaging.Messages;
using System.Linq;
using TLCGen.Extensions;
using TLCGen.ModelManagement;

namespace TLCGen.Plugins.AFM
{
    public class AFMTabViewModel : ViewModelBase
    {
        #region Fields

        private AFMPlugin _plugin;

        #endregion // Fields

        #region Properties

        public List<string> AllFasen { get; } = new List<string>();
        public ObservableCollection<string> SelectableFasen { get; } = new ObservableCollection<string>();
        private ObservableCollection<string> _SelectableDummyFasen = new ObservableCollection<string>();
        public ObservableCollection<string> SelectableDummyFasen => _SelectableDummyFasen;

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
            var i = SelectableFasen.IndexOf(SelectedFaseToAdd);
            AFMFasen.Add(new AFMFaseCyclusDataViewModel(new AFMFaseCyclusDataModel
            {
                FaseCyclus = SelectedFaseToAdd,
                DummyFaseCyclus = "NG",
                MinimaleGroentijd = 6,
                MaximaleGroentijd = 80
            }));
            UpdateSelectableFasen(null);
            if (SelectableFasen.Any())
            {
                if (i >= 0 && SelectableFasen.Count > i) SelectedFaseToAdd = SelectableFasen[i];
                else SelectedFaseToAdd = SelectableFasen.Last();
            }
            else
            {
                SelectedFaseToAdd = null;
            }
            AFMFasen.BubbleSort();
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
            var i = AFMFasen.IndexOf(SelectedAFMFase);
            var iA = SelectableFasen.IndexOf(SelectedFaseToAdd);
            AFMFasen.Remove(SelectedAFMFase);
            if (AFMFasen.Count > 0)
            {
                if (i >= 0 && AFMFasen.Count > i) SelectedAFMFase = AFMFasen[i];
                else SelectedAFMFase = AFMFasen.Last();
            }
            else
            {
                SelectedAFMFase = null;
            }
            UpdateSelectableFasen(null);
            if (SelectableFasen.Any())
            {
                if (iA >= 0 && SelectableFasen.Count > iA) SelectedFaseToAdd = SelectableFasen[iA];
                else SelectedFaseToAdd = SelectableFasen.Last();
            }
            else
            {
                SelectedFaseToAdd = null;
            }
            MessengerInstance.Send(new ControllerDataChangedMessage());
        }

        #endregion // Commands

        #region TLCGen messaging

        private void OnFasenChanged(FasenChangedMessage msg)
        {
            if(msg.RemovedFasen != null && msg.RemovedFasen.Any())
            {
                foreach(var fc in msg.RemovedFasen)
                {
                    var afmFc = AFMFasen.FirstOrDefault(x => x.FaseCyclus == fc.Naam);
                    if(afmFc != null)
                    {
                        AFMFasen.Remove(afmFc);
                    }
                    var afmFcDummy = AFMFasen.FirstOrDefault(x => x.DummyFaseCyclus == fc.Naam);
                    if (afmFcDummy != null)
                    {
                        afmFcDummy.DummyFaseCyclus = "NG";
                    }
                }
            }
        }

        private void OnNameChanged(NameChangedMessage msg)
        {
            if(msg.ObjectType == TLCGen.Models.Enumerations.TLCGenObjectTypeEnum.Fase)
            {
                TLCGenModelManager.Default.ChangeNameOnObject(AfmModel, msg.OldName, msg.NewName);
                AFMFasen.Rebuild();
                AFMFasen.BubbleSort();
            }
        }

        #endregion // TLCGen messaging

        #region Public Methods

        public void UpdateMessaging()
        {
            MessengerInstance.Register<FasenChangedMessage>(this, OnFasenChanged);
            MessengerInstance.Register<NameChangedMessage>(this, OnNameChanged);
        }

        public void UpdateSelectableFasen(IEnumerable<string> allFasen)
        {
            if(allFasen != null)
            {
                AllFasen.Clear();
                _SelectableDummyFasen.Clear();
                foreach (var f in allFasen)
                {
                    AllFasen.Add(f);
                    _SelectableDummyFasen.Add(f);
                }
                _SelectableDummyFasen.Add("NG");
            }
            SelectableFasen.Clear();
            foreach(var f in AllFasen)
            {
                if(!AFMFasen.Any(x => x.FaseCyclus == f))
                {
                    SelectableFasen.Add(f);
                }
            }
            if(SelectableFasen.Any()) SelectedFaseToAdd = SelectableFasen[0];
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
