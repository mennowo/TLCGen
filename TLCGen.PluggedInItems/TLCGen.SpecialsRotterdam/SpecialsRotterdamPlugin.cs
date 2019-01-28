using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public override bool HasFunctionLocalVariables()
        {
            return true;
        }

        public override IEnumerable<Tuple<string, string, string>> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCPostSystemApplication:
                    if (_MyModel.PrmLoggingTfbMax)
                        return new List<Tuple<string, string, string>> { new Tuple<string, string, string>("int", "fc", "") };
                    return base.GetFunctionLocalVariables(c, type);
                default:
                    return base.GetFunctionLocalVariables(c, type);
            }
        }

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCPostApplication:
                    return 100;
                case CCOLCodeTypeEnum.RegCPostSystemApplication:
                    return 100;
                default:
                    return 0;
            }
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {

            StringBuilder sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCPostApplication:
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
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCPostSystemApplication:
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
                    sb.AppendLine($"{ts}{ts}{ts}{ts}xyprintf(32, 2, \"Fc %s TFB:%4d sec\", FC_code[fc], TFB_timer[fc]);");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}{ts}{ts}xyprintf(32,3, \"Tijd %02d\", (CIF_KLOK[CIF_UUR]));");
                    sb.AppendLine($"{ts}{ts}{ts}{ts}xyprintf(39,3,     \":%02d\", (CIF_KLOK[CIF_MINUUT]));");
                    sb.AppendLine($"{ts}{ts}{ts}{ts}xyprintf(42,3,     \":%02d\", (CIF_KLOK[CIF_SECONDE]));");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}{ts}{ts}xyprintf(32,4, \"d.d. %02d\", (CIF_KLOK[CIF_DAG]));");
                    sb.AppendLine($"{ts}{ts}{ts}{ts}xyprintf(39,4,     \"-%02d\", (CIF_KLOK[CIF_MAAND]));");
                    sb.AppendLine($"{ts}{ts}{ts}{ts}xyprintf(42,4,     \"-%04d\", (CIF_KLOK[CIF_JAAR]));");
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
        
        #endregion // CCOLCodePieceGenerator

        #region Constructor

        public SpecialsRotterdamPlugin() : base()
        {
            _SpecialsRotterdamTabVM = new SpecialsRotterdamViewModel();
        }

        #endregion // Constructor
    }
}
