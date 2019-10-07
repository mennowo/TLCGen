using GalaSoft.MvvmLight;
using System;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class RISFaseCyclusLaneSimulatedStationViewModel : ViewModelBase, IViewModelWithItem, IComparable
    {
        public RISFaseCyclusLaneSimulatedStationModel StationData { get; }

        public RISStationTypeSimEnum Type
        {
            get => StationData.Type;
            set
            {
                StationData.Type = value;
                StationData.SimulationData.RelatedName = StationData.Naam;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public bool Prioriteit
        {
            get => StationData.Prioriteit;
            set
            {
                StationData.Prioriteit = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int Flow
        {
            get => StationData.Flow;
            set
            {
                StationData.Flow = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int Snelheid
        {
            get => StationData.Snelheid;
            set
            {
                StationData.Snelheid = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int Afstand
        {
            get => StationData.Afstand;
            set
            {
                StationData.Afstand = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int Q1
        {
            get => StationData.SimulationData.Q1;
            set
            {
                StationData.SimulationData.Q1 = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int Q2
        {
            get => StationData.SimulationData.Q2;
            set
            {
                StationData.SimulationData.Q2 = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }
        public int Q3
        {
            get => StationData.SimulationData.Q3;
            set
            {
                StationData.SimulationData.Q3 = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int Q4
        {
            get => StationData.SimulationData.Q4;
            set
            {
                StationData.SimulationData.Q4 = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int Stopline
        {
            get => StationData.SimulationData.Stopline;
            set
            {
                StationData.SimulationData.Stopline = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public object GetItem()
        {
            return StationData;
        }

        public int CompareTo(object obj)
        {
            return 0;
        }

        public RISFaseCyclusLaneSimulatedStationViewModel(RISFaseCyclusLaneSimulatedStationModel stationData)
        {
            StationData = stationData;
        }
    }
}
