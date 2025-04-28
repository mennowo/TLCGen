
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Linq;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.DataAccess;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;


namespace TLCGen.ViewModels
{
    public class RISFaseCyclusLaneDataViewModel : ObservableObjectEx, IViewModelWithItem, IComparable
    {
        private RISFaseCyclusLaneDataModel _laneData;
        private RISFaseCyclusLaneSimulatedStationViewModel _selectedStation;

        public ObservableCollectionAroundList<RISFaseCyclusLaneSimulatedStationViewModel, RISFaseCyclusLaneSimulatedStationModel> SimulatedStations { get; }

        public RISFaseCyclusLaneSimulatedStationViewModel SelectedStation
        {
            get => _selectedStation;
            set
            {
                _selectedStation = value;
                OnPropertyChanged();
            }
        }

        public int LaneID
        {
            get => _laneData.LaneID;
            set
            {
                _laneData.LaneID = value;
                OnPropertyChanged(broadcast: true);
                foreach (var s in SimulatedStations)
                {
                    s.StationData.LaneID = value;
                }
            }
        }

        public string SystemITF
        {
            get => _laneData.SystemITF;
            set
            {
                _laneData.SystemITF = value;
                OnPropertyChanged(broadcast: true);
                foreach (var s in SimulatedStations)
                {
                    s.StationData.SystemITF = value;
                }
            }
        }
        
        public bool UseHeading
        {
            get => _laneData.UseHeading;
            set
            {
                _laneData.UseHeading = value;
                OnPropertyChanged(broadcast: true);
            }
        }
        
        public int Heading
        {
            get => _laneData.Heading;
            set
            {
                _laneData.Heading = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int HeadingMarge
        {
            get => _laneData.HeadingMarge;
            set
            {
                _laneData.HeadingMarge = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public string SignalGroupName => _laneData.SignalGroupName;

        public int RijstrookIndex => _laneData.RijstrookIndex;

        public string LaneIDDescription => "Fase " + SignalGroupName + " - Rijstrook " + RijstrookIndex;

        private AddRemoveItemsManager<RISFaseCyclusLaneSimulatedStationViewModel, RISFaseCyclusLaneSimulatedStationModel, string> _stationsManager;
        public AddRemoveItemsManager<RISFaseCyclusLaneSimulatedStationViewModel, RISFaseCyclusLaneSimulatedStationModel, string> StationsManager =>
            _stationsManager ??= new AddRemoveItemsManager<RISFaseCyclusLaneSimulatedStationViewModel, RISFaseCyclusLaneSimulatedStationModel, string>(
                SimulatedStations,
                x => 
                {
                    var sg = ModelManagement.TLCGenModelManager.Default.Controller.Fasen.FirstOrDefault(x2 => x2.Naam == _laneData.SignalGroupName);
                    return FasenRISTabViewModel.GetNewStationForSignalGroup(sg, LaneID, RijstrookIndex, SystemITF);
                },
                (x, y) => false,
                () => WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage())
            );

        public object GetItem()
        {
            return _laneData;
        }

        public int CompareTo(object obj)
        {
            var other = obj as RISFaseCyclusLaneDataViewModel;
            if(_laneData.SignalGroupName == other.SignalGroupName)
            {
                return RijstrookIndex.CompareTo(other.RijstrookIndex);
            }
            else
            {
                return string.CompareOrdinal(_laneData.SignalGroupName, other.SignalGroupName);
            }
        }

        public RISFaseCyclusLaneDataViewModel(RISFaseCyclusLaneDataModel laneData)
        {
            _laneData = laneData;
            SimulatedStations = new ObservableCollectionAroundList<RISFaseCyclusLaneSimulatedStationViewModel, RISFaseCyclusLaneSimulatedStationModel>(laneData.SimulatedStations);
            foreach (var ss in SimulatedStations)
            {
                if (ss.StationData.SimulationData.FCNr != "NG" && 
                    TLCGenControllerDataProvider.Default.Controller.Fasen.All(x => x.Naam != ss.StationData.SimulationData.FCNr))
                {
                    ss.StationData.SimulationData.FCNr = _laneData.SignalGroupName;
                }
            }
        }
    }
}
