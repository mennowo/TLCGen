using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Messaging.Messages;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class OVIngreepSGInstellingenLijstViewModel : ViewModelBase
    {
        #region Fields

        private OVDataModel _OVData;
        private ControllerModel _Controller;
        private ObservableCollection<OVIngreepSignaalGroepParametersViewModel> _OVIngreepSGParameters; 

        #endregion // Fields

        #region Properties

        public ObservableCollection<OVIngreepSignaalGroepParametersViewModel> OVIngreepSGParameters
        {
            get
            {
                if (_OVIngreepSGParameters == null)
                {
                    _OVIngreepSGParameters = new ObservableCollection<OVIngreepSignaalGroepParametersViewModel>();
                }
                return _OVIngreepSGParameters;
            }
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
                foreach (FaseCyclusModel fcm in message.AddedFasen)
                {
                    AddFase(fcm.Naam);
                }
                foreach (FaseCyclusModel fcm in message.RemovedFasen)
                {
                    RemoveFase(fcm.Naam);
                }
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

        #endregion TLCGen events

        #region Collection changed

        private void OVIngreepSGParameters_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (OVIngreepSignaalGroepParametersViewModel prms in e.NewItems)
                {
                    _OVData.OVIngreepSignaalGroepParameters.Add(prms.Parameters);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (OVIngreepSignaalGroepParametersViewModel prms in e.OldItems)
                {
                    _OVData.OVIngreepSignaalGroepParameters.Remove(prms.Parameters);
                }
            }
        }

        #endregion

        #region Constructor

        public OVIngreepSGInstellingenLijstViewModel(ControllerModel controller)
        {
            _OVData = controller.OVData;
            _Controller = controller;

            foreach (OVIngreepSignaalGroepParametersModel prms in _OVData.OVIngreepSignaalGroepParameters)
            {
                OVIngreepSGParameters.Add(new OVIngreepSignaalGroepParametersViewModel(prms));
            }

            OVIngreepSGParameters.CollectionChanged += OVIngreepSGParameters_CollectionChanged;

            Messenger.Default.Register(this, new Action<FasenChangedMessage>(OnFasenChanged));
            Messenger.Default.Register(this, new Action<ControllerHasOVChangedMessage>(OnControllerHasOVChanged));
        }

        #endregion // Constructor
    }
}
