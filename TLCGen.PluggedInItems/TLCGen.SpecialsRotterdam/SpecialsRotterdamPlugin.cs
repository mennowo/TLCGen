using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using TLCGen.Generators.CCOL.CodeGeneration;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Plugins;

namespace TLCGen.SpecialsRotterdam
{
    [CCOLCodePieceGenerator]
    [TLCGenTabItem(-1, TabItemTypeEnum.SpecialsTab)]
    [TLCGenPlugin(TLCGenPluginElems.PlugMessaging | 
                  TLCGenPluginElems.TabControl | 
                  TLCGenPluginElems.XMLNodeWriter)]
    public class SpecialsRotterdamPlugin : CCOLCodePieceGeneratorBase, ITLCGenPlugMessaging, ITLCGenTabItem, ITLCGenXMLNodeWriter
    { 
        #region Fields

        private SpecialsRotterdamViewModel _SpecialsRotterdamTabVM;
        private SpecialsRotterdamModel _MyModel;

        #endregion // Fields

        #region Properties

        #endregion // Properties

        #region ITLCGen plugin shared items

        private ControllerModel _Controller;
        public ControllerModel Controller
        {
            get { return _Controller; }
            set
            {
                _Controller = value;
                if(_Controller == null)
                {
                    _MyModel = new SpecialsRotterdamModel();
                    _SpecialsRotterdamTabVM.Specials = _MyModel;
                }
            }
        }

        public string DisplayName
        {
            get
            {
                return "Rotterdam";
            }
        }

        public string GetPluginName()
        {
            return "SpecialsRotterdam";
        }

        private bool _IsEnabled;
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set
            {
                _IsEnabled = value;
            }
        }

        #endregion // ITLCGen plugin shared items

        #region ITLCGenTabItem

        DataTemplate _ContentDataTemplate;
        public DataTemplate ContentDataTemplate
        {
            get
            {
                if (_ContentDataTemplate == null)
                {
                    _ContentDataTemplate = new DataTemplate();
                    var tab = new FrameworkElementFactory(typeof(SpecialsRotterdamView));
                    tab.SetValue(SpecialsRotterdamView.DataContextProperty, _SpecialsRotterdamTabVM);
                    _ContentDataTemplate.VisualTree = tab;
                }
                return _ContentDataTemplate;
            }
        }

        public ImageSource Icon
        {
            get
            {
                //ResourceDictionary dict = new ResourceDictionary();
                //Uri u = new Uri("pack://application:,,,/" +
                //    System.Reflection.Assembly.GetExecutingAssembly().GetName().Name +
                //    ";component/" + "Resources/Icon.xaml");
                //dict.Source = u;
                //return (DrawingImage)dict["AdditorIconDrawingImage"];
                return null;
            }
        }

        public bool CanBeEnabled()
        {
            return true;
        }

        public void OnSelected()
        {

        }

        public bool OnSelectedPreview()
        {
            return true;
        }

        public void OnDeselected()
        {

        }

        public bool OnDeselectedPreview()
        {
            return true;
        }


        public void LoadTabs()
        {

        }

        #endregion // ITLCGenTabItem

        #region ITLCGenPlugMessaging

        public void UpdateTLCGenMessaging()
        {
            _SpecialsRotterdamTabVM?.UpdateTLCGenMessaging();
        }

        #endregion // ITLCGenPlugMessaging

        #region ITLCGenXMLNodeWriter

        public void GetXmlFromDocument(XmlDocument document)
        {
            _MyModel = null;

            foreach (XmlNode node in document.FirstChild.ChildNodes)
            {
                if (node.LocalName == "SpecialsRotterdam")
                {
                    _MyModel = XmlNodeConverter.ConvertNode<SpecialsRotterdamModel>(node);
                    break;
                }
            }

            if (_MyModel == null)
            {
                _MyModel = new SpecialsRotterdamModel();
            }
            _SpecialsRotterdamTabVM.Specials = _MyModel;
            _SpecialsRotterdamTabVM.RaisePropertyChanged("");
        }

        public void SetXmlInDocument(XmlDocument document)
        {
            XmlDocument doc = TLCGenSerialization.SerializeToXmlDocument(_MyModel);
            XmlNode node = document.ImportNode(doc.DocumentElement, true);
            document.DocumentElement.AppendChild(node);
        }

        #endregion // ITLCGenXMLNodeWriter

