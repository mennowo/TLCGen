using GalaSoft.MvvmLight;
using TLCGen.Helpers;

namespace TLCGen.Plugins.MultiSim
{
    public class MultiSimEntryViewModel : ViewModelBase, IViewModelWithItem
    {
        public MultiSimEntryModel MultiSimEntry { get; }

        public string DetectorName
        {
            get => MultiSimEntry.DetectorName;
            set
            {
                MultiSimEntry.DetectorName = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int Q1
        {
            get => MultiSimEntry.SimulationModel.Q1;
            set
            {
                MultiSimEntry.SimulationModel.Q1 = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int Q2
        {
            get => MultiSimEntry.SimulationModel.Q2;
            set
            {
                MultiSimEntry.SimulationModel.Q2 = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int Q3
        {
            get => MultiSimEntry.SimulationModel.Q3;
            set
            {
                MultiSimEntry.SimulationModel.Q3 = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int Q4
        {
            get => MultiSimEntry.SimulationModel.Q4;
            set
            {
                MultiSimEntry.SimulationModel.Q4 = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public object GetItem()
        {
            return MultiSimEntry;
        }

        public MultiSimEntryViewModel(MultiSimEntryModel multiSimEntry)
        {
            MultiSimEntry = multiSimEntry;
        }
    }
}
