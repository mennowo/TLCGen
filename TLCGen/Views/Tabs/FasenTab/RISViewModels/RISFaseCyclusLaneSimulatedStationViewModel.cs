using CommunityToolkit.Mvvm.ComponentModel;
using System;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class RISFaseCyclusLaneSimulatedStationViewModel : ObservableObjectEx, IViewModelWithItem, IComparable
    {
        public RISFaseCyclusLaneSimulatedStationModel StationData { get; }

        public RISStationTypeSimEnum StationType
        {
            get => StationData.Type;
            set
            {
                StationData.Type = value;
                StationData.SimulationData.RelatedName = StationData.Naam;
                OnPropertyChanged(broadcast: true);
                OnPropertyChanged(nameof(HasEx));
            }
        }

        public bool HasEx =>
            StationType == RISStationTypeSimEnum.BUS ||
            StationType == RISStationTypeSimEnum.TRAM ||
            StationType == RISStationTypeSimEnum.SPECIALVEHICLES;
        
        public RISVehicleRole VehicleRole
        {
            get => StationData.VehicleRole;
            set
            {
                StationData.VehicleRole = value;
                OnPropertyChanged(broadcast: true);
            }
        }
        
        public RISVehicleSubrole VehicleSubrole
        {
            get => StationData.VehicleSubrole;
            set
            {
                StationData.VehicleSubrole = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public bool Prioriteit
        {
            get => StationData.Prioriteit;
            set
            {
                StationData.Prioriteit = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int Flow
        {
            get => StationData.Flow;
            set
            {
                StationData.Flow = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int Snelheid
        {
            get => StationData.Snelheid;
            set
            {
                StationData.Snelheid = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int Afstand
        {
            get => StationData.Afstand;
            set
            {
                StationData.Afstand = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int Q1
        {
            get => StationData.SimulationData.Q1;
            set
            {
                StationData.SimulationData.Q1 = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int Q2
        {
            get => StationData.SimulationData.Q2;
            set
            {
                StationData.SimulationData.Q2 = value;
                OnPropertyChanged(broadcast: true);
            }
        }
        public int Q3
        {
            get => StationData.SimulationData.Q3;
            set
            {
                StationData.SimulationData.Q3 = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int Q4
        {
            get => StationData.SimulationData.Q4;
            set
            {
                StationData.SimulationData.Q4 = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int Stopline
        {
            get => StationData.SimulationData.Stopline;
            set
            {
                StationData.SimulationData.Stopline = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public string FCNr
        {
            get => StationData.SimulationData.FCNr;
            set
            {
                StationData.SimulationData.FCNr = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int Importance
        {
            get => StationData.Importance;
            set
            {
                StationData.Importance = value;
                OnPropertyChanged(broadcast: true);
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
