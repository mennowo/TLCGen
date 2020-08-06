using GalaSoft.MvvmLight;

namespace TLCGen.ViewModels
{
    public class PrioIngreepPelotonMeldingViewModel : ViewModelBase
    {
        #region Fields
        #endregion // Fields
        
        #region Properties
        
        public PrioIngreepInUitMeldingViewModel Parent { get; }

        public int RisStart
        {
            get => Parent.PrioIngreepInUitMelding.RisStart;
            set 
            { 
                Parent.PrioIngreepInUitMelding.RisStart = value; 
                RaisePropertyChanged();
            }
        }

        public int RisEnd
        {
            get => Parent.PrioIngreepInUitMelding.RisEnd;
            set 
            { 
                Parent.PrioIngreepInUitMelding.RisEnd = value; 
                RaisePropertyChanged();
            }
        }

        public bool RisMatchSg
        {
            get => Parent.PrioIngreepInUitMelding.RisMatchSg;
            set 
            { 
                Parent.PrioIngreepInUitMelding.RisMatchSg = value; 
                RaisePropertyChanged();
            }
        }

        #endregion // Properties

        #region Constructor

        public PrioIngreepPelotonMeldingViewModel(PrioIngreepInUitMeldingViewModel parent)
        {
            Parent = parent;
        }
        
        #endregion // Constructor
    }
}