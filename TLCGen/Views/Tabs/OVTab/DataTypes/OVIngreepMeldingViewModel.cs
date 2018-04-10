using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Linq;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Messaging.Requests;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class OVIngreepMeldingViewModel : ViewModelBase, IViewModelWithItem
    {
        #region Fields
        
        private ObservableCollection<string> _MassaDetectoren;
        private ObservableCollection<string> _VecomDetectoren;
        private ObservableCollection<string> _VecomIngangen;

        #endregion // Fields

        #region Properties

        public OVIngreepMeldingModel OVIngreepMelding { get; }


        public OVIngreepMeldingTypeEnum Type
        {
            get => OVIngreepMelding.Type;
            set
            {
                OVIngreepMelding.Type = value;
                RaisePropertyChanged<object>(nameof(Type), broadcast: true);
            }
        }

        public bool Inmelding
        {
            get => OVIngreepMelding.Inmelding;
            set
            {
                OVIngreepMelding.Inmelding = value;
                MessengerInstance.Send(new OVIngreepMeldingChangedMessage(OVIngreepMelding.FaseCyclus, Type));
                RaisePropertyChanged<object>(nameof(Inmelding), broadcast: true);
                RaisePropertyChanged(nameof(CanHaveRelatedInput1));
                RaisePropertyChanged(nameof(CanHaveRelatedInput2));
            }
        }

        public bool Uitmelding
        {
            get => OVIngreepMelding.Uitmelding;
            set
            {
                OVIngreepMelding.Uitmelding = value;
                MessengerInstance.Send(new OVIngreepMeldingChangedMessage(OVIngreepMelding.FaseCyclus, Type));
                RaisePropertyChanged<object>(nameof(Uitmelding), broadcast: true);
                RaisePropertyChanged(nameof(CanHaveRelatedInput1));
                RaisePropertyChanged(nameof(CanHaveRelatedInput2));
            }
        }

        public int? InmeldingFilterTijd
        {
            get => OVIngreepMelding.InmeldingFilterTijd;
            set
            {
                OVIngreepMelding.InmeldingFilterTijd = value;
                RaisePropertyChanged<object>(nameof(InmeldingFilterTijd), broadcast: true);
            }
        }

        public bool CanHaveRelatedInput1 => Inmelding && Type == OVIngreepMeldingTypeEnum.MassaPaarIn ||
                                            Uitmelding && Type == OVIngreepMeldingTypeEnum.MassaPaarUit ||
                                            Inmelding && Type == OVIngreepMeldingTypeEnum.VECOM_io ||
                                            Inmelding && Type == OVIngreepMeldingTypeEnum.VECOM ||
                                            ((Inmelding || Uitmelding) &&
                                             (Type == OVIngreepMeldingTypeEnum.VerlosDetector ||
                                              Type == OVIngreepMeldingTypeEnum.WisselDetector ||
                                              Type == OVIngreepMeldingTypeEnum.WisselStroomKringDetector));

        public bool CanHaveRelatedInput2 => Inmelding && Type == OVIngreepMeldingTypeEnum.MassaPaarIn ||
                                            Uitmelding && Type == OVIngreepMeldingTypeEnum.MassaPaarUit ||
                                            Uitmelding && Type == OVIngreepMeldingTypeEnum.VECOM_io ||
                                            Uitmelding && Type == OVIngreepMeldingTypeEnum.VECOM;

        public string RelatedInput1
        {
            get => OVIngreepMelding.RelatedInput1;
            set
            {
                if (value != null)
                {
                    OVIngreepMelding.RelatedInput1 = value;
                    RaisePropertyChanged<object>(nameof(RelatedInput1), broadcast: true);
                }
            }
        }

        public string RelatedInput2
        {
            get => OVIngreepMelding.RelatedInput2;
            set
            {
                if (value != null)
                {
                    OVIngreepMelding.RelatedInput2 = value;
                    RaisePropertyChanged<object>(nameof(RelatedInput2), broadcast: true);
                }
            }
        }

        public bool HasRelatedSelectableInput1 => Type != OVIngreepMeldingTypeEnum.KAR;

        public bool HasRelatedSelectableInput2 => Type == OVIngreepMeldingTypeEnum.MassaPaarIn || 
                                                  Type == OVIngreepMeldingTypeEnum.MassaPaarUit ||
                                                  Type == OVIngreepMeldingTypeEnum.VECOM ||
                                                  Type == OVIngreepMeldingTypeEnum.VECOM_io;

        public bool HasInmelding => Type != OVIngreepMeldingTypeEnum.WisselDetector && Type != OVIngreepMeldingTypeEnum.MassaPaarUit;

        public bool HasUitmelding => Type != OVIngreepMeldingTypeEnum.MassaPaarIn;

        public ObservableCollection<string> Detectoren
        {
            get
            {
                switch (Type)
                {
                    case OVIngreepMeldingTypeEnum.VECOM:
                        return VecomDetectoren;
                    case OVIngreepMeldingTypeEnum.VECOM_io:
                        return VecomIngangen;
                    case OVIngreepMeldingTypeEnum.VerlosDetector:
                    case OVIngreepMeldingTypeEnum.WisselStroomKringDetector:
                    case OVIngreepMeldingTypeEnum.WisselDetector:
                    case OVIngreepMeldingTypeEnum.MassaPaarIn:
                    case OVIngreepMeldingTypeEnum.MassaPaarUit:
                        return MassaDetectoren;
                    case OVIngreepMeldingTypeEnum.KAR:
                    default:
                        return null;
                }
            }
        }

        public ObservableCollection<string> MassaDetectoren
        {
            get
            {
                if (_MassaDetectoren == null)
                {
                    _MassaDetectoren = new ObservableCollection<string>();
                }
                return _MassaDetectoren;
            }
        }

        public ObservableCollection<string> VecomDetectoren
        {
            get
            {
                if (_VecomDetectoren == null)
                {
                    _VecomDetectoren = new ObservableCollection<string>();
                }
                return _VecomDetectoren;
            }
        }

        public ObservableCollection<string> VecomIngangen
        {
            get
            {
                if (_VecomIngangen == null)
                {
                    _VecomIngangen = new ObservableCollection<string>();
                }
                return _VecomIngangen;
            }
        }

        #endregion // Properties

        private void RefreshDetectoren()
        {
            MassaDetectoren.Clear();
            VecomDetectoren.Clear();
            VecomIngangen.Clear();
            if (DataAccess.TLCGenControllerDataProvider.Default.Controller == null) return;

            foreach (var d in DataAccess.TLCGenControllerDataProvider.Default.Controller.Fasen.
                SelectMany(x => x.Detectoren))
            {
                switch (d.Type)
                {
                    case DetectorTypeEnum.Kop:
                    case DetectorTypeEnum.Lang:
                    case DetectorTypeEnum.Verweg:
                        MassaDetectoren.Add(d.Naam);
                        break;
                    case DetectorTypeEnum.VecomDetector:
                        VecomDetectoren.Add(d.Naam);
                        break;
                    case DetectorTypeEnum.VecomIngang:
                        VecomIngangen.Add(d.Naam);
                        break;
                }
            }
            foreach (var d in DataAccess.TLCGenControllerDataProvider.Default.Controller.Detectoren)
            {
                switch (d.Type)
                {
                    case DetectorTypeEnum.Kop:
                    case DetectorTypeEnum.Lang:
                    case DetectorTypeEnum.Verweg:
                        MassaDetectoren.Add(d.Naam);
                        break;
                    case DetectorTypeEnum.VecomDetector:
                        VecomDetectoren.Add(d.Naam);
                        break;
                    case DetectorTypeEnum.VecomIngang:
                        VecomIngangen.Add(d.Naam);
                        break;
                }
            }
        }

        #region IViewModelWithItem

        public object GetItem()
        {
            return OVIngreepMelding;
        }

        #endregion // IViewModelWithItem

        #region TLCGen Messaging

        private void OnNameChanged(NameChangedMessage msg)
        {
            RefreshDetectoren();
            RaisePropertyChanged("");
        }

        private void OnDetectorenChanged(DetectorenChangedMessage msg)
        {
            RefreshDetectoren();
            RaisePropertyChanged("");
        }

        #endregion // TLCGen Messaging

        #region Constructor

        public OVIngreepMeldingViewModel(OVIngreepMeldingModel oVIngreepMelding)
        {
            OVIngreepMelding = oVIngreepMelding;

            MessengerInstance.Register<DetectorenChangedMessage>(this, OnDetectorenChanged);
            MessengerInstance.Register<NameChangedMessage>(this, OnNameChanged);

            RefreshDetectoren();
        }

        #endregion Constructor
    }
}
