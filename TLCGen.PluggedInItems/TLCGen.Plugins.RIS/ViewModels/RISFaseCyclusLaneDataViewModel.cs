using GalaSoft.MvvmLight;
using System;
using System.Linq;
using TLCGen.Helpers;
using TLCGen.Plugins.RIS.Models;

namespace TLCGen.Plugins.RIS
{
    public class RISFaseCyclusLaneDataViewModel : ViewModelBase, IViewModelWithItem, IComparable
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
                RaisePropertyChanged();
            }
        }

        public int LaneID
        {
            get => _laneData.LaneID;
            set
            {
                _laneData.LaneID = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public string SignalGroupName => _laneData.SignalGroupName;

        public int RijstrookIndex => _laneData.RijstrookIndex;

        public string LaneIDDescription => "Fase " + SignalGroupName + " - Lane " + LaneID;

        private AddRemoveItemsManager<RISFaseCyclusLaneSimulatedStationViewModel, RISFaseCyclusLaneSimulatedStationModel, string> _stationsManager;
        public AddRemoveItemsManager<RISFaseCyclusLaneSimulatedStationViewModel, RISFaseCyclusLaneSimulatedStationModel, string> StationsManager =>
            _stationsManager ??
            (_stationsManager = new AddRemoveItemsManager<RISFaseCyclusLaneSimulatedStationViewModel, RISFaseCyclusLaneSimulatedStationModel, string>(
                SimulatedStations,
                x => 
                {
                    var sg = ModelManagement.TLCGenModelManager.Default.Controller.Fasen.FirstOrDefault(x2 => x2.Naam == _laneData.SignalGroupName);
                    return RISPlugin.GetNewStationForSignalGroup(sg);
                },
                (x, y) => false
                ));

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
        }
    }
}
