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
                RaisePropertyChanged();
            }
        }

        public bool Inmelding
        {
            get => OVIngreepMelding.Inmelding;
            set
            {
                OVIngreepMelding.Inmelding = value;
                MessengerInstance.Send(new OVIngreepMeldingChangedMessage(OVIngreepMelding.FaseCyclus, Type));
                RaisePropertyChanged();
            }
        }

        public bool Uitmelding
        {
            get => OVIngreepMelding.Uitmelding;
            set
            {
                OVIngreepMelding.Uitmelding = value;
                MessengerInstance.Send(new OVIngreepMeldingChangedMessage(OVIngreepMelding.FaseCyclus, Type));
                RaisePropertyChanged();
            }
        }

        public int? InmeldingHiaattijd
        {
            get => OVIngreepMelding.InmeldingHiaattijd;
            set
            {
                OVIngreepMelding.InmeldingHiaattijd = value;
                RaisePropertyChanged();
            }
        }

        public string RelatedInput
        {
            get => OVIngreepMelding.RelatedInput;
            set
            {
                OVIngreepMelding.RelatedInput = value;
                RaisePropertyChanged();
            }
        }

        public bool HasRelatedInput => Type != OVIngreepMeldingTypeEnum.KAR && Type != OVIngreepMeldingTypeEnum.VECOM;

        #endregion // IViewModelWithItem

        #region Constructor

        public OVIngreepMeldingViewModel(OVIngreepMeldingModel oVIngreepMelding)
        {
            OVIngreepMelding = oVIngreepMelding;
        }

        #endregion Constructor
    }
}
