using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Extensions;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Helpers;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    public class OVIngreepSGInstellingenLijstViewModel : ViewModelBase
    {
        #region Fields

        private OVDataModel _OVData;
        private ControllerModel _Controller;

        #endregion // Fields

        #region Properties

        public ControllerModel Controller
        {
            get { return _Controller; }
            set
            {
                _Controller = value;
                if (_Controller != null)
                {
                    _OVData = Controller.OVData;
                    OVIngreepSGParameters =
                        new ObservableCollectionAroundList<OVIngreepSignaalGroepParametersViewModel, OVIngreepSignaalGroepParametersModel>(Controller.OVData.OVIngreepSignaalGroepParameters);
                }
                else
                {
                    OVIngreepSGParameters = null;
                }
                OnPropertyChanged("OVIngreepSGParameters");
            }
        }

        public ObservableCollectionAroundList<OVIngreepSignaalGroepParametersViewModel, OVIngreepSignaalGroepParametersModel> OVIngreepSGParameters
        {
            get;
            private set;
        }

        #endregion // Properties

        #region Commands

        #endregion // Commands

        #region Command functionality

        #endregion // Command functionality

        #region Private methods

        #endregion // Private methods

        #region Public methods

        #endregion // Public methods

        #region TLCGen events

        public void OnFasenChanged(FasenChangedMessage message)
        {
            if (_Controller.OVData.OVIngreepType != Models.Enumerations.OVIngreepTypeEnum.Geen)
            {
                OVIngreepSGParameters.Rebuild();
            }
        }

#warning This would probably be better done right there where the "has OV" prop is set
        public void OnControllerHasOVChanged(ControllerHasOVChangedMessage message)
        {
            switch (message.Type)
            {
                case Models.Enumerations.OVIngreepTypeEnum.Geen:
                    OVIngreepSGParameters.Clear();
                    break;
                case Models.Enumerations.OVIngreepTypeEnum.Uitgebreid:
                    foreach(FaseCyclusModel fcm in _Controller.Fasen)
                    {
                        var prms = new OVIngreepSignaalGroepParametersModel();
                        DefaultsProvider.Default.SetDefaultsOnModel(prms);
                        prms.FaseCyclus = fcm.Naam;
                        OVIngreepSGParameters.Add(new OVIngreepSignaalGroepParametersViewModel(prms));
                    }
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

        public OVIngreepSGInstellingenLijstViewModel()
        {
            Messenger.Default.Register(this, new Action<FasenChangedMessage>(OnFasenChanged));
            Messenger.Default.Register(this, new Action<ControllerHasOVChangedMessage>(OnControllerHasOVChanged));
            Messenger.Default.Register(this, new Action<FasenSortedMessage>(OnFasenSorted));
            Messenger.Default.Register(this, new Action<OVIngreepSignaalGroepParametersChangedMessage>(OnOVIngreepSignaalGroepParametersChanged));
        }

        #endregion // Constructor
    }
}
