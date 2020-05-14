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
            get => Parameters.FaseCyclus;
            set
            {
                Parameters.FaseCyclus = value;
                RaisePropertyChanged("FaseCyclus");
            }
        }

        public int AantalKerenNietAfkappen
        {
            get => Parameters.AantalKerenNietAfkappen;
            set
            {
                Parameters.AantalKerenNietAfkappen = value;
                SendTLCGenChangedMessage();
                RaisePropertyChanged<object>(nameof(AantalKerenNietAfkappen), broadcast: true);
            }
        }
        public int MinimumGroentijdConflictOVRealisatie
        {
            get => Parameters.MinimumGroentijdConflictOVRealisatie;
            set
            {
                Parameters.MinimumGroentijdConflictOVRealisatie = value;
                SendTLCGenChangedMessage();
                RaisePropertyChanged<object>(nameof(MinimumGroentijdConflictOVRealisatie), broadcast: true);
            }
        }

        public int PercMaxGroentijdConflictOVRealisatie
        {
            get => Parameters.PercMaxGroentijdConflictOVRealisatie;
            set
            {
                Parameters.PercMaxGroentijdConflictOVRealisatie = value;
                SendTLCGenChangedMessage();
                RaisePropertyChanged<object>(nameof(PercMaxGroentijdConflictOVRealisatie), broadcast: true);
            }
        }

        public int PercMaxGroentijdVoorTerugkomen
        {
            get => Parameters.PercMaxGroentijdVoorTerugkomen;
            set
            {
                Parameters.PercMaxGroentijdVoorTerugkomen = value;
                SendTLCGenChangedMessage();
                RaisePropertyChanged<object>(nameof(PercMaxGroentijdVoorTerugkomen), broadcast: true);
            }
        }

        public int OndergrensNaTerugkomen
        {
            get => Parameters.OndergrensNaTerugkomen;
            set
            {
                Parameters.OndergrensNaTerugkomen = value;
                SendTLCGenChangedMessage();
                RaisePropertyChanged<object>(nameof(OndergrensNaTerugkomen), broadcast: true);
            }
        }

        public int OphoogpercentageNaAfkappen
        {
            get => Parameters.OphoogpercentageNaAfkappen;
            set
            {
                Parameters.OphoogpercentageNaAfkappen = value;
                SendTLCGenChangedMessage();
                RaisePropertyChanged<object>(nameof(OphoogpercentageNaAfkappen), broadcast: true);
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
