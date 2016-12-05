using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models.Enumerations;
using TLCGen.Models;
using TLCGen.DataAccess;
using TLCGen.Settings;
using TLCGen.Messaging;
using TLCGen.Messaging.Requests;
using TLCGen.Messaging.Messages;
using GalaSoft.MvvmLight.Messaging;

namespace TLCGen.ViewModels
{
    public class PeriodeViewModel : ViewModelBase
    {
        #region Fields

        private PeriodeModel _Periode;

        #endregion // Fields

        #region Properties

        public PeriodeModel Periode
        {
            get { return _Periode; }
        }
        
        public string Naam
        {
            get { return _Periode.Naam; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    var message = new IsElementIdentifierUniqueRequest(value, ElementIdentifierType.Naam);
                    Messenger.Default.Send(message);
                    if (message.Handled && message.IsUnique)
                    {
                        _Periode.Naam = value;
                    }
                }
                OnMonitoredPropertyChanged("Naam");
            }
        }

        public PeriodeTypeEnum Type
        {
            get { return _Periode.Type; }
            set
            {
                _Periode.Type = value;
                if(value != PeriodeTypeEnum.Groentijden)
                {
                    GroentijdenSet = null;
                }
                OnMonitoredPropertyChanged("Type");
                OnMonitoredPropertyChanged("IsPeriodeForGroentijdenSet");
            }
        }

        public PeriodeDagCodeEnum DagCode
        {
            get { return _Periode.DagCode; }
            set
            {
                _Periode.DagCode = value;
                OnMonitoredPropertyChanged("DagCode");
            }
        }

        public TimeSpan StartTijd
        {
            get { return _Periode.StartTijd; }
            set
            {
                _Periode.StartTijd = value;
                OnMonitoredPropertyChanged("StartTijd");
            }
        }

        public TimeSpan EindTijd
        {
            get { return _Periode.EindTijd; }
            set
            {
                _Periode.EindTijd = value;
                OnMonitoredPropertyChanged("EindTijd");
            }
        }

        public string GroentijdenSet
        {
            get { return _Periode.GroentijdenSet; }
            set
            {
                _Periode.GroentijdenSet = value;
                OnMonitoredPropertyChanged("GroentijdenSet");
            }
        }

        public bool IsPeriodeForGroentijdenSet
        {
            get
            {
                return Type == PeriodeTypeEnum.Groentijden;
            }
        }

        #endregion // Properties

        #region Constructor

        public PeriodeViewModel(PeriodeModel periode)
        {
            _Periode = periode;
        }

        #endregion // Constructor
    }
}
