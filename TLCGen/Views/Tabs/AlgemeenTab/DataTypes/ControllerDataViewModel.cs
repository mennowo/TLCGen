using GalaSoft.MvvmLight.Messaging;
using System.ComponentModel;
using System.Text.RegularExpressions;
using GalaSoft.MvvmLight;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Controls;
using TLCGen.Dependencies.Providers;
using System;
using System.Linq;

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
                if (Helpers.NameSyntaxChecker.IsValidFileName(value))
                {
                    _Controller.Data.Naam = value;
                }
                RaisePropertyChanged<object>("Naam", broadcast: true);
            }
        }

        public string Nummer
        {
            get { return _Controller?.Data?.Nummer; }
            set
            {
                _Controller.Data.Nummer = value;
                RaisePropertyChanged<object>("Nummer", broadcast: true);
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
                if (Intergroen && value < CCOLVersieEnum.CCOL95)
                {
                    TLCGenDialogProvider.Default.ShowMessageBox(
                        "Voor deze regeling is intergroen ingesteld. Voor een intergroen regeling " +
                        "kan de CCOL versie niet lager dan 9.5 worden ingesteld.", "Intergroen regeling", System.Windows.MessageBoxButton.OK);
                    // see here https://stackoverflow.com/questions/2585183/wpf-combobox-selecteditem-change-to-previous-value
                    GalaSoft.MvvmLight.Threading.DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => RaisePropertyChanged(nameof(CCOLVersie))));
                }
                else
                {
                    _Controller.Data.CCOLVersie = value;
                    if(value > CCOLVersieEnum.CCOL8)
                    {
                        if(_Controller.Data.VLOGSettings == null)
                        {
                            _Controller.Data.VLOGSettings = new VLOGSettingsDataModel();
                            Settings.DefaultsProvider.Default.SetDefaultsOnModel(_Controller.Data.VLOGSettings, _Controller.Data.VLOGSettings.VLOGVersie.ToString());
                        }
                    }
                    MessengerInstance.Send(new ControllerIntergreenTimesTypeChangedMessage());
                    RaisePropertyChanged<object>(nameof(CCOLVersie), broadcast: true);
                }
                RaisePropertyChanged(nameof(IsVLOGVersieLowerThan9));
                RaisePropertyChanged(nameof(IsVLOGVersieHigherThan9));
                MessengerInstance.Send(new UpdateTabsEnabledMessage());
            }
        }


        [Description("Intergroen")]
        [BrowsableCondition("IsVLOGVersieHigherThan9")]
        public bool Intergroen
        {
            get { return _Controller?.Data == null ? false : _Controller.Data.Intergroen; }
            set
            {
                if (_Controller.Data.Intergroen != value)
                {
                    if (TLCGenDialogProvider.Default.ShowDialogs)
                    {
                        if (value == true)
                        {
                            var result = TLCGenDialogProvider.Default.ShowMessageBox("De regeling wordt omgezet naar intergroen. Huidige ontruimingstijden ophogen met de geeltijd?", "Conversie naar intergroen", System.Windows.MessageBoxButton.YesNoCancel);
                            if(result == System.Windows.MessageBoxResult.Yes)
                            {
                                ModelManagement.TLCGenModelManager.Default.ConvertToIntergroen(_Controller);
                            }
                            else if(result == System.Windows.MessageBoxResult.Cancel)
                            {
                                RaisePropertyChanged();
                                return;
                            }
                        }
                        else
                        {
                            var result = TLCGenDialogProvider.Default.ShowMessageBox("De regeling wordt omgezet naar ontruimingstijden. Huidige intergroentijden verlagen met de geeltijd?", "Conversie naar ontruimingstijden", System.Windows.MessageBoxButton.YesNoCancel);
                            if (result == System.Windows.MessageBoxResult.Yes)
                            {
                                ModelManagement.TLCGenModelManager.Default.ConvertToOntruimingstijden(_Controller);
                            }
                            else if (result == System.Windows.MessageBoxResult.Cancel)
                            {
                                RaisePropertyChanged();
                                return;
                            }
                        }
                        _Controller.Data.Intergroen = value;
                        RaisePropertyChanged<object>("Intergroen", broadcast: true);
                    }
                    else
                    {
                        RaisePropertyChanged();
                    }
                }
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
        [BrowsableCondition("IsVLOGVersieLowerThan9")]
        public VLOGTypeEnum VLOGType
        {
            get { return _Controller?.Data == null ? VLOGTypeEnum.Geen : _Controller.Data.VLOGType; }
            set
            {
                _Controller.Data.VLOGType = value;
                RaisePropertyChanged<object>("VLOGType", broadcast: true);
            }
        }

        [Browsable(false)]
        public bool IsVLOGVersieHigherThan9 => CCOLVersie > CCOLVersieEnum.CCOL9;

        [Browsable(false)]
        public bool IsVLOGVersieLowerThan9 => CCOLVersie < CCOLVersieEnum.CCOL9;

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

        [Description("Type segmenten display")]
        public SegmentDisplayTypeEnum SegmentDisplayType
        {
            get { return _Controller?.Data?.SegmentDisplayType ?? SegmentDisplayTypeEnum.GeenSegmenten; }
            set
            {
                if(_Controller.Data.SegmentDisplayType != value)
                {
                    _Controller.Data.SegmentDisplayType = value;
                    _Controller.Data.SetSegmentOutputs();
                }
                RaisePropertyChanged<object>(nameof(SegmentDisplayType), broadcast: true);
            }
        }

        [Description("Uitgang per module")]
        public bool UitgangPerModule
        {
            get { return _Controller?.Data?.UitgangPerModule ?? false; }
            set
            {
                if (_Controller.Data.UitgangPerModule != value)
                {
                    _Controller.Data.UitgangPerModule = value;
                    if (value)
                    {
                        if (!_Controller.Data.MultiModuleReeksen)
                        {
                            foreach (var m in _Controller.ModuleMolen.Modules)
                            {
                                _Controller.Data.ModulenDisplayBitmapData.Add(new ModuleDisplayElementModel
                                {
                                    Naam = m.Naam
                                });
                            }
                        }
                        else
                        {
                            foreach (var r in _Controller.MultiModuleMolens)
                            {
                                foreach (var m in r.Modules)
                                {
                                    _Controller.Data.ModulenDisplayBitmapData.Add(new ModuleDisplayElementModel
                                    {
                                        Naam = m.Naam
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        _Controller.Data.ModulenDisplayBitmapData.Clear();
                    }
                }
                RaisePropertyChanged<object>(nameof(SegmentDisplayType), broadcast: true);
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
                RaisePropertyChanged(nameof(IsTypeGroenVerlengGroen));
                Messenger.Default.Send(new GroentijdenTypeChangedMessage(value));
            }
        }

        public bool IsTypeGroenVerlengGroen => TypeGroentijden == GroentijdenTypeEnum.VerlengGroentijden;

        [Description("Verlenggroentijden direct in TVG max")]
        [BrowsableCondition("IsTypeGroenVerlengGroen")]
        public bool VerlengGroenInTVGMax
        {
            get { return _Controller?.Data.VerlengGroenInTVGMax ?? false; }
            set
            {
                _Controller.Data.VerlengGroenInTVGMax = value;
                RaisePropertyChanged<object>(nameof(VerlengGroenInTVGMax), broadcast: true);
            }
        }

        [Description("Toevoegen OVM code")]
        public bool ToevoegenOVM
        {
            get { return _Controller?.Data?.ToevoegenOVM ?? false; }
            set
            {
                _Controller.Data.ToevoegenOVM = value;
                RaisePropertyChanged<object>(nameof(ToevoegenOVM), broadcast: true);
            }
        }

        [Browsable(false)]
        public bool HasWTV => _Controller?.Fasen.Any(x => x.WachttijdVoorspeller) == true;

        [Description("Opnemen wtv venster in testomgeving")]
        [BrowsableCondition(nameof(HasWTV))]
        public bool WachttijdvoorspellerVensterTestomgeving
        {
            get { return _Controller?.Data?.WachttijdvoorspellerVensterTestomgeving ?? false; }
            set
            {
                _Controller.Data.WachttijdvoorspellerVensterTestomgeving = value;
                RaisePropertyChanged<object>(nameof(WachttijdvoorspellerVensterTestomgeving), broadcast: true);
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

        [Description("Meerdere module reeksen")]
        public bool MultiModuleReeksen
        {
            get => _Controller?.Data?.MultiModuleReeksen ?? false;
            set
            {
                _Controller.Data.MultiModuleReeksen = value;
                RaisePropertyChanged<object>(nameof(MultiModuleReeksen), broadcast: true);

                _Controller.Data.ModulenDisplayBitmapData.Clear();
                if (UitgangPerModule) MessengerInstance.Send(new ModulesChangedMessage());
            }
        }

        [Description("Waarde CCOLSLAVE")]
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

        [Description("Wtv niet halteren als meer dan # leds")]
        public int WachttijdvoorspellerNietHalterenMax
        {
            get => _Controller?.Data?.WachttijdvoorspellerNietHalterenMax ?? 0;
            set
            {
                _Controller.Data.WachttijdvoorspellerNietHalterenMax = value;
                RaisePropertyChanged<object>(nameof(WachttijdvoorspellerNietHalterenMax), broadcast: true);
            }
        }

        [Description("Wtv niet halteren als minder dan # leds")]
        public int WachttijdvoorspellerNietHalterenMin
        {
            get => _Controller?.Data?.WachttijdvoorspellerNietHalterenMin ?? 0;
            set
            {
                _Controller.Data.WachttijdvoorspellerNietHalterenMin = value;
                RaisePropertyChanged<object>(nameof(WachttijdvoorspellerNietHalterenMin), broadcast: true);
            }
        }

        [Description("Aansturing BUS tijdens OV ingreep")]
        public bool WachttijdvoorspellerAansturenBus
        {
            get => _Controller?.Data?.WachttijdvoorspellerAansturenBus ?? false;
            set
            {
                _Controller.Data.WachttijdvoorspellerAansturenBus = value;
                RaisePropertyChanged<object>(nameof(WachttijdvoorspellerAansturenBus), broadcast: true);
            }
        }

        [Description("Ook aansturing BUS tijdens HD ingreep")]
        [BrowsableCondition("WachttijdvoorspellerAansturenBus")]
        public bool WachttijdvoorspellerAansturenBusHD
        {
            get => _Controller?.Data?.WachttijdvoorspellerAansturenBusHD ?? false;
            set
            {
                _Controller.Data.WachttijdvoorspellerAansturenBusHD = value;
                RaisePropertyChanged<object>(nameof(WachttijdvoorspellerAansturenBusHD), broadcast: true);
            }
        }

        [Description("Loggen max. TFB in PRM")]
        public bool PrmLoggingTfbMax
        {
            get { return _Controller?.Data?.PrmLoggingTfbMax ?? false; }
            set
            {
                _Controller.Data.PrmLoggingTfbMax = value;
                RaisePropertyChanged<object>(nameof(PrmLoggingTfbMax), broadcast: true);
            }
        }

        [Category("Opties ontwikkel omgeving")]
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

        [Description("Genereren code t.b.v. DUURTEST")]
        public bool GenererenDuurtestCode
        {
            get { return _Controller?.Data == null ? false : _Controller.Data.GenererenDuurtestCode; }
            set
            {
                _Controller.Data.GenererenDuurtestCode = value;
                RaisePropertyChanged<object>("GenererenDuurtestCode", broadcast: true);
            }
        }

        [Description("Niet gebruiken bitmap")]
        public bool GebruikBitmap
        {
            get { return _Controller?.Data == null ? false : _Controller.Data.NietGebruikenBitmap; }
            set
            {
                _Controller.Data.NietGebruikenBitmap = value;
                RaisePropertyChanged<object>("GebruikBitmap", broadcast: true);
                Messenger.Default.Send(new UpdateTabsEnabledMessage());
            }
        }

        [Description("Practice omgeving")]
        public bool PracticeOmgeving
        {
            get { return _Controller?.Data == null ? false : _Controller.Data.PracticeOmgeving; }
            set
            {
                _Controller.Data.PracticeOmgeving = value;
                RaisePropertyChanged<object>("PracticeOmgeving", broadcast: true);
            }
        }

        [Description("Opnemen code mirakel monitor")]
        public bool MirakelMonitor
        {
            get { return _Controller?.Data?.MirakelMonitor ?? false; }
            set
            {
                _Controller.Data.MirakelMonitor = value;
                RaisePropertyChanged<object>(nameof(MirakelMonitor), broadcast: true);
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
