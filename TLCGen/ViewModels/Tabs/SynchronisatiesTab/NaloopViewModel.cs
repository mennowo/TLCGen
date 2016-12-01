using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Messaging;
using TLCGen.Messaging.Requests;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class NaloopViewModel : ViewModelBase
    {
        #region Fields

        private NaloopModel _Naloop;
        private ObservableCollection<NaloopTijdModel> _Tijden;
        private ObservableCollection<NaloopDetectorModel> _Detectoren;
        private bool _DetectieAfhankelijkPossible;

        #endregion // Fields

        #region Properties

        public NaloopViewModel(NaloopModel nm)
        {
            _Naloop = nm;
            SetNaloopTijden();
        }

        public NaloopTypeEnum Type
        {
            get { return _Naloop.Type; }
            set
            {
                if (value != _Naloop.Type)
                {
                    _Naloop.Type = value;
                    SetNaloopTijden();
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
            get { return _Naloop.DetectieAfhankelijk; }
            set
            {
                _Naloop.DetectieAfhankelijk = value;
                SetNaloopTijden();
                OnMonitoredPropertyChanged("DetectieAfhankelijk");
            }
        }

        public ObservableCollection<NaloopTijdModel> Tijden
        {
            get
            {
                if (_Tijden == null)
                    _Tijden = new ObservableCollection<NaloopTijdModel>();
                return _Tijden;
            }
        }

        public ObservableCollection<NaloopDetectorModel> Detectoren
        {
            get
            {
                if (_Detectoren == null)
                    _Detectoren = new ObservableCollection<NaloopDetectorModel>();
                return _Detectoren;
            }
        }

        #endregion // Properties

        #region Private methods

        private void SetNaloopTijden()
        {
            List<NaloopTijdModel> _oldtijden = _Naloop.Tijden;
            _Naloop.Tijden = new List<NaloopTijdModel>();
            switch (_Naloop.Type)
            {
                case NaloopTypeEnum.StartGroen:
                    _Naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.StartGroen });
                    if (_Naloop.DetectieAfhankelijk)
                    {
                        _Naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.StartGroenDetectie });
                    }
                    break;
                case NaloopTypeEnum.EindeGroen:
                    _Naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.VastGroen });
                    if (_Naloop.DetectieAfhankelijk)
                    {
                        _Naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.VastGroenDetectie });
                    }
                    _Naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.EindeGroen });
                    if (_Naloop.DetectieAfhankelijk)
                    {
                        _Naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.EindeGroenDetectie });
                    }
                    break;
                case NaloopTypeEnum.CyclischVerlengGroen:
                    _Naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.VastGroen });
                    if (_Naloop.DetectieAfhankelijk)
                    {
                        _Naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.VastGroenDetectie });
                    }
                    _Naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.EindeVerlengGroen });
                    if (_Naloop.DetectieAfhankelijk)
                    {
                        _Naloop.Tijden.Add(new NaloopTijdModel() { Type = NaloopTijdTypeEnum.EindeVerlengGroenDetectie });
                    }
                    break;
            }
            foreach(NaloopTijdModel tmo in _oldtijden)
            {
                foreach(NaloopTijdModel tmn in _Naloop.Tijden)
                {
                    if(tmo.Type == tmn.Type)
                    {
                        tmn.Waarde = tmo.Waarde;
                    }
                }
            }
            _Tijden = new ObservableCollection<NaloopTijdModel>(_Naloop.Tijden);
            OnPropertyChanged("Tijden");
        }

        #endregion // Private methods
    }
}
