using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using TLCGen.Extensions;
using TLCGen.Generators.CCOL.CodeGeneration;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Plugins.RIS.Models;

namespace TLCGen.Plugins.RIS
{
    [TLCGenTabItem(-1, TabItemTypeEnum.FasenTab)]
    [TLCGenPlugin(
        TLCGenPluginElems.TabControl | 
        TLCGenPluginElems.XMLNodeWriter |
        TLCGenPluginElems.PlugMessaging | 
        TLCGenPluginElems.IOElementProvider)]
    [CCOLCodePieceGenerator]
    public partial class RISPlugin : CCOLCodePieceGeneratorBase, ITLCGenTabItem, ITLCGenXMLNodeWriter, ITLCGenPlugMessaging, ITLCGenElementProvider
    {
        #region Fields

        private RISTabViewModel _RISVM;
        private RISDataModel _RISModel;

#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _prmrisastart;
        private CCOLGeneratorCodeStringSettingModel _prmrisaend;
        private CCOLGeneratorCodeStringSettingModel _prmrisvstart;
        private CCOLGeneratorCodeStringSettingModel _prmrisvend;
#pragma warning restore 0649

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
                    _RISModel = new RISDataModel();
                    _RISVM.RISModel = _RISModel;
                }
                UpdateModel();
            }
        }

        public string GetPluginName()
        {
            return "RIS";
        }

        #endregion // TLCGen plugin shared

        #region ITLCGenTabItem

        public string DisplayName => "RIS";
        public ImageSource Icon => null;

        DataTemplate _ContentDataTemplate;
        public DataTemplate ContentDataTemplate
        {
            get
            {
                if (_ContentDataTemplate == null)
                {
                    _ContentDataTemplate = new DataTemplate();
                    var tab = new FrameworkElementFactory(typeof(RISTabView));
                    tab.SetValue(RISTabView.DataContextProperty, _RISVM);
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
            if (!_RISVM.RISRequestLanes.IsSorted())
            {
                _RISVM.RISRequestLanes.BubbleSort();
            }
            if (!_RISVM.RISExtendLanes.IsSorted())
            {
                _RISVM.RISExtendLanes.BubbleSort();
            }
        }

        public bool OnSelectedPreview()
        {
            return true;
        }

        #endregion // ITLCGenTabItem

        #region ITLCGenXMLNodeWriter

        public void GetXmlFromDocument(XmlDocument document)
        {
            _RISModel = null;

            foreach (XmlNode node in document.FirstChild.ChildNodes)
            {
                if (node.LocalName == "RISData")
                {
                    _RISModel = XmlNodeConverter.ConvertNode<RISDataModel>(node);
                    break;
                }
            }

            if (_RISModel == null)
            {
                _RISModel = new RISDataModel();
            }
            _RISVM.RISModel = _RISModel;
            _RISVM.RaisePropertyChanged("");
        }

        public void SetXmlInDocument(XmlDocument document)
        {
            XmlDocument doc = TLCGenSerialization.SerializeToXmlDocument(_RISModel);
            XmlNode node = document.ImportNode(doc.DocumentElement, true);
            document.DocumentElement.AppendChild(node);
        }

        #endregion // ITLCGenXMLNodeWriter

        #region ITLCGenPlugMessaging

        public void UpdateTLCGenMessaging()
        {
            _RISVM.UpdateMessaging();
        }

        #endregion // ITLCGenPlugMessaging

        #region ITLCGenElementProvider

        public List<IOElementModel> GetOutputItems()
        {
            return new List<IOElementModel>();
        }

        public List<IOElementModel> GetInputItems()
        {
            List<IOElementModel> items = new List<IOElementModel>();
            if (!_RISModel.RISToepassen) return items;
            foreach (var station in _RISModel.RISFasen.SelectMany(x => x.LaneData).SelectMany(x => x.SimulatedStations))
            {
                station.StationBitmapData.Naam = station.Naam;
                station.StationBitmapData.Dummy = true;
                items.Add(station.StationBitmapData);
            }
            return items;
        }

        public bool IsElementNameUnique(string name)
        {
            return true;
        }

        public List<object> GetAllItems()
        {
            return new List<object>();
        }

        #endregion // ITLCGenElementProvider

        #region CCOLCodePieceGenerator

        public override bool HasSettings()
        {
            return true;
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            return base.SetSettings(settings);
        }

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();
            _myBitmapInputs = new List<CCOLIOElement>();

            if (_RISModel.RISToepassen)
            {
                foreach (var l in _RISModel.RISRequestLanes)
                {
                    if (l.RISAanvraag)
                    {
                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_prmrisastart}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}",
                            l.AanvraagStart,
                            CCOLElementTimeTypeEnum.None,
                            _prmrisastart, l.SignalGroupName));
                    }
                }
                foreach (var l in _RISModel.RISRequestLanes)
                {
                    if (l.RISAanvraag)
                    {
                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_prmrisaend}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}",
                            l.AanvraagEnd,
                            CCOLElementTimeTypeEnum.None,
                            _prmrisaend, l.SignalGroupName));
                    }
                }
                foreach (var l in _RISModel.RISExtendLanes)
                {
                    if (l.RISVerlengen)
                    {
                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_prmrisvstart}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}",
                            l.VerlengenStart,
                            CCOLElementTimeTypeEnum.None,
                            _prmrisvstart, l.SignalGroupName));
                    }
                }
                foreach (var l in _RISModel.RISExtendLanes)
                {
                    if (l.RISVerlengen)
                    {
                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_prmrisvend}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}",
                            l.VerlengenEnd,
                            CCOLElementTimeTypeEnum.None,
                            _prmrisvend, l.SignalGroupName));
                    }
                }
            }
            var lanesSim = _RISModel.RISFasen.SelectMany(x => x.LaneData);
            foreach (var s in lanesSim.SelectMany(x => x.SimulatedStations))
            {
                s.StationBitmapData.Naam = s.Naam;
                var e = CCOLGeneratorSettingsProvider.Default.CreateElement(s.Naam, CCOLElementTypeEnum.Ingang, "");
                e.Dummy = true;
                _myElements.Add(e);
                _myBitmapInputs.Add(new CCOLIOElement(s.StationBitmapData, $"{_ispf}{s.Naam}"));
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

        public override bool HasCCOLBitmapInputs()
        {
            return false; // No need: ITLCGenElementProvider arranges for this
        }

        public override bool HasSimulationElements()
        {
            return _RISModel.RISToepassen;
        }

        public override IEnumerable<DetectorSimulatieModel> GetSimulationElements()
        {
            return _RISModel.RISFasen.SelectMany(x => x.LaneData).SelectMany(x => x.SimulatedStations).Select(x => x.SimulationData);
        }

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCIncludes:
                    return 110;
                case CCOLCodeTypeEnum.RegCInitApplication:
                    return 110;
                case CCOLCodeTypeEnum.RegCAanvragen:
                    return 110;
                case CCOLCodeTypeEnum.RegCMeetkriterium:
                    return 110;
                case CCOLCodeTypeEnum.RegCPostSystemApplication:
                    return 110;
                case CCOLCodeTypeEnum.SysHBeforeUserDefines:
                    return 110;
                default:
                    return 0;
            }
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {

            if (!_RISModel.RISToepassen) return "";

            StringBuilder sb = new StringBuilder();

            var lanes = _RISModel.RISFasen.SelectMany(x => x.LaneData);
            
            switch (type)
            {
                case CCOLCodeTypeEnum.SysHBeforeUserDefines:
                    sb.AppendLine($"/* Systeem naam in het topologiebestand */");
                    sb.AppendLine($"/* ------------------------------------ */");
                    sb.AppendLine($"#define SYSTEM_ITF \"{_RISModel.SystemITF}\"");
                    sb.AppendLine();
                    sb.AppendLine($"/* Definitie lane id in het topologiebestand */");
                    sb.AppendLine($"/* ----------------------------------------- */");
                    foreach (var l in lanes)
                    {
                        sb.AppendLine($"#define ris_lane{l.SignalGroupName}{l.RijstrookIndex} {l.LaneID} /* lane ID van richting {l.SignalGroupName} van strook {l.RijstrookIndex + 1} */");
                    }
                    sb.AppendLine();
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCIncludes:

                    // Generate rissim.c now
                    GenerateRisSimC(c, _RISModel, ts);

                    sb.AppendLine($"{ts}#ifndef NO_RIS");
                    sb.AppendLine($"{ts}{ts}#include \"risvar.c\" /* ccol ris controller */");
                    sb.AppendLine($"{ts}{ts}#include \"risappl.c\" /* RIS applicatiefuncties */");
                    sb.AppendLine($"{ts}{ts}#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST)");
                    sb.AppendLine($"{ts}{ts}{ts}#include \"rissimvar.h\" /* ccol ris simulatie functie */");
                    sb.AppendLine($"{ts}{ts}#endif");
                    sb.AppendLine($"{ts}#endif");
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCInitApplication:
                    sb.AppendLine($"{ts}#ifndef NO_RIS");
                    sb.AppendLine($"{ts}{ts}#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST)");
                    sb.AppendLine($"{ts}{ts}{ts}/* zet display van RIS-berichten aan in de testomgeving */");
                    sb.AppendLine($"{ts}{ts}{ts}/* ---------------------------------------------------- */");
                    sb.AppendLine($"{ts}{ts}{ts}RIS_DIPRM[RIS_DIPRM_ALL] = 1;");
                    sb.AppendLine($"{ts}{ts}#endif");
                    sb.AppendLine($"{ts}#endif");
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCAanvragen:
                    sb.AppendLine($"{ts}#ifndef NO_RIS");
                    foreach(var l in _RISModel.RISRequestLanes.Where(x => x.RISAanvraag))
                    {
                        sb.AppendLine($"{ts}{ts}if (ris_aanvraag({_fcpf}{l.SignalGroupName}, SYSTEM_ITF, ris_lane{l.SignalGroupName}{l.RijstrookIndex}, RIS_{l.Type}, PRM[{_prmpf}{_prmrisastart}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}], PRM[{_prmpf}{_prmrisaend}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}], TRUE)) A[{_fcpf}{l.SignalGroupName}] |= BIT10;");
                    }
                    sb.AppendLine($"{ts}#endif");
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCMeetkriterium:
                    sb.AppendLine($"{ts}#ifndef NO_RIS");
                    foreach (var l in _RISModel.RISExtendLanes.Where(x => x.RISVerlengen))
                    {
                        sb.AppendLine($"{ts}{ts}if (ris_verlengen({_fcpf}{l.SignalGroupName}, SYSTEM_ITF, ris_lane{l.SignalGroupName}{l.RijstrookIndex}, RIS_{l.Type}, PRM[{_prmpf}{_prmrisvstart}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}], PRM[{_prmpf}{_prmrisvend}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}], TRUE)) MK[{_fcpf}{l.SignalGroupName}] |= BIT10; else  MK[{_fcpf}{l.SignalGroupName}] &= ~BIT10;");
                    }
                    sb.AppendLine($"{ts}#endif");
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCPostSystemApplication:
                    sb.AppendLine($"{ts}#ifndef NO_RIS");
                    sb.AppendLine($"{ts}{ts}#if (!defined AUTOMAAT && !defined AUTOMAAT_TEST)");
                    sb.AppendLine($"{ts}{ts}{ts}/* simulatie van RIS berichten */");
                    sb.AppendLine($"{ts}{ts}{ts}/* --------------------------- */");
                    sb.AppendLine($"{ts}{ts}{ts}ris_simulation(SAPPLPROG);");
                    sb.AppendLine($"{ts}{ts}#endif");
                    sb.AppendLine($"{ts}{ts}{ts}/* RIS-Controller */");
                    sb.AppendLine($"{ts}{ts}{ts}/* -------------- */");
                    sb.AppendLine($"{ts}{ts}{ts}ris_controller(SAPPLPROG, TRUE);");
                    sb.AppendLine($"{ts}#endif");
                    return sb.ToString();
                default:
                    return null;
            }
        }

        public override List<string> GetSourcesToCopy()
        {
            if (!_RISModel.RISToepassen) return null;
            return new List<string>
            {
                "risappl.c"
            };
        }

        #endregion // CCOLCodePieceGenerator

        #region Private Methods

        internal static RISFaseCyclusLaneSimulatedStationViewModel GetNewStationForSignalGroup(FaseCyclusModel sg, int LaneID, int RijstrookIndex)
        {
            var st = new RISFaseCyclusLaneSimulatedStationViewModel(new RISFaseCyclusLaneSimulatedStationModel());
            st.StationData.SignalGroupName = sg.Naam;
            st.StationData.RijstrookIndex = RijstrookIndex;
            st.StationData.LaneID = LaneID;
            if (sg != null)
            {
                switch (sg.Type)
                {
                    case TLCGen.Models.Enumerations.FaseTypeEnum.Auto:
                        st.Type = RISStationTypeSimEnum.PASSENGERCAR;
                        st.Flow = 200;
                        st.Snelheid = 50;
                        break;
                    case TLCGen.Models.Enumerations.FaseTypeEnum.Fiets:
                        st.Type = RISStationTypeSimEnum.CYCLIST;
                        st.Flow = 20;
                        st.Snelheid = 15;
                        break;
                    case TLCGen.Models.Enumerations.FaseTypeEnum.Voetganger:
                        st.Type = RISStationTypeSimEnum.PEDESTRIAN;
                        st.Flow = 20;
                        st.Snelheid = 5;
                        break;
                    case TLCGen.Models.Enumerations.FaseTypeEnum.OV:
                        st.Type = RISStationTypeSimEnum.BUS;
                        st.Flow = 10;
                        st.Snelheid = 45;
                        break;
                }
            }
            st.StationData.SimulationData.RelatedName = st.StationData.Naam;
            return st;
        }

        internal void UpdateModel()
        {
            if (_controller != null && _RISModel != null)
            {
                foreach (var fc in Controller.Fasen)
                {
                    if (_RISVM.RISFasen.All(x => x.FaseCyclus != fc.Naam))
                    {
                        var risfc = new RISFaseCyclusDataViewModel(
                                new RISFaseCyclusDataModel { FaseCyclus = fc.Naam });
                        for (int i = 0; i < fc.AantalRijstroken; i++)
                        {
                            risfc.Lanes.Add(new RISFaseCyclusLaneDataViewModel(new RISFaseCyclusLaneDataModel() { SignalGroupName = fc.Naam, RijstrookIndex = i + 1 }));
                        }
                        _RISVM.RISFasen.Add(risfc);
                    }
                    else
                    {
                        var risfc = _RISVM.RISFasen.FirstOrDefault(x => x.FaseCyclus == fc.Naam);
                        if (risfc != null)
                        {
                            if (fc.AantalRijstroken > risfc.Lanes.Count)
                            {
                                var i = risfc.Lanes.Count;
                                for (; i < fc.AantalRijstroken; i++)
                                {
                                    risfc.Lanes.Add(new RISFaseCyclusLaneDataViewModel(new RISFaseCyclusLaneDataModel() { SignalGroupName = fc.Naam, RijstrookIndex = i + 1}));
                                }
                            }
                            else if (fc.AantalRijstroken < risfc.Lanes.Count)
                            {
                                var i = risfc.Lanes.Count - fc.AantalRijstroken;
                                for (int j = 0; j < i; j++)
                                {
                                    if (risfc.Lanes.Any())
                                        risfc.Lanes.Remove(risfc.Lanes.Last());
                                }
                                var rem = _RISModel.RISRequestLanes.Where(x => x.SignalGroupName == fc.Naam && x.RijstrookIndex >= fc.AantalRijstroken).ToList();
                                foreach (var r in rem) _RISModel.RISRequestLanes.Remove(r);
                                var rem2 = _RISModel.RISExtendLanes.Where(x => x.SignalGroupName == fc.Naam && x.RijstrookIndex >= fc.AantalRijstroken).ToList();
                                foreach (var r in rem2) _RISModel.RISExtendLanes.Remove(r);
                            }
                        }
                    }
                }
                var rems = _RISVM.RISFasen.Where(x => Controller.Fasen.All(x2 => x2.Naam != x.FaseCyclus)).ToList(); new List<RISFaseCyclusDataViewModel>();
                foreach (var sg in rems)
                {
                    _RISVM.RISFasen.Remove(sg);
                }
                _RISVM.RISFasen.BubbleSort();
                foreach (var lre in _RISVM.RISRequestLanes) lre.UpdateRijstroken();
                foreach (var lre in _RISVM.RISExtendLanes) lre.UpdateRijstroken();
                _RISVM.RISRequestLanes.BubbleSort();
                _RISVM.RISExtendLanes.BubbleSort();
                _RISVM.UpdateRISLanes();
                _RISVM.RaisePropertyChanged("");
            }
        }

        #endregion // Private Methods

        #region Constructor

        public RISPlugin()
        {
            IsEnabled = true;
            _RISVM = new RISTabViewModel(this);
        }

        #endregion // Constructor
    }
}
