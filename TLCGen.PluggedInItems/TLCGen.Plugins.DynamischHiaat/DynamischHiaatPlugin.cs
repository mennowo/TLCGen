using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml;
using TLCGen.Plugins.DynamischHiaat.Models;
using TLCGen.Plugins.DynamischHiaat.ViewModels;
using TLCGen.Plugins.DynamischHiaat.Views;
using TLCGen.Extensions;
using TLCGen.Generators.CCOL.CodeGeneration;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Dependencies.Providers;
using System;
using System.IO;
using TLCGen.Generators.CCOL.CodeGeneration.Functionality;
using TLCGen.Generators.CCOL.CodeGeneration.HelperClasses;
using TLCGen.Models.Enumerations;

namespace TLCGen.Plugins.DynamischHiaat
{
    [TLCGenTabItem(-1, TabItemTypeEnum.DetectieTab)]
    [TLCGenPlugin(TLCGenPluginElems.PlugMessaging | TLCGenPluginElems.TabControl | TLCGenPluginElems.XMLNodeWriter | TLCGenPluginElems.HasSettings)]
    [CCOLCodePieceGenerator]
    public class DynamischHiaatPlugin : CCOLCodePieceGeneratorBase, ITLCGenPlugMessaging, ITLCGenTabItem, ITLCGenXMLNodeWriter, ITLCGenHasSettings, ITLCGenHasSpecification
    {
        #region Fields

        private ControllerModel _controller;
        private const string _myName = "Dynamisch hiaat";
        private DynamischHiaatModel _myModel;
        private DynamischHiaatPluginTabViewModel _myTabViewModel;
        private string _mmk;

        #endregion Fields

        #region Properties

        public DynamischHiaatDefaultsModel MyDefaults { get; private set; }
        #endregion // Properties

        #region ITLCGen shared items

        public ControllerModel Controller
        {
            get => _controller;
            set
            {
                _controller = value;
                if (_controller == null)
                {
                    _myModel = new DynamischHiaatModel();
                    _myTabViewModel.Controller = null;
                    _myTabViewModel.Model = _myModel;
                }
                if (_controller != null && _myModel != null)
                {
                    _myTabViewModel.Controller = _controller;
                }
                UpdateModel();
            }
        }

        public string GetPluginName()
        {
            return _myName;
        }

        #endregion // ITLCGen shared items

        #region ITLCGenPlugMessaging

        public void UpdateTLCGenMessaging()
        {
            _myTabViewModel.UpdateTLCGenMessaging();
        }

        #endregion // ITLCGenPlugMessaging

        #region ITLCGenTabItem

        public string DisplayName => _myName;

        public System.Windows.Media.ImageSource Icon => null;

        DataTemplate _ContentDataTemplate;
        public DataTemplate ContentDataTemplate
        {
            get
            {
                if (_ContentDataTemplate == null)
                {
                    _ContentDataTemplate = new DataTemplate();
                    var tab = new FrameworkElementFactory(typeof(DynamischHiaatPluginTabView));
                    tab.SetValue(FrameworkElement.DataContextProperty, _myTabViewModel);
                    _ContentDataTemplate.VisualTree = tab;
                }
                return _ContentDataTemplate;
            }
        }

        public bool IsEnabled { get; set; }
        
        public bool Visibility { get; set; }

        public bool CanBeEnabled()
        {
            return true;
        }

        public void LoadTabs()
        {
            
        }

        public void OnDeselected()
        {
            
        }

        public bool OnDeselectedPreview()
        {
            return true;
        }

        public void OnSelected()
        {
            if(_myTabViewModel.SelectedDynamischHiaatSignalGroup == null && _myTabViewModel.DynamischHiaatSignalGroups.Any())
            {
                _myTabViewModel.SelectedDynamischHiaatSignalGroup = _myTabViewModel.DynamischHiaatSignalGroups[0];
            }
        }

        public bool OnSelectedPreview()
        {
            return true;
        }

        #endregion ITLCGenTabItem

        #region ITLCGenXMLNodeWriter

