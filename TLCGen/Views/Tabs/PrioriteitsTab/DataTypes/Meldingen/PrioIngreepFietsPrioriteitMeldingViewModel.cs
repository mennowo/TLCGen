using System.Collections.ObjectModel;
using TLCGen.Helpers;
using TLCGen.Models.Enumerations;


namespace TLCGen.ViewModels
{
    public class PrioIngreepFietsPrioriteitMeldingViewModel : ObservableObjectEx
    {
        #region Fields
        #endregion // Fields
        
        #region Properties
        
        public PrioIngreepInUitMeldingViewModel Parent { get; }

        public ObservableCollection<string> AvailableDetectors => Parent.Type == null ? null : ControllerAccessProvider.Default.AllDetectorStrings;
        
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

        public bool FietsPrioriteitGebruikLus
        {
            get => Parent.PrioIngreepInUitMelding.FietsPrioriteitGebruikLus;
            set
            {
                Parent.PrioIngreepInUitMelding.FietsPrioriteitGebruikLus = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public PrioIngreepInUitMeldingTypeEnum InUit => Parent.InUit;

        public int FietsPrioriteitBlok
        {
            get => Parent.PrioIngreepInUitMelding.FietsPrioriteitBlok;
            set 
            { 
                Parent.PrioIngreepInUitMelding.FietsPrioriteitBlok = value; 
                OnPropertyChanged(broadcast: true);
            }
        }

        public int FietsPrioriteitAantalKeerPerCyclus
        {
            get => Parent.PrioIngreepInUitMelding.FietsPrioriteitAantalKeerPerCyclus;
            set 
            { 
                Parent.PrioIngreepInUitMelding.FietsPrioriteitAantalKeerPerCyclus = value; 
                OnPropertyChanged(broadcast: true);
            }
        }
        
        public int FietsPrioriteitMinimumAantalVoertuigen
        {
            get => Parent.PrioIngreepInUitMelding.FietsPrioriteitMinimumAantalVoertuigen;
            set 
            { 
                Parent.PrioIngreepInUitMelding.FietsPrioriteitMinimumAantalVoertuigen = value; 
                OnPropertyChanged(broadcast: true);
            }
        }

        public int FietsPrioriteitMinimumWachttijdVoorPrioriteit
        {
            get => Parent.PrioIngreepInUitMelding.FietsPrioriteitMinimumWachttijdVoorPrioriteit;
            set 
            { 
                Parent.PrioIngreepInUitMelding.FietsPrioriteitMinimumWachttijdVoorPrioriteit = value; 
                OnPropertyChanged(broadcast: true);
            }
        }

        public bool FietsPrioriteitGebruikRIS
        {
            get => Parent.PrioIngreepInUitMelding.FietsPrioriteitGebruikRIS;
            set
            {
                Parent.PrioIngreepInUitMelding.FietsPrioriteitGebruikRIS = value;
                OnPropertyChanged(broadcast: true);
                OnPropertyChanged(nameof(HasAndUsesRIS));
            }
        }

        public int FietsPrioriteitMinimumAantalVoertuigenRIS
        {
            get => Parent.PrioIngreepInUitMelding.FietsPrioriteitMinimumAantalVoertuigenRIS;
            set 
            { 
                Parent.PrioIngreepInUitMelding.FietsPrioriteitMinimumAantalVoertuigenRIS = value; 
                OnPropertyChanged(broadcast: true);
            }
        }
        
        public bool HasRIS => ControllerAccessProvider.Default.Controller.RISData.RISToepassen;

        public bool HasAndUsesRIS => HasRIS && FietsPrioriteitGebruikRIS;

        #endregion // Properties

        #region Constructor

        public PrioIngreepFietsPrioriteitMeldingViewModel(PrioIngreepInUitMeldingViewModel parent)
        {
            Parent = parent;
        }

        #endregion // Constructor
    }
}
