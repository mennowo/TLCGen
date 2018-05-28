using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using TLCGen.DataAccess;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
	public class VoorstartViewModel : ViewModelBase
	{
        #region Fields

        private VoorstartModel _voorstart;
        
        #endregion // Fields

        #region Properties

		public int VoorstartTijd
		{
			get => _voorstart.VoorstartTijd;
			set
			{
				_voorstart.VoorstartTijd = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
		}

		public int VoorstartOntruimingstijd
		{
			get => _voorstart.VoorstartOntruimingstijd;
			set
			{
				_voorstart.VoorstartOntruimingstijd = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
		}

        #endregion // Properties

        #region Private methods

        #endregion // Private methods

        #region Collection changed

		#endregion // Collection changed

        #region TLCGen Events
		
        #endregion // TLCGen Events

        #region Constructor

        public VoorstartViewModel(VoorstartModel vs)
        {
            _voorstart = vs;
        }

        #endregion // Constructor
	}
}