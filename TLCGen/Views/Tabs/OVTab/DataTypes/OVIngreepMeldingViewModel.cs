using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
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

        public string RelatedInput
        {
            get => OVIngreepMelding.RelatedInput;
            set
            {
                OVIngreepMelding.RelatedInput = value;
                RaisePropertyChanged<object>(nameof(RelatedInput), broadcast: true);
            }
        }

        public bool HasRelatedInput => Type != OVIngreepMeldingTypeEnum.KAR && Type != OVIngreepMeldingTypeEnum.VECOM;
        public bool HasInmelding => Type != OVIngreepMeldingTypeEnum.WisselDetector;

        #endregion // IViewModelWithItem

        #region Constructor

        public OVIngreepMeldingViewModel(OVIngreepMeldingModel oVIngreepMelding)
        {
            OVIngreepMelding = oVIngreepMelding;
        }

        #endregion Constructor
    }
}
