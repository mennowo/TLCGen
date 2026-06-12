using System.Collections.ObjectModel;
using TLCGen.Helpers;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class PrioIngreepOpticomMeldingViewModel : ObservableObjectEx
    {
        #region Fields
        #endregion // Fields
        
        #region Properties
        
        public PrioIngreepInUitMeldingViewModel Parent { get; }

        public ObservableCollection<string> AvailableDetectors => Parent.Type == null ? null : ControllerAccessProvider.Default.AllOpticomDetectorStrings;
        
        public string RelatedInput1
        {
            get => Parent.PrioIngreepInUitMelding.RelatedInput1;
            set
            {
                if(value != null)
                {
                    Parent.PrioIngreepInUitMelding.RelatedInput1 = value;
                }
                OnPropertyChanged(broadcast: true);
            }
        }

        public PrioIngreepInUitMeldingTypeEnum InUit => Parent.InUit;

        public bool AntiJutterTijdToepassen
        {
            get => Parent.PrioIngreepInUitMelding.AntiJutterTijdToepassen;
            set
            {
                Parent.PrioIngreepInUitMelding.AntiJutterTijdToepassen = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int AntiJutterTijd
        {
            get => Parent.PrioIngreepInUitMelding.AntiJutterTijd;
            set 
            { 
                Parent.PrioIngreepInUitMelding.AntiJutterTijd = value; 
                OnPropertyChanged(broadcast: true);
            }
        }
        
        #endregion // Properties

        #region Constructor

        public PrioIngreepOpticomMeldingViewModel(PrioIngreepInUitMeldingViewModel parent)
        {
            Parent = parent;
        }

        #endregion // Constructor
    }
}
