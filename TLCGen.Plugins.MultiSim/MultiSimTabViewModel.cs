using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using RelayCommand = GalaSoft.MvvmLight.CommandWpf.RelayCommand;

namespace TLCGen.Plugins.MultiSim
{
    public class MultiSimTabViewModel : ViewModelBase
    {
        #region Fields

        private MultiSimPlugin _plugin;
        private MultiSimEntrySetViewModel _selectedSimEntrySet;
        private RelayCommand _addMultiSimEntrySetCommand;
        private RelayCommand _removeMultiSimEntrySetCommand;
        private RelayCommand _applyMultiSimEntrySetCommand;
        private MultiSimDataModel _multiSimDataModel;

        #endregion // Fields

        #region Properties

        public List<string> AllFasen { get; } = new List<string>();

        public MultiSimEntrySetViewModel SelectedSimEntrySet
        {
            get => _selectedSimEntrySet;
            set
            {
                _selectedSimEntrySet = value;
                RaisePropertyChanged();
            }
        }

        public MultiSimDataModel MultiSimDataModel
        {
            get => _multiSimDataModel;
            set
            {
                _multiSimDataModel = value;
                MultiSimEntrySets = new ObservableCollectionAroundList<MultiSimEntrySetViewModel, MultiSimEntrySetModel>(value.SimulationEntries);
                RaisePropertyChanged("");
            }
        }

        public ObservableCollectionAroundList<MultiSimEntrySetViewModel, MultiSimEntrySetModel> MultiSimEntrySets { get; private set; }

        #endregion // Properties

        #region Commands

        public ICommand AddMultiSimEntrySetCommand => _addMultiSimEntrySetCommand ?? (_addMultiSimEntrySetCommand = new RelayCommand(() =>
        {
            var set = new MultiSimEntrySetModel { Description = "Sim set" };

            var fasendets = _plugin.Controller.Fasen.SelectMany(x => x.Detectoren);
            var controllerdets = _plugin.Controller.Detectoren;
            var ovdummydets = _plugin.Controller.OVData.GetAllDummyDetectors();
            var alldets = fasendets.Concat(controllerdets).Concat(ovdummydets).ToList();
            foreach (var d in alldets)
            {
                var entry = new MultiSimEntryModel
                {
                    DetectorName = d.Naam,
                    SimulationModel = DeepCloner.DeepClone(d.Simulatie)
                };
                set.SimulationEntries.Add(entry);
            }
            MultiSimEntrySets.Add(new MultiSimEntrySetViewModel(set));
            MessengerInstance.Send(new ControllerDataChangedMessage());
        }));

        public ICommand RemoveMultiSimEntrySetCommand => _removeMultiSimEntrySetCommand ?? (_removeMultiSimEntrySetCommand = new RelayCommand(() =>
        {
            MultiSimEntrySets.Remove(SelectedSimEntrySet);
            MessengerInstance.Send(new ControllerDataChangedMessage());
        },
        () => SelectedSimEntrySet != null));

        public ICommand ApplyMultiSimEntrySetCommand => _applyMultiSimEntrySetCommand ?? (_applyMultiSimEntrySetCommand = new RelayCommand(() =>
        {
            var fasendets = _plugin.Controller.Fasen.SelectMany(x => x.Detectoren);
            var controllerdets = _plugin.Controller.Detectoren;
            var ovdummydets = _plugin.Controller.OVData.GetAllDummyDetectors();
            var alldets = fasendets.Concat(controllerdets).Concat(ovdummydets).ToList();
            foreach (var e in SelectedSimEntrySet.Entries)
            {
                var d = alldets.FirstOrDefault(x => x.Naam == e.DetectorName);
                if (d != null)
                {
                    d.Simulatie.Q1 = e.Q1;
                    d.Simulatie.Q2 = e.Q2;
                    d.Simulatie.Q3 = e.Q3;
                    d.Simulatie.Q4 = e.Q4;
                }
            }
            MessengerInstance.Send(new ControllerDataChangedMessage());
        },
        () => SelectedSimEntrySet != null));

        #endregion // Commands

        #region TLCGen messaging

        private void OnDetectorenChanged(DetectorenChangedMessage msg)
        {
            // TODO
        }

        private void OnNameChanged(NameChangedMessage msg)
        {
            if (msg.ObjectType == TLCGen.Models.Enumerations.TLCGenObjectTypeEnum.Detector)
            {
                // TODO
            }
        }

        #endregion // TLCGen messaging

        #region Public Methods

        public void UpdateMessaging()
        {
            MessengerInstance.Register<DetectorenChangedMessage>(this, OnDetectorenChanged);
            MessengerInstance.Register<NameChangedMessage>(this, OnNameChanged);
        }

        #endregion // Public Methods

        #region Constructor

        public MultiSimTabViewModel(MultiSimPlugin plugin)
        {
            _plugin = plugin;
        }

        #endregion // Constructor
    }
}
