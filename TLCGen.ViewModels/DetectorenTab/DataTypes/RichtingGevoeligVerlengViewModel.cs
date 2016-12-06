using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class RichtingGevoeligVerlengViewModel : ViewModelBase, IComparable
    {
        #region Fields

        private RichtingGevoeligVerlengModel _RichtingGevoeligVerlng;
        private ObservableCollection<string> _Detectoren;

        #endregion // Fields

        #region Properties

        public RichtingGevoeligVerlengModel RichtingGevoeligVerleng
        {
            get { return _RichtingGevoeligVerlng; }
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
            get { return _RichtingGevoeligVerlng.FaseCyclus; }
            set
            {
                _RichtingGevoeligVerlng.FaseCyclus = value;
                OnMonitoredPropertyChanged("FaseCyclus");
            }
        }

        public string VanDetector
        {
            get { return _RichtingGevoeligVerlng.VanDetector; }
            set
            {
                _RichtingGevoeligVerlng.VanDetector = value;
                OnMonitoredPropertyChanged("VanDetector");
            }
        }

        public string NaarDetector
        {
            get { return _RichtingGevoeligVerlng.NaarDetector; }
            set
            {
                _RichtingGevoeligVerlng.NaarDetector = value;
                OnMonitoredPropertyChanged("NaarDetector");
            }
        }

        public int MaxTijdsVerschil
        {
            get { return _RichtingGevoeligVerlng.MaxTijdsVerschil; }
            set
            {
                _RichtingGevoeligVerlng.MaxTijdsVerschil = value;
                OnMonitoredPropertyChanged("MaxTijdsVerschil");
            }
        }

        public int VerlengTijd
        {
            get { return _RichtingGevoeligVerlng.VerlengTijd; }
            set
            {
                _RichtingGevoeligVerlng.VerlengTijd = value;
                OnMonitoredPropertyChanged("VerlengTijd");
            }
        }

        public int HiaatTijd
        {
            get { return _RichtingGevoeligVerlng.HiaatTijd; }
            set
            {
                _RichtingGevoeligVerlng.HiaatTijd = value;
                OnMonitoredPropertyChanged("HiaatTijd");
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
            _RichtingGevoeligVerlng = rgv;
        }

        #endregion // Constructor
    }
}
