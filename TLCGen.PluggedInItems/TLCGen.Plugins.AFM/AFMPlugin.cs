using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Plugins.AFM.Models;

namespace TLCGen.Plugins.AFM
{
    [TLCGenTabItem(-1, TabItemTypeEnum.FasenTab)]
    [TLCGenPlugin(TLCGenPluginElems.TabControl | TLCGenPluginElems.XMLNodeWriter)]
    public class AFMPlugin : ITLCGenTabItem, ITLCGenXMLNodeWriter
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

        #region Constructor

        public AFMPlugin()
        {
            IsEnabled = true;
            _afmVM = new AFMTabViewModel(this);
        }
        
        #endregion // Constructor
    }
}
