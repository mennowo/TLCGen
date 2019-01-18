using GalaSoft.MvvmLight;
using System;
using System.Linq;
using TLCGen.Helpers;
using TLCGen.Plugins.RIS.Models;

namespace TLCGen.Plugins.RIS
{
    public class RISFaseCyclusDataViewModel : ViewModelBase, IViewModelWithItem, IComparable
    {
        private RISFaseCyclusDataModel _faseCyclus;

        private RISFaseCyclusLaneDataViewModel _selectedLane;

        public ObservableCollectionAroundList<RISFaseCyclusLaneDataViewModel, RISFaseCyclusLaneDataModel> Lanes { get; }

        public RISFaseCyclusLaneDataViewModel SelectedLane
        {
            get => _selectedLane;
            set
            {
                _selectedLane = value;
                RaisePropertyChanged();
            }
        }

        public string FaseCyclus
        {

            get => _faseCyclus.FaseCyclus;
            set
            {
                _faseCyclus.FaseCyclus = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public object GetItem()
        {
            return _faseCyclus;
        }

        public int CompareTo(object obj)
        {
            return string.CompareOrdinal(FaseCyclus, ((RISFaseCyclusDataViewModel)obj).FaseCyclus);
        }

        public RISFaseCyclusDataViewModel(RISFaseCyclusDataModel faseCyclus)
        {
            _faseCyclus = faseCyclus;
            Lanes = new ObservableCollectionAroundList<RISFaseCyclusLaneDataViewModel, RISFaseCyclusLaneDataModel>(faseCyclus.LaneData);
        }
    }
}
