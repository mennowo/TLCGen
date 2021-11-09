using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using TLCGen.Extensions;
using TLCGen.Generators.CCOL.CodeGeneration;
using TLCGen.Generators.CCOL.CodeGeneration.HelperClasses;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Plugins.Timings.CodeGeneration;
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
        private TimingsCodeGenerator _codeGenerator = new TimingsCodeGenerator();
        
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
                    tab.SetValue(FrameworkElement.DataContextProperty, _timingsVM);
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
            var doc = TLCGenSerialization.SerializeToXmlDocument(_timingsModel);
            var node = document.ImportNode(doc.DocumentElement, true);
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

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>(0);

            if (_controller.Data.CCOLVersie <= CCOLVersieEnum.CCOL8 || !_timingsModel.TimingsToepassen) return;

            _myElements.AddRange(_codeGenerator.GetCCOLElements(c));
        }

        public override bool HasCCOLElements() => true;

        public override IEnumerable<CCOLElement> GetCCOLElements(CCOLElementTypeEnum type) => _myElements.Where(x => x.Type == type);

        public override IEnumerable<CCOLLocalVariable> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCRealisatieAfhandeling:
                    if (!_timingsModel.TimingsUsePredictions)
                        return base.GetFunctionLocalVariables(c, type);
                    return new List<CCOLLocalVariable> { new CCOLLocalVariable("int", "fc") };
                case CCOLCodeTypeEnum.RegCSystemApplication2:
                    if (!_timingsModel.TimingsToepassen)
                        return base.GetFunctionLocalVariables(c, type);
                    return new List<CCOLLocalVariable> { new CCOLLocalVariable("int", "i", defineCondition: "(!defined NO_TIMETOX)") };
                default:
                    return base.GetFunctionLocalVariables(c, type);
            }
        }
        
        public override int[] HasCode(CCOLCodeTypeEnum type)
        {
            return _codeGenerator.HasCode(type);
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts, int order)
        {
            _codeGenerator._fcpf = _fcpf;
            _codeGenerator._schpf = _schpf;
            _codeGenerator._prmpf = _prmpf;
            _codeGenerator._mpf = _mpf;
            _codeGenerator._ctpf = _ctpf;
            _codeGenerator._tpf = _tpf;
            return _codeGenerator.GetCode(_timingsModel, c, type, ts);
        }

        public override List<string> GetSourcesToCopy()
        {
            if (_controller.Data.CCOLVersie <= CCOLVersieEnum.CCOL8 || !_timingsModel.TimingsToepassen) return null;
            var files = new List<string>
            {
                "timingsfunc.c",
                "timingsvar.c"
            };
            if (_timingsModel.TimingsUsePredictions) files.Add("timings_uc4.c");
            return files;
        }
        
        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _codeGenerator._mrealtijd = CCOLGeneratorSettingsProvider.Default.GetElementName("mrealtijd");
            _codeGenerator._mrealtijdmin = CCOLGeneratorSettingsProvider.Default.GetElementName("mrealtijdmin");
            _codeGenerator._mrealtijdmax = CCOLGeneratorSettingsProvider.Default.GetElementName("mrealtijdmax");
            _codeGenerator._cvc = CCOLGeneratorSettingsProvider.Default.GetElementName("cvc");
            _codeGenerator._cvchd = CCOLGeneratorSettingsProvider.Default.GetElementName("cvchd");
            _codeGenerator._schgs = CCOLGeneratorSettingsProvider.Default.GetElementName("schgs");
            _codeGenerator._tfo = CCOLGeneratorSettingsProvider.Default.GetElementName("tfo");

            return base.SetSettings(settings);
        }
        
        #endregion // CCOLCodePieceGenerator

        #region Private Methods
        
        internal void UpdateModel()
        {
            if (_controller != null && _timingsModel != null)
            {
                if (_controller.Data.CCOLVersie < CCOLVersieEnum.CCOL110)
                {
                    _timingsModel.TimingsUsePredictions = false;
                }
                
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
            ElementGenerationOrder = int.MaxValue;
        }

        #endregion // Constructor
    }
}
