using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class NevenMeldingViewModel : ViewModelBase, IViewModelWithItem
    {
        #region Properties

        public NevenMeldingModel NevenMelding { get; }

        public string FaseCyclus1
        {
            get => NevenMelding.FaseCyclus1;
            set
            {
                NevenMelding.FaseCyclus1 = value;
                if (FaseCyclus2 == value) FaseCyclus2 = null;
                if (FaseCyclus3 == value) FaseCyclus3 = null;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }
        
        public string FaseCyclus2
        {
            get => NevenMelding.FaseCyclus2;
            set
            {
                NevenMelding.FaseCyclus2 = value; 
                if (FaseCyclus1 == value) FaseCyclus1 = null;
                if (FaseCyclus3 == value) FaseCyclus3 = null;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }
        
        public string FaseCyclus3
        {
            get => NevenMelding.FaseCyclus3;
            set
            {
                NevenMelding.FaseCyclus3 = value; 
                if (FaseCyclus1 == value) FaseCyclus1 = null;
                if (FaseCyclus2 == value) FaseCyclus2 = null;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }
        
        public int BezetTijdLaag
        {
            get => NevenMelding.BezetTijdLaag;
            set
            {
                NevenMelding.BezetTijdLaag = value; 
                RaisePropertyChanged<object>(broadcast: true);
            }
        }
        
        public int BezetTijdHoog
        {
            get => NevenMelding.BezetTijdHoog;
            set
            {
                NevenMelding.BezetTijdHoog = value; 
                RaisePropertyChanged<object>(broadcast: true);
            }
        }
        
        public int Rijtijd
        {
            get => NevenMelding.Rijtijd;
            set
            {
                NevenMelding.Rijtijd = value; 
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        #endregion // Properties

        #region IViewModelWithItem

        public object GetItem()
        {
            return NevenMelding;
        }

        #endregion // IViewModelWithItem

        #region Constructor

        public NevenMeldingViewModel(NevenMeldingModel nevenMelding)
        {
            NevenMelding = nevenMelding;
        }
        
        #endregion // Constructor
    }
}