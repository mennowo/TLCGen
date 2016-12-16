using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TLCGen.Models;
using TLCGen.Helpers;
using TLCGen.Plugins;
using TLCGen.ViewModels;
using System.Windows.Input;
using System.Xml;
using System.Windows.Media;
using TLCGen.DataAccess;

namespace TLCGen.GebruikersOpties
{
    [TLCGenPlugin(TLCGenPluginElems.TabControl | TLCGenPluginElems.XMLNodeWriter)]
    public class GebruikersOptiesTabViewModel : ViewModelBase, ITLCGenXMLNodeWriter, ITLCGenTabItem
    {
        #region Constants

        const int UitgangenConst = 0;
        const int IngangenConst = 1;
        const int HulpElementenConst = 2;
        const int TimersConst = 3;
        const int CountersConst = 4;
        const int SchakelaarsConst = 5;
        const int GeheugenElementenConst = 6;
        const int ParametersConst = 7;

        #endregion // Constants

        #region Fields

        private GebruikersOptiesModel _MyGebruikersOpties;
        private int _SelectedTabIndex;

        private RelayCommand _AddGebruikersOptieCommand;
        private RelayCommand _RemoveGebruikersOptieCommand;

        object[] _AlleOpties;

        #endregion // Fields

        #region Properties

        public ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel> Uitgangen { get; private set; }
        public ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel> Ingangen { get; private set; }
        public ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel> HulpElementen { get; private set; }
        public ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel> Timers { get; private set; }
        public ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel> Counters { get; private set; }
        public ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel> Schakelaars { get; private set; }
        public ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel> GeheugenElementen { get; private set; }
        public ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel> Parameters { get; private set; }

        public GebruikersOptiesModel MyGebruikersOpties
        {
            get { return _MyGebruikersOpties; }
            set
            {
                _MyGebruikersOpties = value;

                OnMonitoredPropertyChanged(null);
            }
        }

        public int SelectedTabIndex
        {
            get { return _SelectedTabIndex; }
            set
            {
                _SelectedTabIndex = value;
                OnMonitoredPropertyChanged("SelectedTabIndex");
            }
        }

        #endregion // Properties

        #region Commands

        public ICommand AddGebruikersOptieCommand
        {
            get
            {
                if (_AddGebruikersOptieCommand == null)
                {
                    _AddGebruikersOptieCommand = new RelayCommand(AddNewGebruikersOptieCommand_Executed, AddNewGebruikersOptieCommand_CanExecute);
                }
                return _AddGebruikersOptieCommand;
            }
        }

        public ICommand RemoveGebruikersOptieCommand
        {
            get
            {
                if (_RemoveGebruikersOptieCommand == null)
                {
                    _RemoveGebruikersOptieCommand = new RelayCommand(RemoveGebruikersOptieCommand_Executed, RemoveGebruikersOptieCommand_CanExecute);
                }
                return _RemoveGebruikersOptieCommand;
            }
        }

        #endregion // Commands

        #region Command functionality

        void AddNewGebruikersOptieCommand_Executed(object prm)
        {
            if (SelectedTabIndex == UitgangenConst || SelectedTabIndex == IngangenConst)
                ((ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>)_AlleOpties[SelectedTabIndex]).Add(
                    new GebruikersOptieWithIOViewModel(new GebruikersOptieWithIOModel()));
            else
                ((ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>)_AlleOpties[SelectedTabIndex]).Add(
                    new GebruikersOptieViewModel(new GebruikersOptieModel()));
        }

        bool AddNewGebruikersOptieCommand_CanExecute(object prm)
        {
            return true;
        }

        void RemoveGebruikersOptieCommand_Executed(object prm)
        {
            
        }

        bool RemoveGebruikersOptieCommand_CanExecute(object prm)
        {
            return false;
        }

        #endregion // Command functionality

        #region ITLCGenTabItem

        private ControllerModel _Controller;
        public ControllerModel Controller
        {
            get { return _Controller; }
            set
            {
                _Controller = value;
                OnMonitoredPropertyChanged("Controller");
            }
        }

