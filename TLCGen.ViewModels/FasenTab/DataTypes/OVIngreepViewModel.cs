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
    public class OVIngreepLijnNummer
    {
        public string Nummer { get; set; }

        public OVIngreepLijnNummer(string num)
        {
            Nummer = num;
        }
    }

    public class OVIngreepViewModel : ViewModelBase
    {
        #region Fields

        private OVIngreepModel _OVIngreep;
        private ObservableCollection<OVIngreepLijnNummer> _LijnNummers;

        #endregion // Fields

        #region Properties

        public OVIngreepModel OVIngreep
        {
            get { return _OVIngreep; }
            set
            {
                _OVIngreep = value;
            }
        }

        public ObservableCollection<OVIngreepLijnNummer> LijnNummers
        {
            get
            {
                if(_LijnNummers == null)
                {
                    _LijnNummers = new ObservableCollection<OVIngreepLijnNummer>();
                }
                return _LijnNummers;
            }
        }

        public bool KAR
        {
            get { return _OVIngreep.KAR; }
            set
            {
                _OVIngreep.KAR = value;
                OnMonitoredPropertyChanged("KAR");
            }
        }

        public bool Vecom
        {
            get { return _OVIngreep.Vecom; }
            set
            {
                _OVIngreep.Vecom = value;
                OnMonitoredPropertyChanged("Vecom");
            }
        }

        public bool MassaDetectie
        {
            get { return _OVIngreep.MassaDetectie; }
            set
            {
                _OVIngreep.MassaDetectie = value;
                OnMonitoredPropertyChanged("MassaDetectie");
            }
        }

        public OVIngreepVoertuigTypeEnum Type
        {
            get { return _OVIngreep.Type; }
            set
            {
                _OVIngreep.Type = value;
                OnMonitoredPropertyChanged("Type");
            }
        }

        public bool AlleLijnen
        {
            get { return _OVIngreep.AlleLijnen; }
            set
            {
                _OVIngreep.AlleLijnen = value;
                OnMonitoredPropertyChanged("AlleLijnen");
            }
        }

        public int RijTijdOngehinderd
        {
            get { return _OVIngreep.RijTijdOngehinderd; }
            set
            {
                _OVIngreep.RijTijdOngehinderd = value;
                OnMonitoredPropertyChanged("RijTijdOngehinderd");
            }
        }

        public int RijTijdBeperktgehinderd
        {
            get { return _OVIngreep.RijTijdBeperktgehinderd; }
            set
            {
                _OVIngreep.RijTijdBeperktgehinderd = value;
                OnMonitoredPropertyChanged("RijTijdBeperktgehinderd");
            }
        }

        public int RijTijdGehinderd
        {
            get { return _OVIngreep.RijTijdGehinderd; }
            set
            {
                _OVIngreep.RijTijdGehinderd = value;
                OnMonitoredPropertyChanged("RijTijdGehinderd");
            }
        }

        public int OnderMaximum
        {
            get { return _OVIngreep.OnderMaximum; }
            set
            {
                _OVIngreep.OnderMaximum = value;
                OnMonitoredPropertyChanged("OnderMaximum");
            }
        }

        public int GroenBewaking
        {
            get { return _OVIngreep.GroenBewaking; }
            set
            {
                _OVIngreep.GroenBewaking = value;
                OnMonitoredPropertyChanged("GroenBewaking");
            }
        }

        #endregion // Properties

        #region Collection changed

        private void LijnNummers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (OVIngreepLijnNummer num in e.NewItems)
                {
                    _OVIngreep.LijnNummers.Add(num.Nummer);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (OVIngreepLijnNummer num in e.OldItems)
                {
                    _OVIngreep.LijnNummers.Remove(num.Nummer);
                }
            }
        }

        #endregion // Collection changed

        #region Constructor

        public OVIngreepViewModel(OVIngreepModel ovingreep)
        {
            _OVIngreep = ovingreep;

            foreach(string s in _OVIngreep.LijnNummers)
            {
                LijnNummers.Add(new OVIngreepLijnNummer(s));
            }

            LijnNummers.CollectionChanged += LijnNummers_CollectionChanged;
        }

        #endregion // Constructor
    }
}