        #region CCOLCodePieceGenerator

        private List<CCOLElement> _MyElements;
        private List<string> _FasenWithDummies;

        public override void CollectCCOLElements(ControllerModel c)
        {
            _FasenWithDummies = new List<string>();
            _MyElements = new List<CCOLElement>();

            #region OVM

            if (_MyModel.ToevoegenOVM)
            {
                foreach (var fc in c.Fasen.Where(x => x.Type == FaseTypeEnum.Auto && !(x.Naam.Length == 3 && x.Naam.StartsWith("9"))))
                {
                    _MyElements.Add(new CCOLElement($"ovmextragroen_{fc.Naam}", 0, CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Parameter));
                    _MyElements.Add(new CCOLElement($"ovmmindergroen_{fc.Naam}", 0, CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Parameter));
                }
            }

            #endregion // OVM

            #region AFM

            if (_MyModel.ToepassenAFM)
            {
                // Search fasen with dummies
                bool error = false;
                foreach (var fc in c.Fasen)
                {
                    foreach (var fc2 in c.Fasen)
                    {
                        if(fc2.Naam.Length == 3 && fc2.Naam.StartsWith("9") && fc.Naam == fc2.Naam.Substring(1))
                        {
                            _FasenWithDummies.Add(fc.Naam);
                            if (!error &&
                                (fc2.Meeverlengen != NooitAltijdAanUitEnum.Nooit ||
                                 fc2.Wachtgroen != NooitAltijdAanUitEnum.Nooit ||
                                 fc2.VasteAanvraag != NooitAltijdAanUitEnum.Nooit ||
                                 ((c.ModuleMolen.FasenModuleData.Any() &&
                                   c.ModuleMolen.FasenModuleData.Any(x => x.FaseCyclus == fc2.Naam) &&
                                   c.ModuleMolen.FasenModuleData.First(x => x.FaseCyclus == fc2.Naam).AlternatiefToestaan))))
                            {
                                error = true;
                                MessageBox.Show($"Dummy fase {fc2.Naam} is niet juist als dummy geconfigureerd.\nControleer: nooit meeverlengen/wachtgroen/vaste aanvraag, en niet alternatief",
                                                "AFM: Foutieve instellingen dummy fase");
                            }
                        }
                    }
                }

                foreach (var fc in c.Fasen)
                {
                    if (fc.Type == FaseTypeEnum.Auto || fc.Type == FaseTypeEnum.OV)
                    {
                        int i = 0;
                        if (Int32.TryParse(fc.Naam, out i))
                        {
                            if (i < 900)
                            {
                                _MyElements.Add(new CCOLElement($"AFM{fc.Naam}_FC", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                                _MyElements.Add(new CCOLElement($"AFM{fc.Naam}_GmaxCCOL", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                                _MyElements.Add(new CCOLElement($"AFM{fc.Naam}_GmaxMin", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                                _MyElements.Add(new CCOLElement($"AFM{fc.Naam}_GmaxMax", 80, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                                _MyElements.Add(new CCOLElement($"AFM{fc.Naam}_Gmaxact", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                                _MyElements.Add(new CCOLElement($"AFM{fc.Naam}_Gmaxgem", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                                _MyElements.Add(new CCOLElement($"AFM{fc.Naam}_Afgekapt", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                                _MyElements.Add(new CCOLElement($"AFM{fc.Naam}_GmaxAFM", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                                _MyElements.Add(new CCOLElement($"AFM{fc.Naam}_Sturing", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                                _MyElements.Add(new CCOLElement($"AFM{fc.Naam}_Qlenght", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                                _MyElements.Add(new CCOLElement($"AFM{fc.Naam}_AbsBufferRuimte", 100, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                                _MyElements.Add(new CCOLElement($"AFM{fc.Naam}_RelBufferRuimte", 100, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                                _MyElements.Add(new CCOLElement($"AFM{fc.Naam}_RelBufferVulling", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                            }
                        }
                    }
                }
                _MyElements.Add(new CCOLElement("AFM_Strikt", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                _MyElements.Add(new CCOLElement("AFM_TC", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                _MyElements.Add(new CCOLElement("AFM_TCgem", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                _MyElements.Add(new CCOLElement("AFM_Watchdog", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                _MyElements.Add(new CCOLElement("AFM_WatchdogReturn", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                _MyElements.Add(new CCOLElement("AFM_Versie", 5, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                _MyElements.Add(new CCOLElement("AFM_Beschikbaar", 5, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));

                _MyElements.Add(new CCOLElement("AFMLeven", CCOLElementTypeEnum.Uitgang));

                _MyElements.Add(new CCOLElement("AFMLeven", 120, CCOLElementTimeTypeEnum.TS_type, CCOLElementTypeEnum.Timer));
                _MyElements.Add(new CCOLElement("VRILeven", 60, CCOLElementTimeTypeEnum.TS_type, CCOLElementTypeEnum.Timer));
                _MyElements.Add(new CCOLElement("AFMExtraGroenBijFile", 1, CCOLElementTimeTypeEnum.SCH_type, CCOLElementTypeEnum.Schakelaar));
            }

            #endregion // AFM

            #region Logging TFB max

            if(_MyModel.PrmLoggingTfbMax)
            {
                _MyElements.Add(new CCOLElement("tfbfc", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                _MyElements.Add(new CCOLElement("tfbmax", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                _MyElements.Add(new CCOLElement("tfbtijd", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                _MyElements.Add(new CCOLElement("tfbdat", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                _MyElements.Add(new CCOLElement("tfbjaar", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
            }

            #endregion // Logging TFB max
        }

        public override bool HasCCOLElements()
        {
            return true;
        }

        public override IEnumerable<CCOLElement> GetCCOLElements(CCOLElementTypeEnum type)
        {
            return _MyElements.Where(x => x.Type == type);
        }

        public override int HasCode(CCOLRegCCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLRegCCodeTypeEnum.Top:
                    return 5;
                case CCOLRegCCodeTypeEnum.InitApplication:
                    return 2;
                case CCOLRegCCodeTypeEnum.PreApplication:
                    return 1;
                case CCOLRegCCodeTypeEnum.Alternatieven:
                    return 1;
                case CCOLRegCCodeTypeEnum.PostApplication:
                    return 1;
                case CCOLRegCCodeTypeEnum.PostSystemApplication:
                    return 2;
                default:
                    return 0;
            }
        }

        public override string GetCode(ControllerModel c, CCOLRegCCodeTypeEnum type, string ts)
        {
            StringBuilder sb = new StringBuilder();

            switch (type)
            {
                case CCOLRegCCodeTypeEnum.Top:
                    if (!_MyModel.ToepassenAFM)
                        return "";
                    sb.AppendLine("/* Ten behoeve van AFM */");
                    int index = 0;
                    foreach (var fc in _FasenWithDummies)
                    {
                        sb.AppendLine($"#define AFM_{_fcpf}{fc} {index++}");
                    }
                    sb.AppendLine($"#define AFM_{_fcpf}max {index}");
                    sb.AppendLine("#include \"AFMroutines.c\"");
                    sb.AppendLine("static AFM_FC_STRUCT verwerken_fcs[AFM_fcmax] = { 0 };");
                    sb.AppendLine($"int prmAFM_watchdog_return_old;");
                    sb.AppendLine($"int TVG_temp[FCMAX];");
                    sb.AppendLine();
                    return sb.ToString();

                case CCOLRegCCodeTypeEnum.InitApplication:
                    if (!_MyModel.ToepassenAFM)
                        return "";
                    sb.AppendLine($"{ts}/* Initialiseer AFM routines */");
                    sb.AppendLine($"{ts}AFMinit();");
                    sb.AppendLine();
                    foreach (var fc in _FasenWithDummies)
                    {
                        sb.AppendLine($"{ts}AFM_fc_initfc(&verwerken_fcs[AFM_{_fcpf}{fc}], {_fcpf}{fc}, prmAFM{fc}_FC);");
                    }
                    sb.AppendLine();
                    return sb.ToString();

                case CCOLRegCCodeTypeEnum.PreApplication:
                    if (!_MyModel.ToepassenAFM)
                        return "";
                    sb.AppendLine("#if defined AUTOMAAT && !defined VISSIM ");
                    sb.AppendLine($"{ts}RT[{_tpf}AFMLeven] = (PRM[{_prmpf}AFM_WatchdogReturn] != prmAFM_watchdog_return_old);");
                    sb.AppendLine("#else");
                    sb.AppendLine($"{ts}RT[{_tpf}AFMLeven] = TRUE;");
                    sb.AppendLine("#endif");
                    sb.AppendLine($"{ts}prmAFM_watchdog_return_old = PRM[{_prmpf}AFM_Watchdog];");
                    sb.AppendLine($"{ts}CIF_GUS[{_uspf}AFMLeven] = RT[{_tpf}AFMLeven];");
                    sb.AppendLine($"{ts}RT[tVRILeven] = !T[tVRILeven];");
                    sb.AppendLine($"{ts}if (ET[{_tpf}VRILeven])");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}if (PRM[{_prmpf}AFM_Watchdog] < 9999) PRM[{_prmpf}AFM_Watchdog]++;");
                    sb.AppendLine($"{ts}{ts}else                            PRM[{_prmpf}AFM_Watchdog] = 0;");
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}AFMResetBits();");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* Niet bewaken schaduw fasen AFM */");
                    foreach (var fc in _FasenWithDummies)
                    {
                        sb.AppendLine($"{ts}TFB_timer[{_fcpf}9{fc}] = 0;");
                    }
                    sb.AppendLine();
                    return sb.ToString();

                case CCOLRegCCodeTypeEnum.Alternatieven:
                    if (!_MyModel.ToepassenAFM)
                        return "";
                    sb.AppendLine($"{ts}/* AFM */");
                    sb.AppendLine($"{ts}if (T[{_tpf}AFMLeven] && PRM[{_prmpf}AFM_Beschikbaar])");
                    sb.AppendLine($"{ts}{{");
                    foreach (var fc in _FasenWithDummies)
                    {
                        sb.AppendLine($"{ts}{ts}AFMacties_alternatieven(&verwerken_fcs[AFM_{_fcpf}{fc}]);");
                    }
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine();
                    return sb.ToString();

                case CCOLRegCCodeTypeEnum.PostApplication:
                    if (!_MyModel.ToepassenAFM && !_MyModel.ToevoegenOVM)
                        return "";
                    if (_MyModel.ToevoegenOVM)
                    {
                        sb.AppendLine($"{ts}/* OVM Rotterdam: extra/minder groen */");
                        foreach (var fc in c.Fasen.Where(x => x.Type == FaseTypeEnum.Auto && !(x.Naam.Length == 3 && x.Naam.StartsWith("9"))))
                        {
                            sb.AppendLine($"{ts}if (TVG_max[{_fcpf}{fc.Naam}] > -1) TVG_max[{_fcpf}{fc.Naam}] += PRM[{_prmpf}ovmextragroen_{fc.Naam}];");
                            sb.AppendLine($"{ts}if (TVG_max[{_fcpf}{fc.Naam}] > -1) TVG_max[{_fcpf}{fc.Naam}] -= PRM[{_prmpf}ovmmindergroen_{fc.Naam}];");
                        }
                        sb.AppendLine();
                    }
                    if (_MyModel.ToepassenAFM)
                    {
                        sb.AppendLine($"{ts}/* AFM */");
                        foreach (var fc in _FasenWithDummies)
                        {
                            sb.AppendLine($"{ts}TVG_temp[{_fcpf}{fc}] = TVG_max[{_fcpf}{fc}];");
                        }
                        sb.AppendLine($"{ts}if (T[{_tpf}AFMLeven])");
                        sb.AppendLine($"{ts}{{");
                        foreach (var fc in _FasenWithDummies)
                        {
                            sb.AppendLine($"{ts}{ts}AFMdata(&verwerken_fcs[AFM_{_fcpf}{fc}]);");
                        }
                        sb.AppendLine($"{ts}{ts}AFM_tc({_prmpf}AFM_TC,prmAFM_TCgem);");
                        sb.AppendLine($"{ts}{ts}if (PRM[{_prmpf}AFM_Beschikbaar])");
                        sb.AppendLine($"{ts}{ts}{{");
                        foreach (var fc in _FasenWithDummies)
                        {
                            sb.AppendLine(
                                $"{ts}{ts}AFMacties(&verwerken_fcs[AFM_{_fcpf}{fc}], {_fcpf}9{fc}, verwerken_fcs);");
                        }
                        sb.AppendLine($"{ts}{ts}}}");
                        sb.AppendLine();

                        string _hfile = CCOLGeneratorSettingsProvider.Default.GetElementName("hfile");
                        foreach (var fc in _FasenWithDummies)
                        {
                            foreach (var fi in c.FileIngrepen)
                            {
                                foreach (var _fc in fi.TeDoserenSignaalGroepen)
                                {
                                    if (fc == _fc.FaseCyclus)
                                    {
                                        sb.AppendLine(
                                            $"{ts}{ts}if ((TVG_max[{_fcpf}{fc}] > TVG_temp[{_fcpf}{fc}]) && !SCH[{_schpf}AFMExtraGroenBijFile] && IH[{_hpf}{_hfile}{fi.Naam}]) TVG_max[{_fcpf}{fc}] = TVG_temp[{_fcpf}{fc}];");
                                    }
                                }
                            }
                        }
                        foreach (var fc in _FasenWithDummies)
                        {
                            sb.AppendLine($"{ts}{ts}AFMinterface(&verwerken_fcs[AFM_{_fcpf}{fc}]);");
                        }

                        sb.AppendLine($"{ts}}}");
                        sb.AppendLine();
                    }
                    return sb.ToString();

                case CCOLRegCCodeTypeEnum.PostSystemApplication:
                    if (!_MyModel.PrmLoggingTfbMax)
                        return "";
                    sb.AppendLine($"{ts}/* Onthouden hoogste tfb waarde + tijdstip */");
                    sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}if (TFB_timer[fc]>PRM[{_prmpf}tfbmax])");
                    sb.AppendLine($"{ts}{ts}{{");
                    sb.AppendLine($"{ts}{ts}{ts}#if (!defined AUTOMAAT) || (defined VISSIM)");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}{ts}{ts}xyprintf(32, 0, \"Hoogste TFB waarde\");");
                    sb.AppendLine($"{ts}{ts}{ts}{ts}xyprintf(32, 1, \"------------------\");");
                    sb.AppendLine($"{ts}{ts}{ts}{ts}xyprintf(32, 2, \"Fc % s TFB:% 4d sec\", FC_code[fc], TFB_timer[fc]);");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}{ts}{ts}xyprintf(32,3, \"Tijd % 02d\", (CIF_KLOK[CIF_UUR]));");
                    sb.AppendLine($"{ts}{ts}{ts}{ts}xyprintf(39,3,     \":% 02d\", (CIF_KLOK[CIF_MINUUT]));");
                    sb.AppendLine($"{ts}{ts}{ts}{ts}xyprintf(42,3,     \":% 02d\", (CIF_KLOK[CIF_SECONDE]));");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}{ts}{ts}xyprintf(32,4, \"d.d. % 02d\", (CIF_KLOK[CIF_DAG]));");
                    sb.AppendLine($"{ts}{ts}{ts}{ts}xyprintf(39,4,     \" -% 02d\", (CIF_KLOK[CIF_MAAND]));");
                    sb.AppendLine($"{ts}{ts}{ts}{ts}xyprintf(42,4,     \" -% 04d\", (CIF_KLOK[CIF_JAAR]));");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}{ts}#endif");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}{ts}PRM[{_prmpf}tfbfc]   = fc;");
                    sb.AppendLine($"{ts}{ts}{ts}PRM[{_prmpf}tfbmax]  = TFB_timer[fc];");
                    sb.AppendLine($"{ts}{ts}{ts}PRM[{_prmpf}tfbtijd] = CIF_KLOK[CIF_UUR]*100+CIF_KLOK[CIF_MINUUT];");
                    sb.AppendLine($"{ts}{ts}{ts}PRM[{_prmpf}tfbdat]  = CIF_KLOK[CIF_DAG]*100+CIF_KLOK[CIF_MAAND];");
                    sb.AppendLine($"{ts}{ts}{ts}PRM[{_prmpf}tfbjaar] = CIF_KLOK[CIF_JAAR];");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}{ts}CIF_PARM1WIJZAP = CIF_MEER_PARMWIJZ;");
                    sb.AppendLine($"{ts}{ts}}}");
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine();
                    return sb.ToString();
                default:
                    return null;
            }
        }

        public override List<string> GetSourcesToCopy()
        {
            return new List<string>
            {
                "afmroutines.c",
                "afmroutines.h"
            };
        }

        #endregion // CCOLCodePieceGenerator

        #region Constructor

        public SpecialsRotterdamPlugin() : base()
        {
            _SpecialsRotterdamTabVM = new SpecialsRotterdamViewModel();
        }

        #endregion // Constructor
    }
}
