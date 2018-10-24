using GalaSoft.MvvmLight;
using System;
using TLCGen.Helpers;
using TLCGen.Plugins.RIS.Models;

namespace TLCGen.Plugins.RIS
{
    public class RISFaseCyclusLaneSimulatedStationViewModel : ViewModelBase, IViewModelWithItem, IComparable
    {
        private RISFaseCyclusLaneSimulatedStationModel _stationData;

        public RISFaseCyclusLaneSimulatedStationModel StationData => _stationData;

        public RISStationTypeSimEnum Type
        {
            get => _stationData.Type;
            set
            {
                _stationData.Type = value;
                _stationData.SimulationData.RelatedName = _stationData.Naam;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public bool Prioriteit
        {
            get => _stationData.Prioriteit;
            set
            {
                _stationData.Prioriteit = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int Flow
        {
            get => _stationData.Flow;
            set
            {
                _stationData.Flow = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int Snelheid
        {
            get => _stationData.Snelheid;
            set
            {
                _stationData.Snelheid = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int Afstand
        {
            get => _stationData.Afstand;
            set
            {
                _stationData.Afstand = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int Q1
        {
            get => _stationData.SimulationData.Q1;
            set
            {
                _stationData.SimulationData.Q1 = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int Q2
        {
            get => _stationData.SimulationData.Q2;
            set
            {
                _stationData.SimulationData.Q2 = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }
        public int Q3
        {
            get => _stationData.SimulationData.Q3;
            set
            {
                _stationData.SimulationData.Q3 = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int Q4
        {
            get => _stationData.SimulationData.Q4;
            set
            {
                _stationData.SimulationData.Q4 = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int Stopline
        {
            get => _stationData.SimulationData.Stopline;
            set
            {
                _stationData.SimulationData.Stopline = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public object GetItem()
        {
            return _stationData;
        }

        public int CompareTo(object obj)
        {
            return 0;
        }

        public RISFaseCyclusLaneSimulatedStationViewModel(RISFaseCyclusLaneSimulatedStationModel stationData)
        {
            _stationData = stationData;
        }
    }
}
