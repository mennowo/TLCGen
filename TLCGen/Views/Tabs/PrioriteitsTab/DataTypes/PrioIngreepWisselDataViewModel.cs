using System.Collections.ObjectModel;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using TLCGen.Controls;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class PrioIngreepWisselDataViewModel : ViewModelBase
    {
        public PrioIngreepMeldingenDataModel PrioIngreepMeldingenData { get; }

        [Browsable(false)]
        public ICollectionView WisselDetectoren =>
            ControllerAccessProvider.Default.GetCollectionView(DetectorTypeEnum.WisselStandDetector);

        [Browsable(false)]
        public ICollectionView WisselInputs =>
            ControllerAccessProvider.Default.GetCollectionView(IngangTypeEnum.WisselContact);

        [Category("Anti jutter uitmelding")]
        [Description("Toepassen")]
        public bool AntiJutterVoorAlleUitmeldingen
        {
            get => PrioIngreepMeldingenData.AntiJutterVoorAlleUitmeldingen;
            set
            {
                PrioIngreepMeldingenData.AntiJutterVoorAlleUitmeldingen = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        [Description("Anti jutter tijd")]
        public int AntiJutterTijdVoorAlleUitmeldingen
        {
            get => PrioIngreepMeldingenData.AntiJutterTijdVoorAlleUitmeldingen;
            set
            {
                PrioIngreepMeldingenData.AntiJutterTijdVoorAlleUitmeldingen = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        [Browsable(false)]
        public bool IsWissel1Ingang => Wissel1Type == PrioIngreepInUitDataWisselTypeEnum.Ingang;
        [Browsable(false)]
        public bool IsWissel2Ingang => Wissel2Type == PrioIngreepInUitDataWisselTypeEnum.Ingang;
        [Browsable(false)]
        public bool IsWissel1Detector => Wissel1Type == PrioIngreepInUitDataWisselTypeEnum.Detector;
        [Browsable(false)]
        public bool IsWissel2Detector => Wissel2Type == PrioIngreepInUitDataWisselTypeEnum.Detector;

        [Category("Wissels")]
        [Description("Wissel 1")]
        public bool Wissel1
        {
            get => PrioIngreepMeldingenData.Wissel1;
            set
            {
                PrioIngreepMeldingenData.Wissel1 = value;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(HasWissel));
            }
        }

        [Description("Wissel 1 type")]
        public PrioIngreepInUitDataWisselTypeEnum Wissel1Type
        {
            get => PrioIngreepMeldingenData.Wissel1Type;
            set
            {
                PrioIngreepMeldingenData.Wissel1Type = value;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(IsWissel1Ingang));
                RaisePropertyChanged(nameof(IsWissel1Detector));
            }
        }

        [Description("Wissel 1 input")]
        [BrowsableCondition(nameof(IsWissel1Ingang))]
        public string Wissel1Input
        {
            get => PrioIngreepMeldingenData.Wissel1Input;
            set
            {
                if (value != null)
                {
                    PrioIngreepMeldingenData.Wissel1Input = value;
                }
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        [Description("Wissel 1 voorwaarde")]
        [BrowsableCondition(nameof(IsWissel1Ingang))]
        public bool Wissel1Voorwaarde
        {
            get => PrioIngreepMeldingenData.Wissel1InputVoorwaarde;
            set
            {
                PrioIngreepMeldingenData.Wissel1InputVoorwaarde = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        [Description("Wissel 1 detector")]
        [BrowsableCondition(nameof(IsWissel1Detector))]
        public string Wissel1Detector
        {
            get => PrioIngreepMeldingenData.Wissel1Detector;
            set
            {
                if (value != null)
                {
                    PrioIngreepMeldingenData.Wissel1Detector = value;
                }
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        [Description("Wissel 2")]
        public bool Wissel2
        {
            get => PrioIngreepMeldingenData.Wissel2;
            set
            {
                PrioIngreepMeldingenData.Wissel2 = value;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(HasWissel));
            }
        }

        [Description("Wissel 2 type")]
        public PrioIngreepInUitDataWisselTypeEnum Wissel2Type
        {
            get => PrioIngreepMeldingenData.Wissel2Type;
            set
            {
                PrioIngreepMeldingenData.Wissel2Type = value;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(IsWissel2Ingang));
                RaisePropertyChanged(nameof(IsWissel2Detector));
            }
        }

        [Description("Wissel 2 input")]
        [BrowsableCondition(nameof(IsWissel2Ingang))]
        public string Wissel2Input
        {
            get => PrioIngreepMeldingenData.Wissel2Input;
            set
            {
                if (value != null)
                {
                    PrioIngreepMeldingenData.Wissel2Input = value;
                }
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        [Description("Wissel 2 detector")]
        [BrowsableCondition(nameof(IsWissel2Detector))]
        public string Wissel2Detector
        {
            get => PrioIngreepMeldingenData.Wissel2Detector;
            set
            {
                if (value != null)
                {
                    PrioIngreepMeldingenData.Wissel2Detector = value;
                }
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        [Description("Wissel 2 voorwaarde")]
        [BrowsableCondition(nameof(IsWissel2Detector))]
        public bool Wissel2Voorwaarde
        {
            get => PrioIngreepMeldingenData.Wissel2InputVoorwaarde;
            set
            {
                PrioIngreepMeldingenData.Wissel2InputVoorwaarde = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        [Browsable(false)]
        public bool HasWissel => Wissel1 || Wissel2;

        public PrioIngreepWisselDataViewModel(PrioIngreepMeldingenDataModel prioIngreepMeldingenData)
        {
            PrioIngreepMeldingenData = prioIngreepMeldingenData;
        }
    }
}
