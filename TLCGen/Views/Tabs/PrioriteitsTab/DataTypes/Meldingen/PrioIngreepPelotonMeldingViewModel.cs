using TLCGen.Helpers;

namespace TLCGen.ViewModels
{
    public class PrioIngreepPelotonMeldingViewModel : ObservableObjectEx
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
                OnPropertyChanged();
            }
        }

        public int RisEnd
        {
            get => Parent.PrioIngreepInUitMelding.RisEnd;
            set 
            { 
                Parent.PrioIngreepInUitMelding.RisEnd = value; 
                OnPropertyChanged();
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