        DataTemplate _ContentDataTemplate;
        public DataTemplate ContentDataTemplate
        {
            get
            {
                if (_ContentDataTemplate == null)
                {
                    _ContentDataTemplate = new DataTemplate();
                    _ContentDataTemplate.VisualTree = new FrameworkElementFactory(typeof(GebruikersOptiesTabView));
                }
                return _ContentDataTemplate;
            }
        }

        public string DisplayName
        {
            get
            {
                return "Gebruikersopties";
            }
        }

        public string GetPluginName()
        {
            return "CCOLGebruikersOpties";
        }

        private bool _IsEnabled;
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set
            {
                _IsEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }

        public ImageSource Icon
        {
            get
            {
                ResourceDictionary dict = new ResourceDictionary();
                Uri u = new Uri("pack://application:,,,/" +
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Name +
                    ";component/" + "GebruikersOptiesIcon.xaml");
                dict.Source = u;
                return (DrawingImage)dict["GebruikersOptiesTabDrawingImage"];
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

        #endregion // ITLCGenTabItem

        #region ITLCGenXMLNodeWriter

        public void GetXmlFromDocument(XmlDocument document)
        {
            bool found = false;
            foreach(XmlNode node in document.FirstChild.ChildNodes)
            {
                if(node.LocalName == "GebruikersOpties")
                {
                    MyGebruikersOpties = XmlNodeConverter.ConvertNode<GebruikersOptiesModel>(node);
                    found = true;
                    break;
                }
            }

            if (found)
            {
                Uitgangen = new ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>(_MyGebruikersOpties.Uitgangen);
                Ingangen = new ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>(_MyGebruikersOpties.Ingangen);
                HulpElementen = new ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>(_MyGebruikersOpties.HulpElementen);
                Timers = new ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>(_MyGebruikersOpties.Timers);
                Counters = new ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>(_MyGebruikersOpties.Counters);
                Schakelaars = new ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>(_MyGebruikersOpties.Schakelaars);
                GeheugenElementen = new ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>(_MyGebruikersOpties.GeheugenElementen);
                Parameters = new ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>(_MyGebruikersOpties.Parameters);

                _AlleOpties = new object[8];
                _AlleOpties[UitgangenConst] = Uitgangen;
                _AlleOpties[IngangenConst] = Ingangen;
                _AlleOpties[HulpElementenConst] = HulpElementen;
                _AlleOpties[TimersConst] = Timers;
                _AlleOpties[CountersConst] = Counters;
                _AlleOpties[SchakelaarsConst] = Schakelaars;
                _AlleOpties[GeheugenElementenConst] = GeheugenElementen;
                _AlleOpties[ParametersConst] = Parameters;

                OnPropertyChanged(null);
            }
        }

        public void SetXmlInDocument(XmlDocument document)
        {
            XmlDocument doc = TLCGenSerialization.SerializeToXmlDocument(MyGebruikersOpties);
            XmlNode node = document.ImportNode(doc.DocumentElement, true);
            document.DocumentElement.AppendChild(node);
        }

        #endregion // ITLCGenXMLNodeWriter

        #region Constructor

        public GebruikersOptiesTabViewModel()
        {
            MyGebruikersOpties = new GebruikersOptiesModel();

            Uitgangen = new ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>(_MyGebruikersOpties.Uitgangen);
            Ingangen = new ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>(_MyGebruikersOpties.Ingangen);
            HulpElementen = new ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>(_MyGebruikersOpties.HulpElementen);
            Timers = new ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>(_MyGebruikersOpties.Timers);
            Counters = new ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>(_MyGebruikersOpties.Counters);
            Schakelaars = new ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>(_MyGebruikersOpties.Schakelaars);
            GeheugenElementen = new ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>(_MyGebruikersOpties.GeheugenElementen);
            Parameters = new ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>(_MyGebruikersOpties.Parameters);

            _AlleOpties = new object[8];
            _AlleOpties[UitgangenConst] = Uitgangen;
            _AlleOpties[IngangenConst] = Ingangen;
            _AlleOpties[HulpElementenConst] = HulpElementen;
            _AlleOpties[TimersConst] = Timers;
            _AlleOpties[CountersConst] = Counters;
            _AlleOpties[SchakelaarsConst] = Schakelaars;
            _AlleOpties[GeheugenElementenConst] = GeheugenElementen;
            _AlleOpties[ParametersConst] = Parameters;
        }

        #endregion // Constructor
    }
}