        public void GetXmlFromDocument(XmlDocument document)
        {
            _myModel = null;

            foreach (XmlNode node in document.FirstChild.ChildNodes)
            {
                if (node.LocalName == "DynamischHiaat")
                {
                    _myModel = XmlNodeConverter.ConvertNode<DynamischHiaatModel>(node);
                    break;
                }
            }

            if (_myModel == null)
            {
                _myModel = new DynamischHiaatModel();
            }
            _myTabViewModel.Model = _myModel;
            _myTabViewModel.OnPropertyChanged("");
        }

        public void SetXmlInDocument(XmlDocument document)
        {
            var doc = TLCGenSerialization.SerializeToXmlDocument(_myModel);
            var node = document.ImportNode(doc.DocumentElement, true);
            document.DocumentElement.AppendChild(node);
        }

        #endregion // ITLCGenXMLNodeWriter

        #region ITLCGenHasSettings

        public void LoadSettings()
        {
            MyDefaults =
                TLCGenSerialization.DeSerializeData<DynamischHiaatDefaultsModel>(
                    ResourceReader.GetResourceTextFile("TLCGen.Plugins.DynamischHiaat.Settings.DynamischHiaatDefaults.xml", this));

            var appdatpath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var setpath = Path.Combine(appdatpath, @"TLCGen\DynamischHiaat\");
            if (!Directory.Exists(setpath))
                Directory.CreateDirectory(setpath);
            var setfile = Path.Combine(setpath, @"defaults.xml");

            if (File.Exists(setfile))
            {
                MyDefaults = TLCGenSerialization.DeSerialize<DynamischHiaatDefaultsModel>(setfile);
            }
            else
            {
                SaveSettings();
            }
        }

        public void SaveSettings()
        {
            // saving not needed: will never change from inside the application
            //var appdatpath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            //var setpath = Path.Combine(appdatpath, @"TLCGen\DynamischHiaat\");
            //if (!Directory.Exists(setpath))
            //    Directory.CreateDirectory(setpath);
            //var setfile = Path.Combine(setpath, @"defaults.xml");
            //
            //TLCGenSerialization.Serialize(setfile, MyDefaults);
        }

        #endregion // ITLCGenHasSettings

        #region CCOLCodePieceGenerator

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            var warning = false;
            foreach (var msg in _myModel.SignaalGroepenMetDynamischHiaat.Where(x => x.HasDynamischHiaat))
            {
                var ofc = c.Fasen.FirstOrDefault(x => x.Naam == msg.SignalGroupName);
                if (ofc != null)
                {
                    if (ofc.AantalRijstroken > 1 && !ofc.ToepassenMK2 && !warning)
                    {
                        warning = true;
                        TLCGenDialogProvider.Default.ShowMessageBox(
                            $"Let op!\n\n" +
                            $"Voor fase {ofc.Naam} met {ofc.AantalRijstroken} rijstroken is toepassen van Meetkriterium2() uitgeschakeld.\n" +
                            $"Dynamische hiaattijden zijn hier voor juist werking van afhankelijk.",
                            "Foutieve signaalgroep instellingen", MessageBoxButton.OK);
                    }
                }

                _myElements.Add(new CCOLElement($"dynhiaat{msg.SignalGroupName}", PrioCodeGeneratorHelper.CAT_Basisfuncties, PrioCodeGeneratorHelper.SUBCAT_Verlengen, 1, CCOLElementTimeTypeEnum.SCH_type, CCOLElementTypeEnum.Schakelaar, $"Toepassen dynamisch hiaat bij fase {msg.SignalGroupName}"));
                _myElements.Add(new CCOLElement($"opdrempelen{msg.SignalGroupName}", PrioCodeGeneratorHelper.CAT_Basisfuncties, PrioCodeGeneratorHelper.SUBCAT_Verlengen, CCOLElementTypeEnum.HulpElement, $"Opdrempelen toepassen voor fase {msg.SignalGroupName}"));
                _myElements.Add(new CCOLElement($"opdrempelen{msg.SignalGroupName}", PrioCodeGeneratorHelper.CAT_Basisfuncties, PrioCodeGeneratorHelper.SUBCAT_Verlengen, msg.Opdrempelen ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, CCOLElementTypeEnum.Schakelaar, $"Opdrempelen toepassen voor fase {msg.SignalGroupName}"));
                _myElements.Add(new CCOLElement($"geendynhiaat{msg.SignalGroupName}", PrioCodeGeneratorHelper.CAT_Basisfuncties, PrioCodeGeneratorHelper.SUBCAT_Verlengen, CCOLElementTypeEnum.HulpElement, "Tegenhouden toepassen dynamische hiaattijden voor fase " + msg.SignalGroupName));
                _myElements.Add(new CCOLElement($"edkop_{msg.SignalGroupName}", PrioCodeGeneratorHelper.CAT_Basisfuncties, PrioCodeGeneratorHelper.SUBCAT_Verlengen, msg.KijkenNaarKoplus ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, CCOLElementTypeEnum.Schakelaar, $"Start timers dynamische hiaat fase {msg.SignalGroupName} op einde detectie koplus"));
                foreach(var d in msg.DynamischHiaatDetectoren)
                {
                    _myElements.Add(new CCOLElement($"{d.DetectorName}_1", PrioCodeGeneratorHelper.CAT_Basisfuncties, PrioCodeGeneratorHelper.SUBCAT_Verlengen, d.Moment1, CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Timer, $"Dynamische hiaattijden moment 1 voor detector {d.DetectorName}"));
                    _myElements.Add(new CCOLElement($"{d.DetectorName}_2", PrioCodeGeneratorHelper.CAT_Basisfuncties, PrioCodeGeneratorHelper.SUBCAT_Verlengen, d.Moment2, CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Timer, $"Dynamische hiaattijden moment 2 voor detector {d.DetectorName}"));
                    _myElements.Add(new CCOLElement($"tdh_{d.DetectorName}_1", PrioCodeGeneratorHelper.CAT_Basisfuncties, PrioCodeGeneratorHelper.SUBCAT_Verlengen, d.TDH1, CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Timer, $"Dynamische hiaattijden TDH 1 voor detector {d.DetectorName}"));
                    _myElements.Add(new CCOLElement($"tdh_{d.DetectorName}_2", PrioCodeGeneratorHelper.CAT_Basisfuncties, PrioCodeGeneratorHelper.SUBCAT_Verlengen, d.TDH2, CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Timer, $"Dynamische hiaattijden TDH 2 voor detector {d.DetectorName}"));
                    _myElements.Add(new CCOLElement($"max_{d.DetectorName}", PrioCodeGeneratorHelper.CAT_Basisfuncties, PrioCodeGeneratorHelper.SUBCAT_Verlengen, d.Maxtijd, CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Timer, $"Dynamische hiaattijden maximale tijd 2 voor detector {d.DetectorName}"));
                    _myElements.Add(new CCOLElement($"verleng_{d.DetectorName}", PrioCodeGeneratorHelper.CAT_Basisfuncties, PrioCodeGeneratorHelper.SUBCAT_Verlengen, CCOLElementTypeEnum.HulpElement, $"Instructie verlengen op detector {d.DetectorName} ongeacht dynamische hiaat"));
                    var schprm = 0;
                    if (d.SpringStart) schprm += 0x01;
                    if (d.VerlengNiet) schprm += 0x02;
                    if (d.VerlengExtra) schprm += 0x04;
                    if (d.DirectAftellen) schprm += 0x08;
                    if (d.SpringGroen) schprm += 0x10;
                    _myElements.Add(new CCOLElement($"springverleng_{d.DetectorName}", PrioCodeGeneratorHelper.CAT_Basisfuncties, PrioCodeGeneratorHelper.SUBCAT_Verlengen, schprm, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter, $"Dyn. hiaattij instelling voor det. {d.DetectorName} (via bitsturing)"));
                }
            }
        }

