using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class RichtingGevoeligVerlengViewModel : ViewModelBase, IComparable, IViewModelWithItem
    {
        #region Fields

        private RichtingGevoeligVerlengModel _RichtingGevoeligVerleng;
        private ObservableCollection<string> _Detectoren;

        #endregion // Fields

        #region Properties

        public RichtingGevoeligVerlengModel RichtingGevoeligVerleng
        {
            get { return _RichtingGevoeligVerleng; }
        }

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
            get { return _RichtingGevoeligVerleng.FaseCyclus; }
            set
            {
                _RichtingGevoeligVerleng.FaseCyclus = value;
                RaisePropertyChanged<object>("FaseCyclus", broadcast: true);
            }
        }

        public string VanDetector
        {
            get { return _RichtingGevoeligVerleng.VanDetector; }
            set
            {
                _RichtingGevoeligVerleng.VanDetector = value;
                RaisePropertyChanged<object>("VanDetector", broadcast: true);
            }
        }

        public string NaarDetector
        {
            get { return _RichtingGevoeligVerleng.NaarDetector; }
            set
            {
                _RichtingGevoeligVerleng.NaarDetector = value;
                RaisePropertyChanged<object>("NaarDetector", broadcast: true);
            }
        }

        public int MaxTijdsVerschil
        {
            get { return _RichtingGevoeligVerleng.MaxTijdsVerschil; }
            set
            {
                _RichtingGevoeligVerleng.MaxTijdsVerschil = value;
                RaisePropertyChanged<object>("MaxTijdsVerschil", broadcast: true);
            }
        }

        public int VerlengTijd
        {
            get { return _RichtingGevoeligVerleng.VerlengTijd; }
            set
            {
                _RichtingGevoeligVerleng.VerlengTijd = value;
                RaisePropertyChanged<object>("VerlengTijd", broadcast: true);
            }
        }

        public RichtingGevoeligVerlengenTypeEnum TypeVerlengen
        {
            get { return _RichtingGevoeligVerleng.TypeVerlengen; }
            set
            {
                _RichtingGevoeligVerleng.TypeVerlengen = value;
                RaisePropertyChanged<object>("TypeVerlengen", broadcast: true);
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
