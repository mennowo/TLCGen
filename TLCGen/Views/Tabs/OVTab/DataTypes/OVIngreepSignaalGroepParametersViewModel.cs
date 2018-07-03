using GalaSoft.MvvmLight.Messaging;
using System;
using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class OVIngreepSignaalGroepParametersViewModel : ViewModelBase, IComparable, IViewModelWithItem
    {
        #region Fields

        private OVIngreepSignaalGroepParametersModel _Parameters;

        #endregion // Fields

        #region Properties

        public OVIngreepSignaalGroepParametersModel Parameters
        {
            get { return _Parameters; }
        }

        public string FaseCyclus
        {
            get { return _Parameters.FaseCyclus; }
            set
            {
                _Parameters.FaseCyclus = value;
                RaisePropertyChanged("FaseCyclus");
            }
        }

        public int AantalKerenNietAfkappen
        {
            get { return _Parameters.AantalKerenNietAfkappen; }
            set
            {
                _Parameters.AantalKerenNietAfkappen = value;
                SendTLCGenChangedMessage();
                RaisePropertyChanged<object>("AantalKerenNietAfkappen", broadcast: true);
            }
        }
        public int MinimumGroentijdConflictOVRealisatie
        {
            get { return _Parameters.MinimumGroentijdConflictOVRealisatie; }
            set
            {
                _Parameters.MinimumGroentijdConflictOVRealisatie = value;
                SendTLCGenChangedMessage();
                RaisePropertyChanged<object>("MinimumGroentijdConflictOVRealisatie", broadcast: true);
            }
        }

        public int PercMaxGroentijdConflictOVRealisatie
        {
            get { return _Parameters.PercMaxGroentijdConflictOVRealisatie; }
            set
            {
                _Parameters.PercMaxGroentijdConflictOVRealisatie = value;
                SendTLCGenChangedMessage();
                RaisePropertyChanged<object>("PercMaxGroentijdConflictOVRealisatie", broadcast: true);
            }
        }

        public int PercMaxGroentijdVoorTerugkomen
        {
            get { return _Parameters.PercMaxGroentijdVoorTerugkomen; }
            set
            {
                _Parameters.PercMaxGroentijdVoorTerugkomen = value;
                SendTLCGenChangedMessage();
                RaisePropertyChanged<object>("PercMaxGroentijdVoorTerugkomen", broadcast: true);
            }
        }

        public int OndergrensNaTerugkomen
        {
            get { return _Parameters.OndergrensNaTerugkomen; }
            set
            {
                _Parameters.OndergrensNaTerugkomen = value;
                SendTLCGenChangedMessage();
                RaisePropertyChanged<object>("OndergrensNaTerugkomen", broadcast: true);
            }
        }

        public int OphoogpercentageNaAfkappen
        {
            get { return _Parameters.OphoogpercentageNaAfkappen; }
            set
            {
                _Parameters.OphoogpercentageNaAfkappen = value;
                SendTLCGenChangedMessage();
                RaisePropertyChanged<object>("OphoogpercentageNaAfkappen", broadcast: true);
            }
        }

        public int BlokkeertijdNaOVIngreep
        {
            get { return _Parameters.BlokkeertijdNaOVIngreep; }
            set
            {
                _Parameters.BlokkeertijdNaOVIngreep = value;
                SendTLCGenChangedMessage();
                RaisePropertyChanged<object>("BlokkeertijdNaOVIngreep", broadcast: true);
            }
        }

        #endregion // Properties

        #region Commands

        #endregion // Commands

        #region Command functionality

        #endregion // Command functionality

        #region Private methods

        private void SendTLCGenChangedMessage()
        {
            Messenger.Default.Send(
                    new Messaging.Messages.OVIngreepSignaalGroepParametersChangedMessage(
                        (OVIngreepSignaalGroepParametersModel)this.GetItem()));
        }

        #endregion // Private methods

        #region Public methods

        public void CopyValueNoMessaging(OVIngreepSignaalGroepParametersModel sgprm)
        {
            var t = sgprm.GetType();
            var props = t.GetProperties();
            foreach(var prop in props)
            {
                if(prop.PropertyType.IsValueType)
                {
                    prop.SetValue(this.GetItem(), prop.GetValue(sgprm));
                }
            }
            RaisePropertyChanged("");
        }

        #endregion // Public methods

        #region IViewModelWithItem

        public object GetItem()
        {
            return Parameters;
        }

        #endregion // IViewModelWithItem

        #region IComparable

        public int CompareTo(object obj)
        {
            var they = obj as OVIngreepSignaalGroepParametersViewModel;
            if (they == null) return 0;
            else return FaseCyclus.CompareTo(they.FaseCyclus);
        }

        #endregion // IComparable

        #region Constructor

        public OVIngreepSignaalGroepParametersViewModel(OVIngreepSignaalGroepParametersModel prms)
        {
            _Parameters = prms;
        }

        #endregion // Constructor
    }
}
