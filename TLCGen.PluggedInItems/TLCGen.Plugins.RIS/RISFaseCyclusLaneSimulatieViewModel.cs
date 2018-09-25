using GalaSoft.MvvmLight;
using System;
using System.Linq;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Plugins.RIS.Models;

namespace TLCGen.Plugins.RIS
{
    public class RISFaseCyclusLaneSimulatieViewModel : ViewModelBase, IViewModelWithItem
    {
        private RISFaseCyclusLaneSimulatieModel _laneData;
        private RISFaseCyclusLaneStationTypeSimulatieViewModel _selectedLane;

        public ObservableCollectionAroundList<RISFaseCyclusLaneStationTypeSimulatieViewModel, RISFaseCyclusLaneStationTypeSimulatieModel> Stations { get; }

        public RISFaseCyclusLaneStationTypeSimulatieViewModel SelectedStation
        {
            get => _selectedLane;
            set
            {
                _selectedLane = value;
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

        private GalaSoft.MvvmLight.CommandWpf.RelayCommand _addStationCommand;
        public ICommand AddStationCommand => _addStationCommand ?? (_addStationCommand = new GalaSoft.MvvmLight.CommandWpf.RelayCommand(AddStationCommand_executed));

        private void AddStationCommand_executed()
        {
            SelectedStation = new RISFaseCyclusLaneStationTypeSimulatieViewModel(new RISFaseCyclusLaneStationTypeSimulatieModel());
            Stations.Add(SelectedStation);
        }

        private GalaSoft.MvvmLight.CommandWpf.RelayCommand _removeStationCommand;
        public ICommand RemoveStationCommand => _removeStationCommand ?? (_removeStationCommand = new GalaSoft.MvvmLight.CommandWpf.RelayCommand(RemoveStationCommand_executed, RemoveStationCommand_canExecute));

        private bool RemoveStationCommand_canExecute()
        {
            return SelectedStation != null;
        }

        private void RemoveStationCommand_executed()
        {
            var i = Stations.IndexOf(SelectedStation);
            Stations.Remove(SelectedStation);
            if(i != -1 && i < Stations.Count)
            {
                SelectedStation = Stations[i];
            }
            else if (Stations.Any())
            {
                SelectedStation = Stations.Last();
            }
            else
            {
                SelectedStation = null;
            }
        }

        public object GetItem()
        {
            return _laneData;
        }

        public RISFaseCyclusLaneSimulatieViewModel(RISFaseCyclusLaneSimulatieModel laneData)
        {
            _laneData = laneData;
            Stations = new ObservableCollectionAroundList<RISFaseCyclusLaneStationTypeSimulatieViewModel, RISFaseCyclusLaneStationTypeSimulatieModel>(laneData.StationTypes);
        }
    }
}
