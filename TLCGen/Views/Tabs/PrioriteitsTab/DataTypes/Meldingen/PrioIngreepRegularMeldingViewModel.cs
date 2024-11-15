﻿using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class PrioIngreepRegularMeldingViewModel : ViewModelBase
    {
        #region Fields
        #endregion // Fields

        #region Properties

        public PrioIngreepInUitMeldingViewModel Parent { get; }
        
        public ObservableCollection<string> AvailableInputs
        {
            get
            {
                if (Parent.Type == null) return null;

                switch (Parent.Type.Value)
                {
                    case PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector:
                        return ControllerAccessProvider.Default.AllDetectorStrings;
                    case PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector:
                        return ControllerAccessProvider.Default.AllSelectiveDetectorStrings;
                    case PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding:
                        break;
                    case PrioIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector:
                        return ControllerAccessProvider.Default.AllVecomDetectorStrings;
                    case PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde:
                        break;
                    case PrioIngreepInUitMeldingVoorwaardeTypeEnum.Ingang:
                        return ControllerAccessProvider.Default.OVIngangenStrings;
                        
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return null;
            }
        }
        
        public bool HasInput1 => Parent.Type?.Value != PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding;
        
        public bool DisabledIfDSI => Parent.Type?.Value != PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding;

        public bool HasInput1Type => Parent.Type?.Value != PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding && 
                                     Parent.Type?.Value != PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector;

        public bool HasInput2 => CanHaveInput2 && TweedeInput;
        
        public bool CanHaveInput2 => Parent.Type?.Value != PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding &&
                                     Parent.Type?.Value != PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector;

        
        public bool HasKAR => Parent.Type?.Value == PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding;
        public bool HasSD => Parent.Type?.Value == PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector;
        public bool HasInpSD => Parent.Type?.Value == PrioIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector;
        public bool HasDet => Parent.Type?.Value == PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector;
        
        public bool TweedeInput
        {
            get => Parent.PrioIngreepInUitMelding.TweedeInput;
            set
            {
                Parent.PrioIngreepInUitMelding.TweedeInput = value;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(HasInput2));
            }
        }

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

        public PrioIngreepInUitMeldingVoorwaardeInputTypeEnum RelatedInput1Type
        {
            get => Parent.PrioIngreepInUitMelding.RelatedInput1Type;
            set
            {
                Parent.PrioIngreepInUitMelding.RelatedInput1Type = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public string RelatedInput2
        {
            get => Parent.PrioIngreepInUitMelding.RelatedInput2;
            set
            {
                if (value != null)
                {
                    Parent.PrioIngreepInUitMelding.RelatedInput2 = value;
                }
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public PrioIngreepInUitMeldingVoorwaardeInputTypeEnum RelatedInput2Type
        {
            get => Parent.PrioIngreepInUitMelding.RelatedInput2Type;
            set
            {
                Parent.PrioIngreepInUitMelding.RelatedInput2Type = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public bool KijkNaarWisselStand
        {
            get => Parent.PrioIngreepInUitMelding.KijkNaarWisselStand;
            set
            {
                Parent.PrioIngreepInUitMelding.KijkNaarWisselStand = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public bool OpvangStoring
        {
            get => Parent.OpvangStoring;
            set
            {
                Parent.OpvangStoring = value;
                RaisePropertyChanged();
            }
        }

        public bool AlleenIndienGeenInmelding
        {
            get => Parent.PrioIngreepInUitMelding.AlleenIndienGeenInmelding;
            set
            {
                Parent.PrioIngreepInUitMelding.AlleenIndienGeenInmelding = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public bool AlleenIndienRood
        {
            get => Parent.PrioIngreepInUitMelding.AlleenIndienRood;
            set
            {
                Parent.PrioIngreepInUitMelding.AlleenIndienRood = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public bool AntiJutterTijdToepassen
        {
            get => Parent.PrioIngreepInUitMelding.AntiJutterTijdToepassen;
            set
            {
                Parent.PrioIngreepInUitMelding.AntiJutterTijdToepassen = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public bool CheckAltijdOpDsinBijVecom
        {
            get => Parent.PrioIngreepInUitMelding.CheckAltijdOpDsinBijVecom;
            set
            {
                Parent.PrioIngreepInUitMelding.CheckAltijdOpDsinBijVecom = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int AntiJutterTijd
        {
            get => Parent.PrioIngreepInUitMelding.AntiJutterTijd;
            set
            {
                Parent.PrioIngreepInUitMelding.AntiJutterTijd = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }
        
        #endregion // Properties

        #region Constructor

        public PrioIngreepRegularMeldingViewModel(PrioIngreepInUitMeldingViewModel parent)
        {
            Parent = parent;
        }
        
        #endregion // Constructor
    }
}