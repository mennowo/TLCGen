using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class RichtingGevoeligVerlengViewModel : ViewModelBase, IComparable
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
                OnMonitoredPropertyChanged("FaseCyclus");
            }
        }

        public string VanDetector
        {
            get { return _RichtingGevoeligVerleng.VanDetector; }
            set
            {
                _RichtingGevoeligVerleng.VanDetector = value;
                OnMonitoredPropertyChanged("VanDetector");
            }
        }

        public string NaarDetector
        {
            get { return _RichtingGevoeligVerleng.NaarDetector; }
            set
            {
                _RichtingGevoeligVerleng.NaarDetector = value;
                OnMonitoredPropertyChanged("NaarDetector");
            }
        }

        public int MaxTijdsVerschil
        {
            get { return _RichtingGevoeligVerleng.MaxTijdsVerschil; }
            set
            {
                _RichtingGevoeligVerleng.MaxTijdsVerschil = value;
                OnMonitoredPropertyChanged("MaxTijdsVerschil");
            }
        }

        public int VerlengTijd
        {
            get { return _RichtingGevoeligVerleng.VerlengTijd; }
            set
            {
                _RichtingGevoeligVerleng.VerlengTijd = value;
                OnMonitoredPropertyChanged("VerlengTijd");
            }
        }

        public int HiaatTijd
        {
            get { return _RichtingGevoeligVerleng.HiaatTijd; }
            set
            {
                _RichtingGevoeligVerleng.HiaatTijd = value;
                OnMonitoredPropertyChanged("HiaatTijd");
            }
        }

        public RichtingGevoeligVerlengenTypeEnum TypeVerlengen
        {
            get { return _RichtingGevoeligVerleng.TypeVerlengen; }
            set
            {
                _RichtingGevoeligVerleng.TypeVerlengen = value;
                OnMonitoredPropertyChanged("TypeVerlengen");
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

        #region Constructor

        public RichtingGevoeligVerlengViewModel(RichtingGevoeligVerlengModel rgv)
        {
            _RichtingGevoeligVerleng = rgv;
        }

        #endregion // Constructor
    }
}
