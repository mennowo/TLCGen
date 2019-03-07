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
using TLCGen.Plugins.AFM.Models;

namespace TLCGen.Plugins.AFM
{
    [TLCGenTabItem(-1, TabItemTypeEnum.FasenTab)]
    [TLCGenPlugin(
        TLCGenPluginElems.TabControl | 
        TLCGenPluginElems.XMLNodeWriter |
        TLCGenPluginElems.PlugMessaging)]
    [CCOLCodePieceGenerator]
    public class AFMPlugin : CCOLCodePieceGeneratorBase, ITLCGenTabItem, ITLCGenXMLNodeWriter, ITLCGenPlugMessaging
    {
        #region Fields

        private AFMTabViewModel _afmVM;
        private AFMDataModel _afmModel;

        #endregion // Fields

        #region Properties
        #endregion // Properties

        #region TLCGen plugin shared

        private ControllerModel _controller;
        public ControllerModel Controller
        {
            get => _controller;
            set
            {
                _controller = value;
                if (_controller == null)
                {
                    _afmModel = new AFMDataModel();
                    _afmVM.AfmModel = _afmModel;
                }
            }
        }

        public string GetPluginName()
        {
            return "AFM";
        }

        #endregion // TLCGen plugin shared

        #region ITLCGenTabItem

        public string DisplayName => "AFM";
        public ImageSource Icon => null;

        DataTemplate _ContentDataTemplate;
        public DataTemplate ContentDataTemplate
        {
            get
            {
                if (_ContentDataTemplate == null)
                {
                    _ContentDataTemplate = new DataTemplate();
                    var tab = new FrameworkElementFactory(typeof(AFMTabView));
                    tab.SetValue(AFMTabView.DataContextProperty, _afmVM);
                    _ContentDataTemplate.VisualTree = tab;
                }
                return _ContentDataTemplate;
            }
        }

        public bool IsEnabled { get; set; }

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
            _afmVM.UpdateSelectableFasen(Controller.Fasen.Select(x => x.Naam));
        }

        public bool OnSelectedPreview()
        {
            return true;
        }

        #endregion // ITLCGenTabItem

        #region ITLCGenXMLNodeWriter

        public void GetXmlFromDocument(XmlDocument document)
        {
            _afmModel = null;

            foreach (XmlNode node in document.FirstChild.ChildNodes)
            {
                if (node.LocalName == "AFMData")
                {
                    _afmModel = XmlNodeConverter.ConvertNode<AFMDataModel>(node);
                    break;
                }
            }

            if (_afmModel == null)
            {
                _afmModel = new AFMDataModel();
            }
            _afmVM.AfmModel = _afmModel;
            _afmVM.RaisePropertyChanged("");
        }

        public void SetXmlInDocument(XmlDocument document)
        {
            XmlDocument doc = TLCGenSerialization.SerializeToXmlDocument(_afmModel);
            XmlNode node = document.ImportNode(doc.DocumentElement, true);
            document.DocumentElement.AppendChild(node);
        }

        #endregion // ITLCGenXMLNodeWriter

        #region ITLCGenPlugMessaging

        public void UpdateTLCGenMessaging()
        {
            _afmVM.UpdateMessaging();
        }

        #endregion // ITLCGenPlugMessaging

