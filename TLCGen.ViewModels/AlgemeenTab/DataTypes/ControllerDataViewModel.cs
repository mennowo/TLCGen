using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class ControllerDataViewModel : ViewModelBase
    {
        #region Fields

        #endregion // Fields

        #region Properties

        private ControllerModel _Controller;
        public ControllerModel Controller
        {
            set
            {
                _Controller = value;
                OnPropertyChanged(null);
            }
        }

        [Category("Algemeen")]

        public string Naam
        {
            get { return _Controller?.Data?.Naam; }
            set
            {
                _Controller.Data.Naam = value;
                OnMonitoredPropertyChanged("Naam");
            }
        }

        public string Stad
        {
            get { return _Controller?.Data?.Stad; }
            set
            {
                _Controller.Data.Stad = value;
                OnMonitoredPropertyChanged("Stad");
            }
        }

        public string Straat1
        {
            get { return _Controller?.Data?.Straat1; }
            set
            {
                _Controller.Data.Straat1 = value;
                OnMonitoredPropertyChanged("Straat1");
            }
        }

        public string Straat2
        {
            get { return _Controller?.Data?.Straat2; }
            set
            {
                _Controller.Data.Straat2 = value;
                OnMonitoredPropertyChanged("Straat2");
            }
        }

        [Description("Bitmap naam")]
        public string BitmapNaam
        {
            get { return _Controller?.Data?.BitmapNaam; }
            set
            {
                _Controller.Data.BitmapNaam = value;
                OnMonitoredPropertyChanged("BitmapNaam");
                Messenger.Default.Send(new UpdateTabsEnabledMessage());
            }
        }
        [Category("Opties regeling")]
        [Description("CCOL versie")]
        public CCOLVersieEnum CCOLVersie
        {
            get { return _Controller?.Data == null ? CCOLVersieEnum.CCOL8 : _Controller.Data.CCOLVersie; }
            set
            {
                _Controller.Data.CCOLVersie = value;
                OnMonitoredPropertyChanged("CCOLVersie");
            }
        }

        [Description("Type KWC")]
        public KWCTypeEnum KWCType
        {
            get { return _Controller?.Data == null ? KWCTypeEnum.Geen : _Controller.Data.KWCType; }
            set
            {
                _Controller.Data.KWCType = value;
                OnMonitoredPropertyChanged("KWCType");
            }
        }

        [Description("Type VLOG")]
        public VLOGTypeEnum VLOGType
        {
            get { return _Controller?.Data == null ? VLOGTypeEnum.Geen : _Controller.Data.VLOGType; }
            set
            {
                _Controller.Data.VLOGType = value;
                OnMonitoredPropertyChanged("VLOGType");
            }
        }

        [Description("VLOG in testomgeving")]
        public bool VLOGInTestOmgeving
        {
            get { return _Controller?.Data == null ? false : _Controller.Data.VLOGInTestOmgeving; }
            set
            {
                _Controller.Data.VLOGInTestOmgeving = value;
                OnMonitoredPropertyChanged("VLOGInTestOmgeving");
            }
        }

        [Description("Extra meeverlengen in WG")]
        public bool ExtraMeeverlengenInWG
        {
            get { return _Controller?.Data == null ? false : _Controller.Data.ExtraMeeverlengenInWG; }
            set
            {
                _Controller.Data.ExtraMeeverlengenInWG = value;
                OnMonitoredPropertyChanged("ExtraMeeverlengenInWG");
            }
        }

        [Description("Aansturing waitsignalen")]
        public AansturingWaitsignalenEnum AansturingWaitsignalen
        {
            get { return _Controller?.Data == null ? AansturingWaitsignalenEnum.AanvraagGezet : _Controller.Data.AansturingWaitsignalen; }
            set
            {
                _Controller.Data.AansturingWaitsignalen = value;
                OnMonitoredPropertyChanged("AansturingWaitsignalen");
            }
        }

        [Description("Fixatie mogelijk")]
        public bool FixatieMogelijk
        {
            get { return _Controller?.Data == null ? false : _Controller.Data.FixatieData.FixatieMogelijk; }
            set
            {
                _Controller.Data.FixatieData.FixatieMogelijk = value;
                OnMonitoredPropertyChanged("FixatieMogelijk");
            }
        }

        [Description("Bijkomen tijdens fixatie")]
        public bool BijkomenTijdensFixatie
        {
            get { return _Controller?.Data == null ? false : _Controller.Data.FixatieData.BijkomenTijdensFixatie; }
            set
            {
                _Controller.Data.FixatieData.BijkomenTijdensFixatie = value;
                OnMonitoredPropertyChanged("BijkomenTijdensFixatie");
            }
        }

        [Description("Type groentijden")]
        public GroentijdenTypeEnum TypeGroentijden
        {
            get { return _Controller?.Data == null ? GroentijdenTypeEnum.MaxGroentijden : _Controller.Data.TypeGroentijden; }
            set
            {
                _Controller.Data.TypeGroentijden = value;
                OnMonitoredPropertyChanged("TypeGroentijden");
                Messenger.Default.Send(new GroentijdenTypeChangedMessage(value));
            }
        }

        #endregion // Properties

        #region Constructor

        public ControllerDataViewModel()
        {
        }

        #endregion // Constructor
    }
}
