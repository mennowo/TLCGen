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

namespace TLCGen.ViewModels
{
    public class DetectorViewModel : ViewModelBase, IComparable
    {
        #region Fields

        private DetectorModel _Detector;
        private FaseCyclusViewModel _FaseVM;

        #endregion // Fields

        #region Properties

        public DetectorModel Detector
        {
            get { return _Detector; }
        }

        public FaseCyclusViewModel FaseVM
        {
            get { return _FaseVM; }
            set { _FaseVM = value; }
        }

        public string FaseCyclus
        {
            get { return FaseVM?.Naam; }
        }

        public string Naam
        {
            get { return _Detector.Naam; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    var message = new IsNameUniqueRequest(value);
                    MessageManager.Instance.SendWithRespons(message);
                    if (message.Handled && message.IsUnique)
                    {
                        string olddefine = _Detector.Define;
                        string oldname = _Detector.Naam;

                        _Detector.Naam = value;
                        _Detector.Define = SettingsProvider.Instance.GetDetectorDefinePrefix() + value;

                        // Notify the messenger
                        MessageManager.Instance.Send(new NameChangedMessage(oldname, _Detector.Naam));
                        MessageManager.Instance.Send(new DefineChangedMessage(olddefine, _Detector.Define));
                    }
                }
                OnMonitoredPropertyChanged("Naam");
            }
        }

        public string Define
        {
            get { return _Detector.Define; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    var message = new IsDefineUniqueRequest(value);
                    MessageManager.Instance.SendWithRespons(message);
                    if (message.Handled && message.IsUnique)
                    {
                        string olddefine = _Detector.Define;
                        string oldname = _Detector.Naam;

                        _Detector.Naam = value.Replace(SettingsProvider.Instance.GetDetectorDefinePrefix(), "");
                        _Detector.Define = value;

                        // Notify the messenger
                        MessageManager.Instance.Send(new NameChangedMessage(oldname, _Detector.Naam));
                        MessageManager.Instance.Send(new DefineChangedMessage(olddefine, _Detector.Define));
                    }
                }
                OnMonitoredPropertyChanged("Define");
                OnMonitoredPropertyChanged("Naam");
            }
        }

        public DetectorTypeEnum Type
        {
            get { return _Detector.Type; }
            set
            {
                _Detector.Type = value;
                OnMonitoredPropertyChanged("Type");
                FaseVM?.UpdateHasKopmax();
#warning TODO also below...
                //_ControllerVM.SetAllSelectedElementsValue(this, "Type");
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
                return FaseVM == null;
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
