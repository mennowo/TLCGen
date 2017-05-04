using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
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

        public int? Rijstrook
        {
            get { return _Detector.Rijstrook; }
            set
            {
                _Detector.Rijstrook = value;
                RaisePropertyChanged<DetectorViewModel>("Rijstrook", broadcast: true);
            }
        }

        private List<int> _Rijstroken;
        public List<int> Rijstroken
        {
            get
            {
                return _Rijstroken;
            }
        }

        public string FaseCyclus
        {
            get { return _FaseCyclus; }
            set
            {
                _FaseCyclus = value;
                _Rijstroken = new List<int>();
                if (DefaultsProvider.Default.Controller != null)
                {
                    if (DefaultsProvider.Default.Controller.Fasen.Where(x => x.Naam == value).Any())
                    {
                        var f = DefaultsProvider.Default.Controller.Fasen.Where(x => x.Naam == value).First();
                        for (int i = 0; i < f.AantalRijstroken; ++i)
                        {
                            _Rijstroken.Add(i + 1);
                        }
                    }
                }
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
                RaisePropertyChanged<DetectorViewModel>("Naam", broadcast: true);
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
                RaisePropertyChanged<DetectorViewModel>("VissimNaam", broadcast: true);
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
                RaisePropertyChanged<DetectorViewModel>(null, broadcast: true);
                Messenger.Default.Send(new FaseDetectorTypeChangedMessage(Naam, value));
            }
        }

        public bool IsDrukknop
        {
            get
            {
                return 
                    Type == DetectorTypeEnum.Knop ||
                    Type == DetectorTypeEnum.KnopBinnen || 
                    Type == DetectorTypeEnum.KnopBuiten;
            }
        }

        public int? TDB
        {
            get { return _Detector.TDB; }
            set
            {
                if (value == null || value >= 0)
                    _Detector.TDB = value;
                RaisePropertyChanged<DetectorViewModel>("TDB", broadcast: true);
            }
        }

        public int? TDH
        {
            get { return _Detector.TDH; }
            set
            {
                if (value == null || value >= 0)
                    _Detector.TDH = value;
                RaisePropertyChanged<DetectorViewModel>("TDH", broadcast: true);
            }
        }

        public int? TOG
        {
            get { return _Detector.TOG; }
            set
            {
                if (value == null || value >= 0)
                    _Detector.TOG = value;
                RaisePropertyChanged<DetectorViewModel>("TOG", broadcast: true);
            }
        }

        public int? TBG
        {
            get { return _Detector.TBG; }
            set
            {
                if (value == null || value >= 0)
                    _Detector.TBG = value;
                RaisePropertyChanged<DetectorViewModel>("TBG", broadcast: true);
            }
        }

        public int? TFL
        {
            get { return _Detector.TFL; }
            set
            {
                if (value == null || value >= 0)
                    _Detector.TFL = value;
                RaisePropertyChanged<DetectorViewModel>("TFL", broadcast: true);
            }
        }

        public int? CFL
        {
            get { return _Detector.CFL; }
            set
            {
                if (value == null || value >= 0)
                    _Detector.CFL = value;
                RaisePropertyChanged<DetectorViewModel>("CFL", broadcast: true);
            }
        }
        public DetectorAanvraagTypeEnum Aanvraag
        {
            get { return _Detector.Aanvraag; }
            set
            {
                _Detector.Aanvraag = value;
                RaisePropertyChanged<DetectorViewModel>("Aanvraag", broadcast: true);
            }
        }

        public DetectorVerlengenTypeEnum Verlengen
        {
            get { return _Detector.Verlengen; }
            set
            {
                _Detector.Verlengen = value;
                RaisePropertyChanged<DetectorViewModel>("Verlengen", broadcast: true);
            }
        }

        public NooitAltijdAanUitEnum AanvraagBijStoring
        {
            get { return _Detector.AanvraagBijStoring; }
            set
            {
                _Detector.AanvraagBijStoring = value;
                RaisePropertyChanged<DetectorViewModel>("AanvraagBijStoring", broadcast: true);
            }
        }

        public bool AanvraagDirect
        {
            get { return _Detector.AanvraagDirect; }
            set
            {
                _Detector.AanvraagDirect = value;
                RaisePropertyChanged<DetectorViewModel>("AanvraagDirect", broadcast: true);
            }
        }

        public bool Wachtlicht
        {
            get { return _Detector.Wachtlicht; }
            set
            {
                _Detector.Wachtlicht = value;
                RaisePropertyChanged<DetectorViewModel>("Wachtlicht", broadcast: true);
            }
        }


        public int Q1
        {
            get { return _Detector.Simulatie.Q1; }
            set
            {
                _Detector.Simulatie.Q1 = value;
                RaisePropertyChanged<DetectorViewModel>("Q1", broadcast: true);
            }
        }

        public int Q2
        {
            get { return _Detector.Simulatie.Q2; }
            set
            {
                _Detector.Simulatie.Q2 = value;
                RaisePropertyChanged<DetectorViewModel>("Q2", broadcast: true);
            }
        }

        public int Q3
        {
            get { return _Detector.Simulatie.Q3; }
            set
            {
                _Detector.Simulatie.Q3 = value;
                RaisePropertyChanged<DetectorViewModel>("Q3", broadcast: true);
            }
        }

        public int Q4
        {
            get { return _Detector.Simulatie.Q4; }
            set
            {
                _Detector.Simulatie.Q4 = value;
                RaisePropertyChanged<DetectorViewModel>("Q4", broadcast: true);
            }
        }

        public int Stopline
        {
            get { return _Detector.Simulatie.Stopline; }
            set
            {
                _Detector.Simulatie.Stopline = value;
                RaisePropertyChanged<DetectorViewModel>("Stopline", broadcast: true);
            }
        }

        public string FCNr
        {
            get { return _Detector.Simulatie.FCNr; }
            set
            {
                _Detector.Simulatie.FCNr = value;
                RaisePropertyChanged<DetectorViewModel>("FCNr", broadcast: true);
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
