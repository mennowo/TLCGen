﻿using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class RichtingGevoeligeAanvraagViewModel : ObservableObjectEx, IComparable, IViewModelWithItem
    {
        #region Fields

        private RichtingGevoeligeAanvraagModel _RichtingGevoeligeAanvraag;
        private ObservableCollection<string> _Detectoren;

        #endregion // Fields

        #region Properties

        public RichtingGevoeligeAanvraagModel RichtingGevoeligeAanvraag => _RichtingGevoeligeAanvraag;

        public ObservableCollection<string> Detectoren
        {
            get
            {
                if( _Detectoren == null)
                {
                    _Detectoren = new ObservableCollection<string>();
                }
                return _Detectoren;
            }
        }

        public string FaseCyclus
        {
            get => _RichtingGevoeligeAanvraag.FaseCyclus;
            set
            {
                _RichtingGevoeligeAanvraag.FaseCyclus = value;
                OnPropertyChanged(nameof(FaseCyclus), broadcast: true);
            }
        }

        public string VanDetector
        {
            get => _RichtingGevoeligeAanvraag.VanDetector;
            set
            {
                _RichtingGevoeligeAanvraag.VanDetector = value;
                OnPropertyChanged(nameof(VanDetector), broadcast: true);
            }
        }

        public string NaarDetector
        {
            get => _RichtingGevoeligeAanvraag.NaarDetector;
            set
            {
                _RichtingGevoeligeAanvraag.NaarDetector = value;
                OnPropertyChanged(nameof(NaarDetector), broadcast: true);
            }
        }

        public int MaxTijdsVerschil
        {
            get => _RichtingGevoeligeAanvraag.MaxTijdsVerschil;
            set
            {
                _RichtingGevoeligeAanvraag.MaxTijdsVerschil = value;
                OnPropertyChanged(nameof(MaxTijdsVerschil), broadcast: true);
            }
        }

        public bool ResetAanvraag
        {
            get => _RichtingGevoeligeAanvraag.ResetAanvraag;
            set
            {
                _RichtingGevoeligeAanvraag.ResetAanvraag = value;
                OnPropertyChanged(nameof(ResetAanvraag), broadcast: true);
            }
        }

        public AltijdAanUitEnum AltijdAanUit
        {
            get => _RichtingGevoeligeAanvraag.AltijdAanUit;
            set
            {
                _RichtingGevoeligeAanvraag.AltijdAanUit = value;
                OnPropertyChanged(nameof(AltijdAanUit), broadcast: true);
            }
        }

        public int ResetAanvraagTijdsduur
        {
            get => _RichtingGevoeligeAanvraag.ResetAanvraagTijdsduur;
            set
            {
                _RichtingGevoeligeAanvraag.ResetAanvraagTijdsduur = value;
                OnPropertyChanged(nameof(ResetAanvraagTijdsduur), broadcast: true);
            }
        }

        #endregion // Properties

        #region IComparable

        public int CompareTo(object obj)
        {
            if(obj is RichtingGevoeligeAanvraagModel)
            {
                var comp = obj as RichtingGevoeligeAanvraagModel;
                if(this.FaseCyclus == comp.FaseCyclus)
                {
                    return this.VanDetector.CompareTo(comp.VanDetector);
                }
                else
                {
                    return this.FaseCyclus.CompareTo(comp.FaseCyclus);
                }
            }
            else
            {
                return 0;
            }
        }

        #endregion // IComparable

        #region IViewModelWithItem  

        public object GetItem()
        {
            return RichtingGevoeligeAanvraag;
        }
        
        #endregion // IViewModelWithItem

        #region Constructor

        public RichtingGevoeligeAanvraagViewModel(RichtingGevoeligeAanvraagModel rga)
        {
            _RichtingGevoeligeAanvraag = rga;
        }


        #endregion // Constructor
    }
}
