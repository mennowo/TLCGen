using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using TLCGen.Generators.CCOL.CodeGeneration;
using TLCGen.Generators.CCOL.CodeGeneration.Functionality;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Plugins.AFM.Models;

namespace TLCGen.Plugins.AFM
{
    [TLCGenTabItem(-1, TabItemTypeEnum.FasenTab)]
    [TLCGenPlugin(
        TLCGenPluginElems.TabControl | 
        TLCGenPluginElems.XMLNodeWriter |
        TLCGenPluginElems.PlugMessaging |
        TLCGenPluginElems.IOElementProvider)]
    [CCOLCodePieceGenerator]
    public class AFMPlugin : CCOLCodePieceGeneratorBase, ITLCGenTabItem, ITLCGenXMLNodeWriter, ITLCGenPlugMessaging, ITLCGenElementProvider, ITLCGenHasSpecification
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

        public bool Visibility { get; set; } = true;

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
            var doc = TLCGenSerialization.SerializeToXmlDocument(_afmModel);
            var node = document.ImportNode(doc.DocumentElement, true);
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
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"AFM{fc.FaseCyclus}_FC", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter, "", PrioCodeGeneratorHelper.CAT_Optimaliseren, ""));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"AFM{fc.FaseCyclus}_GmaxCCOL", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter, "", PrioCodeGeneratorHelper.CAT_Optimaliseren, ""));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"AFM{fc.FaseCyclus}_GmaxMin", fc.MinimaleGroentijd, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter, "", PrioCodeGeneratorHelper.CAT_Optimaliseren, ""));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"AFM{fc.FaseCyclus}_GmaxMax", fc.MaximaleGroentijd, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter, "", PrioCodeGeneratorHelper.CAT_Optimaliseren, ""));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"AFM{fc.FaseCyclus}_GmaxAct", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter, "", PrioCodeGeneratorHelper.CAT_Optimaliseren, ""));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"AFM{fc.FaseCyclus}_GmaxGem", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter, "", PrioCodeGeneratorHelper.CAT_Optimaliseren, ""));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"AFM{fc.FaseCyclus}_Afgekapt", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter, "", PrioCodeGeneratorHelper.CAT_Optimaliseren, ""));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"AFM{fc.FaseCyclus}_GmaxAFM", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter, "", PrioCodeGeneratorHelper.CAT_Optimaliseren, ""));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"AFM{fc.FaseCyclus}_Sturing", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter, "", PrioCodeGeneratorHelper.CAT_Optimaliseren, ""));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"AFM{fc.FaseCyclus}_Qlength", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter, "", PrioCodeGeneratorHelper.CAT_Optimaliseren, ""));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"AFM{fc.FaseCyclus}_AbsBufferRuimte", 100, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter, "", PrioCodeGeneratorHelper.CAT_Optimaliseren, ""));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"AFM{fc.FaseCyclus}_RelBufferRuimte", 100, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter, "", PrioCodeGeneratorHelper.CAT_Optimaliseren, ""));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"AFM{fc.FaseCyclus}_RelBufferVulling", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter, "", PrioCodeGeneratorHelper.CAT_Optimaliseren, ""));
                }
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement("AFM_Strikt", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter, "", PrioCodeGeneratorHelper.CAT_Optimaliseren, ""));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement("AFM_TC", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter, "", PrioCodeGeneratorHelper.CAT_Optimaliseren, ""));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement("AFM_TCgem", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter, "", PrioCodeGeneratorHelper.CAT_Optimaliseren, ""));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement("AFM_Watchdog", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter, "", PrioCodeGeneratorHelper.CAT_Optimaliseren, ""));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement("AFM_WatchdogReturn", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter, "", PrioCodeGeneratorHelper.CAT_Optimaliseren, ""));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement("AFM_Versie", 7, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter, "", PrioCodeGeneratorHelper.CAT_Optimaliseren, ""));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement("AFM_Test", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter, "", PrioCodeGeneratorHelper.CAT_Optimaliseren, ""));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement("AFM_Beinvloedbaar", 1, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter, "", PrioCodeGeneratorHelper.CAT_Optimaliseren, ""));

                // This happens via ITLCGenElementProvider below
                // -- _myElements.Add(new CCOLElement("AFMLeven", CCOLElementTypeEnum.Uitgang, ioElementData: _afmModel.AFMLevenBitmapCoordinaten));

                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement("AFMLeven", 120, CCOLElementTimeTypeEnum.TS_type, CCOLElementTypeEnum.Timer, "", PrioCodeGeneratorHelper.CAT_Optimaliseren, ""));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement("VRILeven", 60, CCOLElementTimeTypeEnum.TS_type, CCOLElementTypeEnum.Timer, "", PrioCodeGeneratorHelper.CAT_Optimaliseren, ""));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement("AFMExtraGroenBijFile", 1, CCOLElementTimeTypeEnum.SCH_type, CCOLElementTypeEnum.Schakelaar, "", PrioCodeGeneratorHelper.CAT_Optimaliseren, ""));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement("AFMCIFParmWijz", 1, CCOLElementTimeTypeEnum.SCH_type, CCOLElementTypeEnum.Schakelaar, "", PrioCodeGeneratorHelper.CAT_Optimaliseren, ""));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement("AFM_overbrugging", 0, CCOLElementTimeTypeEnum.SCH_type, CCOLElementTypeEnum.Schakelaar, "", PrioCodeGeneratorHelper.CAT_Optimaliseren, ""));
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

        public override int[] HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.SysHBeforeUserDefines: return new []{105};
                case CCOLCodeTypeEnum.RegCIncludes: return new []{105};
                case CCOLCodeTypeEnum.RegCInitApplication: return new []{105};
                case CCOLCodeTypeEnum.RegCPreApplication: return new []{105};
                case CCOLCodeTypeEnum.RegCAlternatieven: return new []{105};
                case CCOLCodeTypeEnum.RegCPostApplication: return new []{105};
                case CCOLCodeTypeEnum.RegCPostSystemApplication: return new []{105};
                case CCOLCodeTypeEnum.PrioCTop: return new []{105};
                case CCOLCodeTypeEnum.PrioCPARCorrecties: return new []{105};
                default:
                    return null;
            }
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts, int order)
        {
            if (!_afmModel.AFMToepassen || !_afmModel.AFMFasen.Any()) return "";

            var sb = new StringBuilder();

            var dFcs = _afmModel.AFMFasen.Where(x => x.DummyFaseCyclus != "NG").Select(x => x.DummyFaseCyclus);

            switch (type)
            {
                case CCOLCodeTypeEnum.SysHBeforeUserDefines:
                    sb.AppendLine("/* Ten behoeve van AFM */");
                    var index = 0;
                    foreach (var fc in _afmModel.AFMFasen)
                    {
                        sb.AppendLine($"#define AFM_{_fcpf}{fc.FaseCyclus} {index++}");
                    }
                    sb.AppendLine($"#define AFM_{_fcpf}max {index}");
                    sb.AppendLine();
                    return sb.ToString();

                case CCOLCodeTypeEnum.PrioCIncludes:
                    sb.AppendLine("/* Ten behoeve van AFM */");
                    sb.AppendLine("#include \"AFMroutines.h\"");
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
                    var _cvchd = CCOLGeneratorSettingsProvider.Default.GetElementName("cvchd");
                    foreach (var fc in _afmModel.AFMFasen)
                    {
                        var hd = c.PrioData.HDIngrepen.FirstOrDefault(x => x.FaseCyclus == fc.FaseCyclus);
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

                    var _hfile = CCOLGeneratorSettingsProvider.Default.GetElementName("hfile");
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
                    var _isfix = CCOLGeneratorSettingsProvider.Default.GetElementName("isfix");
                    sb.AppendLine($"{ts}if ((CIF_WPS[CIF_PROG_STATUS] == CIF_STAT_REG) && !CIF_IS[{_ispf}{_isfix}] && T[{_tpf}AFMLeven])");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}PRM[{_prmpf}AFM_Beinvloedbaar] = 1;");
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine($"{ts}else");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}PRM[{_prmpf}AFM_Beinvloedbaar] = 0;");
                    sb.AppendLine($"{ts}}}");
                    return sb.ToString();

                case CCOLCodeTypeEnum.PrioCTop:
                    sb.AppendLine($"#include \"AFMroutines.h\"");
                    sb.AppendLine($"extern AFM_FC_STRUCT verwerken_fcs[AFM_fcmax];");
                    return sb.ToString();

                case CCOLCodeTypeEnum.PrioCPARCorrecties:
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

        #region ITLCGenElementProvider

        public List<IOElementModel> GetOutputItems()
        {
            if (!_afmModel.AFMToepassen) return null;
            _afmModel.AFMLevenBitmapCoordinaten.Naam = "AFMLeven";
            return new List<IOElementModel>{_afmModel.AFMLevenBitmapCoordinaten}; 
        }

        public List<IOElementModel> GetInputItems()
        {
            return null;
        }

        public List<object> GetAllItems()
        {
            if (!_afmModel.AFMToepassen) return null;

            var allElements = new List<object>
            {
                new CCOLElement(
                    "AFMLeven",
                    PrioCodeGeneratorHelper.CAT_Optimaliseren, "",
                    CCOLElementTypeEnum.Uitgang,
                    "", _afmModel.AFMLevenBitmapCoordinaten)
            };

            return allElements;
        }

        public bool IsElementNameUnique(string name, TLCGenObjectTypeEnum type)
        {
            return type != TLCGenObjectTypeEnum.Output || name != "AFMLeven";
        }

        #endregion

        #region ITLCGenHasSpecification

        public SpecificationData GetSpecificationData(ControllerModel c)
        {
            if (!_afmModel.AFMToepassen) return null;

            var data = new SpecificationData
            {
                Subject = SpecificationSubject.AFM
            };

            data.Elements.Add(new SpecificationParagraph
            {
                Type = SpecificationParagraphType.Header1,
                Text = "AFM"
            });

            data.Elements.Add(new SpecificationParagraph
            {
                Type = SpecificationParagraphType.Body,
                Text = "De regeling is voorzien van Adaptief FileManagement (AFM)."
            });
            data.Elements.Add(new SpecificationParagraph
            {
                Type = SpecificationParagraphType.Body,
                Text = "AFM is een methodiek om een vorm van netwerkmanagement te bewerkstelligen vanuit een - in principe - extern systeem (de 'AFM centrale'). " +
                "AFM is van oorsprong ontworpen als veiligheidssysteem voor de Maastunnel in Rotterdam en is bedoeld om de instroom op een traject te beperken " +
                "of de uitstroom te vergroten. Daartoe worden via parameters de maximum groentijd en de cyclustijd beïnvloed. "
            });
            data.Elements.Add(new SpecificationParagraph
            {
                Type = SpecificationParagraphType.Body,
                Text = "Omdat de werking van AFM is gepaseerd op parameters, is het in theorie mogelijk om alle systemen die kunnen schrijven naar " +
                "parameters als extern systeem te gebruiken (bijvoorbeeld een naburige regegelautomaat die via PTP is verbonden, of bij iVRI's ook " +
                "een provider applicatie die verbonden is via een localhost PTP verbinding). "
            });

            data.Elements.Add(new SpecificationParagraph
            {
                Type = SpecificationParagraphType.Spacer,
                Text = ""
            });

            data.Elements.Add(new SpecificationParagraph
            {
                Type = SpecificationParagraphType.Header2,
                Text = "AFM werking en instellingen"
            });

            data.Elements.Add(new SpecificationParagraph
            {
                Type = SpecificationParagraphType.Body,
                Text = "De beïnvloeding van de hoeveelheid groen voor een richting loopt via schaduw-fc's; de schaduw-fc (bijvoorbeeld fc902) bepaalt " +
                "het gedrag van de de conflicten van de daadwerkelijke fc (in dit voorbeeld fc02). Wanneer de instroom van het afvoerend wegvak van fc02 moet worden beperkt, " +
                "is het wenselijk dat de groensturing van fc02 wordt beëindigd maar de regeling nog voortgaat alsof richting 02 groen is (waardoor " +
                "bijvoorbeeld alternatieven mee kunnen blijven komen); wanneer de regeling meteen zou doorstappen wordt het afvoerende wegvak van fc02 " +
                "meteen weer gevoed door (bijvoorbeeld) fc06 of fc10, wat de genomen maatregel om de instroom te beperken weer deels teniet zou doen. " +
                "Daarom wordt fc02 afgekapt maar blijft de interne schaduw-fc902 groen tot het einde van het maximumgroen voor fc02, waarbij fc902 de " +
                "conflicten krijgt van fc02 voor zover die conflicten richtingen betreffen die leiden naar hetzelfde afvoerend wegvak. " +
                "fc902 conflicteert dan wel met fc06 maar bijvoorbeeld niet met fc31. "
            });
            data.Elements.Add(new SpecificationParagraph
            {
                Type = SpecificationParagraphType.Body,
                Text = "Per deelnemende fase zijn er in ccol (slechts) twee instellingen: de minimale groentijd en de maximale groentijd, beide in seconden."
            });

            data.Elements.Add(new SpecificationParagraph
            {
                Type = SpecificationParagraphType.TableHeader,
                Text = "CCOL instellingen AFM"
            });

            var table = new List<List<string>>();
            table.Add(new List<string>
            {
                    "Fase" + " (##)",
                    "Schaduw fase",
                    "Min. groen                         in seconden                           PRM AFM##_GmaxMin",
                    "Max. groen                         in seconden                           PRM AFM##_GmaxMax",

            });
            foreach (var fc in _afmModel.AFMFasen)
            {
                table.Add(new List<string>
                    {
                        fc.FaseCyclus.ToString(),
                        fc.DummyFaseCyclus.ToString(),
                        fc.MinimaleGroentijd.ToString(),
                        fc.MaximaleGroentijd.ToString(),
                    });
            }
            data.Elements.Add(new SpecificationTable { TableData = table });

            data.Elements.Add(new SpecificationParagraph
            {
                Type = SpecificationParagraphType.Spacer,
                Text = ""
            });

            data.Elements.Add(new SpecificationParagraph
            {
                Type = SpecificationParagraphType.TableHeader,
                Text = "AFM conflicten shadow-fc's"
            });

            table = new List<List<string>>();
            var AFMlist= new List<FaseCyclusModel> { };
            var AFMDummylist = new List<FaseCyclusModel> { };
            var top = new List<string> { "" };

            /* bepalen fasen die meedoen aan AFM; veronderstelt symmetrische conflictmatrix (ook voor FK, GK, GKL) */
            foreach (var fcVan in c.Fasen)
            {
                foreach (var AFMfc in _afmModel.AFMFasen)
                {
                    var ot = c.InterSignaalGroep.Conflicten.FirstOrDefault(x => x.FaseVan == fcVan.Naam && x.FaseNaar == AFMfc.DummyFaseCyclus);
                    if (ot != null)
                    {
                        if (!AFMlist.Contains(fcVan))
                        {
                            AFMlist.Add(fcVan);
                        }
                    }
                    if (fcVan.Naam == AFMfc.DummyFaseCyclus)
                    {
                        AFMlist.Add(fcVan);
                        AFMDummylist.Add(fcVan);
                    }
                }
            }

            foreach (var AFMlistfc in AFMlist)
            {
                top.Add(AFMlistfc.Naam);
            }
            table.Add(top);

            foreach (var fcVan in AFMlist)
            {
                var tijden = new List<string> { fcVan.Naam };
            
                foreach (var fcNaar in AFMlist)
                {
                    if (ReferenceEquals(fcVan, fcNaar))
                    {
                        tijden.Add("X");
                    }
                    else
                    {
                        var ot = c.InterSignaalGroep.Conflicten.FirstOrDefault(x => x.FaseVan == fcVan.Naam && x.FaseNaar == fcNaar.Naam);
                        if ((ot != null) && (AFMDummylist.Contains(fcVan) || AFMDummylist.Contains(fcNaar)))
                        {
                            if (ot.Waarde == -2)
                            {
                                tijden.Add("FK");
                            }
                            else if (ot.Waarde == -3)
                            {
                                tijden.Add("GK");
                            }
                            else if (ot.Waarde == -4)
                            {
                                tijden.Add("GKL");
                            }
                            else
                            {
                                tijden.Add(ot.Waarde.ToString());
                            }
                        }
                        else
                        {
                            tijden.Add("");
                        }
                    }
                }
                table.Add(tijden);
            }

            data.Elements.Add(new SpecificationParagraph
            {
                Type = SpecificationParagraphType.Spacer,
                Text = ""
            });

            data.Elements.Add(new SpecificationParagraph
            {
                Type = SpecificationParagraphType.Header2,
                Text = "AFM interface beschrijving"
            });

            data.Elements.Add(new SpecificationParagraph
            {
                Type = SpecificationParagraphType.Body,
                Text = "De informatie uitwisseling tussen de regeling en de AFM centrale loopt via vrije parameters op de CVN-C interface. " +
                "Deze parameters dienen niet door de gebruiker (handmatig) te worden gewijzigd. " +
                "In onderstaand overzicht wordt per gebruikte parameter een korte uitleg gegeven. De parameters bestaan voor alle fasen " +
                "die deelnemen aan AFM."
            });

            data.Elements.Add(new SpecificationParagraph
            {
                Type = SpecificationParagraphType.Body,
                Text = "NB: Het wijzigen van de parameters door " +
                "de regeling of vanuit de AFM centrale kan gevolgen hebben voor het vullen / vervuilen van het parameterlogboek. " 
            });
            
            var bulletlist = new List<Tuple<string, int>>();

            bulletlist.Add(new Tuple<string, int>("Parameters die door de gebruiker worden bepaald:", 0));
            bulletlist.Add(new Tuple<string, int>("PRM AFM##_GmaxMin:                                                                                                                  " +
                "Ondergrens van de maximum groentijd voor fase ##, in seconden.", 1));
            bulletlist.Add(new Tuple<string, int>("PRM AFM##_GmaxMax:                                                                                                                  " +
                "Bovengrens van de maximum groentijd voor fase ##, in seconden.", 1));
            data.Elements.Add(new SpecificationBulletList { BulletData = bulletlist });

            data.Elements.Add(new SpecificationParagraph
            {
                Type = SpecificationParagraphType.Spacer,
                Text = ""
            });

            bulletlist = new List<Tuple<string, int>>();
            bulletlist.Add(new Tuple<string, int>("Parameters die door de regeling worden bepaald:", 0));
            bulletlist.Add(new Tuple<string, int>("PRM AFM##_FC:                                                                                                                       " +
               "Integer waarde van fase ## (voor bijvoorbeeld fc05 is de integer waarde 5).", 1));
            bulletlist.Add(new Tuple<string, int>("PRM AFM##_GmaxCCOL:                                                                                                                 " +
                "Huidige (klokperiode afhankelijke) waarde van de maximum groentijd van fase ##.", 1));
            bulletlist.Add(new Tuple<string, int>("PRM AFM##_GmaxAct:                                                                                                                  " +
                "Laatst gerealiseerde groentijd voor fase ## (gemeten in een cyclus die niet is afgekapt door een prio ingreep).", 1));
            bulletlist.Add(new Tuple<string, int>("PRM AFM##_GmaxGem:                                                                                                                  " +
                "Gemiddelde van de laatste drie gerealiseerde groentijden (gemeten over cycli die niet zijn afgekapt door een prio ingreep).", 1));
            bulletlist.Add(new Tuple<string, int>("PRM AFM##_Afgekapt:                                                                                                                 " +
                "Indicatie dat fase ## is afgekapt door een prio ingreep.", 1));
            bulletlist.Add(new Tuple<string, int>("PRM AFM_TC:                                                                                                                         " +
                "Laatst gerealiseerde cyclustijd (gemeten vanaf start ML2).", 1));
            bulletlist.Add(new Tuple<string, int>("PRM AFM_TCGem:                                                                                                                      " +
                "Gemiddelde van de laatste drie gerealiseerde cyclustijden", 1));
            bulletlist.Add(new Tuple<string, int>("PRM AFM_Beinvloedbaar:                                                                                                              " +
                "Melding aan de AFM centrale dat de regeling beïnvloedbaar is (criteria: toestand regelen, geen fixatie en levensignaal van AFM centrale aanwezig).", 1));
            bulletlist.Add(new Tuple<string, int>("PRM AFM_Watchdog:                                                                                                                   " +
                "Waarde van het levensignaal vanuit de AFM centrale (wordt iedere 'tick' verhoogd).", 1));
            data.Elements.Add(new SpecificationBulletList { BulletData = bulletlist });

            data.Elements.Add(new SpecificationParagraph
            {
                Type = SpecificationParagraphType.Spacer,
                Text = ""
            });

            bulletlist = new List<Tuple<string, int>>();
            bulletlist.Add(new Tuple<string, int>("Parameters die door de AFM centrale worden bepaald:", 0));
            bulletlist.Add(new Tuple<string, int>("PRM AFM##_GmaxAFM:                                                                                                                  " +
                "Door de AFM centrale gewenste maximum groentijd voor fase ##.", 1));
            bulletlist.Add(new Tuple<string, int>("PRM AFM##_Sturing:                                                                                                                  " +
                "De AFM centrale wenst controle over de groentijd van fase ##.", 1));
            bulletlist.Add(new Tuple<string, int>("PRM AFM##_Qlength:                                                                                                                  " +
                "Door de AFM centrale berekende wachtrij voor fase ## (kan in de regeling worden gebruikt indien gewenst).", 1));
            bulletlist.Add(new Tuple<string, int>("PRM AFM##_AbsBufferRuimte:                                                                                                          " +
                "Door de AFM centrale berekende absolute bufferruimte voor fase ## (kan in de regeling worden gebruikt indien gewenst).", 1));
            bulletlist.Add(new Tuple<string, int>("PRM AFM##_RelBufferRuimte:                                                                                                          " +
                "Door de AFM centrale berekende relatieve bufferruimte voor fase ## (kan in de regeling worden gebruikt indien gewenst).", 1));
            bulletlist.Add(new Tuple<string, int>("PRM AFM##_RelBufferVulling:                                                                                                         " +
                "Door de AFM centrale berekende relatieve buffervulling voor fase ## (kan in de regeling worden gebruikt indien gewenst).", 1));
            bulletlist.Add(new Tuple<string, int>("PRM AFM_Strikt:                                                                                                                     " +
                "Regelen met zo goed mogelijk behoud van cyclustijd (dit kan leiden tot 'zinloos groen').", 1));
            bulletlist.Add(new Tuple<string, int>("PRM AFM_WatchdogReturn:                                                                                                             " +
                "Waarde van het levensignaal naar de AFM centrale (wordt iedere 'tick' verhoogd).", 1));
            data.Elements.Add(new SpecificationBulletList { BulletData = bulletlist });
            
            data.Elements.Add(new SpecificationParagraph
            {
                Type = SpecificationParagraphType.Spacer,
                Text = ""
            });

            data.Elements.Add(new SpecificationParagraph
            {
                Type = SpecificationParagraphType.Body,
                Text = "Daarnaast is in PRM AFM_Versie het gebruikte versienummer opgeslagen en kan via PRM AFM_Test de verbinding met " +
                "de AFM centrale getest worden zonder dat AFM de regeling beïnvloedt."
            });

            data.Elements.Add(new SpecificationParagraph
            {
                Type = SpecificationParagraphType.Spacer,
                Text = ""
            });

            data.Elements.Add(new SpecificationParagraph
            {
                Type = SpecificationParagraphType.Body,
                Text = "Naast bovenstaande vrije parameters bestaan er voor AFM twee timers en drie schakelaars: " +
                ""
            });

            bulletlist = new List<Tuple<string, int>>();
            bulletlist.Add(new Tuple<string, int>("T AFMLeven, default 120 seconden:                                                                                                           " +
                "Ten behoeve van levensignaal vanuit de AFM centrale ('zaagtandfrekwentie' via PRM AFM_Watchdog).", 0));
            bulletlist.Add(new Tuple<string, int>("T VRILeven, default 60 seconden:                                                                                                            " +
                "Ten behoeve van levensignaal richting de AFM centrale ('zaagtandfrekwentie', via PRM AFM_WatchdogReturn).", 0));
            bulletlist.Add(new Tuple<string, int>("SCH AFMExtraGroenBijFile, default Aan:                                                                                                      " +
                "Niet (meer) gebruikt.", 0));
            bulletlist.Add(new Tuple<string, int>("SCH AFMCIFParmWijz, default Aan:                                                                                                            " +
                "Ten behoeve van doorgeven van gewijzigde waarden aan de CVN-C interface via CIF_PARM1WIJZAP.", 0));
            bulletlist.Add(new Tuple<string, int>("SCH AFM_overbrugging, default Uit:                                                                                                          " +
                "Ten behoeve van verwerken aansturing vanuit AFM centrale zonder aanwezigheid levensignaal.", 0));
            data.Elements.Add(new SpecificationBulletList { BulletData = bulletlist });

            return data;
        }

        #endregion //ITLCGenHasSpecification
    }
}
