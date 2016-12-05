using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Messaging;
using TLCGen.Messaging.Messages;
using TLCGen.Messaging.Requests;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class MeeaanvraagViewModel : ViewModelBase
    {
        #region Fields

        private MeeaanvraagModel _Meeaanvraag;
        private ObservableCollection<MeeaanvraagDetectorModel> _Detectoren;
        private bool _DetectieAfhankelijkPossible;

        #endregion // Fields

        #region Properties

        public MeeaanvraagTypeEnum Type
        {
            get { return _Meeaanvraag.Type; }
            set
            {
                if (value != _Meeaanvraag.Type)
                {
                    _Meeaanvraag.Type = value;
                    OnMonitoredPropertyChanged("Type");
                }
            }
        }

        public bool DetectieAfhankelijkPossible
        {
            get { return _DetectieAfhankelijkPossible; }
            set
            {
                _DetectieAfhankelijkPossible = value;
                OnPropertyChanged("DetectieAfhankelijkPossible");
            }
        }

        public bool DetectieAfhankelijk
        {
            get { return _Meeaanvraag.DetectieAfhankelijk; }
            set
            {
                _Meeaanvraag.DetectieAfhankelijk = value;
                OnMonitoredPropertyChanged("DetectieAfhankelijk");
            }
        }

        public ObservableCollection<MeeaanvraagDetectorModel> Detectoren
        {
            get
            {
                if (_Detectoren == null)
                    _Detectoren = new ObservableCollection<MeeaanvraagDetectorModel>();
                return _Detectoren;
            }
        }

        #endregion // Properties

        #region Collection changed

        private void Detectoren_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (MeeaanvraagDetectorModel d in e.NewItems)
                {
                    _Meeaanvraag.Detectoren.Add(d);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (MeeaanvraagDetectorModel d in e.OldItems)
                {
                    _Meeaanvraag.Detectoren.Remove(d);
                }
            }
            Messenger.Default.Send(new ControllerDataChangedMessage());
        }

        #endregion // Collection changed

        #region Constructor

        public MeeaanvraagViewModel(MeeaanvraagModel mm)
        {
            _Meeaanvraag = mm;
            foreach(MeeaanvraagDetectorModel d in _Meeaanvraag.Detectoren)
            {
                Detectoren.Add(d);
            }
            Detectoren.CollectionChanged += Detectoren_CollectionChanged;
        }

        #endregion // Constructor

    }
}
