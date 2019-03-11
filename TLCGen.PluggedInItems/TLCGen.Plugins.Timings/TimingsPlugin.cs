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
using TLCGen.Models.Enumerations;
using TLCGen.Plugins.Timings.Models;

namespace TLCGen.Plugins.Timings
{
    [TLCGenTabItem(-1, TabItemTypeEnum.FasenTab)]
    [TLCGenPlugin(
        TLCGenPluginElems.TabControl | 
        TLCGenPluginElems.XMLNodeWriter |
        TLCGenPluginElems.PlugMessaging | 
        TLCGenPluginElems.IOElementProvider)]
    [CCOLCodePieceGenerator]
    public partial class TimingsPlugin : CCOLCodePieceGeneratorBase, ITLCGenTabItem, ITLCGenXMLNodeWriter, ITLCGenPlugMessaging, ITLCGenElementProvider
    {
        #region Fields

        private TimingsTabViewModel _timingsVM;
        private TimingsDataModel _timingsModel;

#pragma warning disable 0649
        // private CCOLGeneratorCodeStringSettingModel _prm...;
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
                    _timingsModel = new TimingsDataModel();
                    _timingsVM.TimingsModel = _timingsModel;
                }
                UpdateModel();
            }
        }

        public string GetPluginName()
        {
            return "Timings";
        }

        #endregion // TLCGen plugin shared

        #region ITLCGenTabItem

        public string DisplayName => "Timings";
        public ImageSource Icon => null;

        DataTemplate _ContentDataTemplate;
        public DataTemplate ContentDataTemplate
        {
            get
            {
                if (_ContentDataTemplate == null)
                {
                    _ContentDataTemplate = new DataTemplate();
                    var tab = new FrameworkElementFactory(typeof(TimingsTabView));
                    tab.SetValue(TimingsTabView.DataContextProperty, _timingsVM);
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
            _timingsModel = null;

            foreach (XmlNode node in document.FirstChild.ChildNodes)
            {
                if (node.LocalName == "TimingsData")
                {
                    _timingsModel = XmlNodeConverter.ConvertNode<TimingsDataModel>(node);
                    break;
                }
            }

            if (_timingsModel == null)
            {
                _timingsModel = new TimingsDataModel();
            }
            _timingsVM.TimingsModel = _timingsModel;
            _timingsVM.RaisePropertyChanged("");
        }

        public void SetXmlInDocument(XmlDocument document)
        {
            XmlDocument doc = TLCGenSerialization.SerializeToXmlDocument(_timingsModel);
            XmlNode node = document.ImportNode(doc.DocumentElement, true);
            document.DocumentElement.AppendChild(node);
        }

        #endregion // ITLCGenXMLNodeWriter

        #region ITLCGenPlugMessaging

        public void UpdateTLCGenMessaging()
        {
            _timingsVM.UpdateMessaging();
        }

        #endregion // ITLCGenPlugMessaging

        #region ITLCGenElementProvider

        public List<IOElementModel> GetOutputItems()
        {
            return new List<IOElementModel>();
        }

        public List<IOElementModel> GetInputItems()
        {
            return new List<IOElementModel>();
        }

        public bool IsElementNameUnique(string name, TLCGenObjectTypeEnum type)
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
        }

        public override bool HasCCOLElements()
        {
            return false;
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
                    return 120;
                case CCOLCodeTypeEnum.RegCSystemApplication2:
                    return 120;
                case CCOLCodeTypeEnum.TabCControlIncludes:
                case CCOLCodeTypeEnum.TabCControlParameters:
                    return 120;
                default:
                    return 0;
            }
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            if (_controller.Data.CCOLVersie <= TLCGen.Models.Enumerations.CCOLVersieEnum.CCOL8 || !_timingsModel.TimingsToepassen) return null;

            StringBuilder sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCIncludes:

                    // Generate rissim.c now
                    GenerateFcTimingsC(c, _timingsModel, ts);

                    sb.AppendLine($"{ts}#include \"timingsvar.c\" /* FCTiming functies */");
                    sb.AppendLine($"{ts}#include \"timingsfunc.c\" /* FCTiming functies */");
                    sb.AppendLine($"{ts}#include \"{c.Data.Naam}fctimings.c\" /* FCTiming functies */");
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCSystemApplication2:
                    sb.AppendLine($"{ts}msg_fctiming();");
                    return sb.ToString();
                case CCOLCodeTypeEnum.TabCControlIncludes:
                    sb.AppendLine($"{ts}void Timings_Eventstate_Definition(void);");
                    return sb.ToString();
                case CCOLCodeTypeEnum.TabCControlParameters:
                    sb.AppendLine($"{ts}Timings_Eventstate_Definition();");
                    return sb.ToString();
                default:
                    return null;
            }
        }

        public override List<string> GetSourcesToCopy()
        {
            if (_controller.Data.CCOLVersie <= TLCGen.Models.Enumerations.CCOLVersieEnum.CCOL8 || !_timingsModel.TimingsToepassen) return null;
            return new List<string>
            {
                "timingsfunc.c",
                "timingsvar.c"
            };
        }

        #endregion // CCOLCodePieceGenerator

        #region Private Methods


        internal void UpdateModel()
        {
            if (_controller != null && _timingsModel != null)
            {
                foreach (var fc in Controller.Fasen)
                {
                    if (_timingsVM.TimingsFasen.All(x => x.FaseCyclus != fc.Naam))
                    {
                        var risfc = new TimingsFaseCyclusDataViewModel(new TimingsFaseCyclusDataModel { FaseCyclus = fc.Naam });
                        _timingsVM.TimingsFasen.Add(risfc);
                    }
                }
                var rems = new List<TimingsFaseCyclusDataViewModel>();
                foreach (var fc in _timingsVM.TimingsFasen)
                {
                    if (Controller.Fasen.All(x => x.Naam != fc.FaseCyclus))
                    {
                        rems.Add(fc);
                    }
                }
                foreach (var sg in rems)
                {
                    _timingsVM.TimingsFasen.Remove(sg);
                }
                _timingsVM.TimingsFasen.BubbleSort();
                _timingsVM.RaisePropertyChanged("");
            }
        }

        #endregion // Private Methods

        #region Constructor

        public TimingsPlugin()
        {
            IsEnabled = true;
            _timingsVM = new TimingsTabViewModel(this);
        }

        #endregion // Constructor
    }
}
