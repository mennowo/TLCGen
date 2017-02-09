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

namespace TLCGen.ViewModels
{
    public class OVIngreepSGInstellingenLijstViewModel : ViewModelBase
    {
        #region Fields

        private OVDataModel _OVData;
        private ControllerModel _Controller;

        #endregion // Fields

        #region Properties

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

        private void AddFase(string fasename)
        {
            var prms = new OVIngreepSignaalGroepParametersModel();
            prms.FaseCyclus = fasename;
            OVIngreepSGParameters.Add(new OVIngreepSignaalGroepParametersViewModel(prms));
        }

        private void RemoveFase(string fasedefine)
        {
            OVIngreepSignaalGroepParametersViewModel _prms = null;
            foreach (OVIngreepSignaalGroepParametersViewModel prms in OVIngreepSGParameters)
            {
                if (prms.FaseCyclus == fasedefine)
                {
                    _prms = prms;
                }
            }
            if (_prms != null)
            {
                OVIngreepSGParameters.Remove(_prms);
            }
        }

        #endregion // Private methods

        #region Public methods

        #endregion // Public methods

        #region TLCGen events

        private void OnFasenChanged(FasenChangedMessage message)
        {
            if (_Controller.Data.OVIngreep != Models.Enumerations.OVIngreepTypeEnum.Geen)
            {
                if (message.AddedFasen != null)
                {
                    foreach (FaseCyclusModel fcm in message.AddedFasen)
                    {
                        AddFase(fcm.Naam);
                    }
                }
                if (message.RemovedFasen != null)
                {
                    foreach (FaseCyclusModel fcm in message.RemovedFasen)
                    {
                        RemoveFase(fcm.Naam);
                    }
                }
                OVIngreepSGParameters.BubbleSort();
            }
        }

        private void OnControllerHasOVChanged(ControllerHasOVChangedMessage message)
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
                        prms.FaseCyclus = fcm.Naam;
                        OVIngreepSGParameters.Add(new OVIngreepSignaalGroepParametersViewModel(prms));
                    }
                    break;
            }
        }

        public void OnFasenSorted(FasenSortedMessage message)
        {
            if (_Controller.Data.OVIngreep != Models.Enumerations.OVIngreepTypeEnum.Geen)
            {
                OVIngreepSGParameters.BubbleSort();
                OVIngreepSGParameters.RebuildList();
            }
        }

        #endregion TLCGen events

        #region Constructor

        public OVIngreepSGInstellingenLijstViewModel(ControllerModel controller)
        {
            _OVData = controller.OVData;
            _Controller = controller;

            OVIngreepSGParameters = 
                new ObservableCollectionAroundList<OVIngreepSignaalGroepParametersViewModel, OVIngreepSignaalGroepParametersModel>(controller.OVData.OVIngreepSignaalGroepParameters);

            Messenger.Default.Register(this, new Action<FasenChangedMessage>(OnFasenChanged));
            Messenger.Default.Register(this, new Action<ControllerHasOVChangedMessage>(OnControllerHasOVChanged));
            Messenger.Default.Register(this, new Action<FasenSortedMessage>(OnFasenSorted));
        }

        #endregion // Constructor
    }
}
