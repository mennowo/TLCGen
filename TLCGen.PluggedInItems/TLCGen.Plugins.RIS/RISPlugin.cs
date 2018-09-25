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
        TLCGenPluginElems.PlugMessaging)]
    [CCOLCodePieceGenerator]
    public class RISPlugin : CCOLCodePieceGeneratorBase, ITLCGenTabItem, ITLCGenXMLNodeWriter, ITLCGenPlugMessaging
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

        #region CCOLCodePieceGenerator

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            if (_RISModel.RISToepassen)
            {
                foreach (var fc in _RISModel.RISFasen)
                {
                    if (fc.RISAanvraag)
                    {
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_prmrisastart}{fc.FaseCyclus}",
                            fc.AanvraagStart,
                            CCOLElementTimeTypeEnum.None,
                            _prmrisastart, fc.FaseCyclus);
                    }
                }
                foreach (var fc in _RISModel.RISFasen)
                {
                    if (fc.RISAanvraag)
                    {
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_prmrisaend}{fc.FaseCyclus}",
                            fc.AanvraagEnd,
                            CCOLElementTimeTypeEnum.None,
                            _prmrisaend, fc.FaseCyclus);
                    }
                }
                foreach (var fc in _RISModel.RISFasen)
                {
                    if (fc.RISVerlengen)
                    {
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_prmrisvstart}{fc.FaseCyclus}",
                            fc.VerlengenStart,
                            CCOLElementTimeTypeEnum.None,
                            _prmrisvstart, fc.FaseCyclus);
                    }
                }
                foreach (var fc in _RISModel.RISFasen)
                {
                    if (fc.RISVerlengen)
                    {
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_prmrisvend}{fc.FaseCyclus}",
                            fc.VerlengenEnd,
                            CCOLElementTimeTypeEnum.None,
                            _prmrisvend, fc.FaseCyclus);
                    }
                }
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
                default:
                    return 0;
            }
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {

            StringBuilder sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCIncludes:
                    sb.AppendLine($"{ts}#ifndef NO_RIS");
                    sb.AppendLine($"{ts}{ts}#include \"risvar.c\" /* ccol ris controller */");
                    sb.AppendLine($"{ts}{ts}#include \"risappl.c\" /* RIS applicatiefuncties */");
                    sb.AppendLine($"{ts}{ts}#ifndef AUTOMAAT");
                    sb.AppendLine($"{ts}{ts}{ts}#include \"ris_simvar.h\" /* ccol ris simulatie functie */");
                    sb.AppendLine($"{ts}{ts}#endif");
                    sb.AppendLine($"{ts}#endif");
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCInitApplication:
                    sb.AppendLine($"{ts}#ifndef NO_RIS");
                    sb.AppendLine($"{ts}{ts}#ifndef AUTOMAAT");
                    sb.AppendLine($"{ts}{ts}{ts}/* zet display van RIS-berichten aan in de testomgeving */");
                    sb.AppendLine($"{ts}{ts}{ts}/* ---------------------------------------------------- */");
                    sb.AppendLine($"{ts}{ts}{ts}RIS_DIPRM[RIS_DIPRM_ALL] = 1;");
                    sb.AppendLine($"{ts}{ts}#endif");
                    sb.AppendLine($"{ts}#endif");
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCAanvragen:
                    sb.AppendLine($"{ts}#ifndef NO_RIS");
                    foreach(var fc in _RISModel.RISFasen.Where(x => x.RISAanvraag))
                    {
                        var vtype = "RIS_VEHICLES";
                        var rfc = c.Fasen.First(x => x.Naam == fc.FaseCyclus);
                        if(rfc != null)
                        {
                            switch (rfc.Type)
                            {
                                case TLCGen.Models.Enumerations.FaseTypeEnum.Auto:
                                    break;
                                case TLCGen.Models.Enumerations.FaseTypeEnum.Fiets:
                                    vtype = "RIS_CYCLIST";
                                    break;
                                case TLCGen.Models.Enumerations.FaseTypeEnum.Voetganger:
                                    vtype = "RIS_PEDESTRIAN";
                                    break;
                                case TLCGen.Models.Enumerations.FaseTypeEnum.OV:
                                    break;
                            }
                        }
                        sb.AppendLine($"{ts}{ts}ris_aanvraag({_fcpf}{fc.FaseCyclus}, {vtype}, {_prmpf}{_prmrisastart}{fc.AanvraagStart}, {_prmpf}{_prmrisaend}{fc.AanvraagEnd}, BIT8);");
                    }
                    sb.AppendLine($"{ts}#endif");
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCMeetkriterium:
                    sb.AppendLine($"{ts}#ifndef NO_RIS");
                    foreach (var fc in _RISModel.RISFasen.Where(x => x.RISVerlengen))
                    {
                        var vtype = "RIS_VEHICLES";
                        var rfc = c.Fasen.First(x => x.Naam == fc.FaseCyclus);
                        if (rfc != null)
                        {
                            switch (rfc.Type)
                            {
                                case TLCGen.Models.Enumerations.FaseTypeEnum.Auto:
                                    break;
                                case TLCGen.Models.Enumerations.FaseTypeEnum.Fiets:
                                    vtype = "RIS_CYCLIST";
                                    break;
                                case TLCGen.Models.Enumerations.FaseTypeEnum.Voetganger:
                                    vtype = "RIS_PEDESTRIAN";
                                    break;
                                case TLCGen.Models.Enumerations.FaseTypeEnum.OV:
                                    break;
                            }
                        }
                        sb.AppendLine($"{ts}{ts}ris_verlengen({_fcpf}{fc.FaseCyclus}, {vtype}, {_prmpf}{_prmrisvstart}{fc.VerlengenStart}, {_prmpf}{_prmrisvend}{fc.VerlengenEnd}, BIT8);");
                    }
                    sb.AppendLine($"{ts}#endif");
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCPostSystemApplication:
                    sb.AppendLine($"{ts}#ifndef NO_RIS");
                    sb.AppendLine($"{ts}{ts}#ifndef AUTOMAAT");
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
                            risfc.SimulatieVM.Lanes.Add(new RISFaseCyclusLaneSimulatieViewModel(new RISFaseCyclusLaneSimulatieModel()));
                        }
                        _RISVM.RISFasen.Add(risfc);
                    }
                    else
                    {
                        var risfc = _RISVM.RISFasen.FirstOrDefault(x => x.FaseCyclus == fc.Naam);
                        if (risfc != null)
                        {
                            if (fc.AantalRijstroken > risfc.SimulatieVM.Lanes.Count)
                            {
                                var i = fc.AantalRijstroken - risfc.SimulatieVM.Lanes.Count;
                                for (int j = 0; j < i; j++)
                                {
                                    risfc.SimulatieVM.Lanes.Add(new RISFaseCyclusLaneSimulatieViewModel(new RISFaseCyclusLaneSimulatieModel()));
                                }
                            }
                            else if (fc.AantalRijstroken < risfc.SimulatieVM.Lanes.Count)
                            {
                                var i = risfc.SimulatieVM.Lanes.Count - fc.AantalRijstroken;
                                for (int j = 0; j < i; j++)
                                {
                                    if (risfc.SimulatieVM.Lanes.Any())
                                        risfc.SimulatieVM.Lanes.Remove(risfc.SimulatieVM.Lanes.Last());
                                }
                            }
                        }
                    }
                }
                var rems = new List<RISFaseCyclusDataViewModel>();
                foreach (var fc in _RISVM.RISFasen)
                {
                    if (Controller.Fasen.All(x => x.Naam != fc.FaseCyclus))
                    {
                        rems.Add(fc);
                    }
                }
                foreach (var sg in rems)
                {
                    _RISVM.RISFasen.Remove(sg);
                }
                _RISVM.RISFasen.BubbleSort();
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
