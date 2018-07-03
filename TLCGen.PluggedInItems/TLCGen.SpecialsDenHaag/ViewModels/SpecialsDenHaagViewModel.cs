using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Linq;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.SpecialsDenHaag.Models;

namespace TLCGen.SpecialsDenHaag
{
    internal class SpecialsDenHaagViewModel : ViewModelBase
    {
        #region Fields

        SpecialsDenHaagModel _Specials;

        #endregion // Fields

        #region Properties

        public SpecialsDenHaagModel Specials
        {
            get => _Specials;
            set
            {
                _Specials = value;
                if (_Specials != null)
                {
                    AlternatievenPerBlok = new ObservableCollectionAroundList<FaseCyclusAlternatiefPerBlokViewModel, FaseCyclusAlternatiefPerBlokModel>(_Specials.AlternatievenPerBlok);
                }
                RaisePropertyChanged("");
            }
        }

        public bool ToepassenAlternatievenPerBlok
        {
            get => _Specials != null && _Specials.ToepassenAlternatievenPerBlok;
            set
            {
                if (_Specials != null)
                {
                    _Specials.ToepassenAlternatievenPerBlok = value;
                    RaisePropertyChanged<object>("ToepassenAlternatievenPerBlok", true);
                }
            }
        }

        public ObservableCollectionAroundList<FaseCyclusAlternatiefPerBlokViewModel, FaseCyclusAlternatiefPerBlokModel> AlternatievenPerBlok
        {
            get;
            private set;
        }

        #endregion // Properties

        #region Commands

        #endregion // Commands

        #region Command Functionality

        #endregion // Command Functionality

        #region Public Methods

        public void UpdateTLCGenMessaging()
        {
            Messenger.Default.Register(this, new Action<FasenChangedMessage>(OnFasenChanged));
            Messenger.Default.Register(this, new Action<NameChangedMessage>(OnNameChanged));
            Messenger.Default.Register(this, new Action<FasenSortedMessage>(OnFasenSorted));
        }

        #endregion // Public Methods

        #region Private Methods

        #endregion // Private Methods

        #region TLCGen Events

        private void OnFasenChanged(FasenChangedMessage message)
        {
            if (message.AddedFasen?.Count > 0)
            {
                foreach (var fc in message.AddedFasen)
                {
                    AlternatievenPerBlok.Add(
                        new FaseCyclusAlternatiefPerBlokViewModel(
                            new FaseCyclusAlternatiefPerBlokModel {FaseCyclus = fc.Naam, BitWiseBlokAlternatief = 1023 }));
                }
            }
            if (message.RemovedFasen?.Count > 0)
            {
                foreach (var fc in message.RemovedFasen)
                {
                    var rfc = AlternatievenPerBlok.FirstOrDefault(x => x.FaseCyclus == fc.Naam);
                    if (rfc != null)
                    {
                        AlternatievenPerBlok.Remove(rfc);
                    }
                }
            }
            AlternatievenPerBlok.Rebuild();
        }

        private void OnNameChanged(NameChangedMessage message)
        {
            var fc = AlternatievenPerBlok.FirstOrDefault(x => x.FaseCyclus == message.OldName);
            if (fc != null)
            {
                fc.FaseCyclus = message.NewName;
            }
        }

        private void OnFasenSorted(FasenSortedMessage message)
        {
            AlternatievenPerBlok.BubbleSort();
        }

        #endregion // TLCGen Events

        #region Constructor

        public SpecialsDenHaagViewModel(IMessenger messenger = null) : base(messenger)
        {

        }

        #endregion // Constructor
    }
}