        #region CCOLCodePieceGenerator

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            if (_afmModel.AFMToepassen)
            {
                foreach (var fc in _afmModel.AFMFasen)
                {
                    _myElements.Add(new CCOLElement($"AFM{fc.FaseCyclus}_FC", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                    _myElements.Add(new CCOLElement($"AFM{fc.FaseCyclus}_GmaxCCOL", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                    _myElements.Add(new CCOLElement($"AFM{fc.FaseCyclus}_GmaxMin", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                    _myElements.Add(new CCOLElement($"AFM{fc.FaseCyclus}_GmaxMax", 80, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                    _myElements.Add(new CCOLElement($"AFM{fc.FaseCyclus}_GmaxAct", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                    _myElements.Add(new CCOLElement($"AFM{fc.FaseCyclus}_GmaxGem", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                    _myElements.Add(new CCOLElement($"AFM{fc.FaseCyclus}_Afgekapt", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                    _myElements.Add(new CCOLElement($"AFM{fc.FaseCyclus}_GmaxAFM", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                    _myElements.Add(new CCOLElement($"AFM{fc.FaseCyclus}_Sturing", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                    _myElements.Add(new CCOLElement($"AFM{fc.FaseCyclus}_Qlength", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                    _myElements.Add(new CCOLElement($"AFM{fc.FaseCyclus}_AbsBufferRuimte", 100, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                    _myElements.Add(new CCOLElement($"AFM{fc.FaseCyclus}_RelBufferRuimte", 100, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                    _myElements.Add(new CCOLElement($"AFM{fc.FaseCyclus}_RelBufferVulling", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                }
                _myElements.Add(new CCOLElement("AFM_Strikt", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                _myElements.Add(new CCOLElement("AFM_TC", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                _myElements.Add(new CCOLElement("AFM_TCgem", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                _myElements.Add(new CCOLElement("AFM_Watchdog", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                _myElements.Add(new CCOLElement("AFM_WatchdogReturn", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                _myElements.Add(new CCOLElement("AFM_Versie", 7, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                _myElements.Add(new CCOLElement("AFM_Test", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                _myElements.Add(new CCOLElement("AFM_Beinvloedbaar", 1, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));

                _myElements.Add(new CCOLElement("AFMLeven", CCOLElementTypeEnum.Uitgang));

                _myElements.Add(new CCOLElement("AFMLeven", 120, CCOLElementTimeTypeEnum.TS_type, CCOLElementTypeEnum.Timer));
                _myElements.Add(new CCOLElement("VRILeven", 60, CCOLElementTimeTypeEnum.TS_type, CCOLElementTypeEnum.Timer));
                _myElements.Add(new CCOLElement("AFMExtraGroenBijFile", 1, CCOLElementTimeTypeEnum.SCH_type, CCOLElementTypeEnum.Schakelaar));

                _myElements.Add(new CCOLElement("AFMCIFParmWijz", 1, CCOLElementTimeTypeEnum.SCH_type, CCOLElementTypeEnum.Schakelaar));
                _myElements.Add(new CCOLElement("AFM_overbrugging", 0, CCOLElementTimeTypeEnum.SCH_type, CCOLElementTypeEnum.Schakelaar));
            }
        }

        public override bool HasCCOLElements()
        {
            return true;
        }

        public override IEnumerable<CCOLElement> GetCCOLElements(CCOLElementTypeEnum type)
        {
            return _myElements.Where(x => x.Type == type);
        }

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.SysHBeforeUserDefines:
                    return 105;
                case CCOLCodeTypeEnum.RegCIncludes:
                    return 105;
                case CCOLCodeTypeEnum.RegCInitApplication:
                    return 105;
                case CCOLCodeTypeEnum.RegCPreApplication:
                    return 105;
                case CCOLCodeTypeEnum.RegCAlternatieven:
                    return 105;
                case CCOLCodeTypeEnum.RegCPostApplication:
                    return 105;
                case CCOLCodeTypeEnum.RegCPostSystemApplication:
                    return 105;
                case CCOLCodeTypeEnum.OvCTop:
                    return 105;
                case CCOLCodeTypeEnum.OvCPARCorrecties:
                    return 105;
                default:
                    return 0;
            }
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            if (!_afmModel.AFMToepassen || !_afmModel.AFMFasen.Any()) return "";

            StringBuilder sb = new StringBuilder();

            var dFcs = _afmModel.AFMFasen.Where(x => x.DummyFaseCyclus != "NG").Select(x => x.DummyFaseCyclus);

            switch (type)
            {
                case CCOLCodeTypeEnum.SysHBeforeUserDefines:
                    sb.AppendLine("/* Ten behoeve van AFM */");
                    int index = 0;
                    foreach (var fc in _afmModel.AFMFasen)
                    {
                        sb.AppendLine($"#define AFM_{_fcpf}{fc.FaseCyclus} {index++}");
                    }
                    sb.AppendLine($"#define AFM_{_fcpf}max {index}");
                    sb.AppendLine();
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCIncludes:
                    sb.AppendLine("/* Ten behoeve van AFM */");
                    sb.AppendLine("#include \"AFMroutines.c\"");
                    sb.AppendLine("AFM_FC_STRUCT verwerken_fcs[AFM_fcmax] = { 0 };");
                    sb.AppendLine($"int prmAFM_watchdog_return_old;");
                    sb.AppendLine($"int TVG_temp[FCMAX];");
                    sb.AppendLine();
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCInitApplication:
                    sb.AppendLine($"{ts}/* Initialiseer AFM routines */");
                    sb.AppendLine($"{ts}AFMinit();");
                    sb.AppendLine();
                    foreach (var fc in _afmModel.AFMFasen)
                    {
                        sb.AppendLine($"{ts}AFM_fc_initfc(&verwerken_fcs[AFM_{_fcpf}{fc.FaseCyclus}], {_fcpf}{fc.FaseCyclus}, prmAFM{fc.FaseCyclus}_FC);");
                    }
                    sb.AppendLine();
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCPreApplication:
                    sb.AppendLine($"{ts}AFM_CIF_changed = FALSE;");
                    sb.AppendLine("#if (defined AUTOMAAT || defined AUTOMAAT_TEST) && !defined VISSIM");
                    sb.AppendLine($"{ts}RT[{_tpf}AFMLeven] = (PRM[{_prmpf}AFM_WatchdogReturn] != prmAFM_watchdog_return_old) || SCH[schAFM_overbrugging];");
                    sb.AppendLine("#else");
                    sb.AppendLine($"{ts}RT[{_tpf}AFMLeven] = TRUE;");
                    sb.AppendLine("#endif");
                    sb.AppendLine($"{ts}prmAFM_watchdog_return_old = PRM[{_prmpf}AFM_WatchdogReturn];");
                    sb.AppendLine($"{ts}CIF_GUS[{_uspf}AFMLeven] = T[{_tpf}AFMLeven];");
                    sb.AppendLine($"{ts}RT[{_tpf}VRILeven] = !T[{_tpf}VRILeven];");
                    sb.AppendLine($"{ts}if (ET[{_tpf}VRILeven])");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}if (PRM[{_prmpf}AFM_Watchdog] < 9999) PRM[{_prmpf}AFM_Watchdog]++;");
                    sb.AppendLine($"{ts}{ts}else                            PRM[{_prmpf}AFM_Watchdog] = 0;");
                    sb.AppendLine($"{ts}AFM_CIF_changed = TRUE;");
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}AFMResetBits();");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* Niet bewaken schaduw fasen AFM */");
                    foreach (var dummyFc in dFcs)
                    {
                        sb.AppendLine($"{ts}TFB_timer[{_fcpf}{dummyFc}] = 0;");
                    }
                    sb.AppendLine();
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCAlternatieven:
                    sb.AppendLine($"{ts}/* AFM */");
                    sb.AppendLine($"{ts}if (T[{_tpf}AFMLeven] && !PRM[{_prmpf}AFM_Test])");
                    sb.AppendLine($"{ts}{{");
                    foreach (var fc in _afmModel.AFMFasen)
                    {
                        sb.AppendLine($"{ts}{ts}AFMacties_alternatieven(&verwerken_fcs[AFM_{_fcpf}{fc.FaseCyclus}]);");
                    }
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine();
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCPostApplication:
                    sb.AppendLine($"{ts}/* AFM */");
                    foreach (var dummyFc in dFcs)
                    {
                        sb.AppendLine($"{ts}TVG_temp[{_fcpf}{dummyFc}] = TVG_max[{_fcpf}{dummyFc}];");
                    }
                    sb.AppendLine($"{ts}if (T[{_tpf}AFMLeven])");
                    sb.AppendLine($"{ts}{{");
                    foreach (var fc in _afmModel.AFMFasen)
                    {
                        sb.AppendLine($"{ts}{ts}AFMdata(&verwerken_fcs[AFM_{_fcpf}{fc.FaseCyclus}]);");
                    }
                    sb.AppendLine($"{ts}{ts}AFM_tc({_prmpf}AFM_TC, {_prmpf}AFM_TCgem);");
                    sb.AppendLine($"{ts}{ts}if (!PRM[{_prmpf}AFM_Test] && !IS[{_ispf}fix])");
                    sb.AppendLine($"{ts}{ts}{{");
                    string _cvchd = CCOLGeneratorSettingsProvider.Default.GetElementName("cvchd");
                    foreach (var fc in _afmModel.AFMFasen)
                    {
                        var hd = c.OVData.HDIngrepen.FirstOrDefault(x => x.FaseCyclus == fc.FaseCyclus);
                        var dummy = (fc.DummyFaseCyclus == "NG" ? "NG" : _fcpf + fc.DummyFaseCyclus);
                        if (hd == null)
                        {
                            sb.AppendLine(
                                $"{ts}{ts}AFMacties(&verwerken_fcs[AFM_{_fcpf}{fc.FaseCyclus}], {dummy}, verwerken_fcs);");
                        }
                        else
                        {
                            sb.AppendLine(
                                $"{ts}{ts}if (!C[{_ctpf}{_cvchd}{hd.FaseCyclus}]) AFMacties(&verwerken_fcs[AFM_{_fcpf}{fc.FaseCyclus}], {dummy}, verwerken_fcs);");
                        }
                    }
                    sb.AppendLine($"{ts}{ts}}}");
                    sb.AppendLine();

                    string _hfile = CCOLGeneratorSettingsProvider.Default.GetElementName("hfile");
                    foreach (var dummyFc in dFcs)
                    {
                        foreach (var fi in c.FileIngrepen)
                        {
                            foreach (var _fc in fi.TeDoserenSignaalGroepen)
                            {
                                if (dummyFc == _fc.FaseCyclus)
                                {
                                    sb.AppendLine(
                                        $"{ts}{ts}if ((TVG_max[{_fcpf}{dummyFc}] > TVG_temp[{_fcpf}{dummyFc}]) && !SCH[{_schpf}AFMExtraGroenBijFile] && IH[{_hpf}{_hfile}{fi.Naam}]) TVG_max[{_fcpf}{dummyFc}] = TVG_temp[{_fcpf}{dummyFc}];");
                                }
                            }
                        }
                    }
                    foreach (var fc in _afmModel.AFMFasen)
                    {
                        sb.AppendLine($"{ts}{ts}AFMinterface(&verwerken_fcs[AFM_{_fcpf}{fc.FaseCyclus}]);");
                    }

                    sb.AppendLine($"{ts}{ts}if (AFM_CIF_changed && SCH[{_schpf}AFMCIFParmWijz])");
                    sb.AppendLine($"{ts}{ts}{{");
                    sb.AppendLine($"{ts}{ts}{ts}CIF_PARM1WIJZAP = CIF_MEER_PARMWIJZ;");
                    sb.AppendLine($"{ts}{ts}}}");

                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine();
                    
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCPostSystemApplication:
                    string _isfix = CCOLGeneratorSettingsProvider.Default.GetElementName("isfix");
                    sb.AppendLine($"{ts}if ((CIF_WPS[CIF_PROG_STATUS] == CIF_STAT_REG) && !CIF_IS[{_ispf}{_isfix}] && T[{_tpf}AFMLeven])");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}PRM[{_prmpf}AFM_Beinvloedbaar] = 1;");
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine($"{ts}else");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}PRM[{_prmpf}AFM_Beinvloedbaar] = 0;");
                    sb.AppendLine($"{ts}}}");
                    return sb.ToString();

                case CCOLCodeTypeEnum.OvCTop:
                    sb.AppendLine($"#include \"AFMroutines.h\"");
                    sb.AppendLine($"extern AFM_FC_STRUCT verwerken_fcs[AFM_fcmax];");
                    return sb.ToString();

                case CCOLCodeTypeEnum.OvCPARCorrecties:
                    sb.AppendLine($"{ts}/* AFM */");
                    sb.AppendLine($"{ts}if (T[{_tpf}AFMLeven] && !PRM[{_prmpf}AFM_Test])");
                    sb.AppendLine($"{ts}{{");
                    foreach (var fc in _afmModel.AFMFasen)
                    {
                        sb.AppendLine($"{ts}{ts}AFMacties_alternatieven(&verwerken_fcs[AFM_{_fcpf}{fc.FaseCyclus}]);");
                    }
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine();
                    return sb.ToString();

                default:
                    return null;
            }
        }

        public override List<string> GetSourcesToCopy()
        {
            if (!_afmModel.AFMToepassen || !_afmModel.AFMFasen.Any()) return null;
            return new List<string>
            {
                "afmroutines.c",
                "afmroutines.h"
            };
        }

        #endregion // CCOLCodePieceGenerator

        #region Constructor

        public AFMPlugin()
        {
            IsEnabled = true;
            _afmVM = new AFMTabViewModel(this);
        }
        
        #endregion // Constructor
    }
}
