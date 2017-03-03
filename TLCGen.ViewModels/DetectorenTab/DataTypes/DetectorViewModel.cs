using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models.Enumerations;
using TLCGen.Models;
using TLCGen.DataAccess;
using TLCGen.Settings;
using TLCGen.Messaging;
using TLCGen.Messaging.Requests;
using TLCGen.Messaging.Messages;
using GalaSoft.MvvmLight.Messaging;
using TLCGen.Helpers;

namespace TLCGen.ViewModels
{
    public class DetectorViewModel : ViewModelBase, IComparable
    {
        #region Fields

        private DetectorModel _Detector;
        private string _FaseCyclus;

        #endregion // Fields

        #region Properties

        public DetectorModel Detector
        {
            get { return _Detector; }
        }
        
        public string FaseCyclus
        {
            get { return _FaseCyclus; }
            set
            {
                _FaseCyclus = value;
            }
        }

        public string Naam
        {
            get { return _Detector.Naam; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && NameSyntaxChecker.IsValidName(value))
                {
                    var message = new IsElementIdentifierUniqueRequest(value, ElementIdentifierType.Naam);
                    Messenger.Default.Send(message);
                    if (message.Handled && message.IsUnique)
                    {
                        string oldname = _Detector.Naam;

                        _Detector.Naam = value;
                        
                        // Notify the messenger
                        Messenger.Default.Send(new NameChangedMessage(oldname, _Detector.Naam));
                    }
                }
                OnMonitoredPropertyChanged("Naam");
            }
        }

