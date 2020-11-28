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
    [TLCGenTabItem(-1, TabItemTypeEnum.SpecialsTab)]
    [TLCGenPlugin(
        TLCGenPluginElems.TabControl |
        TLCGenPluginElems.XMLNodeWriter |
        TLCGenPluginElems.PlugMessaging)]
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
                if (_ContentDataTemplate != null) return _ContentDataTemplate;

                _ContentDataTemplate = new DataTemplate();
                var tab = new FrameworkElementFactory(typeof(RangeerElementenTabView));
                tab.SetValue(FrameworkElement.DataContextProperty, _rangeerElementenVm);
                _ContentDataTemplate.VisualTree = tab;
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
            var ioCollector = new IOCollector();
            //ioCollector.CollectItems(_controller, );
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
            var doc = TLCGenSerialization.SerializeToXmlDocument(_rangeerElementenModel);
            var node = document.ImportNode(doc.DocumentElement, true);
            document.DocumentElement.AppendChild(node);
        }

        #endregion // ITLCGenXMLNodeWriter

        #region ITLCGenPlugMessaging

        public void UpdateTLCGenMessaging()
        {
            _rangeerElementenVm.UpdateMessaging();
        }

        #endregion // ITLCGenPlugMessaging

        #region Private Methods

        internal void UpdateModel()
        {
            if (_controller != null && _rangeerElementenModel != null)
            {
                // detectie
                if (_rangeerElementenModel.RangeerElementenToepassen)
                {
                    foreach (var d in Controller.GetAllDetectors())
                    {
                        if (_rangeerElementenVm.RangeerDetectors.All(x => x.Element != d.Naam))
                        {
                            var dvm = new RangeerElementViewModel(new RangeerElementModel {Element = d.Naam});
                            _rangeerElementenVm.RangeerDetectors.Add(dvm);
                        }
                    }

                    var rems = new List<RangeerElementViewModel>();
                    foreach (var d in _rangeerElementenVm.RangeerDetectors)
                    {
                        if (Controller.GetAllDetectors().All(x => x.Naam != d.Element))
                        {
                            rems.Add(d);
                        }
                    }

                    foreach (var sg in rems)
                    {
                        _rangeerElementenVm.RangeerDetectors.Remove(sg);
                    }
                }

                // signaalgroepen
                if (_rangeerElementenModel.RangeerSignaalGroepenToepassen)
                {
                    foreach (var fc in Controller.Fasen)
                    {
                        if (_rangeerElementenVm.RangeerSignalGroups.All(x => x.SignalGroup != fc.Naam))
                        {
                            var dvm = new RangeerSignalGroupViewModel(new RangeerSignaalGroepModel
                                {SignaalGroep = fc.Naam});
                            _rangeerElementenVm.RangeerSignalGroups.Add(dvm);
                        }
                    }

                    var remsSg = new List<RangeerSignalGroupViewModel>();
                    foreach (var sg in _rangeerElementenVm.RangeerSignalGroups)
                    {
                        if (Controller.Fasen.All(x => x.Naam != sg.SignalGroup))
                        {
                            remsSg.Add(sg);
                        }
                    }

                    foreach (var sg in remsSg)
                    {
                        _rangeerElementenVm.RangeerSignalGroups.Remove(sg);
                    }
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