        public override bool HasCCOLElements() => true;

        public override IEnumerable<CCOLLocalVariable> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type)
        {
            if (!_myModel.SignaalGroepenMetDynamischHiaat.Any(x => x.HasDynamischHiaat)) return base.GetFunctionLocalVariables(c, type);
            return type switch
            {
                CCOLCodeTypeEnum.RegCMeetkriterium => new List<CCOLLocalVariable>{new("int", "d", defineCondition:"(defined TDHAMAX)")},
                _ => base.GetFunctionLocalVariables(c, type)
            };
        }

        public override int[] HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCIncludes: return new []{115};
                case CCOLCodeTypeEnum.RegCInitApplication: return new []{115};
                case CCOLCodeTypeEnum.RegCPreApplication: return new []{115};
                case CCOLCodeTypeEnum.RegCMeetkriterium: return new []{9, 115};
                case CCOLCodeTypeEnum.SysHDefines: return new []{115};
                default:
                    return null;
            }
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts, int order)
        {
            var sb = new StringBuilder();

            var sgs = _myModel.SignaalGroepenMetDynamischHiaat.Where(x => x.HasDynamischHiaat);
            if (!sgs.Any()) return "";

            switch (type)
            {
                case CCOLCodeTypeEnum.SysHDefines:
                    //if (c.Data.CCOLVersie >= CCOLVersieEnum.CCOL110 && c.Data.TDHAMaxToepassen)
                    //{
                    //    sb.AppendLine($"#define TDHAMAX /* gebruik van TDHA_max[] */");
                    //}
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCIncludes:
                    sb.AppendLine($"{ts}#include \"dynamischhiaat.c\"");
                    return sb.ToString();
                
                case CCOLCodeTypeEnum.RegCInitApplication:
                    sb.AppendLine($"{ts}#if (CCOL_V >= 110 && !defined TDHAMAX) || (CCOL_V < 110)");
                    sb.AppendLine($"{ts}{ts}init_tdhdyn();");
                    sb.AppendLine($"{ts}#endif");
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCPreApplication:
                    sb.AppendLine($"{ts}/* Instellen basis waarde hulpelementen 'geen dynamisch hiaat gebruiken'.");
                    sb.AppendLine($"{ts}   Dit hulpelement kan in gebruikers code worden gebruikt voor eigen aansturing. */");
                    foreach (var sg in sgs)
                    {
                        sb.AppendLine($"{ts}IH[{_hpf}geendynhiaat{sg.SignalGroupName}] = !SCH[{_schpf}dynhiaat{sg.SignalGroupName}];");
                    }
                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* Instellen basis waarde hulpelementen opdrempelen t.b.v. dynamische hiaattijden.");
                    sb.AppendLine($"{ts}   Dit hulpelement kan in gebruikers code worden gebruikt voor eigen aansturing. */");
                    foreach (var sg in sgs)
                    {
                        sb.AppendLine($"{ts}IH[{_hpf}opdrempelen{sg.SignalGroupName}] = SCH[{_schpf}opdrempelen{sg.SignalGroupName}];");
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCMeetkriterium:
                    switch (order)
                    {
                        case 9:
                            //if (c.Data.CCOLVersie >= CCOLVersieEnum.CCOL110 && c.Data.TDHAMaxToepassen)
                            //{
                            //    sb.AppendLine("#ifdef TDHAMAX");
                            //    sb.AppendLine($"{ts}/* TDH_max vullen bij gebruik van TDHAMAX */");
                            //    sb.AppendLine($"{ts}for (d = 0; d < DP_MAX; ++d)");
                            //    sb.AppendLine($"{ts}{{");
                            //    sb.AppendLine($"{ts}{ts}TDH_max[d] = TDHA_max[d];");
                            //    sb.AppendLine($"{ts}}}");
                            //    sb.AppendLine("#endif // TDHAMAX");
                            //}

                            break;
                        case 115:
                            foreach(var sg in sgs)
                            {
                                var ofc = c.Fasen.FirstOrDefault(x => x.Naam == sg.SignalGroupName);
                                if (ofc == null) continue;
                                sb.AppendLine($"{ts}hiaattijden_verlenging(IH[{_hpf}geendynhiaat{sg.SignalGroupName}], SCH[{_schpf}edkop_{sg.SignalGroupName}], {(c.Data.ExtraMeeverlengenInWG ? "TRUE" : "FALSE")}, {_mpf}{_mmk}{sg.SignalGroupName}, IH[{_hpf}opdrempelen{sg.SignalGroupName}], {_fcpf}{sg.SignalGroupName}, ");
                                for (var i = 0; i < ofc.AantalRijstroken; i++)
                                {
                                    foreach(var dd in sg.DynamischHiaatDetectoren)
                                    {
                                        var od = ofc.Detectoren.FirstOrDefault(x => x.Naam == dd.DetectorName);
                                        if (od == null || od.Rijstrook - 1 != i) continue;
                                        sb.AppendLine(
                                            $"{ts}{ts}{i + 1}, " +
                                            $"{_dpf}{od.Naam}, " +
                                            $"{_tpf}{dd.DetectorName}_1, " +
                                            $"{_tpf}{dd.DetectorName}_2, " +
                                            $"{_tpf}tdh_{dd.DetectorName}_1, " +
                                            $"{_tpf}tdh_{dd.DetectorName}_2, " +
                                            $"{_tpf}max_{dd.DetectorName}, " +
                                            $"{_prmpf}springverleng_{dd.DetectorName}, " +
                                            $"{_hpf}verleng_{dd.DetectorName}, ");
                                    }
                                }
                                sb.AppendLine($"{ts}{ts}END);");
                            }
                            break;
                    }
                    return sb.ToString();
                default:
                    return "";
            }
        }

        public override List<string> GetSourcesToCopy()
        {
            if (!_myModel.SignaalGroepenMetDynamischHiaat.Any(x => x.HasDynamischHiaat)) return null;
            return new List<string>
            {
                "dynamischhiaat.c"
            };
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _mmk = CCOLGeneratorSettingsProvider.Default.GetElementName("mmk");
            return base.SetSettings(settings);
        }

        #endregion // CCOLCodePieceGenerator

        #region Private Methods

        internal void UpdateModel()
        {
            if (_controller != null && _myModel != null)
            {
                foreach (var fc in Controller.Fasen)
                {
                    if (fc.Type == TLCGen.Models.Enumerations.FaseTypeEnum.Auto &&
                        _myTabViewModel.DynamischHiaatSignalGroups.All(x => x.SignalGroupName != fc.Naam))
                    {
                        var msg = new DynamischHiaatSignalGroupViewModel(new DynamischHiaatSignalGroupModel { SignalGroupName = fc.Naam });
                        msg.SelectedDefault = MyDefaults.Defaults.FirstOrDefault(x => x.Name == _myModel.TypeDynamischHiaat);
                        if (string.IsNullOrEmpty(msg.Snelheid) || !msg.SelectedDefault.Snelheden.Any(x => x.Name == msg.Snelheid))
                        {
                            if (msg.SelectedDefault != null) msg.Snelheid = msg.SelectedDefault.DefaultSnelheid;
                        }
                        _myTabViewModel.DynamischHiaatSignalGroups.Add(msg);
                    }
                }
                var rems = new List<DynamischHiaatSignalGroupViewModel>();
                foreach (var fc in _myTabViewModel.DynamischHiaatSignalGroups)
                {
                    if (Controller.Fasen.All(x => x.Naam != fc.SignalGroupName) || Controller.Fasen.Any(x => x.Naam == fc.SignalGroupName && x.Type != TLCGen.Models.Enumerations.FaseTypeEnum.Auto))
                    {
                        rems.Add(fc);
                    }
                }
                foreach (var sg in rems)
                {
                    _myTabViewModel.DynamischHiaatSignalGroups.Remove(sg);
                }
                _myTabViewModel.DynamischHiaatSignalGroups.BubbleSort();
                _myTabViewModel.OnPropertyChanged("");
            }
        }

        #endregion // Private Methods

        #region Constructor

        public DynamischHiaatPlugin()
        {
            _myTabViewModel = new DynamischHiaatPluginTabViewModel(this);
        }

        #endregion //Constructor

        #region ITLCGenHasSpecification

        public SpecificationData GetSpecificationData(ControllerModel c)
        {
            if (!_myModel.SignaalGroepenMetDynamischHiaat.Any(x => x.HasDynamischHiaat)) return null;

            var data = new SpecificationData
            {
                Subject = SpecificationSubject.DynHiaat
            };

            data.Elements.Add(new SpecificationParagraph
            {
                Type = SpecificationParagraphType.Header2,
                Text = "Dynamische hiaattijden"
            });

            data.Elements.Add(new SpecificationParagraph
            {
                Type = SpecificationParagraphType.Body,
                Text = "De regeling is voorzien van dynamische hiaattijden. Hiermee kan - afhankelijk van de ingestelde waarden - zowel " +
                "een Groen op Maat (GOM) detectieveld als een detectieveld conform het IVER rapport uit 2018 (IVER'18) worden bediend. " +
                "Dynamische hiaattijden zijn bedoeld om op efficiënte wijze groen te verlengen, waarbij aan de 'voorkant' " +
                "minder vastgroen en geen koplusmaximum nodig zijn, en aan de 'achterkant' gebruik gemaakt kan worden " +
                "van een deel van de geeltijd."
            });
            data.Elements.Add(new SpecificationParagraph
            {
                Type = SpecificationParagraphType.Body,
                Text = "De methodiek is als GOM ontwikkeld door Luuk Misdom en IT&T (nu Vialis) en in augustus 2018 in aangepaste vorm " +
                "overgenomen door IVER. Zie voor de rapportage 'Onderzoek detectieconfiguratie en signaalgroepafhandeling' " +
                "van Goudappel Coffeng (in opdracht van IVER): https://www.crow.nl/thema-s/verkeersmanagement/iver onder 'Downloads'."
            });
            data.Elements.Add(new SpecificationParagraph
            {
                Type = SpecificationParagraphType.Body,
                Text = "Bij dynamische hiaattijden volgens GOM of IVER'18 wordt de ingestelde maximum hiaattijd op de verschillende detectielussen " +
                "na een instelbare tijd lager naarmate de (groen)tijd vordert. Het aflopen van de maximum hiaattijd begint op 'moment 1' " +
                "en eindigt op 'moment 2'. Vanaf start groen tot aan moment 1 geldt TDH 1, vanaf moment 2 tot aan einde groen geldt TDH 2. " +
                "Tussen moment 1 en moment 2 loopt de maximum hiaattijd lineair af van TDH 1 tot TDH 2. " +
                "In de tabel aan het eind van deze paragraaf wordt een overzicht gegeven van de gebruikte instellingen, die eerst worden verklaard: "
            });

            var bulletlist = new List<Tuple<string, int>>();

             bulletlist.Add(new Tuple<string, int>("SCH edkop_##:                                                                                                                                   " +
                "Betreft keuze voor moment start aftellen t.b.v. moment 1 en moment 2. AAN is beginnen op einde detectie koplus, UIT is beginnen op start groen.", 0));
            bulletlist.Add(new Tuple<string, int>("SCH opdrempelen##:                                                                                                                              " +
                "Keuze wel / niet opdrempelen; bij AAN mag er worden opgedrempeld, bij UIT geldt gescheiden hiaatmeting per rijstrook (aleen bij meerdere rijstroken per signaalgroep).", 0));
            bulletlist.Add(new Tuple<string, int>("PRM springverleng_$$:                                                                                                                           " +
                "BITsgewijze instelling van het gewenste gedrag van de detectoren onder verschillende omstandigheden, vaak bedoeld om onderscheid " +
                "te kunnen maken in detectiegedrag tussen verkeer op snelheid en vertragend of optrekkend verkeer. Daarbij geldt:", 0));
            bulletlist.Add(new Tuple<string, int>(" 1 = SpringStart:                                                                                                                               " +
                "op start groen, als er geen hiaatmeting is op de stroomafwaartse lussen, meteen naar de 2e / lagere hiaattijd overgaan: ", 1));
            bulletlist.Add(new Tuple<string, int>(" 2 = VerlengNiet:                                                                                                                               " +
                "op start groen, als er geen hiaatmeting (meer) aktief is op deze en de stroomafwaartse lussen, de verlengfunctie UITschakelen", 1));
            bulletlist.Add(new Tuple<string, int>(" 4 = VerlengExtra:                                                                                                                              " +
                "altijd verlengen op deze lus; bijvoorbeeld bij permanente aanwezigheid deelconflict", 1));
            bulletlist.Add(new Tuple<string, int>(" 8 = DirectAftel:                                                                                                                               " +
                "tijdens groen, als er wél hiaatmeting is op deze lus maar niet op de stroomafwaartse lussen, meteen TDH_max[] gaan aftellen", 1));
            bulletlist.Add(new Tuple<string, int>("16 = SpringGroen:                                                                                                                               " +
                "wanneer tijdens groen het hiaat valt, wordt de volgende detector stroomopwaarts de aktieve verlenglus", 1));
            bulletlist.Add(new Tuple<string, int>("T $$_1: moment 1 (tijd na start groen)", 0));
            bulletlist.Add(new Tuple<string, int>("T $$_2: moment 2 (tijd na start groen)", 0));
            bulletlist.Add(new Tuple<string, int>("T tdh_$$_1: TDH van start groen tot aan moment 1", 0));
            bulletlist.Add(new Tuple<string, int>("T tdh_$$_2: TDH van moment 2 tot aan einde groen", 0));
            bulletlist.Add(new Tuple<string, int>("T max_$$: maximale tijd na start groen dat de detector mag verlengen (0 = de vigerende verlenggroentijd aanhouden)", 0));

            data.Elements.Add(new SpecificationBulletList { BulletData = bulletlist });

            data.Elements.Add(new SpecificationParagraph
            {
                Type = SpecificationParagraphType.Spacer,
                Text = ""
            });

            // table heading
            data.Elements.Add(new SpecificationParagraph
            {
                Type = SpecificationParagraphType.TableHeader,
                Text = "Instellingen dynamische hiaattijden"
            });

            var table = new List<List<string>>();
            table.Add(new List<string>
            {
                    "Fase" + " (##)",
                    "Snelheid",
                    "Toepassen                            SCH dynhiaat##",
                    "Opdrempelen                          SCH opdrempelen##",
                    "Start op ED koplus                   SCH edkop_##",
                    "Detector" + " ($$)",
                  //"Rijstrook",
                    "Moment 1                             T $$_1",
                    "Moment 2                             T $$_2",
                    "TDH 1                                T tdh_$$_1",
                    "TDH 2                                T tdh_$$_2",
                    "Max. verlengen                       T max_$$",
                    "Detector instelling                  PRM springverleng_$$",
            });
            string eerste = "";
            foreach (var fc in _myModel.SignaalGroepenMetDynamischHiaat)
            {
                foreach (var d in fc.DynamischHiaatDetectoren)
                {
                    var schprm = 0;
                    if (d.SpringStart) schprm += 0x01;
                    if (d.VerlengNiet) schprm += 0x02;
                    if (d.VerlengExtra) schprm += 0x04;
                    if (d.DirectAftellen) schprm += 0x08;
                    if (d.SpringGroen) schprm += 0x10;
                    table.Add(new List<string>
                    {
                        (fc.SignalGroupName != eerste) ? fc.SignalGroupName.ToString() : "",
                        (fc.SignalGroupName != eerste) ? fc.Snelheid.ToString() : "",
                        (fc.SignalGroupName != eerste) ? (fc.HasDynamischHiaat ? "Aan" : "Uit") : "",
                        (fc.SignalGroupName != eerste) ? (fc.Opdrempelen ? "Aan" : "Uit") : "",
                        (fc.SignalGroupName != eerste) ? (fc.KijkenNaarKoplus ? "Aan" : "Uit") : "",
                    
                        d.DetectorName.ToString(),
                      //rijstrook
                        d.Moment1.ToString(),
                        d.Moment2.ToString(),
                        d.TDH1.ToString(),
                        d.TDH2.ToString(),
                        d.Maxtijd.ToString(),
                        schprm.ToString(),
                    });
                    eerste = fc.SignalGroupName;
                }
            }

            data.Elements.Add(new SpecificationTable { TableData = table });

            data.Elements.Add(new SpecificationParagraph
            {
                Type = SpecificationParagraphType.Spacer,
                Text = ""
            });

            return data;
        }

        #endregion //ITLCGenHasSpecification
    }
}
