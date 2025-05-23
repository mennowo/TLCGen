﻿using System;
using System.Linq;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Plugins;
using TLCGen.Settings;


namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 2, type: TabItemTypeEnum.PrioriteitTab)]
    public class PrioriteitSignaalGroepInstellingenTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        #endregion // Fields

        #region Properties

        public ObservableCollectionAroundList<PrioIngreepSignaalGroepParametersViewModel, PrioIngreepSignaalGroepParametersModel> PrioriteitIngreepSGParameters
        {
            get;
            private set;
        }

        public bool PrioriteitIngreepSGParametersHard
        {
            get => _Controller.PrioData.PrioIngreepSignaalGroepParametersHard;
            set
            {
                _Controller.PrioData.PrioIngreepSignaalGroepParametersHard = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        #endregion // Properties

        #region TabItem Overrides

        public override string DisplayName => "Conflicten";

        public override bool CanBeEnabled()
        {
            return _Controller?.PrioData?.PrioIngreepType != Models.Enumerations.PrioIngreepTypeEnum.Geen;
        }

        public override void OnSelected()
        {

        }

        public override ControllerModel Controller
        {
            get => base.Controller;
            set
            {
                base.Controller = value;
                if (base.Controller != null)
                {
                    PrioriteitIngreepSGParameters =
                        new ObservableCollectionAroundList<PrioIngreepSignaalGroepParametersViewModel, PrioIngreepSignaalGroepParametersModel>(Controller.PrioData.PrioIngreepSignaalGroepParameters);

                    if (_Controller.PrioData.PrioIngreepType != Models.Enumerations.PrioIngreepTypeEnum.Geen)
                    {
                        foreach (var sg in Controller.Fasen)
                        {
                            if (Controller.PrioData.PrioIngreepSignaalGroepParameters.All(x => x.FaseCyclus != sg.Naam))
                            {
                                PrioriteitIngreepSGParameters.Add(
                                    new PrioIngreepSignaalGroepParametersViewModel(
                                        new PrioIngreepSignaalGroepParametersModel
                                        {
                                            FaseCyclus = sg.Naam
                                        }));

                            }
                        }
                    }
                }
                else
                {
                    PrioriteitIngreepSGParameters = null;
                }
                OnPropertyChanged(nameof(PrioriteitIngreepSGParameters));
            }
        }

        #endregion // TabItem Overrides

        #region Commands

        #endregion // Commands

        #region Command functionality

        #endregion // Command functionality

        #region Private methods

        #endregion // Private methods

        #region Public methods

        #endregion // Public methods

        #region TLCGen events

        private void OnFasenChanged(object sender, FasenChangedMessage message)
        {
            if (_Controller.PrioData.PrioIngreepType != Models.Enumerations.PrioIngreepTypeEnum.Geen)
            {
                PrioriteitIngreepSGParameters.Rebuild();
            }
        }

        public void OnControllerHasOVChanged(object sender, ControllerHasOVChangedMessage message)
        {
            switch (message.Type)
            {
                case Models.Enumerations.PrioIngreepTypeEnum.Geen:
                    PrioriteitIngreepSGParameters.RemoveAll();
                    break;
                default:
                    foreach (var fcm in _Controller.Fasen)
                    {
                        if (PrioriteitIngreepSGParameters.Any(x => x.FaseCyclus == fcm.Naam))
                        {
                            continue;
                        }
                        var prms = new PrioIngreepSignaalGroepParametersModel();
                        DefaultsProvider.Default.SetDefaultsOnModel(prms);
                        prms.FaseCyclus = fcm.Naam;
                        PrioriteitIngreepSGParameters.Add(new PrioIngreepSignaalGroepParametersViewModel(prms));
                    }
                    PrioriteitIngreepSGParameters.BubbleSort();
                    PrioriteitIngreepSGParameters.RebuildList();
                    break;
            }
        }

        public void OnFasenSorted(object sender, FasenSortedMessage message)
        {
            if (_Controller.PrioData.PrioIngreepType != Models.Enumerations.PrioIngreepTypeEnum.Geen)
            {
                PrioriteitIngreepSGParameters.BubbleSort();
                PrioriteitIngreepSGParameters.RebuildList();
            }
        }

        public void OnOVIngreepSignaalGroepParametersChanged(object sender, PrioIngreepSignaalGroepParametersChangedMessage message)
        {
            /* Set all options equal for signal groups that are synchronised */
            foreach (var gs in _Controller.InterSignaalGroep.Gelijkstarten)
            {
                if (gs.FaseVan == message.SignaalGroepParameters.FaseCyclus)
                {
                    foreach (var fcprmvm in PrioriteitIngreepSGParameters)
                    {
                        if (fcprmvm.FaseCyclus == gs.FaseNaar)
                        {
                            fcprmvm.CopyValueNoMessaging(message.SignaalGroepParameters);
                        }
                    }
                }
                if (gs.FaseNaar == message.SignaalGroepParameters.FaseCyclus)
                {
                    foreach (var fcprmvm in PrioriteitIngreepSGParameters)
                    {
                        if (fcprmvm.FaseCyclus == gs.FaseVan)
                        {
                            fcprmvm.CopyValueNoMessaging(message.SignaalGroepParameters);
                        }
                    }
                }
            }
        }

        #endregion TLCGen events

        #region Constructor

        public PrioriteitSignaalGroepInstellingenTabViewModel()
        {
            WeakReferenceMessengerEx.Default.Register<FasenChangedMessage>(this, OnFasenChanged);
            WeakReferenceMessengerEx.Default.Register<FasenSortedMessage>(this, OnFasenSorted);
            WeakReferenceMessengerEx.Default.Register<ControllerHasOVChangedMessage>(this, OnControllerHasOVChanged);
            WeakReferenceMessengerEx.Default.Register<PrioIngreepSignaalGroepParametersChangedMessage>(this, OnOVIngreepSignaalGroepParametersChanged);
        }

        #endregion // Constructor
    }
}
