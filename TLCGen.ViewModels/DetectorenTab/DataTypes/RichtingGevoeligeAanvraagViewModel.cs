using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class RichtingGevoeligeAanvraagViewModel : ViewModelBase, IComparable, IViewModelWithItem
    {
        #region Fields

        private RichtingGevoeligeAanvraagModel _RichtingGevoeligeAanvraag;
        private ObservableCollection<string> _Detectoren;

        #endregion // Fields

        #region Properties

        public RichtingGevoeligeAanvraagModel RichtingGevoeligeAanvraag
        {
            get { return _RichtingGevoeligeAanvraag; }
        }

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
            get { return _RichtingGevoeligeAanvraag.FaseCyclus; }
            set
            {
                _RichtingGevoeligeAanvraag.FaseCyclus = value;
                OnMonitoredPropertyChanged("FaseCyclus");
            }
        }

        public string VanDetector
        {
            get { return _RichtingGevoeligeAanvraag.VanDetector; }
            set
            {
                _RichtingGevoeligeAanvraag.VanDetector = value;
                OnMonitoredPropertyChanged("VanDetector");
            }
        }

        public string NaarDetector
        {
            get { return _RichtingGevoeligeAanvraag.NaarDetector; }
            set
            {
                _RichtingGevoeligeAanvraag.NaarDetector = value;
                OnMonitoredPropertyChanged("NaarDetector");
            }
        }

        public int MaxTijdsVerschil
        {
            get { return _RichtingGevoeligeAanvraag.MaxTijdsVerschil; }
            set
            {
                _RichtingGevoeligeAanvraag.MaxTijdsVerschil = value;
                OnMonitoredPropertyChanged("MaxTijdsVerschil");
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
