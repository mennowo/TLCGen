using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using TLCGen.Extensions;
using TLCGen.Generators.CCOL.CodeGeneration;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.Plugins.Sumo
{
    [CCOLCodePieceGenerator]
    [TLCGenTabItem(-1, TabItemTypeEnum.SpecialsTab)]
    [TLCGenPlugin(TLCGenPluginElems.PlugMessaging |
                  TLCGenPluginElems.TabControl |
                  TLCGenPluginElems.XMLNodeWriter)]
    public class SumoPlugin : CCOLCodePieceGeneratorBase, ITLCGenPlugMessaging, ITLCGenTabItem, ITLCGenXMLNodeWriter
    {
        #region Fields

        private SumoPluginTabViewModel _sumoPluginViewModel;
        private SumoDataModel _MyModel;

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
                if (_Controller == null)
                {
                    _MyModel = new SumoDataModel();
                    _sumoPluginViewModel.Data = _MyModel;
                }
                UpdateModel();
            }
        }

        public string DisplayName
        {
            get
            {
                return "Sumo";
            }
        }

        public string GetPluginName()
        {
            return "Sumo";
        }
        public bool IsEnabled { get; set; }

        #endregion // ITLCGen plugin shared items

        internal void UpdateModel()
        {
            if (_Controller != null && _MyModel != null)
            {
                foreach (var fc in Controller.Fasen)
                {
                    if (_sumoPluginViewModel.FaseCycli.All(x => x.Naam != fc.Naam))
                    {
                        _sumoPluginViewModel.FaseCycli.Add(
                            new FaseCyclusSumoDataViewModel(
                                new FaseCyclusSumoDataModel { Naam = fc.Naam, SumoIds = "" }));
                    }
                }
                var rems = new List<FaseCyclusSumoDataViewModel>();
                foreach (var fc in _sumoPluginViewModel.FaseCycli)
                {
                    if (Controller.Fasen.All(x => x.Naam != fc.Naam))
                    {
                        rems.Add(fc);
                    }
                }
                foreach (var sg in rems)
                {
                    _sumoPluginViewModel.FaseCycli.Remove(sg);
                }
                _sumoPluginViewModel.FaseCycli.BubbleSort();
                var ds = Controller.GetAllDetectors().Concat(Controller.PrioData.GetAllDummyDetectors()).ToList();
                foreach (var d in ds)
                {
                    if (_sumoPluginViewModel.Detectoren.All(x => x.Naam != d.Naam))
                    {
                        _sumoPluginViewModel.Detectoren.Add(
                            new DetectorSumoDataViewModel(
                                new DetectorSumoDataModel
                                {
                                    Naam = d.Naam,
                                    SumoNaam1 = d.Naam,
                                    SumoNaam2 = "",
                                    SumoNaam3 = ""
                                }));
                    }
                }
                var drems = new List<DetectorSumoDataViewModel>();
                foreach (var fc in _sumoPluginViewModel.Detectoren)
                {
                    if (ds.All(x => x.Naam != fc.Naam))
                    {
                        drems.Add(fc);
                    }
                }
                foreach (var rd in drems)
                {
                    _sumoPluginViewModel.Detectoren.Remove(rd);
                }

                _sumoPluginViewModel.Detectoren.BubbleSort();

                _sumoPluginViewModel.RaisePropertyChanged("");
            }
        }

        #region ITLCGenTabItem

        DataTemplate _ContentDataTemplate;
        public DataTemplate ContentDataTemplate
        {
            get
            {
                if (_ContentDataTemplate == null)
                {
                    _ContentDataTemplate = new DataTemplate();
                    var tab = new FrameworkElementFactory(typeof(SumoPluginTabView));
                    tab.SetValue(SumoPluginTabView.DataContextProperty, _sumoPluginViewModel);
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
            _sumoPluginViewModel?.UpdateTLCGenMessaging();
        }

        #endregion // ITLCGenPlugMessaging

        #region ITLCGenXMLNodeWriter

        public void GetXmlFromDocument(XmlDocument document)
        {
            _MyModel = null;

            foreach (XmlNode node in document.FirstChild.ChildNodes)
            {
                if (node.LocalName == "SumoKoppeling")
                {
                    _MyModel = XmlNodeConverter.ConvertNode<SumoDataModel>(node);
                    break;
                }
            }

            if (_MyModel == null)
            {
                _MyModel = new SumoDataModel();
            }
            _sumoPluginViewModel.Data = _MyModel;
            _sumoPluginViewModel.FaseCycli.BubbleSort();
            _sumoPluginViewModel.Detectoren.BubbleSort();
            _sumoPluginViewModel.RaisePropertyChanged("");
        }

        public void SetXmlInDocument(XmlDocument document)
        {
            XmlDocument doc = TLCGenSerialization.SerializeToXmlDocument(_MyModel);
            XmlNode node = document.ImportNode(doc.DocumentElement, true);
            document.DocumentElement.AppendChild(node);
        }

        #endregion // ITLCGenXMLNodeWriter

        #region CCOLCodePieceGenerator

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.TabCIncludes:
                    return 201;
                case CCOLCodeTypeEnum.TabCControlParameters:
                    return 201;
                case CCOLCodeTypeEnum.RegCIncludes:
                    return 201;
                case CCOLCodeTypeEnum.RegCTop:
                    return 201;
                case CCOLCodeTypeEnum.RegCPreApplication:
                    return 201;
                case CCOLCodeTypeEnum.RegCPostApplication:
                    return 201;
                case CCOLCodeTypeEnum.RegCSpecialSignals:
                    return 201;
                default:
                    return 0;
            }
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            StringBuilder sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.TabCIncludes:
                    if (!_MyModel.GenererenSumoCode)
                    {
                        return "";
                    }
                    sb.AppendLine("#ifdef SUMO");
                    sb.AppendLine($"{ts}typedef struct sumoDetStruct {{");
                    sb.AppendLine($"{ts}	char SumoNamen[3][32];");
                    sb.AppendLine($"{ts}	char Selectief;");
                    sb.AppendLine($"{ts}}} SUMODET;");
                    sb.AppendLine($"{ts}SUMODET SUMODetectors[DPMAX];");
                    sb.AppendLine($"{ts}int SUMOIds[{_MyModel.SumoKruispuntLinkMax}];");
                    sb.AppendLine($"{ts}int isumo;");
                    sb.AppendLine("#endif // #ifdef SUMO");
                    return sb.ToString();

                case CCOLCodeTypeEnum.TabCControlParameters:
                    if (!_MyModel.GenererenSumoCode)
                    {
                        return "";
                    }
                    sb.AppendLine("#ifdef SUMO");
                    sb.AppendLine($"{ts}/* SUMO KOPPELING */");
                    sb.AppendLine($"{ts}/* ============== */");
                    var id = _MyModel.PrependIdToDetectors ? _MyModel.SumoKruispuntNaam : "";
                    foreach(var d in _MyModel.Detectoren.Where(x => !string.IsNullOrWhiteSpace(x.SumoNaam1)))
                    {

                        sb.AppendLine($"{ts}sprintf_s(SUMODetectors[{_dpf}{d.Naam}].SumoNamen[0], 32, \"%s\", \"{id}{d.SumoNaam1}\");");
                        if (!string.IsNullOrWhiteSpace(d.SumoNaam2)) sb.AppendLine($"{ts}sprintf_s(SUMODetectors[{_dpf}{d.Naam}].SumoNamen[1], 32, \"%s\", \"{id}{d.SumoNaam2}\");");
                        if (!string.IsNullOrWhiteSpace(d.SumoNaam3)) sb.AppendLine($"{ts}sprintf_s(SUMODetectors[{_dpf}{d.Naam}].SumoNamen[2], 32, \"%s\", \"{id}{d.SumoNaam3}\");");
                        if (d.Selectief) sb.AppendLine($"{ts}SUMODetectors[{_dpf}{d.Naam}].Selectief = TRUE;");
                    }
                    sb.AppendLine($"{ts}for (isumo = 0; isumo < {_MyModel.SumoKruispuntLinkMax}; ++isumo)");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}SUMOIds[isumo] = NG;");
                    sb.AppendLine($"{ts}}}");
                    foreach (var fc in _MyModel.FaseCycli)
                    {
                        var ids = fc.SumoIds.Replace(" ", "").Split(',');
                        foreach(var sid in ids)
                        {
                            sb.AppendLine($"{ts}SUMOIds[{sid}] = {_fcpf}{fc.Naam};");
                        }
                    }
                    sb.AppendLine("#endif // #ifdef SUMO");
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCIncludes:
                    if (!_MyModel.GenererenSumoCode)
                    {
                        return "";
                    }
                    sb.AppendLine("#ifdef SUMO");
                    sb.AppendLine("#include \"cctracic_public.h\"");
                    sb.AppendLine("#endif // #ifdef SUMO");
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCTop:
                    if (!_MyModel.GenererenSumoCode)
                    {
                        return "";
                    }
                    sb.AppendLine($"{ts}#ifdef SUMO");
                    sb.AppendLine($"{ts}/* SUMO KOPPELING */");
                    sb.AppendLine($"{ts}/* ============== */");
                    sb.AppendLine($"{ts}static char sumostart = TRUE;");
                    sb.AppendLine($"{ts}static int isumo = 0;");
                    sb.AppendLine($"{ts}static char csumotmp[64];");
                    sb.AppendLine($"{ts}typedef struct sumoDetStruct {{");
                    sb.AppendLine($"{ts}	char SumoNamen[3][32];");
                    sb.AppendLine($"{ts}	char Selectief;");
                    sb.AppendLine($"{ts}}} SUMODET;");
                    sb.AppendLine($"{ts}char SUMOStateString[{_MyModel.SumoKruispuntLinkMax + 1}];");
                    sb.AppendLine($"{ts}extern int SUMOIds[{_MyModel.SumoKruispuntLinkMax}];");
                    sb.AppendLine($"{ts}extern SUMODET SUMODetectors[DPMAX];");
                    if (_MyModel.AutoStartSumo)
                    {
                        sb.AppendLine($"{ts}PROCESS_INFORMATION pi;");
                        sb.AppendLine($"{ts}void CloseSumo(void)");
                        sb.AppendLine($"{ts}{{");
                        sb.AppendLine($"{ts}{ts}TerminateProcess(pi.hProcess, 0);");
                        sb.AppendLine($"{ts}}}");
                    }
                    sb.AppendLine($"{ts}#endif // #ifdef SUMO");
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCPreApplication:
                    if (!_MyModel.GenererenSumoCode)
                    {
                        return "";
                    }
                    sb.AppendLine($"{ts}/* SUMO KOPPELING */");
                    sb.AppendLine($"{ts}/* ============== */");
                    sb.AppendLine($"{ts}#ifdef SUMO");
                    sb.AppendLine($"{ts}if (sumostart)");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}sumostart = FALSE;");
                    if (_MyModel.AutoStartSumo)
                    {
                        sb.AppendLine($"{ts}{ts}STARTUPINFO si;");
                        sb.AppendLine($"{ts}{ts}ZeroMemory(&si, sizeof(si));");
                        sb.AppendLine($"{ts}{ts}si.cb = sizeof(si);");
                        sb.AppendLine($"{ts}{ts}ZeroMemory(&pi, sizeof(pi));");
                        sb.AppendLine();
                        sb.AppendLine($"{ts}{ts}// Start SUMO!");
                        sb.AppendLine($"{ts}{ts}if (!CreateProcess(NULL,");
                        sb.AppendLine($"{ts}{ts}\"\\\"{System.IO.Path.Combine(_MyModel.SumoHomePath ?? "", "bin", "sumo-gui.exe").Replace(@"\", @"\\")}\\\" \\\"{_MyModel.SumoConfigPath?.Replace(@"\", @"\\") ?? ""}\\\"\",");
                        sb.AppendLine($"{ts}{ts}NULL, NULL, FALSE, 0, NULL, NULL, &si, &pi))");
                        sb.AppendLine($"{ts}{ts}{{");
                        sb.AppendLine($"{ts}{ts}{ts}sprintf(csumotmp, \"CreateProcess failed (%d).\\n\", GetLastError());");
                        sb.AppendLine($"{ts}{ts}{ts}uber_puts(csumotmp);");
                        sb.AppendLine($"{ts}{ts}}}");
                        sb.AppendLine($"{ts}{ts}else");
                        sb.AppendLine($"{ts}{ts}{{");
                        sb.AppendLine($"{ts}{ts}{ts}atexit(CloseSumo);");
                        sb.AppendLine($"{ts}{ts}}}");
                    }
                    sb.AppendLine($"{ts}{ts}TraCIConnect(\"127.0.0.1\", \"{_MyModel.SumoPort}\");");
                    sb.AppendLine($"{ts}{ts}CIF_KLOK[CIF_UUR] = {_MyModel.StartTijdUur};");
                    sb.AppendLine($"{ts}{ts}CIF_KLOK[CIF_MINUUT] = {_MyModel.StartTijdMinuut};");
                    sb.AppendLine($"{ts}{ts}   TraCISetOrder({_MyModel.SumoOrder});");
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine($"{ts}#endif // #ifdef SUMO");
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCPostApplication:
                    if (!_MyModel.GenererenSumoCode)
                    {
                        return "";
                    }
                    sb.AppendLine($"{ts}#ifdef SUMO");
                    sb.AppendLine($"{ts}{ts}for (isumo = 0; isumo < {_MyModel.SumoKruispuntLinkMax}; ++isumo)");
                    sb.AppendLine($"{ts}{ts}{{");
                    sb.AppendLine($"{ts}{ts}{ts}SUMOStateString[isumo] = 'O';");
                    sb.AppendLine($"{ts}{ts}{ts}if (SUMOIds[isumo] != NG)");
                    sb.AppendLine($"{ts}{ts}{ts}{{");
                    sb.AppendLine($"{ts}{ts}{ts}{ts}SUMOStateString[isumo] = G[SUMOIds[isumo]] ? 'G' : GL[SUMOIds[isumo]] ? 'y' : 'r';");
                    sb.AppendLine($"{ts}{ts}{ts}}}");
                    sb.AppendLine($"{ts}{ts}}}");
                    sb.AppendLine($"{ts}{ts}SUMOStateString[{_MyModel.SumoKruispuntLinkMax}] = '\\0';");
                    sb.AppendLine($"{ts}{ts}TraCISetTrafficLightState(\"{_MyModel.SumoKruispuntNaam ?? ""}\", SUMOStateString);");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}TraCIControlSimStep();");
                    sb.AppendLine($"{ts}#endif // #ifdef SUMO");
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCSpecialSignals:
                    if (!_MyModel.GenererenSumoCode)
                    {
                        return "";
                    }
                    sb.AppendLine($"{ts}#ifdef SUMO");
                    sb.AppendLine($"{ts}for (isumo = 0; isumo < DPMAX; isumo++)");
                    sb.AppendLine($"{ts}{{");
                    if (_MyModel.Detectoren.Any(x => string.IsNullOrWhiteSpace(x.SumoNaam1)))
                    {
                        sb.Append($"{ts}{ts}if (");
                        var first = true;
                        foreach (var d in _MyModel.Detectoren.Where(x => string.IsNullOrWhiteSpace(x.SumoNaam1)))
                        {
                            if (!first) sb.Append(" || ");
                            first = false;
                            sb.Append($"isumo == {_dpf}{d.Naam}");
                        }
                        sb.AppendLine($"{ts}{ts})");
                        sb.AppendLine($"{ts}{ts}{{");
                        sb.AppendLine($"{ts}{ts}{ts}continue;");
                        sb.AppendLine($"{ts}{ts}}}");
                    }
                    sb.AppendLine($"{ts}{ts}CIF_IS[isumo] = FALSE;");
                    sb.AppendLine($"{ts}{ts}for (int d = 0; d < 3; ++d)");
                    sb.AppendLine($"{ts}{ts}{{");
                    sb.AppendLine($"{ts}{ts}{ts}if (strlen(SUMODetectors[isumo].SumoNamen[d]) > 1)");
                    sb.AppendLine($"{ts}{ts}{ts}{{");
                    sb.AppendLine($"{ts}{ts}{ts}{ts}if (!SUMODetectors[isumo].Selectief)");
                    sb.AppendLine($"{ts}{ts}{ts}{ts}{{");
                    sb.AppendLine($"{ts}{ts}{ts}{ts}{ts}double dd = TraCIGetLaneAreaLastStepOccupancy(SUMODetectors[isumo].SumoNamen[d]);");
                    sb.AppendLine($"{ts}{ts}{ts}{ts}{ts}if (dd > 0.1) CIF_IS[isumo] = TRUE;");
                    sb.AppendLine($"{ts}{ts}{ts}{ts}}}");
                    sb.AppendLine($"{ts}{ts}{ts}{ts}else");
                    sb.AppendLine($"{ts}{ts}{ts}{ts}{{");
                    sb.AppendLine($"{ts}{ts}{ts}{ts}{ts}int dd = TraCIGetInductionLoopLastStepVehicleNumber(SUMODetectors[isumo].SumoNamen[d], 0);");
                    sb.AppendLine($"{ts}{ts}{ts}{ts}{ts}if (dd != 0) CIF_IS[isumo] = TRUE;");
                    sb.AppendLine($"{ts}{ts}{ts}{ts}}}");
                    sb.AppendLine($"{ts}{ts}{ts}}}");
                    sb.AppendLine($"{ts}{ts}}}");
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine($"{ts}CIF_ISWIJZ = TRUE;");
                    sb.AppendLine($"{ts}#endif // #ifdef SUMO");
                    return sb.ToString();

                default:
                    return null;
            }
        }

        #endregion // CCOLCodePieceGenerator

        #region Constructor

        public SumoPlugin() : base()
        {
            _sumoPluginViewModel = new SumoPluginTabViewModel(this);
        }

        #endregion // Constructor
    }
}
