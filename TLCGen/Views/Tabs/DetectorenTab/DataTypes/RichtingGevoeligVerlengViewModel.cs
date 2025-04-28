using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class RichtingGevoeligVerlengViewModel : ObservableObjectEx, IComparable, IViewModelWithItem
    {
        #region Fields

        private RichtingGevoeligVerlengModel _RichtingGevoeligVerleng;
        private ObservableCollection<string> _Detectoren;

        #endregion // Fields

        #region Properties

        public RichtingGevoeligVerlengModel RichtingGevoeligVerleng => _RichtingGevoeligVerleng;

        public ObservableCollection<string> Detectoren
        {
            get
            {
                if (_Detectoren == null)
                {
                    _Detectoren = new ObservableCollection<string>();
                }
                return _Detectoren;
            }
        }

        public string FaseCyclus
        {
            get => _RichtingGevoeligVerleng.FaseCyclus;
            set
            {
                _RichtingGevoeligVerleng.FaseCyclus = value;
                OnPropertyChanged(nameof(FaseCyclus), broadcast: true);
            }
        }

        public string VanDetector
        {
            get => _RichtingGevoeligVerleng.VanDetector;
            set
            {
                _RichtingGevoeligVerleng.VanDetector = value;
                OnPropertyChanged(nameof(VanDetector), broadcast: true);
            }
        }

        public string NaarDetector
        {
            get => _RichtingGevoeligVerleng.NaarDetector;
            set
            {
                _RichtingGevoeligVerleng.NaarDetector = value;
                OnPropertyChanged(nameof(NaarDetector), broadcast: true);
            }
        }

        public int MaxTijdsVerschil
        {
            get => _RichtingGevoeligVerleng.MaxTijdsVerschil;
            set
            {
                _RichtingGevoeligVerleng.MaxTijdsVerschil = value;
                OnPropertyChanged(nameof(MaxTijdsVerschil), broadcast: true);
            }
        }

        public int VerlengTijd
        {
            get => _RichtingGevoeligVerleng.VerlengTijd;
            set
            {
                _RichtingGevoeligVerleng.VerlengTijd = value;
                OnPropertyChanged(nameof(VerlengTijd), broadcast: true);
            }
        }

        public RichtingGevoeligVerlengenTypeEnum TypeVerlengen
        {
            get => _RichtingGevoeligVerleng.TypeVerlengen;
            set
            {
                _RichtingGevoeligVerleng.TypeVerlengen = value;
                OnPropertyChanged(nameof(TypeVerlengen), broadcast: true);
            }
        }

        public AltijdAanUitEnum AltijdAanUit
        {
            get => _RichtingGevoeligVerleng.AltijdAanUit;
            set
            {
                _RichtingGevoeligVerleng.AltijdAanUit = value;
                OnPropertyChanged(nameof(AltijdAanUit), broadcast: true);
            }
        }

        #endregion // Properties

        #region IComparable

        public int CompareTo(object obj)
        {
            if (obj is RichtingGevoeligeAanvraagModel)
            {
                var comp = obj as RichtingGevoeligeAanvraagModel;
                if (this.FaseCyclus == comp.FaseCyclus)
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
            return RichtingGevoeligVerleng;
        }

        #endregion // IViewModelWithItem

        #region Constructor

        public RichtingGevoeligVerlengViewModel(RichtingGevoeligVerlengModel rgv)
        {
            _RichtingGevoeligVerleng = rgv;
        }

        #endregion // Constructor
    }
}
