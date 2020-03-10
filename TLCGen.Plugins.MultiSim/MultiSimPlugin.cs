using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using TLCGen.Generators.CCOL.CodeGeneration;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.Plugins.MultiSim
{
    [TLCGenTabItem(41, TabItemTypeEnum.DetectieTab)]
    [TLCGenPlugin(
        TLCGenPluginElems.TabControl | 
        TLCGenPluginElems.XMLNodeWriter |
        TLCGenPluginElems.PlugMessaging)]
    public class MultiSimPlugin : ITLCGenTabItem, ITLCGenXMLNodeWriter, ITLCGenPlugMessaging
    {
        #region Fields

        private MultiSimTabViewModel _multiSimVM;
        private MultiSimDataModel _model;

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
                    _model = new MultiSimDataModel();
                    _multiSimVM.MultiSimDataModel = _model;
                }
            }
        }

        public string GetPluginName()
        {
            return "MultiSim";
        }

        #endregion // TLCGen plugin shared

        #region ITLCGenTabItem

        public string DisplayName => "Simulatie sets";
        public ImageSource Icon => null;

        DataTemplate _ContentDataTemplate;
        public DataTemplate ContentDataTemplate
        {
            get
            {
                if (_ContentDataTemplate == null)
                {
                    _ContentDataTemplate = new DataTemplate();
                    var tab = new FrameworkElementFactory(typeof(MultiSimTabView));
                    tab.SetValue(MultiSimTabView.DataContextProperty, _multiSimVM);
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
            _model = null;

            foreach (XmlNode node in document.FirstChild.ChildNodes)
            {
                if (node.LocalName == "MultiSimData")
                {
                    _model = XmlNodeConverter.ConvertNode<MultiSimDataModel>(node);
                    break;
                }
            }

            if (_model == null)
            {
                _model = new MultiSimDataModel();
            }
            _multiSimVM.MultiSimDataModel = _model;
            _multiSimVM.RaisePropertyChanged("");
        }

        public void SetXmlInDocument(XmlDocument document)
        {
            var doc = TLCGenSerialization.SerializeToXmlDocument(_model);
            var node = document.ImportNode(doc.DocumentElement, true);
            document.DocumentElement.AppendChild(node);
        }

        #endregion // ITLCGenXMLNodeWriter

        #region ITLCGenPlugMessaging

        public void UpdateTLCGenMessaging()
        {
            _multiSimVM.UpdateMessaging();
        }

        #endregion // ITLCGenPlugMessaging

        #region Constructor

        public MultiSimPlugin()
        {
            IsEnabled = true;
            _multiSimVM = new MultiSimTabViewModel(this);
        }
        
        #endregion // Constructor
    }
}
