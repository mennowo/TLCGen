using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Generators.CCOL.CodeGeneration;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.Specificator
{
    [CCOLCodePieceGenerator]
    [TLCGenTabItem(-1, TabItemTypeEnum.AlgemeenTab)]
    [TLCGenPlugin(
        TLCGenPluginElems.Generator |
        TLCGenPluginElems.PlugMessaging |
        TLCGenPluginElems.TabControl |
        TLCGenPluginElems.XMLNodeWriter)]
    public class SpecificatorPlugin : CCOLCodePieceGeneratorBase, ITLCGenGenerator, ITLCGenPlugMessaging, ITLCGenXMLNodeWriter, ITLCGenTabItem
    {

        #region Fields

        #endregion // Fields

        #region Properties

        public string ControllerFileName { get; set; }

        public SpecificatorDataModel Data => _data ?? (_data = new SpecificatorDataModel());

        public SpecificatorViewModel SpecificatorVM { get; }
        public SpecificatorTabViewModel SpecificatorTabVM { get; }

        #endregion // Properties

        #region ITLCGenGenerator

        public UserControl GeneratorView { get; }

        public List<IOElementModel> GetAllIOElements(ControllerModel c)
        {
            return null;
        }

        public string GetGeneratorName()
        {
            return "Specificator";
        }

        public string GetGeneratorVersion()
        {
            return "0.1";
        }

        public string GetPluginName()
        {
            return GetGeneratorName();
        }

        public ControllerModel Controller
        {
            get => _controller;
            set
            {
                if (value == null)
                {
                    _data = null;
                }
                else
                {
                    if(_data == null)
                    {
                        _data = new SpecificatorDataModel();
                    }
                    SpecificatorVM.Data = _data;
                    SpecificatorTabVM.Data = _data;
                    SpecificatorTabVM.SpecialsParagrafen = new ObservableCollectionAroundList<SpecificatorSpecialsParagraafViewModel, SpecificatorSpecialsParagraaf>(_data.SpecialsParagrafen);
                    SpecificatorTabVM.SelectedSpecialsParagraaf = SpecificatorTabVM.SpecialsParagrafen.FirstOrDefault();
                }
                _controller = value;
                SpecificatorVM.UpdateCommands();
            }
        }

        public void GenerateController()
        {
            if (SpecificatorVM.GenerateCommand.CanExecute(null))
            {
                SpecificatorVM.GenerateCommand.Execute(null);
            }
        }

        public bool CanGenerateController()
        {
            return SpecificatorVM.GenerateCommand.CanExecute(null);
        }

        #endregion // ITLCGenGenerator

        #region ITLCGenTabItem

        DataTemplate _ContentDataTemplate;
        public DataTemplate ContentDataTemplate
        {
            get
            {
                if (_ContentDataTemplate == null)
                {
                    _ContentDataTemplate = new DataTemplate();
                    var tab = new FrameworkElementFactory(typeof(SpecificatorTabView));
                    tab.SetValue(SpecificatorTabView.DataContextProperty, SpecificatorTabVM);
                    _ContentDataTemplate.VisualTree = tab;
                }
                return _ContentDataTemplate;
            }
        }

        public string DisplayName => "Specificator";

        private bool _IsEnabled;
        private ControllerModel _controller;
        private SpecificatorDataModel _data;

        public bool IsEnabled
        {
            get => _IsEnabled;
            set => _IsEnabled = value;
        }

        public bool Visibility { get; set; } = true;

        //ResourceDictionary dict = new ResourceDictionary();
        //Uri u = new Uri("pack://application:,,,/" +
        //    System.Reflection.Assembly.GetExecutingAssembly().GetName().Name +
        //    ";component/" + "GebruikersOptiesIcon.xaml");
        //dict.Source = u;
        //return (DrawingImage)dict["GebruikersOptiesTabDrawingImage"];
        public ImageSource Icon => null;

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

        #region ITLCGenXMLNodeWriter

        public void GetXmlFromDocument(XmlDocument document)
        {
            var found = false;
            foreach (XmlNode node in document.FirstChild.ChildNodes)
            {
                if (node.LocalName == "Specificator")
                {
                    _data = XmlNodeConverter.ConvertNode<SpecificatorDataModel>(node);
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                _data = new SpecificatorDataModel();
            }
            SpecificatorVM.Data = Data;
        }

        public void SetXmlInDocument(XmlDocument document)
        {
            var doc = TLCGenSerialization.SerializeToXmlDocument(Data);
            var node = document.ImportNode(doc.DocumentElement, true);
            document.DocumentElement.AppendChild(node);
        }

        #endregion // ITLCGenXMLNodeWriter

        #region ITLCGenPlugMessaging

        public void UpdateTLCGenMessaging()
        {
            WeakReferenceMessengerEx.Default.Register<ControllerFileNameChangedMessage>(this, OnControllerFileNameChanged);
            WeakReferenceMessengerEx.Default.Register<ControllerDataChangedMessage>(this, OnControllerDataChanged);
        }

        #endregion // ITLCGenPlugMessaging

        #region TLCGen Events

        private void OnControllerDataChanged(object recipient, ControllerDataChangedMessage message)
        {
            SpecificatorVM?.UpdateCommands();
        }

        private void OnControllerFileNameChanged(object sender, ControllerFileNameChangedMessage msg)
        {
            if (msg.NewFileName == null) return;

            ControllerFileName = msg.NewFileName;
        }

        #endregion // TLCGen Events

        public void UpdateCCOLGenData()
        {
            CCOLGenHelper.Dpf = _dpf;
        }

        #region Constructor

        public SpecificatorPlugin() : this(false)
        {

        }

        public SpecificatorPlugin(bool tester = false)
        {
            SpecificatorVM = new SpecificatorViewModel(this);
            SpecificatorTabVM = new SpecificatorTabViewModel();
            if (!tester)
            {
                GeneratorView = new SpecificatorView
                {
                    DataContext = SpecificatorVM
                };
            }
        }

        #endregion // Constructor
    }
}