        public string VissimNaam
        {
            get { return _Detector.VissimNaam; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    var message = new IsElementIdentifierUniqueRequest(value, ElementIdentifierType.VissimNaam);
                    Messenger.Default.Send(message);
                    if (message.Handled && message.IsUnique)
                    {
                        _Detector.VissimNaam = value;
                    }
                }
                OnMonitoredPropertyChanged("VissimNaam");
            }
        }

        public DetectorTypeEnum Type
        {
            get { return _Detector.Type; }
            set
            {
                _Detector.Type = value;
                if(FaseCyclus != null && TLCGenControllerDataProvider.Default.Controller.Fasen.Where(x => x.Naam == FaseCyclus).Any())
                {
                    var fctype = TLCGenControllerDataProvider.Default.Controller.Fasen.Where(x => x.Naam == FaseCyclus).First().Type;
                    DefaultsProvider.Default.SetDefaultsOnModel(_Detector, Type.ToString(), fctype.ToString());
                }
                else
                {
                    DefaultsProvider.Default.SetDefaultsOnModel(_Detector, Type.ToString());
                }
                OnMonitoredPropertyChanged(null);
                Messenger.Default.Send(new FaseDetectorTypeChangedMessage(Naam, value));

#warning TODO also below...
                //_ControllerVM.SetAllSelectedElementsValue(this, "Type");
            }
        }

        public bool IsDrukknop
        {
            get
            {
                return Type == DetectorTypeEnum.KnopBinnen || Type == DetectorTypeEnum.KnopBuiten;
            }
        }

        public int? TDB
        {
            get { return _Detector.TDB; }
            set
            {
                if (value == null || value >= 0)
                    _Detector.TDB = value;
                OnMonitoredPropertyChanged("TDB");
                //_ControllerVM.SetAllSelectedElementsValue(this, "TDB");
            }
        }

        public int? TDH
        {
            get { return _Detector.TDH; }
            set
            {
                if (value == null || value >= 0)
                    _Detector.TDH = value;
                OnMonitoredPropertyChanged("TDH");
                //_ControllerVM.SetAllSelectedElementsValue(this, "TDH");
            }
        }

        public int? TOG
        {
            get { return _Detector.TOG; }
            set
            {
                if (value == null || value >= 0)
                    _Detector.TOG = value;
                OnMonitoredPropertyChanged("TOG");
                //_ControllerVM.SetAllSelectedElementsValue(this, "TOG");
            }
        }

        public int? TBG
        {
            get { return _Detector.TBG; }
            set
            {
                if (value == null || value >= 0)
                    _Detector.TBG = value;
                OnMonitoredPropertyChanged("TBG");
                //_ControllerVM.SetAllSelectedElementsValue(this, "TBG");
            }
        }

        public int? TFL
        {
            get { return _Detector.TFL; }
            set
            {
                if (value == null || value >= 0)
                    _Detector.TFL = value;
                OnMonitoredPropertyChanged("TFL");
                //_ControllerVM.SetAllSelectedElementsValue(this, "TBG");
            }
        }

        public int? CFL
        {
            get { return _Detector.CFL; }
            set
            {
                if (value == null || value >= 0)
                    _Detector.CFL = value;
                OnMonitoredPropertyChanged("CFL");
                //_ControllerVM.SetAllSelectedElementsValue(this, "TBG");
            }
        }
        public DetectorAanvraagTypeEnum Aanvraag
        {
            get { return _Detector.Aanvraag; }
            set
            {
                _Detector.Aanvraag = value;
                OnMonitoredPropertyChanged("Aanvraag");
                //_ControllerVM.SetAllSelectedElementsValue(this, "Aanvraag");
            }
        }

        public DetectorVerlengenTypeEnum Verlengen
        {
            get { return _Detector.Verlengen; }
            set
            {
                _Detector.Verlengen = value;
                OnMonitoredPropertyChanged("Verlengen");
                //_ControllerVM.SetAllSelectedElementsValue(this, "Verlengen");
            }
        }

        public bool AanvraagDirect
        {
            get { return _Detector.AanvraagDirect; }
            set
            {
                _Detector.AanvraagDirect = value;
                OnMonitoredPropertyChanged("AanvraagDirect");
            }
        }

        public bool Wachtlicht
        {
            get { return _Detector.Wachtlicht; }
            set
            {
                _Detector.Wachtlicht = value;
                OnMonitoredPropertyChanged("Wachtlicht");
            }
        }


        public int Q1
        {
            get { return _Detector.Simulatie.Q1; }
            set
            {
                _Detector.Simulatie.Q1 = value;
                OnMonitoredPropertyChanged("Q1");
               // _ControllerVM.SetAllSelectedElementsValue(this, "Q1");
            }
        }

        public int Q2
        {
            get { return _Detector.Simulatie.Q2; }
            set
            {
                _Detector.Simulatie.Q2 = value;
                OnMonitoredPropertyChanged("Q2");
                //_ControllerVM.SetAllSelectedElementsValue(this, "Q2");
            }
        }

        public int Q3
        {
            get { return _Detector.Simulatie.Q3; }
            set
            {
                _Detector.Simulatie.Q3 = value;
                OnMonitoredPropertyChanged("Q3");
                //_ControllerVM.SetAllSelectedElementsValue(this, "Q3");
            }
        }

        public int Q4
        {
            get { return _Detector.Simulatie.Q4; }
            set
            {
                _Detector.Simulatie.Q4 = value;
                OnMonitoredPropertyChanged("Q4");
               // _ControllerVM.SetAllSelectedElementsValue(this, "Q4");
            }
        }

        public int Stopline
        {
            get { return _Detector.Simulatie.Stopline; }
            set
            {
                _Detector.Simulatie.Stopline = value;
                OnMonitoredPropertyChanged("Stopline");
               // _ControllerVM.SetAllSelectedElementsValue(this, "Stopline");
            }
        }

        public string FCNr
        {
            get { return _Detector.Simulatie.FCNr; }
            set
            {
                _Detector.Simulatie.FCNr = value;
                OnMonitoredPropertyChanged("FCNr");
              //  _ControllerVM.SetAllSelectedElementsValue(this, "FCNr");
            }
        }

        public bool IsLooseDetector
        {
            get
            {
                return FaseCyclus == null;
            }
        }

        public bool AanvraagDirectPossible
        {
            get
            {
                return FaseCyclus != null && (Type == DetectorTypeEnum.Kop || Type == DetectorTypeEnum.Lang);
            }
        }

        #endregion // Properties

        #region IComparable

        public int CompareTo(object obj)
        {
            DetectorViewModel fcvm = obj as DetectorViewModel;
            if (fcvm == null)
                throw new NotImplementedException();
            else
            {
                string myName = Naam;
                string hisName = fcvm.Naam;
                if (myName.Length < hisName.Length) myName = myName.PadLeft(hisName.Length, '0');
                else if (hisName.Length < myName.Length) hisName = hisName.PadLeft(myName.Length, '0');
                return myName.CompareTo(hisName);
            }
        }

        #endregion // IComparable

        #region Constructor

        public DetectorViewModel(DetectorModel detector)
        {
            _Detector = detector;

        }

        #endregion // Constructor
    }
}
