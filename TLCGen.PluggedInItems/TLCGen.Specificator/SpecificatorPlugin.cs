using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.Specificator
{
    [TLCGenTabItem(-1, TabItemTypeEnum.AlgemeenTab)]
    [TLCGenPlugin(
        TLCGenPluginElems.Generator |
        TLCGenPluginElems.PlugMessaging |
        TLCGenPluginElems.TabControl |
        TLCGenPluginElems.XMLNodeWriter)]
    public class SpecificatorPlugin : ViewModelBase, ITLCGenGenerator, ITLCGenPlugMessaging, ITLCGenXMLNodeWriter, ITLCGenTabItem
    {
        #region Fields

        private readonly UserControl _generatorView;
        private readonly SpecificatorViewModel _myVm;

        #endregion // Fields

        #region Properties

        public string ControllerFileName { get; set; }

        public SpecificatorDataModel Data
        {
            get => _data ?? (_data = new SpecificatorDataModel());
        }

        public SpecificatorViewModel SpecificatorVM => _myVm;

        #endregion // Properties

        #region ITLCGenGenerator

        public UserControl GeneratorView => _generatorView;

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
                    _data = _myVm.Data = null;
                }
                _controller = value;
                RaisePropertyChanged("");
            }
        }

        public void GenerateController()
        {
            if (_myVm.GenerateCommand.CanExecute(null))
            {
                _myVm.GenerateCommand.Execute(null);
            }
        }

        public bool CanGenerateController()
        {
            return false;
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
                    _ContentDataTemplate.VisualTree = new FrameworkElementFactory(typeof(SpecificatorTabView));
                }
                return _ContentDataTemplate;
            }
        }

        public string DisplayName
        {
            get
            {
                return "Specificator";
            }
        }

        private bool _IsEnabled;
        private ControllerModel _controller;
        private SpecificatorDataModel _data;

        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set
            {
                _IsEnabled = value;
                RaisePropertyChanged("IsEnabled");
            }
        }

        public ImageSource Icon
        {
            get
            {
                //ResourceDictionary dict = new ResourceDictionary();
                //Uri u = new Uri("pack://application:,,,/" +
                //    System.Reflection.Assembly.GetExecutingAssembly().GetName().Name +
                //    ";component/" + "GebruikersOptiesIcon.xaml");
                //dict.Source = u;
                //return (DrawingImage)dict["GebruikersOptiesTabDrawingImage"];
                return null;
            }
        }

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
            bool found = false;
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
            _myVm.Data = Data;
            RaisePropertyChanged("");
        }

        public void SetXmlInDocument(XmlDocument document)
        {
            XmlDocument doc = TLCGenSerialization.SerializeToXmlDocument(Data);
            XmlNode node = document.ImportNode(doc.DocumentElement, true);
            document.DocumentElement.AppendChild(node);
        }

        #endregion // ITLCGenXMLNodeWriter


        #region ITLCGenPlugMessaging

        public void UpdateTLCGenMessaging()
        {
            Messenger.Default.Register(this, new Action<Messaging.Messages.ControllerFileNameChangedMessage>(OnControllerFileNameChanged));
        }

        #endregion // ITLCGenPlugMessaging

        #region TLCGen Events

        private void OnControllerFileNameChanged(Messaging.Messages.ControllerFileNameChangedMessage msg)
        {
            ControllerFileName = msg.NewFileName;
        }

        #endregion // TLCGen Events

        #region Constructor

        public SpecificatorPlugin(/*bool test = false*/)
        {
            _myVm = new SpecificatorViewModel(this);
            //if (!test)
            //{
                _generatorView = new SpecificatorView();
                _generatorView.DataContext = _myVm;
            //}
        }

        #endregion // Constructor
    }
}
