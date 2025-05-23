﻿using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Models.Enumerations;
using TLCGen.Models;
using TLCGen.Settings;
using TLCGen.Messaging.Messages;
using TLCGen.Helpers;
using TLCGen.Extensions;
using TLCGen.ModelManagement;

namespace TLCGen.ViewModels
{
    public class PeriodeViewModel : ObservableObjectEx, IViewModelWithItem, IComparable
    {
        #region Fields

        private PeriodeModel _Periode;

        #endregion // Fields

        #region Properties

        public PeriodeModel Periode => _Periode;

        public string Naam
        {
            get => _Periode.Naam;
            set
            {
	            var oldName = _Periode.Naam;
                if (!string.IsNullOrWhiteSpace(value) && NameSyntaxChecker.IsValidCName(value))
                {
                    if (TLCGenModelManager.Default.IsElementIdentifierUnique(TLCGenObjectTypeEnum.Periode, value))
                    {
                        _Periode.Naam = value;
						WeakReferenceMessengerEx.Default.Send(new NameChangingMessage(TLCGenObjectTypeEnum.Periode, oldName, value));
                    }
                }
                OnPropertyChanged(nameof(Naam), broadcast: true);
            }
        }

        public string Commentaar
        {
            get => _Periode.Commentaar;
	        set
            {
                _Periode.Commentaar = value;
                OnPropertyChanged(nameof(Commentaar), broadcast: true);
            }
        }
        
        public string TypeAsString
        {
            get => Type.GetDescription();
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
                else if (value == PeriodeTypeEnum.StarRegelen.GetDescription())
                {
                    Type = PeriodeTypeEnum.StarRegelen;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Unknown period type: " + value);
                }
                OnPropertyChanged(string.Empty);
                OnPropertyChanged(nameof(TypeAsString), broadcast: true);
            }
        }

        public PeriodeTypeEnum Type
        {
            get => _Periode.Type;
            set
            {
                _Periode.Type = value;
                if(value != PeriodeTypeEnum.Groentijden)
                {
                    GroentijdenSet = null;
                    DefaultsProvider.Default.SetDefaultsOnModel(_Periode, _Periode.Type.ToString(), null, false);
                    var name = _Periode.Naam;
                    var newname = _Periode.Naam;
                    _Periode.Naam = "";
                    var i = 0;
                    while (!TLCGenModelManager.Default.IsElementIdentifierUnique(TLCGenObjectTypeEnum.Periode, newname))
                    {
                        newname = name + (i++);
                    }
                    _Periode.Naam = newname;
                }
                OnPropertyChanged(string.Empty);
                OnPropertyChanged(nameof(Type), broadcast: true);
                WeakReferenceMessengerEx.Default.Send(new PeriodenChangedMessage());
            }
        }

        public PeriodeDagCodeEnum DagCode
        {
            get => _Periode.DagCode;
            set
            {
                _Periode.DagCode = value;
                OnPropertyChanged(nameof(DagCode), broadcast: true);
            }
        }
        
        public TimeSpan StartTijd
        {
            get => _Periode.StartTijd;
            set
            {
                _Periode.StartTijd = value;
                OnPropertyChanged(nameof(StartTijd), broadcast: true);
                OnPropertyChanged(nameof(StartTijdAsText));
            }
        }

        public TimeSpan EindTijd
        {
            get => _Periode.EindTijd;
            set
            {
                _Periode.EindTijd = value;
                OnPropertyChanged(nameof(EindTijd), broadcast: true);
                OnPropertyChanged(nameof(EindTijdAsText));
            }
        }

        public string StartTijdAsText
        {
            get
            {
                var hours = StartTijd.Hours;
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
                var hours = EindTijd.Hours;
                if (EindTijd.Days == 1)
                {
                    hours = 24;
                }
                return hours.ToString("00") + ":" + EindTijd.Minutes.ToString("00");
            }
        }

        public string GroentijdenSet
        {
            get => _Periode.GroentijdenSet;
            set
            {
                if(value != null)
                {
                    _Periode.GroentijdenSet = value;
                }
                OnPropertyChanged(nameof(GroentijdenSet), broadcast: true);
            }
        }

        public bool IsPeriodeForGroentijdenSet => Type == PeriodeTypeEnum.Groentijden;

        public bool IsUitgangOverig => Type == PeriodeTypeEnum.Overig;

        public bool GeenUitgangPerOverig
        {
            get => _Periode.GeenUitgangPerOverig;
            set
            {
                _Periode.GeenUitgangPerOverig = value;
                OnPropertyChanged(nameof(GeenUitgangPerOverig), broadcast: true);
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
