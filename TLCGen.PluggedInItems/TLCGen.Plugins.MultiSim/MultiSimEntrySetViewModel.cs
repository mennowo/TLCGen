using GalaSoft.MvvmLight;
using TLCGen.Helpers;

namespace TLCGen.Plugins.MultiSim
{
    public class MultiSimEntrySetViewModel : ViewModelBase, IViewModelWithItem
    {
        public MultiSimEntrySetModel MultiSimEntrySet { get; }

        public string Description
        {
            get => MultiSimEntrySet.Description;
            set
            {
                MultiSimEntrySet.Description = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public ObservableCollectionAroundList<MultiSimEntryViewModel, MultiSimEntryModel> Entries { get; private set; }

        public object GetItem()
        {
            return MultiSimEntrySet;
        }

        public MultiSimEntrySetViewModel(MultiSimEntrySetModel multiSimEntrySet)
        {
            MultiSimEntrySet = multiSimEntrySet;

            Entries = new ObservableCollectionAroundList<MultiSimEntryViewModel, MultiSimEntryModel>(MultiSimEntrySet.SimulationEntries);
        }
    }
}
