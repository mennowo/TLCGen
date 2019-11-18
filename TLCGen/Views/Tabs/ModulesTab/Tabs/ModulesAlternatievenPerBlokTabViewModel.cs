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

        private int _numberOfModules;

        #endregion // Fields

        #region Properties

        public override ControllerModel Controller
        {
            get => base.Controller;
            set
            {
                base.Controller = value;
                if (_Controller != null && _Controller.AlternatievenPerBlokData != null)
                {
                    AlternatievenPerBlok = new ObservableCollectionAroundList<FaseCyclusAlternatiefPerBlokViewModel, FaseCyclusAlternatiefPerBlokModel>(_Controller.AlternatievenPerBlokData.AlternatievenPerBlok);
                    NumberOfModules = _Controller.Data.MultiModuleReeksen ? _Controller.MultiModuleMolens.Max(x => x.Modules.Count) : _Controller.ModuleMolen.Modules.Count;
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

        public bool ToepassenAlternatievenPerBlok
        {
            get => _Controller.AlternatievenPerBlokData != null && _Controller.AlternatievenPerBlokData.ToepassenAlternatievenPerBlok;
            set
            {
                if (_Controller.AlternatievenPerBlokData != null)
                {
                    _Controller.AlternatievenPerBlokData.ToepassenAlternatievenPerBlok = value;
                    AlternatievenPerBlok.RemoveAll();
                    if (value)
                    {
                        foreach (var fc in Controller.Fasen)
                        {
                            AlternatievenPerBlok.Add(
                                new FaseCyclusAlternatiefPerBlokViewModel(
                                    new FaseCyclusAlternatiefPerBlokModel { FaseCyclus = fc.Naam }));
                        }
                        for (int i = 0; i < _numberOfModules; i++)
                        {
                            foreach (var fc in AlternatievenPerBlok)
                            {
                                fc.BitWiseBlokAlternatief |= (1 << i);
                            }
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

        public int NumberOfModules
        {
            get => _numberOfModules;
            private set
            {
                if(_numberOfModules < value)
                {
                    for (int i = _numberOfModules; i < value; i++)
                    {
                        foreach (var fc in AlternatievenPerBlok)
                        {
                            fc.BitWiseBlokAlternatief |= (1 << i);
                        }
                    }
                }
                else if (_numberOfModules > value)
                {
                    for (int i = value; i < _numberOfModules; i++)
                    {
                        foreach (var fc in AlternatievenPerBlok)
                        {
                            fc.BitWiseBlokAlternatief &= ~(1 << i);
                        }
                    }
                }
                _numberOfModules = value;
                RaisePropertyChanged();
            }
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

        private void OnModulesChanged(ModulesChangedMessage obj)
        {
            NumberOfModules = _Controller.Data.MultiModuleReeksen ? _Controller.MultiModuleMolens.Max(x => x.Modules.Count) : _Controller.ModuleMolen.Modules.Count;
        }

        #endregion // TLCGen Events

        #region Constructor

        public ModulesAlternatievenPerBlokTabViewModel() : base()
        {
            MessengerInstance.Register(this, new Action<FasenChangedMessage>(OnFasenChanged));
            MessengerInstance.Register(this, new Action<NameChangedMessage>(OnNameChanged));
            MessengerInstance.Register(this, new Action<FasenSortedMessage>(OnFasenSorted));
            MessengerInstance.Register(this, new Action<ModulesChangedMessage>(OnModulesChanged));
        }

        #endregion // Constructor
    }
}
