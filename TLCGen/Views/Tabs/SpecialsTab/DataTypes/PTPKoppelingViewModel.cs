using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class PTPKoppelingViewModel : ViewModelBase
    {
        #region Fields

        private PTPKoppelingModel _PTPKoppeling;

        #endregion // Fields

        #region Properties

        public PTPKoppelingModel PTPKoppeling
        {
            get { return _PTPKoppeling; }
        }

        public string TeKoppelenKruispunt
        {
            get { return _PTPKoppeling.TeKoppelenKruispunt; }
            set
            {
                _PTPKoppeling.TeKoppelenKruispunt = value;
                RaisePropertyChanged<PTPKoppelingViewModel>("TeKoppelenKruispunt", broadcast: true);
            }
        }
        public int AantalsignalenIn
        {
            get { return _PTPKoppeling.AantalsignalenIn; }
            set
            {
                _PTPKoppeling.AantalsignalenIn = value;
                RaisePropertyChanged<PTPKoppelingViewModel>("AantalsignalenIn", broadcast: true);
            }
        }

        public int AantalsignalenUit
        {
            get { return _PTPKoppeling.AantalsignalenUit; }
            set
            {
                _PTPKoppeling.AantalsignalenUit = value;
                RaisePropertyChanged<PTPKoppelingViewModel>("AantalsignalenUit", broadcast: true);
            }
        }

        public int PortnummerSimuatieOmgeving
        {
            get { return _PTPKoppeling.PortnummerSimuatieOmgeving; }
            set
            {
                _PTPKoppeling.PortnummerSimuatieOmgeving = value;
                RaisePropertyChanged<PTPKoppelingViewModel>("PortnummerSimuatieOmgeving", broadcast: true);
            }
        }

        public int PortnummerAutomaatOmgeving
        {
            get { return _PTPKoppeling.PortnummerAutomaatOmgeving; }
            set
            {
                _PTPKoppeling.PortnummerAutomaatOmgeving = value;
                RaisePropertyChanged<PTPKoppelingViewModel>("PortnummerAutomaatOmgeving", broadcast: true);
            }
        }

        public int NummerSource
        {
            get { return _PTPKoppeling.NummerSource; }
            set
            {
                _PTPKoppeling.NummerSource = value;
                RaisePropertyChanged<PTPKoppelingViewModel>("NummerSource", broadcast: true);
            }
        }

        public int NummerDestination
        {
            get { return _PTPKoppeling.NummerDestination; }
            set
            {
                _PTPKoppeling.NummerDestination = value;
                RaisePropertyChanged<PTPKoppelingViewModel>("NummerDestination", broadcast: true);
            }
        }


        #endregion // Properties

        #region Constructor

        public PTPKoppelingViewModel(PTPKoppelingModel kop)
        {
            _PTPKoppeling = kop;
        }

        #endregion // Constructor
    }
}
