using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Controls;

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
                RaisePropertyChanged("");
            }
        }

        [Category("Algemeen")]

        public string Naam
        {
            get { return _Controller?.Data?.Naam; }
            set
            {
                _Controller.Data.Naam = value;
                RaisePropertyChanged<object>("Naam", broadcast: true);
            }
        }

        public string Stad
        {
            get { return _Controller?.Data?.Stad; }
            set
            {
                _Controller.Data.Stad = value;
                RaisePropertyChanged<object>("Stad", broadcast: true);
            }
        }

        public string Straat1
        {
            get { return _Controller?.Data?.Straat1; }
            set
            {
                _Controller.Data.Straat1 = value;
                RaisePropertyChanged<object>("Straat1", broadcast: true);
            }
        }

        public string Straat2
        {
            get { return _Controller?.Data?.Straat2; }
            set
            {
                _Controller.Data.Straat2 = value;
                RaisePropertyChanged<object>("Straat2", broadcast: true);
            }
        }

        [Description("Bitmap naam")]
        public string BitmapNaam
        {
            get { return _Controller?.Data?.BitmapNaam; }
            set
            {
                _Controller.Data.BitmapNaam = value;
                RaisePropertyChanged<object>("BitmapNaam", broadcast: true);
                Messenger.Default.Send(new UpdateTabsEnabledMessage());
            }
        }

	    [Description("Vissim naam")]
	    public string VissimNaam
	    {
		    get => _Controller?.Data?.VissimNaam;
		    set
		    {
			    if (Regex.IsMatch(value, @"^[0-9]*$"))
			    {
					_Controller.Data.VissimNaam = value;
					RaisePropertyChanged<object>(nameof(VissimNaam), broadcast: true);
			    }
		    }
	    }

		[Category("Opties regeling")]
        [Description("Fasebewaking")]
        public int Fasebewaking
        {
            get { return _Controller?.Data == null ? 0 : _Controller.Data.Fasebewaking; }
            set
            {
                _Controller.Data.Fasebewaking = value;
                RaisePropertyChanged<object>("Fasebewaking", broadcast: true);
            }
        }

        [Description("CCOL versie")]
        public CCOLVersieEnum CCOLVersie
        {
            get { return _Controller?.Data == null ? CCOLVersieEnum.CCOL8 : _Controller.Data.CCOLVersie; }
            set
            {
                _Controller.Data.CCOLVersie = value;
                RaisePropertyChanged<object>("CCOLVersie", broadcast: true);
            }
        }

        [Description("Type KWC")]
        public KWCTypeEnum KWCType
        {
            get { return _Controller?.Data == null ? KWCTypeEnum.Geen : _Controller.Data.KWCType; }
            set
            {
                _Controller.Data.KWCType = value;
                RaisePropertyChanged<object>("KWCType", broadcast: true);
            }
        }

        [Description("Type VLOG")]
        public VLOGTypeEnum VLOGType
        {
            get { return _Controller?.Data == null ? VLOGTypeEnum.Geen : _Controller.Data.VLOGType; }
            set
            {
                _Controller.Data.VLOGType = value;
                RaisePropertyChanged<object>("VLOGType", broadcast: true);
            }
        }

        [Description("VLOG in testomgeving")]
        public bool VLOGInTestOmgeving
        {
            get { return _Controller?.Data == null ? false : _Controller.Data.VLOGInTestOmgeving; }
            set
            {
                _Controller.Data.VLOGInTestOmgeving = value;
                RaisePropertyChanged<object>("VLOGInTestOmgeving", broadcast: true);
            }
        }

        [Description("Extra meeverlengen in WG")]
        public bool ExtraMeeverlengenInWG
        {
            get { return _Controller?.Data == null ? false : _Controller.Data.ExtraMeeverlengenInWG; }
            set
            {
                _Controller.Data.ExtraMeeverlengenInWG = value;
                RaisePropertyChanged<object>("ExtraMeeverlengenInWG", broadcast: true);
            }
        }

        [Description("Aansturing waitsignalen")]
        public AansturingWaitsignalenEnum AansturingWaitsignalen
        {
            get { return _Controller?.Data?.AansturingWaitsignalen ?? AansturingWaitsignalenEnum.AanvraagGezet; }
            set
            {
                _Controller.Data.AansturingWaitsignalen = value;
                RaisePropertyChanged<object>(nameof(AansturingWaitsignalen), broadcast: true);
            }
        }

        [Description("Fixatie mogelijk")]
        public bool FixatieMogelijk
        {
            get { return _Controller?.Data?.FixatieData.FixatieMogelijk ?? false; }
            set
            {
                _Controller.Data.FixatieData.FixatieMogelijk = value;
                RaisePropertyChanged<object>(nameof(FixatieMogelijk), broadcast: true);
            }
        }

        [Description("Bijkomen tijdens fixatie")]
        public bool BijkomenTijdensFixatie
        {
            get { return _Controller?.Data == null ? false : _Controller.Data.FixatieData.BijkomenTijdensFixatie; }
            set
            {
                _Controller.Data.FixatieData.BijkomenTijdensFixatie = value;
                RaisePropertyChanged<object>(nameof(BijkomenTijdensFixatie), broadcast: true);
            }
        }

        [Description("Type groentijden")]
        public GroentijdenTypeEnum TypeGroentijden
        {
            get { return _Controller?.Data?.TypeGroentijden ?? GroentijdenTypeEnum.MaxGroentijden; }
            set
            {
                _Controller.Data.TypeGroentijden = value;
                RaisePropertyChanged<object>(nameof(TypeGroentijden), broadcast: true);
                Messenger.Default.Send(new GroentijdenTypeChangedMessage(value));
            }
        }

        [Description("Type synchronisatie nalopen")]
        public SynchronisatieTypeEnum NaloopSynchronisatieType
        {
            get { return _Controller?.Data?.NaloopSynchronisatieType ?? SynchronisatieTypeEnum.FictiefConflict; }
            set
            {
                _Controller.Data.NaloopSynchronisatieType = value;
                RaisePropertyChanged<object>(nameof(NaloopSynchronisatieType), broadcast: true);
            }
        }

        [Category("CCOL specifieke opties")]
        [Description("Gekoppelde regeling (CCOLMS)")]
        public bool CCOLMulti
        {
            get => _Controller?.Data?.CCOLMulti ?? false;
            set
            {
                _Controller.Data.CCOLMulti = value;
                RaisePropertyChanged<object>(nameof(CCOLMulti), broadcast: true);
            }
        }

        [Description("Waarde CCOL_SLAVE")]
        [EnabledCondition("CCOLMulti")]
        public int CCOLMultiSlave
        {
            get => _Controller?.Data?.CCOLMultiSlave ?? 0;
            set
            {
                _Controller.Data.CCOLMultiSlave = value;
                RaisePropertyChanged<object>(nameof(CCOLMultiSlave), broadcast: true);
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
