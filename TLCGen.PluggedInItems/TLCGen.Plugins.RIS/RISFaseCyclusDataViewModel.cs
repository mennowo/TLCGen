using GalaSoft.MvvmLight;
using System;
using TLCGen.Helpers;
using TLCGen.Plugins.RIS.Models;

namespace TLCGen.Plugins.RIS
{
    public class RISFaseCyclusDataViewModel : ViewModelBase, IViewModelWithItem, IComparable
    {
        private RISFaseCyclusDataModel _faseCyclus;

        private RISFaseCyclusSimulatieViewModel _simulatieVM;

        public RISFaseCyclusSimulatieViewModel SimulatieVM => _simulatieVM ?? (_simulatieVM = new RISFaseCyclusSimulatieViewModel(_faseCyclus.SimulatieData));

        public string FaseCyclus
        {

            get => _faseCyclus.FaseCyclus;
            set
            {
                _faseCyclus.FaseCyclus = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public bool RISAanvraag
        {
            get => _faseCyclus.RISAanvraag;
            set
            {
                _faseCyclus.RISAanvraag = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int AanvraagStart
        {
            get => _faseCyclus.AanvraagStart;
            set
            {
                _faseCyclus.AanvraagStart = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int AanvraagEnd
        {
            get => _faseCyclus.AanvraagEnd;
            set
            {
                _faseCyclus.AanvraagEnd = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public bool RISVerlengen
        {
            get => _faseCyclus.RISVerlengen;
            set
            {
                _faseCyclus.RISVerlengen = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int VerlengenStart
        {
            get => _faseCyclus.VerlengenStart;
            set
            {
                _faseCyclus.VerlengenStart = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int VerlengenEnd
        {
            get => _faseCyclus.VerlengenEnd;
            set
            {
                _faseCyclus.VerlengenEnd = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public bool HasRIS => RISAanvraag || RISVerlengen;

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
        }
    }
}
