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
using TLCGen.Dependencies.Models.Enumerations;

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
            get => _Controller?.Data?.Naam;
            set
            {
                var actualValue = value.Trim();
                if (Helpers.NameSyntaxChecker.IsValidFileName(actualValue))
                {
                    _Controller.Data.Naam = actualValue;
                }
                RaisePropertyChanged<object>(nameof(Naam), broadcast: true);
            }
        }

        public string Nummer
        {
            get => _Controller?.Data?.Nummer;
            set
            {
                _Controller.Data.Nummer = value;
                RaisePropertyChanged<object>(nameof(Nummer), broadcast: true);
            }
        }

        public string Stad
        {
            get => _Controller?.Data?.Stad;
            set
            {
                _Controller.Data.Stad = value;
                RaisePropertyChanged<object>(nameof(Stad), broadcast: true);
            }
        }

        public string Straat1
        {
            get => _Controller?.Data?.Straat1;
            set
            {
                _Controller.Data.Straat1 = value;
                RaisePropertyChanged<object>(nameof(Straat1), broadcast: true);
            }
        }

        public string Straat2
        {
            get => _Controller?.Data?.Straat2;
            set
            {
                _Controller.Data.Straat2 = value;
                RaisePropertyChanged<object>(nameof(Straat2), broadcast: true);
            }
        }

        [Description("Bitmap naam")]
        public string BitmapNaam
        {
            get => _Controller?.Data?.BitmapNaam;
            set
            {
                _Controller.Data.BitmapNaam = value;
                RaisePropertyChanged<object>(nameof(BitmapNaam), broadcast: true);
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
            get => _Controller?.Data?.Fasebewaking ?? 0;
            set
            {
                _Controller.Data.Fasebewaking = value;
                RaisePropertyChanged<object>(nameof(Fasebewaking), broadcast: true);
            }
        }

        [Description("CCOL versie")]
        public CCOLVersieEnum CCOLVersie
        {
            get => _Controller?.Data?.CCOLVersie ?? CCOLVersieEnum.CCOL8;
            set
            {
                var oldValue = _Controller.Data.CCOLVersie;
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
                            Settings.DefaultsProvider.Default.SetDefaultsOnModel(_Controller.Data.VLOGSettings, value < CCOLVersieEnum.CCOL110 ? "VLOG300" : "VLOG310");
                        }
                        else if (value >= CCOLVersieEnum.CCOL110 && oldValue < CCOLVersieEnum.CCOL110)
                        {
                            var result = TLCGenDialogProvider.Default.ShowMessageBox(
                                "Vanaf CCOL11 is de VLOG versie 3.1. Opnieuw toepassen defaults voor VLOG " +
                                "instellingen tbv. juiste instellingen?", "VLOG 3.1 instellen", System.Windows.MessageBoxButton.YesNo);
                            if (result == System.Windows.MessageBoxResult.Yes)
                            {
                                Settings.DefaultsProvider.Default.SetDefaultsOnModel(_Controller.Data.VLOGSettings, "VLOG310");
                            }
                        }
                    }
                    MessengerInstance.Send(new ControllerIntergreenTimesTypeChangedMessage());
                    RaisePropertyChanged<object>(nameof(CCOLVersie), broadcast: true);
                }
                RaisePropertyChanged(nameof(IsCCOLVersieLowerThan9));
                RaisePropertyChanged(nameof(IsCCOLVersieHigherThan9));
                RaisePropertyChanged(nameof(IsCCOLVersieHigherThanOrEqualTo9));
                MessengerInstance.Send(new UpdateTabsEnabledMessage());
                MessengerInstance.Send(new CCOLVersionChangedMessage(oldValue, _Controller.Data.CCOLVersie));
            }
        }


        [Description("Intergroen")]
        [BrowsableCondition("IsCCOLVersieHigherThan9")]
        public bool Intergroen
        {
            get => _Controller?.Data?.Intergroen ?? false;
            set
            {
                if (_Controller.Data.Intergroen != value)
                {
                    if (TLCGenDialogProvider.Default.ShowDialogs)
                    {
                        if (value)
                        {
                            var result = TLCGenDialogProvider.Default.ShowMessageBox("De regeling wordt omgezet naar intergroen. Huidige ontruimingstijden ophogen met de geeltijd?", "Conversie naar intergroen", System.Windows.MessageBoxButton.YesNoCancel);
                            if (result == System.Windows.MessageBoxResult.Yes)
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
                        RaisePropertyChanged<object>(nameof(Intergroen), broadcast: true);
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
            get => _Controller?.Data?.KWCType ?? KWCTypeEnum.Geen;
            set
            {
                _Controller.Data.KWCType = value;
                RaisePropertyChanged<object>(nameof(KWCType), broadcast: true);
            }
        }

        [Description("Type VLOG")]
        [BrowsableCondition("IsCCOLVersieLowerThan9")]
        public VLOGTypeEnum VLOGType
        {
            get => _Controller?.Data?.VLOGType ?? VLOGTypeEnum.Geen;
            set
            {
                _Controller.Data.VLOGType = value;
                RaisePropertyChanged<object>(nameof(VLOGType), broadcast: true);
            }
        }

        [Browsable(false)]
        public bool IsCCOLVersieHigherThan9 => CCOLVersie > CCOLVersieEnum.CCOL9;

        [Browsable(false)]
        public bool IsCCOLVersieHigherThanOrEqualTo9 => CCOLVersie >= CCOLVersieEnum.CCOL9;

        [Browsable(false)]
        public bool IsCCOLVersieLowerThan9 => CCOLVersie < CCOLVersieEnum.CCOL9;

        [Description("Extra meeverlengen in WG")]
        public bool ExtraMeeverlengenInWG
        {
            get => _Controller?.Data?.ExtraMeeverlengenInWG ?? false;
            set
            {
                _Controller.Data.ExtraMeeverlengenInWG = value;
                RaisePropertyChanged<object>(nameof(ExtraMeeverlengenInWG), broadcast: true);
            }
        }

        [Description("Aansturing waitsignalen")]
        public AansturingWaitsignalenEnum AansturingWaitsignalen
        {
            get => _Controller?.Data?.AansturingWaitsignalen ?? AansturingWaitsignalenEnum.AanvraagGezet;
            set
            {
                _Controller.Data.AansturingWaitsignalen = value;
                RaisePropertyChanged<object>(nameof(AansturingWaitsignalen), broadcast: true);
            }
        }

        [Description("Type segmenten display")]
        public SegmentDisplayTypeEnum SegmentDisplayType
        {
            get => _Controller?.Data?.SegmentDisplayType ?? SegmentDisplayTypeEnum.GeenSegmenten;
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
            get => _Controller?.Data?.UitgangPerModule ?? false;
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
            get => _Controller?.Data?.FixatieData.FixatieMogelijk ?? false;
            set
            {
                _Controller.Data.FixatieData.FixatieMogelijk = value;
                RaisePropertyChanged<object>(nameof(FixatieMogelijk), broadcast: true);
            }
        }

        [Description("Bijkomen tijdens fixatie")]
        public bool BijkomenTijdensFixatie
        {
            get => _Controller?.Data?.FixatieData.BijkomenTijdensFixatie ?? false;
            set
            {
                _Controller.Data.FixatieData.BijkomenTijdensFixatie = value;
                RaisePropertyChanged<object>(nameof(BijkomenTijdensFixatie), broadcast: true);
            }
        }

        [Description("Type groentijden")]
        public GroentijdenTypeEnum TypeGroentijden
        {
            get => _Controller?.Data?.TypeGroentijden ?? GroentijdenTypeEnum.MaxGroentijden;
            set
            {
                _Controller.Data.TypeGroentijden = value;
                RaisePropertyChanged<object>(nameof(TypeGroentijden), broadcast: true);
                Messenger.Default.Send(new GroentijdenTypeChangedMessage(value));
            }
        }
        
        [Description("TVGAmax als default groentijdenset")]
        public bool TVGAMaxAlsDefaultGroentijdSet
        {
            get => _Controller?.Data?.TVGAMaxAlsDefaultGroentijdSet ?? false;
            set
            {
                _Controller.Data.TVGAMaxAlsDefaultGroentijdSet = value;
                RaisePropertyChanged<object>(nameof(TVGAMaxAlsDefaultGroentijdSet), broadcast: true);
            }
        }

        [Description("Toevoegen OVM code")]
        public bool ToevoegenOVM
        {
            get => _Controller?.Data?.ToevoegenOVM ?? false;
            set
            {
                _Controller.Data.ToevoegenOVM = value;
                RaisePropertyChanged<object>(nameof(ToevoegenOVM), broadcast: true);
            }
        }

        [Description("Geen OG/BG/flutter in AUTOMAAT omgeving")]
        [BrowsableCondition("IsCCOLVersieHigherThanOrEqualTo9")]
        public bool GeenDetectorGedragInAutomaatOmgeving
        {
            get => _Controller?.Data?.GeenDetectorGedragInAutomaatOmgeving ?? false;
            set
            {
                _Controller.Data.GeenDetectorGedragInAutomaatOmgeving = value;
                RaisePropertyChanged<object>(nameof(GeenDetectorGedragInAutomaatOmgeving), broadcast: true);
            }
        }

        [Browsable(false)]
        public bool HasWTV => _Controller?.Fasen.Any(x => x.WachttijdVoorspeller) == true;

        [Description("Opnemen wtv venster in testomgeving")]
        [BrowsableCondition(nameof(HasWTV))]
        public bool WachttijdvoorspellerVensterTestomgeving
        {
            get => _Controller?.Data?.WachttijdvoorspellerVensterTestomgeving ?? false;
            set
            {
                _Controller.Data.WachttijdvoorspellerVensterTestomgeving = value;
                RaisePropertyChanged<object>(nameof(WachttijdvoorspellerVensterTestomgeving), broadcast: true);
            }
        }

        [Category("CCOL specifieke opties")]
        [Description("CCOL parser password ")]
        public string CCOLParserPassword
        {
            get => _Controller?.Data?.CCOLParserPassword;
            set
            {
                _Controller.Data.CCOLParserPassword = value;
                RaisePropertyChanged<object>(nameof(CCOLParserPassword), broadcast: true);
            }
        }
        
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
        
        [Browsable(false)]
        public bool NotMultiModuleReeksen => !MultiModuleReeksen;

        [Description("Meerdere module reeksen")]
        public bool MultiModuleReeksen
        {
            get => _Controller?.Data?.MultiModuleReeksen ?? false;
            set
            {
                _Controller.Data.MultiModuleReeksen = value;
                RaisePropertyChanged<object>(nameof(MultiModuleReeksen), broadcast: true);
                RaisePropertyChanged(nameof(NotMultiModuleReeksen));

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
            get => _Controller?.Data?.PrmLoggingTfbMax ?? false;
            set
            {
                _Controller.Data.PrmLoggingTfbMax = value;
                RaisePropertyChanged<object>(nameof(PrmLoggingTfbMax), broadcast: true);
            }
        }
        
        [Description("Gebruik functionele namen perioden")]
        public bool GebruikPeriodenNamen
        {
            get => _Controller?.PeriodenData?.GebruikPeriodenNamen ?? false;
            set
            {
                _Controller.PeriodenData.GebruikPeriodenNamen = value;
                RaisePropertyChanged<object>(nameof(GebruikPeriodenNamen), broadcast: true);
            }
        }

        [Description("Opnemen cyclustijdmeting in regeling")]
        [BrowsableCondition("NotMultiModuleReeksen")]
        public bool GenererenCyclustijdMeting 
        {
            get => _Controller?.Data?.GenererenCyclustijdMeting ?? false;
            set
            {
                _Controller.Data.GenererenCyclustijdMeting = value;
                RaisePropertyChanged<object>(nameof(GenererenCyclustijdMeting), broadcast: true);
            }
        }
        
        [Description("Genereren lijst met includes")]
        public bool GenererenIncludesLijst
        {
            get => _Controller?.Data?.GenererenIncludesLijst ?? false;
            set
            {
                _Controller.Data.GenererenIncludesLijst = value;
                RaisePropertyChanged<object>(nameof(GenererenIncludesLijst), broadcast: true);
            }
        }
        
        [Description("Compileren met één bestand")]
        public bool GenererenEnkelCompilatieBestand
        {
            get => _Controller?.Data?.GenererenEnkelCompilatieBestand ?? false;
            set
            {
                _Controller.Data.GenererenEnkelCompilatieBestand = value;
                RaisePropertyChanged<object>(nameof(GenererenEnkelCompilatieBestand), broadcast: true);
            }
        }
        
        [Description("Code in tab.c kleine/hoofd letters")]
        public CCOLCodeCaseEnum CCOLCodeCase
        {
            get => _Controller?.Data?.CCOLCodeCase ?? CCOLCodeCaseEnum.LowerCase;
            set
            {
                _Controller.Data.CCOLCodeCase = value;
                RaisePropertyChanged<object>(nameof(CCOLCodeCase), broadcast: true);
            }
        }

        [Category("Opties ontwikkel omgeving")]
        [Description("VLOG in testomgeving")]
        public bool VLOGInTestOmgeving
        {
            get => _Controller?.Data?.VLOGInTestOmgeving ?? false;
            set
            {
                _Controller.Data.VLOGInTestOmgeving = value;
                RaisePropertyChanged<object>(nameof(VLOGInTestOmgeving), broadcast: true);
            }
        }

        [Description("Genereren code t.b.v. DUURTEST")]
        public bool GenererenDuurtestCode
        {
            get => _Controller?.Data?.GenererenDuurtestCode ?? false;
            set
            {
                _Controller.Data.GenererenDuurtestCode = value;
                RaisePropertyChanged<object>(nameof(GenererenDuurtestCode), broadcast: true);
            }
        }

        [Description("Niet gebruiken bitmap")]
        public bool GebruikBitmap
        {
            get => _Controller?.Data?.NietGebruikenBitmap ?? false;
            set
            {
                _Controller.Data.NietGebruikenBitmap = value;
                RaisePropertyChanged<object>(nameof(GebruikBitmap), broadcast: true);
                Messenger.Default.Send(new UpdateTabsEnabledMessage());
            }
        }

        [Description("Practice omgeving")]
        public bool PracticeOmgeving
        {
            get => _Controller?.Data?.PracticeOmgeving ?? false;
            set
            {
                _Controller.Data.PracticeOmgeving = value;
                RaisePropertyChanged<object>(nameof(PracticeOmgeving), broadcast: true);
            }
        }

        [Description("Opnemen code mirakel monitor")]
        public bool MirakelMonitor
        {
            get => _Controller?.Data?.MirakelMonitor ?? false;
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
