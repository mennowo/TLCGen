using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Extensions;
using TLCGen.Messaging.Messages;
using TLCGen.Models;

namespace TLCGen.ModelManagement
{
    public class TLCGenModelManager : ITLCGenModelManager
    {
        #region Fields

        private static readonly object _Locker = new object();
        private static ITLCGenModelManager _Default;

        private IMessenger _MessengerInstance;

        #endregion // Fields

        #region Properties

        public static ITLCGenModelManager Default
        {
            get
            {
                if (_Default == null)
                {
                    lock (_Locker)
                    {
                        if (_Default == null)
                        {
                            _Default = new TLCGenModelManager();
                        }
                    }
                }
                return _Default;
            }
        }

        private IMessenger MessengerInstance
        {
            get { return _MessengerInstance; }
            set { _MessengerInstance = value; }
        }

        public ControllerModel Controller
        {
            get; set;
        }

        #endregion // Properties

        #region Public Methods

        public static void OverrideDefault(ITLCGenModelManager provider)
        {
            _Default = provider;
        }

        #endregion // Public Methods

        #region TLCGen Messaging

        public void OnFasenChanging(FasenChangingMessage message)
        {
            if (message.AddedFasen != null)
            {
                foreach (var fcm in message.AddedFasen)
                {
                    // PT Conflict prms
                    if (Controller.OVData.OVIngreepType != Models.Enumerations.OVIngreepTypeEnum.Geen)
                    {
                        var prms = new OVIngreepSignaalGroepParametersModel();
                        Settings.DefaultsProvider.Default.SetDefaultsOnModel(prms);
                        prms.FaseCyclus = fcm.Naam;
                        Controller.OVData.OVIngreepSignaalGroepParameters.Add(prms);
                    }

                    // Module settings
                    var fcmlm = new FaseCyclusModuleDataModel() { FaseCyclus = fcm.Naam };
                    Settings.DefaultsProvider.Default.SetDefaultsOnModel(fcmlm);
                    Controller.ModuleMolen.FasenModuleData.Add(fcmlm);
                }
            }
            if (message.RemovedFasen != null)
            {
                foreach (FaseCyclusModel fcm in message.RemovedFasen)
                {
                    // PT Conflict prms
                    if (Controller.OVData.OVIngreepType != Models.Enumerations.OVIngreepTypeEnum.Geen)
                    {
                        OVIngreepSignaalGroepParametersModel _prms = null;
                        foreach (var prms in Controller.OVData.OVIngreepSignaalGroepParameters)
                        {
                            if (prms.FaseCyclus == fcm.Naam)
                            {
                                _prms = prms;
                            }
                        }
                        if (_prms != null)
                        {
                            Controller.OVData.OVIngreepSignaalGroepParameters.Remove(_prms);
                        }
                    }

                    // Module settings
                    FaseCyclusModuleDataModel fcvm = null;
                    foreach(var _f in Controller.ModuleMolen.FasenModuleData)
                    {
                        if(fcm.Naam == _f.FaseCyclus)
                        {
                            fcvm = _f;
                        }
                    }
                    if (fcvm != null)
                    {
                        Controller.ModuleMolen.FasenModuleData.Remove(fcvm);
                    }
                }
            }

            // Sorting
            Controller.OVData.OVIngreepSignaalGroepParameters.BubbleSort();
            foreach (var set in Controller.GroentijdenSets)
            {
                set.Groentijden.BubbleSort();
            }
            Controller.ModuleMolen.FasenModuleData.BubbleSort();

            // Messaging
            MessengerInstance.Send(new FasenChangedMessage(message.AddedFasen, message.RemovedFasen));
        }

        #endregion // TLCGen Messaging

        #region Constructor

        public TLCGenModelManager(IMessenger messengerinstance = null)
        {
            if(messengerinstance == null)
            {
                MessengerInstance = Messenger.Default;
            }
            MessengerInstance.Register(this, new Action<FasenChangingMessage>(OnFasenChanging));
        }

        #endregion // Constructor
    }
}
