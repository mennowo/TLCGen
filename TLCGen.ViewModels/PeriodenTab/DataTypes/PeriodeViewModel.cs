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
using TLCGen.Helpers;

namespace TLCGen.ViewModels
{
    public class PeriodeViewModel : ViewModelBase, IViewModelWithItem, IComparable
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
                if (!string.IsNullOrWhiteSpace(value) && NameSyntaxChecker.IsValidName(value))
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

        public string Commentaar
        {
            get { return _Periode.Commentaar; }
            set
            {
                _Periode.Commentaar = value;
                OnMonitoredPropertyChanged("Commentaar");
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
                    DefaultsProvider.Default.SetDefaultsOnModel(_Periode, Type.ToString(), null, false);
                    string name = _Periode.Naam;
                    string newname = _Periode.Naam;
                    _Periode.Naam = "";
                    var message = new IsElementIdentifierUniqueRequest(newname, ElementIdentifierType.Naam);
                    Messenger.Default.Send(message);
                    int i = 0;
                    while (!(message.Handled && message.IsUnique))
                    {
                        newname = name + (i++).ToString();
                        message = new IsElementIdentifierUniqueRequest(newname, ElementIdentifierType.Naam);
                        Messenger.Default.Send(message);
                    }
                    _Periode.Naam = newname;
                }
                OnMonitoredPropertyChanged(null);
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
                OnMonitoredPropertyChanged("StartTijdAsText");
            }
        }

        public TimeSpan EindTijd
        {
            get { return _Periode.EindTijd; }
            set
            {
                _Periode.EindTijd = value;
                OnMonitoredPropertyChanged("EindTijd");
                OnMonitoredPropertyChanged("EindTijdAsText");
            }
        }

        public string StartTijdAsText
        {
            get
            {
                int hours = StartTijd.Hours;
                if (StartTijd.Days == 1)
                {
                    hours = 24;
                }
                return hours.ToString("00") + ":" + StartTijd.Minutes.ToString("00");
            }
        }

        public string EindTijdAsText
        {
            get
            {
                int hours = EindTijd.Hours;
                if (EindTijd.Days == 1)
                {
                    hours = 24;
                }
                return hours.ToString("00") + ":" + EindTijd.Minutes.ToString("00");
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

        #region IViewModelWithItem

        public object GetItem()
        {
            return Periode;
        }

        #endregion // IViewModelWithItem


        #region IComparable

        public int CompareTo(object obj)
        {
            var per = obj as PeriodeViewModel;
            if (per == null)
            {
                throw new InvalidCastException();
            }
            else
            {
                if (this.Type == PeriodeTypeEnum.Groentijden && per.Type != PeriodeTypeEnum.Groentijden)
                {
                    return 1;
                }
                else if (this.Type != PeriodeTypeEnum.Groentijden && per.Type == PeriodeTypeEnum.Groentijden)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }

        #endregion // IComparable

        #region Constructor

        public PeriodeViewModel(PeriodeModel periode)
        {
            _Periode = periode;
        }

        #endregion // Constructor
    }
}
