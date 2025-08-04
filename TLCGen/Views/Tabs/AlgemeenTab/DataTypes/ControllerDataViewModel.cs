
using System.ComponentModel;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Controls;
using TLCGen.Dependencies.Providers;
using System;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Dependencies.Models.Enumerations;
using TLCGen.Helpers;

namespace TLCGen.ViewModels
{
    public class ControllerDataViewModel : ObservableObjectEx
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
                OnPropertyChanged("");
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
                OnPropertyChanged(nameof(Naam), broadcast: true);
            }
        }

        public string Nummer
        {
            get => _Controller?.Data?.Nummer;
            set
            {
                _Controller.Data.Nummer = value;
                OnPropertyChanged(nameof(Nummer), broadcast: true);
            }
        }

        public string Stad
        {
            get => _Controller?.Data?.Stad;
            set
            {
                _Controller.Data.Stad = value;
                OnPropertyChanged(nameof(Stad), broadcast: true);
            }
        }

        public string Straat1
        {
            get => _Controller?.Data?.Straat1;
            set
            {
                _Controller.Data.Straat1 = value;
                OnPropertyChanged(nameof(Straat1), broadcast: true);
            }
        }

        public string Straat2
        {
            get => _Controller?.Data?.Straat2;
            set
            {
                _Controller.Data.Straat2 = value;
                OnPropertyChanged(nameof(Straat2), broadcast: true);
            }
        }

        [Description("Bitmap naam")]
        public string BitmapNaam
        {
            get => _Controller?.Data?.BitmapNaam;
            set
            {
                _Controller.Data.BitmapNaam = value;
                OnPropertyChanged(nameof(BitmapNaam), broadcast: true);
WeakReferenceMessengerEx.Default.Send(new UpdateTabsEnabledMessage());
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
					OnPropertyChanged(nameof(VissimNaam), broadcast: true);
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
                OnPropertyChanged(nameof(Fasebewaking), broadcast: true);
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
                        "kan de CCOL versie niet lager dan 9.5 worden ingesteld.", "Intergroen regeling", MessageBoxButton.OK);
                    // see here https://stackoverflow.com/questions/2585183/wpf-combobox-selecteditem-change-to-previous-value
                    DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => OnPropertyChanged(nameof(CCOLVersie))));
                }
                else
                {
                    _Controller.Data.CCOLVersie = value;
                    if(value > CCOLVersieEnum.CCOL8)
                    {
                        if(_Controller.Data.VLOGSettings == null)
                        {
                            _Controller.Data.VLOGSettings = new VLOGSettingsDataModel();
                            Settings.DefaultsProvider.Default.SetDefaultsOnModel(_Controller.Data.VLOGSettings, 
                                value < CCOLVersieEnum.CCOL110 
                                    ? "VLOG300" 
                                    : value < CCOLVersieEnum.CCOL121 
                                        ? "VLOG310" 
                                        : "VLOG330");
                        }
                        if (value >= CCOLVersieEnum.CCOL121 && oldValue < CCOLVersieEnum.CCOL121)
                        {
                            var result = TLCGenDialogProvider.Default.ShowMessageBox(
                                "Vanaf CCOL12.1 is de VLOG versie 3.3. Opnieuw toepassen defaults voor VLOG " +
                                "instellingen tbv. juiste instellingen?", "VLOG 3.3 instellen", MessageBoxButton.YesNo);
                            if (result == MessageBoxResult.Yes)
                            {
                                Settings.DefaultsProvider.Default.SetDefaultsOnModel(_Controller.Data.VLOGSettings, "VLOG330");
                            }
                        }
                        else if (value >= CCOLVersieEnum.CCOL110 && oldValue < CCOLVersieEnum.CCOL110)
                        {
                            var result = TLCGenDialogProvider.Default.ShowMessageBox(
                                "Vanaf CCOL11 is de VLOG versie 3.1. Opnieuw toepassen defaults voor VLOG " +
                                "instellingen tbv. juiste instellingen?", "VLOG 3.1 instellen", MessageBoxButton.YesNo);
                            if (result == MessageBoxResult.Yes)
                            {
                                Settings.DefaultsProvider.Default.SetDefaultsOnModel(_Controller.Data.VLOGSettings, "VLOG310");
                            }
                        }
                    }
                    WeakReferenceMessengerEx.Default.Send(new ControllerIntergreenTimesTypeChangedMessage());
WeakReferenceMessengerEx.Default.Send(new UpdateTabsEnabledMessage());
                    OnPropertyChanged(nameof(CCOLVersie), broadcast: true);
                }
                OnPropertyChanged(nameof(IsCCOLVersieLowerThan9));
                OnPropertyChanged(nameof(IsCCOLVersieHigherThan9));
                OnPropertyChanged(nameof(IsCCOLVersieHigherThanOrEqualTo110));
                OnPropertyChanged(nameof(IsCCOLVersieHigherThanOrEqualTo9));
                WeakReferenceMessengerEx.Default.Send(new UpdateTabsEnabledMessage());
                
                if (oldValue >= CCOLVersieEnum.CCOL110 && _Controller.Data.CCOLVersie < CCOLVersieEnum.CCOL110 &&
                    _Controller.PrioData.HasPrio &&
                    (_Controller.PrioData.PrioIngrepen.Any(x => x.MeldingenData.Inmeldingen.Any(x2 => x2.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde))
                    || _Controller.PrioData.PrioIngrepen.Any(x => x.MeldingenData.Uitmeldingen.Any(x2 => x2.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde))))
                {
                    TLCGenDialogProvider.Default.ShowMessageBox(
                        "In CCOL versies lager dan 11 is prioriteit via de RIS niet beschikbaar; controleer de prio instellingen.",
                        "Prioriteit RIS niet beschikbaar", MessageBoxButton.OK);
                }
                
                WeakReferenceMessengerEx.Default.Send(new CCOLVersionChangedMessage(oldValue, _Controller.Data.CCOLVersie));
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
                                OnPropertyChanged();
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
                                OnPropertyChanged();
                                return;
                            }
                        }
                        _Controller.Data.Intergroen = value;
                        OnPropertyChanged(nameof(Intergroen), broadcast: true);
                    }
                    else
                    {
                        OnPropertyChanged();
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
                OnPropertyChanged(nameof(KWCType), broadcast: true);
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
                OnPropertyChanged(nameof(VLOGType), broadcast: true);
            }
        }
        
        [Description("Geen filebased VLOG indien niet in control")]
        [BrowsableCondition("IsCCOLVersieHigherThanOrEqualTo110")]
        public bool GeenControlGeenFileBasedVLOG
        {
            get => _Controller?.Data?.GeenControlGeenFileBasedVLOG == true;
            set
            {
                _Controller.Data.GeenControlGeenFileBasedVLOG = value;
                OnPropertyChanged(nameof(GeenControlGeenFileBasedVLOG), broadcast: true);
            }
        }

        [Browsable(false)]
        public bool IsCCOLVersieHigherThan9 => CCOLVersie > CCOLVersieEnum.CCOL9;

        [Browsable(false)]
        public bool IsCCOLVersieHigherThanOrEqualTo9 => CCOLVersie >= CCOLVersieEnum.CCOL9;
        
        
        [Browsable(false)]
        public bool IsCCOLVersieHigherThanOrEqualTo110 => CCOLVersie >= CCOLVersieEnum.CCOL110;

        [Browsable(false)]
        public bool IsCCOLVersieLowerThan9 => CCOLVersie < CCOLVersieEnum.CCOL9;

        [Description("Extra meeverlengen in WG")]
        public bool ExtraMeeverlengenInWG
        {
            get => _Controller?.Data?.ExtraMeeverlengenInWG ?? false;
            set
            {
                _Controller.Data.ExtraMeeverlengenInWG = value;
                OnPropertyChanged(nameof(ExtraMeeverlengenInWG), broadcast: true);
            }
        }

        [Description("Aansturing waitsignalen")]
        public AansturingWaitsignalenEnum AansturingWaitsignalen
        {
            get => _Controller?.Data?.AansturingWaitsignalen ?? AansturingWaitsignalenEnum.AanvraagGezet;
            set
            {
                _Controller.Data.AansturingWaitsignalen = value;
                OnPropertyChanged(nameof(AansturingWaitsignalen), broadcast: true);
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
                OnPropertyChanged(broadcast: true);
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
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("Fixatie mogelijk")]
        public bool FixatieMogelijk
        {
            get => _Controller?.Data?.FixatieData.FixatieMogelijk ?? false;
            set
            {
                _Controller.Data.FixatieData.FixatieMogelijk = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("Bijkomen tijdens fixatie")]
        public bool BijkomenTijdensFixatie
        {
            get => _Controller?.Data?.FixatieData.BijkomenTijdensFixatie ?? false;
            set
            {
                _Controller.Data.FixatieData.BijkomenTijdensFixatie = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("Type groentijden")]
        public GroentijdenTypeEnum TypeGroentijden
        {
            get => _Controller?.Data?.TypeGroentijden ?? GroentijdenTypeEnum.MaxGroentijden;
            set
            {
                _Controller.Data.TypeGroentijden = value;
                OnPropertyChanged(broadcast: true);
WeakReferenceMessengerEx.Default.Send(new GroentijdenTypeChangedMessage(value));
            }
        }
        
        [Description("TVGAmax als default groentijdenset")]
        public bool TVGAMaxAlsDefaultGroentijdSet
        {
            get => _Controller?.Data?.TVGAMaxAlsDefaultGroentijdSet ?? false;
            set
            {
                _Controller.Data.TVGAMaxAlsDefaultGroentijdSet = value;
                OnPropertyChanged(broadcast: true);
            }
        }
        

        [Description("Toevoegen OVM code")]
        public bool ToevoegenOVM
        {
            get => _Controller?.Data?.ToevoegenOVM ?? false;
            set
            {
                _Controller.Data.ToevoegenOVM = value;
                OnPropertyChanged(broadcast: true);
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
                OnPropertyChanged(nameof(GeenDetectorGedragInAutomaatOmgeving), broadcast: true);
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
                OnPropertyChanged(nameof(WachttijdvoorspellerVensterTestomgeving), broadcast: true);
            }
        }

        [Category("CCOL specifieke opties")]
        [Description("CCOL parser password")]
        public string CCOLParserPassword
        {
            get => _Controller?.Data?.CCOLParserPassword;
            set
            {
                _Controller.Data.CCOLParserPassword = value;
                OnPropertyChanged(nameof(CCOLParserPassword), broadcast: true);
            }
        }
        
        [Description("Gekoppelde regeling (CCOLMS)")]
        public bool CCOLMulti
        {
            get => _Controller?.Data?.CCOLMulti ?? false;
            set
            {
                _Controller.Data.CCOLMulti = value;
                OnPropertyChanged(nameof(CCOLMulti), broadcast: true);
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
                OnPropertyChanged(nameof(MultiModuleReeksen), broadcast: true);
                OnPropertyChanged(nameof(NotMultiModuleReeksen));

                _Controller.Data.ModulenDisplayBitmapData.Clear();
                if (UitgangPerModule) WeakReferenceMessengerEx.Default.Send(new ModulesChangedMessage());
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
                OnPropertyChanged(nameof(CCOLMultiSlave), broadcast: true);
            }
        }

        [Description("Wtv niet halteren als meer dan # leds")]
        public int WachttijdvoorspellerNietHalterenMax
        {
            get => _Controller?.Data?.WachttijdvoorspellerNietHalterenMax ?? 0;
            set
            {
                _Controller.Data.WachttijdvoorspellerNietHalterenMax = value;
                OnPropertyChanged(nameof(WachttijdvoorspellerNietHalterenMax), broadcast: true);
            }
        }

        [Description("Wtv niet halteren als minder dan # leds")]
        public int WachttijdvoorspellerNietHalterenMin
        {
            get => _Controller?.Data?.WachttijdvoorspellerNietHalterenMin ?? 0;
            set
            {
                _Controller.Data.WachttijdvoorspellerNietHalterenMin = value;
                OnPropertyChanged(nameof(WachttijdvoorspellerNietHalterenMin), broadcast: true);
            }
        }

        [Description("Aansturing BUS tijdens OV ingreep")]
        public bool WachttijdvoorspellerAansturenBus
        {
            get => _Controller?.Data?.WachttijdvoorspellerAansturenBus ?? false;
            set
            {
                _Controller.Data.WachttijdvoorspellerAansturenBus = value;
                OnPropertyChanged(nameof(WachttijdvoorspellerAansturenBus), broadcast: true);
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
                OnPropertyChanged(nameof(WachttijdvoorspellerAansturenBusHD), broadcast: true);
            }
        }

        [Description("Loggen max. TFB in PRM")]
        public bool PrmLoggingTfbMax
        {
            get => _Controller?.Data?.PrmLoggingTfbMax ?? false;
            set
            {
                _Controller.Data.PrmLoggingTfbMax = value;
                OnPropertyChanged(nameof(PrmLoggingTfbMax), broadcast: true);
            }
        }
        
        [Description("Gebruik functionele namen perioden")]
        public bool GebruikPeriodenNamen
        {
            get => _Controller?.PeriodenData?.GebruikPeriodenNamen ?? false;
            set
            {
                _Controller.PeriodenData.GebruikPeriodenNamen = value;
                OnPropertyChanged(nameof(GebruikPeriodenNamen), broadcast: true);
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
                OnPropertyChanged(nameof(GenererenCyclustijdMeting), broadcast: true);
            }
        }
        
        [Description("Genereren lijst met includes")]
        public bool GenererenIncludesLijst
        {
            get => _Controller?.Data?.GenererenIncludesLijst ?? false;
            set
            {
                _Controller.Data.GenererenIncludesLijst = value;
                OnPropertyChanged(nameof(GenererenIncludesLijst), broadcast: true);
            }
        }

        [Description("Compileren met één bestand")]
        public bool GenererenEnkelCompilatieBestand
        {
            get => _Controller?.Data?.GenererenEnkelCompilatieBestand ?? false;
            set
            {
                _Controller.Data.GenererenEnkelCompilatieBestand = value;
                OnPropertyChanged(nameof(GenererenEnkelCompilatieBestand), broadcast: true);
            }
        }

        [Description("Compileren afzonderlijke bronbestanden")]
        public bool LosseBrondbestanden
        {
            get => _Controller?.Data?.LosseBrondbestanden ?? false;
            set
            {
                _Controller.Data.LosseBrondbestanden = value;
                OnPropertyChanged(nameof(LosseBrondbestanden), broadcast: true);
            }
        }

        [Description("Code in tab.c kleine/hoofd letters")]
        public CCOLCodeCaseEnum CCOLCodeCase
        {
            get => _Controller?.Data?.CCOLCodeCase ?? CCOLCodeCaseEnum.LowerCase;
            set
            {
                _Controller.Data.CCOLCodeCase = value;
                OnPropertyChanged(nameof(CCOLCodeCase), broadcast: true);
            }
        }
        
        [Description("Traffick compatabiliteit")]
        public bool TraffickCompatible
        {
            get => _Controller?.Data?.TraffickCompatible ?? false;
            set
            {
                _Controller.Data.TraffickCompatible = value;
                OnPropertyChanged(nameof(TraffickCompatible), broadcast: true);
WeakReferenceMessengerEx.Default.Send(new UpdateTabsEnabledMessage());
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
                OnPropertyChanged(nameof(VLOGInTestOmgeving), broadcast: true);
            }
        }

        [Description("Genereren code t.b.v. DUURTEST")]
        public bool GenererenDuurtestCode
        {
            get => _Controller?.Data?.GenererenDuurtestCode ?? false;
            set
            {
                _Controller.Data.GenererenDuurtestCode = value;
                OnPropertyChanged(nameof(GenererenDuurtestCode), broadcast: true);
            }
        }

        [Description("Niet gebruiken bitmap")]
        public bool GebruikBitmap
        {
            get => _Controller?.Data?.NietGebruikenBitmap ?? false;
            set
            {
                _Controller.Data.NietGebruikenBitmap = value;
                OnPropertyChanged(nameof(GebruikBitmap), broadcast: true);
WeakReferenceMessengerEx.Default.Send(new UpdateTabsEnabledMessage());
            }
        }

        [Description("Practice omgeving")]
        public bool PracticeOmgeving
        {
            get => _Controller?.Data?.PracticeOmgeving ?? false;
            set
            {
                _Controller.Data.PracticeOmgeving = value;
                OnPropertyChanged(nameof(PracticeOmgeving), broadcast: true);
            }
        }
        
        [Description("Parameterwijziging PB bij TVG_max")]
        public bool ParameterwijzigingPBBijTVGMax
        {
            get => _Controller?.Data?.ParameterwijzigingPBBijTVGMax ?? false;
            set
            {
                _Controller.Data.ParameterwijzigingPBBijTVGMax = value;
                OnPropertyChanged(nameof(ParameterwijzigingPBBijTVGMax), broadcast: true);
            }
        }
        
        

        [Description("Opnemen code mirakel monitor")]
        public bool MirakelMonitor
        {
            get => _Controller?.Data?.MirakelMonitor ?? false;
            set
            {
                _Controller.Data.MirakelMonitor = value;
                OnPropertyChanged(nameof(MirakelMonitor), broadcast: true);
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
