using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 5, type: TabItemTypeEnum.ModulesTab)]
    public class ModulesAlternatievenPerBlokTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        AlternatievenPerBlokModel _Specials;

        #endregion // Fields

        #region Properties

        public override ControllerModel Controller
        {
            get => base.Controller;

            set
            {
                base.Controller = value;
                Specials = value?.AlternatievenPerBlokData;
                if (_Controller != null && Specials != null)
                {
                    if (ToepassenAlternatievenPerBlok)
                    {
                        foreach (var fc in Controller.Fasen)
                        {
                            if (AlternatievenPerBlok.All(x => x.FaseCyclus != fc.Naam))
                            {
                                AlternatievenPerBlok.Add(
                                    new FaseCyclusAlternatiefPerBlokViewModel(
                                        new FaseCyclusAlternatiefPerBlokModel { FaseCyclus = fc.Naam, BitWiseBlokAlternatief = 511 }));
                            }
                        }
                        var rems = new List<FaseCyclusAlternatiefPerBlokViewModel>();
                        foreach (var fc in AlternatievenPerBlok)
                        {
                            if (Controller.Fasen.All(x => x.Naam != fc.FaseCyclus))
                            {
                                rems.Add(fc);
                            }
                        }
                        foreach (var sg in rems)
                        {
                            AlternatievenPerBlok.Remove(sg);
                        }
                        AlternatievenPerBlok.BubbleSort();
                    }
                    else
                    {
                        AlternatievenPerBlok.RemoveAll();
                    }
                    RaisePropertyChanged("");
                }
            }
        }

        public AlternatievenPerBlokModel Specials
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
                    AlternatievenPerBlok.RemoveAll();
                    if (value)
                    {
                        foreach (var fc in Controller.Fasen)
                        {
                            AlternatievenPerBlok.Add(
                                new FaseCyclusAlternatiefPerBlokViewModel(
                                    new FaseCyclusAlternatiefPerBlokModel { FaseCyclus = fc.Naam, BitWiseBlokAlternatief = 511 }));
                        }
                    }
                    RaisePropertyChanged<object>("ToepassenAlternatievenPerBlok", true);
                }
            }
        }

        public ObservableCollectionAroundList<FaseCyclusAlternatiefPerBlokViewModel, FaseCyclusAlternatiefPerBlokModel> AlternatievenPerBlok
        {
            get;
            private set;
        }

        public override string DisplayName => "Alternatieven per blok";

        #endregion // Properties

        #region Commands

        #endregion // Commands

        #region Command Functionality

        #endregion // Command Functionality

        #region Public Methods

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
                            new FaseCyclusAlternatiefPerBlokModel { FaseCyclus = fc.Naam, BitWiseBlokAlternatief = 1023 }));
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
            AlternatievenPerBlok.BubbleSort();
        }

        private void OnNameChanged(NameChangedMessage message)
        {
            AlternatievenPerBlok.BubbleSort();
        }

        private void OnFasenSorted(FasenSortedMessage message)
        {
            AlternatievenPerBlok.BubbleSort();
        }

        #endregion // TLCGen Events

        #region Constructor

        public ModulesAlternatievenPerBlokTabViewModel() : base()
        {
            MessengerInstance.Register(this, new Action<FasenChangedMessage>(OnFasenChanged));
            MessengerInstance.Register(this, new Action<NameChangedMessage>(OnNameChanged));
            MessengerInstance.Register(this, new Action<FasenSortedMessage>(OnFasenSorted));
        }

        #endregion // Constructor
    }
}
