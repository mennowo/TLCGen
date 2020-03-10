using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using TLCGen.Generators.CCOL.CodeGeneration;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Plugins.RangeerElementen.Models;
using TLCGen.Plugins.RangeerElementen.ViewModels;

namespace TLCGen.Plugins.RangeerElementen
{
    [TLCGenTabItem(-1, TabItemTypeEnum.DetectieTab)]
    [TLCGenPlugin(
        TLCGenPluginElems.TabControl |
        TLCGenPluginElems.XMLNodeWriter |
        TLCGenPluginElems.PlugMessaging)]
    [CCOLCodePieceGenerator]
    public partial class RangeerElementenPlugin : CCOLCodePieceGeneratorBase, ITLCGenTabItem, ITLCGenXMLNodeWriter, ITLCGenPlugMessaging
    {
        #region Fields

        private RangeerElementenTabViewModel _RangeerElementenVM;
        private RangeerElementenDataModel _RangeerElementenModel;

#pragma warning disable 0649
        // private CCOLGeneratorCodeStringSettingModel _prm...;
#pragma warning restore 0649

        #endregion // Fields

        #region Properties

        public string Dpf => _dpf;
        public string Ts { get; set; }

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
                    _RangeerElementenModel = new RangeerElementenDataModel();
                    _RangeerElementenVM.RangeerElementenModel = _RangeerElementenModel;
                }
                UpdateModel();
            }
        }

        public string GetPluginName()
        {
            return "RangeerElementen";
        }

        #endregion // TLCGen plugin shared

        #region ITLCGenTabItem

        public string DisplayName => "Rangeer elementen";
        public ImageSource Icon => null;

        DataTemplate _ContentDataTemplate;
        public DataTemplate ContentDataTemplate
        {
            get
            {
                if (_ContentDataTemplate == null)
                {
                    _ContentDataTemplate = new DataTemplate();
                    var tab = new FrameworkElementFactory(typeof(RangeerElementenTabView));
                    tab.SetValue(RangeerElementenTabView.DataContextProperty, _RangeerElementenVM);
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
            _RangeerElementenModel = null;

            foreach (XmlNode node in document.FirstChild.ChildNodes)
            {
                if (node.LocalName == "RangeerTLCGenElementenData")
                {
                    _RangeerElementenModel = XmlNodeConverter.ConvertNode<RangeerElementenDataModel>(node);
                    break;
                }
            }

            if (_RangeerElementenModel == null)
            {
                _RangeerElementenModel = new RangeerElementenDataModel();
            }
            _RangeerElementenVM.RangeerElementenModel = _RangeerElementenModel;
            _RangeerElementenVM.RaisePropertyChanged("");
        }

        public void SetXmlInDocument(XmlDocument document)
        {
            var doc = TLCGenSerialization.SerializeToXmlDocument(_RangeerElementenModel);
            var node = document.ImportNode(doc.DocumentElement, true);
            document.DocumentElement.AppendChild(node);
        }

        #endregion // ITLCGenXMLNodeWriter

        #region ITLCGenPlugMessaging

        public void UpdateTLCGenMessaging()
        {
            _RangeerElementenVM.UpdateMessaging();
        }

        #endregion // ITLCGenPlugMessaging

        #region ITLCGenElementProvider

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            return type == CCOLCodeTypeEnum.RegCTop ? 10000 : 0;
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            Ts = ts;
            return null;
        }

        #endregion // ITLCGenElementProvider

        #region Private Methods

        internal void UpdateModel()
        {
            if (_controller != null && _RangeerElementenModel != null && _RangeerElementenModel.RangeerElementenToepassen)
            {
                foreach (var d in Controller.GetAllDetectors())
                {
                    if (_RangeerElementenVM.RangeerElementen.All(x => x.Element != d.Naam))
                    {
                        var dvm = new RangeerElementViewModel(new RangeerElementModel { Element = d.Naam });
                        _RangeerElementenVM.RangeerElementen.Add(dvm);
                    }
                }
                var rems = new List<RangeerElementViewModel>();
                foreach (var d in _RangeerElementenVM.RangeerElementen)
                {
                    if (Controller.GetAllDetectors().All(x => x.Naam != d.Element))
                    {
                        rems.Add(d);
                    }
                }
                foreach (var sg in rems)
                {
                    _RangeerElementenVM.RangeerElementen.Remove(sg);
                }
                _RangeerElementenVM.RaisePropertyChanged("");
            }
        }

        #endregion // Private Methods

        #region Constructor

        public RangeerElementenPlugin()
        {
            IsEnabled = true;
            _RangeerElementenVM = new RangeerElementenTabViewModel(this);
        }

        #endregion // Constructor
    }
}
