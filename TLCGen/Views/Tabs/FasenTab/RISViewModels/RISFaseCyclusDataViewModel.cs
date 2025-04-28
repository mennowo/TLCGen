using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Linq;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class RISFaseCyclusDataViewModel : ObservableObjectEx, IViewModelWithItem, IComparable
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
                OnPropertyChanged();
            }
        }

        public string FaseCyclus
        {

            get => _faseCyclus.FaseCyclus;
            set
            {
                _faseCyclus.FaseCyclus = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int ApproachID
        {

            get => _faseCyclus.ApproachID;
            set
            {
                _faseCyclus.ApproachID = value;
                OnPropertyChanged(broadcast: true);
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
