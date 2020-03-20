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

        private readonly RangeerElementenTabViewModel _rangeerElementenVm;
        private RangeerElementenDataModel _rangeerElementenModel;

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
                    _rangeerElementenModel = new RangeerElementenDataModel();
                    _rangeerElementenVm.RangeerElementenModel = _rangeerElementenModel;
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
                    tab.SetValue(RangeerElementenTabView.DataContextProperty, _rangeerElementenVm);
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
            _rangeerElementenModel = null;

            foreach (XmlNode node in document.FirstChild.ChildNodes)
            {
                if (node.LocalName == "RangeerTLCGenElementenData")
                {
                    _rangeerElementenModel = XmlNodeConverter.ConvertNode<RangeerElementenDataModel>(node);
                    break;
                }
            }

            if (_rangeerElementenModel == null)
            {
                _rangeerElementenModel = new RangeerElementenDataModel();
            }
            _rangeerElementenVm.RangeerElementenModel = _rangeerElementenModel;
            _rangeerElementenVm.RaisePropertyChanged("");
        }

        public void SetXmlInDocument(XmlDocument document)
        {
            XmlDocument doc = TLCGenSerialization.SerializeToXmlDocument(_rangeerElementenModel);
            XmlNode node = document.ImportNode(doc.DocumentElement, true);
            document.DocumentElement.AppendChild(node);
        }

        #endregion // ITLCGenXMLNodeWriter

        #region ITLCGenPlugMessaging

        public void UpdateTLCGenMessaging()
        {
            _rangeerElementenVm.UpdateMessaging();
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
            if (_controller != null && _rangeerElementenModel != null && _rangeerElementenModel.RangeerElementenToepassen)
            {
                foreach (var d in Controller.GetAllDetectors())
                {
                    if (_rangeerElementenVm.RangeerElementen.All(x => x.Element != d.Naam))
                    {
                        var dvm = new RangeerElementViewModel(new RangeerElementModel { Element = d.Naam });
                        _rangeerElementenVm.RangeerElementen.Add(dvm);
                    }
                }
                var rems = new List<RangeerElementViewModel>();
                foreach (var d in _rangeerElementenVm.RangeerElementen)
                {
                    if (Controller.GetAllDetectors().All(x => x.Naam != d.Element))
                    {
                        rems.Add(d);
                    }
                }
                foreach (var sg in rems)
                {
                    _rangeerElementenVm.RangeerElementen.Remove(sg);
                }
                _rangeerElementenVm.RaisePropertyChanged("");
            }
        }

        #endregion // Private Methods

        #region Constructor

        public RangeerElementenPlugin()
        {
            IsEnabled = true;
            _rangeerElementenVm = new RangeerElementenTabViewModel(this);
        }

        #endregion // Constructor
    }
}
