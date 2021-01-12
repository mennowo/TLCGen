using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.ViewModels;

namespace TLCGen.ViewModels
{
    public class PrioIngreepFietsPrioriteitMeldingViewModel : ViewModelBase
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
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public bool FietsPrioriteitGebruikLus
        {
            get => Parent.PrioIngreepInUitMelding.FietsPrioriteitGebruikLus;
            set
            {
                Parent.PrioIngreepInUitMelding.FietsPrioriteitGebruikLus = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public PrioIngreepInUitMeldingTypeEnum InUit => Parent.InUit;

        public int FietsPrioriteitBlok
        {
            get => Parent.PrioIngreepInUitMelding.FietsPrioriteitBlok;
            set 
            { 
                Parent.PrioIngreepInUitMelding.FietsPrioriteitBlok = value; 
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int FietsPrioriteitAantalKeerPerCyclus
        {
            get => Parent.PrioIngreepInUitMelding.FietsPrioriteitAantalKeerPerCyclus;
            set 
            { 
                Parent.PrioIngreepInUitMelding.FietsPrioriteitAantalKeerPerCyclus = value; 
                RaisePropertyChanged<object>(broadcast: true);
            }
        }
        
        public int FietsPrioriteitMinimumAantalVoertuigen
        {
            get => Parent.PrioIngreepInUitMelding.FietsPrioriteitMinimumAantalVoertuigen;
            set 
            { 
                Parent.PrioIngreepInUitMelding.FietsPrioriteitMinimumAantalVoertuigen = value; 
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int FietsPrioriteitMinimumWachttijdVoorPrioriteit
        {
            get => Parent.PrioIngreepInUitMelding.FietsPrioriteitMinimumWachttijdVoorPrioriteit;
            set 
            { 
                Parent.PrioIngreepInUitMelding.FietsPrioriteitMinimumWachttijdVoorPrioriteit = value; 
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        #endregion // Properties

        #region Constructor

        public PrioIngreepFietsPrioriteitMeldingViewModel(PrioIngreepInUitMeldingViewModel parent)
        {
            Parent = parent;
        }

        #endregion // Constructor
    }
}
