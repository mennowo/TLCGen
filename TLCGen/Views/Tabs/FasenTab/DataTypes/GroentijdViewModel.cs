using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class GroentijdViewModel : ObservableObjectEx, IComparable, IViewModelWithItem
    {
        #region Fields
        
        private GroentijdModel _Groentijd;

        #endregion // Fields

        #region Properties

        public GroentijdModel Groentijd => _Groentijd;

        public string FaseCyclus
        {
            get => _Groentijd.FaseCyclus;
            set => _Groentijd.FaseCyclus = value;
        }

        public int? Waarde
        {
            get => _Groentijd.Waarde;
            set
            {
                _Groentijd.Waarde = value;
                OnPropertyChanged(nameof(Waarde), broadcast: true);
                WeakReferenceMessenger.Default.Send(new GroentijdChangedMessage());
            }
        }

        #endregion // Properties

        #region Collection Changed

        #endregion // Collection Changed

        #region Public methods

        #endregion // Public methods

        #region IComparable

        public int CompareTo(object obj)
        {
	        if (!(obj is GroentijdViewModel mgvm))
                throw new InvalidCastException();
            else
            {
                var myFase = FaseCyclus;
                var hisFase = mgvm.FaseCyclus;
                if (myFase.Length < hisFase.Length) myFase = myFase.PadLeft(hisFase.Length, '0');
                else if (hisFase.Length < myFase.Length) hisFase = hisFase.PadLeft(myFase.Length, '0');
                var i = myFase.CompareTo(hisFase);
                return i;
            }
        }

        #endregion // IComparable

        #region IViewModelWithItem

        public object GetItem()
        {
            return _Groentijd;
        }

        #endregion // IViewModelWithItem

        #region Constructor

        public GroentijdViewModel(GroentijdModel mgm)
        {
            _Groentijd = mgm;
        }

        #endregion // Constructor
    }
}