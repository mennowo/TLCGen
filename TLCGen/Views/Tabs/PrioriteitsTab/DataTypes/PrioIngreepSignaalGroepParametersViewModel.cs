using GalaSoft.MvvmLight.Messaging;
using System;
using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class PrioIngreepSignaalGroepParametersViewModel : ViewModelBase, IComparable, IViewModelWithItem
    {
        #region Properties

        public PrioIngreepSignaalGroepParametersModel Parameters { get; }

        public string FaseCyclus
        {
            get { return Parameters.FaseCyclus; }
            set
            {
                Parameters.FaseCyclus = value;
                RaisePropertyChanged("FaseCyclus");
            }
        }

        public int AantalKerenNietAfkappen
        {
            get { return Parameters.AantalKerenNietAfkappen; }
            set
            {
                Parameters.AantalKerenNietAfkappen = value;
                SendTLCGenChangedMessage();
                RaisePropertyChanged<object>("AantalKerenNietAfkappen", broadcast: true);
            }
        }
        public int MinimumGroentijdConflictOVRealisatie
        {
            get { return Parameters.MinimumGroentijdConflictOVRealisatie; }
            set
            {
                Parameters.MinimumGroentijdConflictOVRealisatie = value;
                SendTLCGenChangedMessage();
                RaisePropertyChanged<object>("MinimumGroentijdConflictOVRealisatie", broadcast: true);
            }
        }

        public int PercMaxGroentijdConflictOVRealisatie
        {
            get { return Parameters.PercMaxGroentijdConflictOVRealisatie; }
            set
            {
                Parameters.PercMaxGroentijdConflictOVRealisatie = value;
                SendTLCGenChangedMessage();
                RaisePropertyChanged<object>("PercMaxGroentijdConflictOVRealisatie", broadcast: true);
            }
        }

        public int PercMaxGroentijdVoorTerugkomen
        {
            get { return Parameters.PercMaxGroentijdVoorTerugkomen; }
            set
            {
                Parameters.PercMaxGroentijdVoorTerugkomen = value;
                SendTLCGenChangedMessage();
                RaisePropertyChanged<object>("PercMaxGroentijdVoorTerugkomen", broadcast: true);
            }
        }

        public int OndergrensNaTerugkomen
        {
            get { return Parameters.OndergrensNaTerugkomen; }
            set
            {
                Parameters.OndergrensNaTerugkomen = value;
                SendTLCGenChangedMessage();
                RaisePropertyChanged<object>("OndergrensNaTerugkomen", broadcast: true);
            }
        }

        public int OphoogpercentageNaAfkappen
        {
            get { return Parameters.OphoogpercentageNaAfkappen; }
            set
            {
                Parameters.OphoogpercentageNaAfkappen = value;
                SendTLCGenChangedMessage();
                RaisePropertyChanged<object>("OphoogpercentageNaAfkappen", broadcast: true);
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
                    new Messaging.Messages.PrioIngreepSignaalGroepParametersChangedMessage(
                        (PrioIngreepSignaalGroepParametersModel)this.GetItem()));
        }

        #endregion // Private methods

        #region Public methods

        public void CopyValueNoMessaging(PrioIngreepSignaalGroepParametersModel sgprm)
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
            var they = obj as PrioIngreepSignaalGroepParametersViewModel;
            if (they == null) return 0;
            else return FaseCyclus.CompareTo(they.FaseCyclus);
        }

        #endregion // IComparable

        #region Constructor

        public PrioIngreepSignaalGroepParametersViewModel(PrioIngreepSignaalGroepParametersModel prms)
        {
            Parameters = prms;
        }

        #endregion // Constructor
    }
}
