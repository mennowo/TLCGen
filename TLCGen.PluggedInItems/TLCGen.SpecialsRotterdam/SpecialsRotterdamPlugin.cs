using System;
using System.Collections.Generic;
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
using TLCGen.Plugins;

namespace TLCGen.SpecialsRotterdam
{
    [CCOLCodePieceGenerator]
    [TLCGenTabItem(-1, TabItemTypeEnum.SpecialsTab)]
    [TLCGenPlugin(TLCGenPluginElems.PlugMessaging | 
                  TLCGenPluginElems.TabControl | 
                  TLCGenPluginElems.XMLNodeWriter)]
    public class RotterdamSpecialsPlugin : CCOLCodePieceGeneratorBase, ITLCGenPlugMessaging, ITLCGenTabItem, ITLCGenXMLNodeWriter
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
            bool found = false;
            foreach (XmlNode node in document.FirstChild.ChildNodes)
            {
                if (node.LocalName == "SpecialsRotterdam")
                {
                    _MyModel = XmlNodeConverter.ConvertNode<SpecialsRotterdamModel>(node);
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                _MyModel = new SpecialsRotterdamModel();
            }
            _SpecialsRotterdamTabVM.Specials = _MyModel;
            _SpecialsRotterdamTabVM.RaisePropertyChanged(null);
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

            // Search fasen with dummies
            foreach (var fc in c.Fasen)
            {
                foreach (var fc2 in c.Fasen)
                {
                    if(fc2.Naam.Length == 3 && fc2.Naam.StartsWith("9") && fc.Naam == fc2.Naam.Substring(1))
                    {
                        _FasenWithDummies.Add(fc.Naam);
                    }
                }
            }

            _MyElements = new List<CCOLElement>();

            if (_MyModel.ToepassenAFM)
            {
                foreach (var fc in c.Fasen)
                {
                    if (fc.Type == Models.Enumerations.FaseTypeEnum.Auto)
                    {
                        int i = 0;
                        if (Int32.TryParse(fc.Naam, out i))
                        {
                            if (i < 900)
                            {
                                _MyElements.Add(new CCOLElement($"AFM{fc.Naam}_FC", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                                _MyElements.Add(new CCOLElement($"AFM{fc.Naam}_GmaxCCOL", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                                _MyElements.Add(new CCOLElement($"AFM{fc.Naam}_MinGmax", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                                _MyElements.Add(new CCOLElement($"AFM{fc.Naam}_MaxGmax", 80, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                                _MyElements.Add(new CCOLElement($"AFM{fc.Naam}_Gmaxact", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                                _MyElements.Add(new CCOLElement($"AFM{fc.Naam}_Gmaxgem", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                                _MyElements.Add(new CCOLElement($"AFM{fc.Naam}_Afgekapt", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                                _MyElements.Add(new CCOLElement($"AFM{fc.Naam}_GmaxAFM", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                                _MyElements.Add(new CCOLElement($"AFM{fc.Naam}_Qlenght", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                                _MyElements.Add(new CCOLElement($"AFM{fc.Naam}_relBufferRuimte", 100, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                                _MyElements.Add(new CCOLElement($"AFM{fc.Naam}_relBufferVulling", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                            }
                        }
                    }
                }
                _MyElements.Add(new CCOLElement($"AFM_strikt", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                _MyElements.Add(new CCOLElement($"AFM_actief", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                _MyElements.Add(new CCOLElement($"AFM_TC", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                _MyElements.Add(new CCOLElement($"AFM_TCgem", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                _MyElements.Add(new CCOLElement($"AFM_watchdog", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                _MyElements.Add(new CCOLElement($"AFM_watchdog_return", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                _MyElements.Add(new CCOLElement($"AFM_versie", 2, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));

                _MyElements.Add(new CCOLElement($"AFMLeven", CCOLElementTypeEnum.Uitgang));
                _MyElements.Add(new CCOLElement($"TMSLeven", CCOLElementTypeEnum.Uitgang));

                _MyElements.Add(new CCOLElement($"AFMLeven", 120, CCOLElementTimeTypeEnum.TS_type, CCOLElementTypeEnum.Timer));
                _MyElements.Add(new CCOLElement($"TMSLeven", 120, CCOLElementTimeTypeEnum.TS_type, CCOLElementTypeEnum.Timer));
                _MyElements.Add(new CCOLElement($"VRILeven", 60, CCOLElementTimeTypeEnum.TS_type, CCOLElementTypeEnum.Timer));
                _MyElements.Add(new CCOLElement($"AFMExtraGroenBijFile", 1, CCOLElementTimeTypeEnum.SCH_type, CCOLElementTypeEnum.Schakelaar));
            }
        }

        public override bool HasCCOLElements()
        {
            return true;
        }

        public override IEnumerable<CCOLElement> GetCCOLElements(CCOLElementTypeEnum type)
        {
            return _MyElements.Where(x => x.Type == type);
        }

        public override bool HasCode(CCOLRegCCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLRegCCodeTypeEnum.Top:
                case CCOLRegCCodeTypeEnum.InitApplication:
                case CCOLRegCCodeTypeEnum.PreApplication:
                case CCOLRegCCodeTypeEnum.Alternatieven:
                case CCOLRegCCodeTypeEnum.PostApplication:
                    return true;
                default:
                    return false;
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
                    sb.AppendLine($"{ts}AFMinit();");
                    sb.AppendLine();
                    sb.AppendLine("/* Initialiseer AFM routines */");
                    sb.AppendLine($"{ts}AFMinit();");
                    sb.AppendLine();
                    foreach (var fc in _FasenWithDummies)
                    {
                        sb.AppendLine($"{ts}AFM_fc_initfc(&verwerken_fcs[AFM_{_fcpf}{fc}], {_fcpf}{fc}, prmAFM{fc}_FC);");
                    }
                    sb.AppendLine();
                    sb.AppendLine("/* Niet bewaken schaduw fasen AFM */");
                    foreach (var fc in _FasenWithDummies)
                    {
                        sb.AppendLine($"{ts}TFB_timer[{_fcpf}9{fc}] = 0;");
                    }
                    sb.AppendLine();
                    return sb.ToString();

                case CCOLRegCCodeTypeEnum.PreApplication:
                    if (!_MyModel.ToepassenAFM)
                        return "";
                    sb.AppendLine("#if defined AUTOMAAT && !defined VISSIM ");
                    sb.AppendLine($"{ts}RT[{_tpf}AFMLeven] = (PRM[{_prmpf}AFM_watchdog_return] != prmAFM_watchdog_return_old);");
                    sb.AppendLine($"{ts}RT[{_tpf}TMSLeven] = IS_leven[{_ispf}TMS_leven] && !IS_leven_old[{_ispf}TMS_leven] || !IS_leven[{_ispf}TMS_leven] && IS_leven_old[{_ispf}TMS_leven];");
                    sb.AppendLine("#else");
                    sb.AppendLine($"{ts}RT[{_tpf}AFMLeven] = TRUE;");
                    sb.AppendLine($"{ts}RT[{_tpf}TMSLeven] = TRUE;");
                    sb.AppendLine("#endif");
                    sb.AppendLine($"{ts}prmAFM_watchdog_return_old = PRM[{_prmpf}AFM_watchdog];");
                    sb.AppendLine($"{ts}CIF_GUS[{_uspf}AFMLeven] = RT[{_tpf}AFMLeven];");
                    sb.AppendLine($"{ts}CIF_GUS[{_uspf}TMSLeven] = RT[{_tpf}TMSLeven];");
                    sb.AppendLine($"{ts}RT[tVRILeven] = !T[tVRILeven];");
                    sb.AppendLine($"{ts}if (ET[{_tpf}VRILeven])");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}if (PRM[{_prmpf}AFM_watchdog] < 9999) PRM[{_prmpf}AFM_watchdog]++;");
                    sb.AppendLine($"{ts}{ts}else                            PRM[{_prmpf}AFM_watchdog] = 0;");
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}AFMResetBits();");
                    sb.AppendLine();
                    return sb.ToString();

                case CCOLRegCCodeTypeEnum.Alternatieven:
                    if (!_MyModel.ToepassenAFM)
                        return "";
                    sb.AppendLine($"{ts}/* AFM */");
                    sb.AppendLine($"{ts}if (T[{_tpf}AFMLeven])");
                    sb.AppendLine($"{ts}{{");
                    foreach (var fc in _FasenWithDummies)
                    {
                        sb.AppendLine($"{ts}{ts}AFMacties_alternatieven(&verwerken_fcs[AFM_{_fcpf}{fc}]);");
                    }
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine();
                    return sb.ToString();

                case CCOLRegCCodeTypeEnum.PostApplication:
                    if (!_MyModel.ToepassenAFM)
                        return "";
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
                    sb.AppendLine($"{ts}{ts}AFM_tc(prmAFM_TC,prmAFM_TCgem);");
                    foreach (var fc in _FasenWithDummies)
                    {
                        sb.AppendLine($"{ts}{ts}AFMacties(&verwerken_fcs[AFM_{_fcpf}{fc}], {_fcpf}9{fc}, verwerken_fcs);");
                    }
                    string _hfile = CCOLGeneratorSettingsProvider.Default.GetElementName("hfile");
                    foreach (var fc in _FasenWithDummies)
                    {
                        foreach (var fi in c.FileIngrepen)
                        {
                            foreach(var _fc in fi.TeDoserenSignaalGroepen)
                            {
                                if(fc == _fc.FaseCyclus)
                                {
                                   sb.AppendLine($"{ts}{ts}if ((TVG_max[{_fcpf}{fc}] > TVG_temp[{_fcpf}{fc}]) && !SCH[{_schpf}AFMExtraGroenBijFile] && IH[{_hpf}{_hfile}{fi.Naam}]) TVG_max[{_fcpf}{fc}] = TVG_temp[{_fcpf}{fc}];");
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
                    return sb.ToString();
                default:
                    return null;
            }
        }

        #endregion // CCOLCodePieceGenerator

        #region Constructor

        public RotterdamSpecialsPlugin() : base()
        {
            _SpecialsRotterdamTabVM = new SpecialsRotterdamViewModel();
        }

        #endregion // Constructor
    }
}
