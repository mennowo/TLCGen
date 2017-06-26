using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using TLCGen.Models.Enumerations;
using TLCGen.Models;
using TLCGen.DataAccess;
using TLCGen.Settings;
using TLCGen.Messaging;
using TLCGen.Messaging.Requests;
using TLCGen.Messaging.Messages;
using GalaSoft.MvvmLight.Messaging;
using TLCGen.Helpers;
using TLCGen.Extensions;

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
                RaisePropertyChanged<object>("Naam", broadcast: true);
            }
        }

        public string Commentaar
        {
            get { return _Periode.Commentaar; }
            set
            {
                _Periode.Commentaar = value;
                RaisePropertyChanged<object>("Commentaar", broadcast: true);
            }
        }
        
        public string TypeAsString
        {
            get { return Type.GetDescription(); }
            set
            {
                if(value == PeriodeTypeEnum.BellenActief.GetDescription())
                {
                    Type = PeriodeTypeEnum.BellenActief;
                }
                else if (value == PeriodeTypeEnum.BellenDimmen.GetDescription())
                {
                    Type = PeriodeTypeEnum.BellenDimmen;
                }
                else if (value == PeriodeTypeEnum.Overig.GetDescription())
                {
                    Type = PeriodeTypeEnum.Overig;
                }
                else if (value == PeriodeTypeEnum.RateltikkersAanvraag.GetDescription())
                {
                    Type = PeriodeTypeEnum.RateltikkersAanvraag;
                }
                else if (value == PeriodeTypeEnum.RateltikkersAltijd.GetDescription())
                {
                    Type = PeriodeTypeEnum.RateltikkersAltijd;
                }
                else if (value == PeriodeTypeEnum.RateltikkersDimmen.GetDescription())
                {
                    Type = PeriodeTypeEnum.RateltikkersDimmen;
                }
                else if (value == PeriodeTypeEnum.WaarschuwingsLichten.GetDescription())
                {
                    Type = PeriodeTypeEnum.WaarschuwingsLichten;
                }
                else
                {
                    throw new NotImplementedException("Unknown period type in PeriodeViewModel.cs line 100");
                }
                RaisePropertyChanged(string.Empty);
                RaisePropertyChanged<object>(nameof(TypeAsString), broadcast: true);
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
                RaisePropertyChanged(string.Empty);
                RaisePropertyChanged<object>(nameof(Type), broadcast: true);
            }
        }

        public PeriodeDagCodeEnum DagCode
        {
            get { return _Periode.DagCode; }
            set
            {
                _Periode.DagCode = value;
                RaisePropertyChanged<object>("DagCode", broadcast: true);
            }
        }
        
        public TimeSpan StartTijd
        {
            get { return _Periode.StartTijd; }
            set
            {
                _Periode.StartTijd = value;
                RaisePropertyChanged<object>("StartTijd", broadcast: true);
                RaisePropertyChanged<object>("StartTijdAsText", broadcast: true);
            }
        }

        public TimeSpan EindTijd
        {
            get { return _Periode.EindTijd; }
            set
            {
                _Periode.EindTijd = value;
                RaisePropertyChanged<object>("EindTijd", broadcast: true);
                RaisePropertyChanged<object>("EindTijdAsText", broadcast: true);
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
                RaisePropertyChanged<object>("GroentijdenSet", broadcast: true);
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
