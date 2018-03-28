using GalaSoft.MvvmLight;
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
        #endregion // Fields

        #region Properties

        public OVIngreepMeldingModel OVIngreepMelding { get; }

        #endregion // Properties

        #region IViewModelWithItem

        public object GetItem()
        {
            return OVIngreepMelding;
        }

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
                RaisePropertyChanged(nameof(CanHaveInput1));
                RaisePropertyChanged(nameof(CanHaveInput2));
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
                RaisePropertyChanged(nameof(CanHaveInput1));
                RaisePropertyChanged(nameof(CanHaveInput2));
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
                                            ((Inmelding || Uitmelding) &&
                                             (Type == OVIngreepMeldingTypeEnum.VerlosDetector ||
                                              Type == OVIngreepMeldingTypeEnum.WisselDetector ||
                                              Type == OVIngreepMeldingTypeEnum.WisselStroomKringDetector));

        public bool CanHaveRelatedInput2 => Inmelding && Type == OVIngreepMeldingTypeEnum.MassaPaarIn ||
                                            Uitmelding && Type == OVIngreepMeldingTypeEnum.MassaPaarUit ||
                                            Uitmelding && Type == OVIngreepMeldingTypeEnum.VECOM_io;


        public bool CanHaveInput1 => Inmelding && Type == OVIngreepMeldingTypeEnum.VECOM;

        public bool CanHaveInput2 => Uitmelding && Type == OVIngreepMeldingTypeEnum.VECOM;

        public string RelatedInput1
        {
            get => OVIngreepMelding.RelatedInput1;
            set
            {
                OVIngreepMelding.RelatedInput1 = value;
                RaisePropertyChanged<object>(nameof(RelatedInput1), broadcast: true);
            }
        }

        public string Input1
        {
            get => OVIngreepMelding.Input1;
            set
            {
                var message = new IsElementIdentifierUniqueRequest(value, ElementIdentifierType.Naam);
                MessengerInstance.Send(message);
                if (message.Handled && message.IsUnique)
                {
                    string oldname = OVIngreepMelding.Input1;

                    OVIngreepMelding.Input1 = value;

                    // Notify the messenger
                    MessengerInstance.Send(new NameChangedMessage(oldname, OVIngreepMelding.Input1));

                }
                RaisePropertyChanged<object>(nameof(Input1), broadcast: true);
            }
        }

        public string RelatedInput2
        {
            get => OVIngreepMelding.RelatedInput2;
            set
            {
                OVIngreepMelding.RelatedInput2 = value;
                RaisePropertyChanged<object>(nameof(RelatedInput2), broadcast: true);
            }
        }

        public string Input2
        {
            get => OVIngreepMelding.Input2;
            set
            {
                var message = new IsElementIdentifierUniqueRequest(value, ElementIdentifierType.Naam);
                MessengerInstance.Send(message);
                if (message.Handled && message.IsUnique)
                {
                    string oldname = OVIngreepMelding.Input2;

                    OVIngreepMelding.Input2 = value;

                    // Notify the messenger
                    MessengerInstance.Send(new NameChangedMessage(oldname, OVIngreepMelding.Input2));

                }
                RaisePropertyChanged<object>(nameof(Input2), broadcast: true);
            }
        }

        public bool HasRelatedSelectableInput1 => Type != OVIngreepMeldingTypeEnum.KAR && Type != OVIngreepMeldingTypeEnum.VECOM;

        public bool HasRelatedSelectableInput2 => Type == OVIngreepMeldingTypeEnum.MassaPaarIn || 
                                                  Type == OVIngreepMeldingTypeEnum.MassaPaarUit ||
                                                  Type == OVIngreepMeldingTypeEnum.VECOM_io;

        public bool HasTypableInput1 => Type == OVIngreepMeldingTypeEnum.VECOM;

        public bool HasTypableInput2 => Type == OVIngreepMeldingTypeEnum.VECOM;

        public bool HasInmelding => Type != OVIngreepMeldingTypeEnum.WisselDetector && Type != OVIngreepMeldingTypeEnum.MassaPaarUit;

        public bool HasUitmelding => Type != OVIngreepMeldingTypeEnum.MassaPaarIn;

        #endregion // IViewModelWithItem

        #region Constructor

        public OVIngreepMeldingViewModel(OVIngreepMeldingModel oVIngreepMelding)
        {
            OVIngreepMelding = oVIngreepMelding;
        }

        #endregion Constructor
    }
}
