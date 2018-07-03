using GalaSoft.MvvmLight.Messaging;
using System;
using System.Linq;
using TLCGen.Extensions;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Helpers;
using TLCGen.Settings;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 2, type: TabItemTypeEnum.OVTab)]
    public class OVSignaalGroepInstellingenTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        #endregion // Fields

        #region Properties

        public ObservableCollectionAroundList<OVIngreepSignaalGroepParametersViewModel, OVIngreepSignaalGroepParametersModel> OVIngreepSGParameters
        {
            get;
            private set;
        }

        #endregion // Properties

        #region TabItem Overrides

        public override string DisplayName
        {
            get
            {
                return "Conflicten";
            }
        }

        public override bool CanBeEnabled()
        {
            return _Controller?.OVData?.OVIngreepType != Models.Enumerations.OVIngreepTypeEnum.Geen;
        }

        public override void OnSelected()
        {
            
        }

        public override ControllerModel Controller
        {
            get { return base.Controller; }
            set
            {
                base.Controller = value;
                if (base.Controller != null)
                {
                    OVIngreepSGParameters =
                        new ObservableCollectionAroundList<OVIngreepSignaalGroepParametersViewModel, OVIngreepSignaalGroepParametersModel>(Controller.OVData.OVIngreepSignaalGroepParameters);

					if (_Controller.OVData.OVIngreepType != Models.Enumerations.OVIngreepTypeEnum.Geen)
	                {
		                foreach (var sg in Controller.Fasen)
		                {
			                if (Controller.OVData.OVIngreepSignaalGroepParameters.All(x => x.FaseCyclus != sg.Naam))
			                {
				                OVIngreepSGParameters.Add(
									new OVIngreepSignaalGroepParametersViewModel(
										new OVIngreepSignaalGroepParametersModel
										{
											FaseCyclus = sg.Naam
										}));

							}
		                }
	                }
                }
                else
				{
                    OVIngreepSGParameters = null;
                }
                RaisePropertyChanged("OVIngreepSGParameters");
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

        private void OnFasenChanged(FasenChangedMessage message)
        {
            if (_Controller.OVData.OVIngreepType != Models.Enumerations.OVIngreepTypeEnum.Geen)
            {
				OVIngreepSGParameters.Rebuild();
            }
        }

        public void OnControllerHasOVChanged(ControllerHasOVChangedMessage message)
        {
            switch (message.Type)
            {
                case Models.Enumerations.OVIngreepTypeEnum.Geen:
                    OVIngreepSGParameters.RemoveAll();
                    break;
                case Models.Enumerations.OVIngreepTypeEnum.Uitgebreid:
                    foreach(var fcm in _Controller.Fasen)
                    {
	                    if (OVIngreepSGParameters.Any(x => x.FaseCyclus == fcm.Naam))
	                    {
		                    continue;
	                    }
                        var prms = new OVIngreepSignaalGroepParametersModel();
                        DefaultsProvider.Default.SetDefaultsOnModel(prms);
                        prms.FaseCyclus = fcm.Naam;
                        OVIngreepSGParameters.Add(new OVIngreepSignaalGroepParametersViewModel(prms));
                    }
	                OVIngreepSGParameters.BubbleSort();
	                OVIngreepSGParameters.RebuildList();
					break;
            }
        }

        public void OnFasenSorted(FasenSortedMessage message)
        {
            if (_Controller.OVData.OVIngreepType != Models.Enumerations.OVIngreepTypeEnum.Geen)
            {
                OVIngreepSGParameters.BubbleSort();
                OVIngreepSGParameters.RebuildList();
            }
        }

        public void OnOVIngreepSignaalGroepParametersChanged(OVIngreepSignaalGroepParametersChangedMessage message)
        {
            /* Set all options equal for signal groups that are synchronised */
            foreach(var gs in _Controller.InterSignaalGroep.Gelijkstarten)
            {
                if(gs.FaseVan == message.SignaalGroepParameters.FaseCyclus)
                {
                    foreach(var fcprmvm in OVIngreepSGParameters)
                    {
                        if(fcprmvm.FaseCyclus == gs.FaseNaar)
                        {
                            fcprmvm.CopyValueNoMessaging(message.SignaalGroepParameters);
                        }
                    }
                }
                if (gs.FaseNaar == message.SignaalGroepParameters.FaseCyclus)
                {
                    foreach (var fcprmvm in OVIngreepSGParameters)
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

        public OVSignaalGroepInstellingenTabViewModel()
        {
            Messenger.Default.Register(this, new Action<FasenChangedMessage>(OnFasenChanged));
            Messenger.Default.Register(this, new Action<FasenSortedMessage>(OnFasenSorted));
            Messenger.Default.Register(this, new Action<ControllerHasOVChangedMessage>(OnControllerHasOVChanged));
            Messenger.Default.Register(this, new Action<OVIngreepSignaalGroepParametersChangedMessage>(OnOVIngreepSignaalGroepParametersChanged));
        }

        #endregion // Constructor
    }
}
