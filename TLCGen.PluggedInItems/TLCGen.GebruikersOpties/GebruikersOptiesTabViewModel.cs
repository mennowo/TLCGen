using System;
using System.Collections.Generic;
using System.Windows;
using TLCGen.Models;
using TLCGen.Helpers;
using TLCGen.Plugins;
using System.Windows.Input;
using System.Xml;
using System.Windows.Media;
using TLCGen.Integrity;
using GalaSoft.MvvmLight.Messaging;
using System.Collections;
using GalaSoft.MvvmLight;
using TLCGen.Models.Enumerations;

namespace TLCGen.GebruikersOpties
{
    [TLCGenTabItem(-1, TabItemTypeEnum.MainWindow)]
    [TLCGenPlugin(TLCGenPluginElems.TabControl | TLCGenPluginElems.XMLNodeWriter | TLCGenPluginElems.IOElementProvider)]
    public class GebruikersOptiesTabViewModel : ViewModelBase, ITLCGenXMLNodeWriter, ITLCGenTabItem, ITLCGenElementProvider
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
        const int OptiesMax = 8;

        readonly string[] OptiesNames = { "us", "is", "h", "t", "c", "sch", "m", "prm"};

        #endregion // Constants

        #region Fields

        private GebruikersOptiesModel _MyGebruikersOpties;
        private int _SelectedTabIndex;

        private object[] _SelectedOptie = new object[OptiesMax];
        private IList[] _SelectedOpties = new IList[OptiesMax]
        {
            new ArrayList(),
            new ArrayList(),
            new ArrayList(),
            new ArrayList(),
            new ArrayList(),
            new ArrayList(),
            new ArrayList(),
            new ArrayList()
        };

        private RelayCommand _AddGebruikersOptieCommand;
        private RelayCommand _RemoveGebruikersOptieCommand;
        private RelayCommand _OmhoogCommand;
        private RelayCommand _OmlaagCommand;

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

                RaisePropertyChanged("");
            }
        }

        public int SelectedTabIndex
        {
            get { return _SelectedTabIndex; }
            set
            {
                _SelectedTabIndex = value;
                RaisePropertyChanged("SelectedOptie");
                RaisePropertyChanged("SelectedTabIndex");
            }
        }

        public object SelectedOptie
        {
            get
            {
                if (SelectedTabIndex >= 0 && SelectedTabIndex < OptiesMax)
                    return _SelectedOptie[SelectedTabIndex];
                else
                    return null;
            }
            set
            {
                if (SelectedTabIndex >= 0 && SelectedTabIndex < OptiesMax)
                {
                    _SelectedOptie[SelectedTabIndex] = value;
                }
                RaisePropertyChanged("SelectedOptie");
            }
        }

        public IList SelectedOpties
        {
            get
            {
                if (SelectedTabIndex >= 0 && SelectedTabIndex < OptiesMax)
                {
                    return _SelectedOpties[SelectedTabIndex];
                }
                else
                    return null;
            }
            set
            {
                if (SelectedTabIndex >= 0 && SelectedTabIndex < OptiesMax)
                {
                    _SelectedOpties[SelectedTabIndex] = value;
                }
                RaisePropertyChanged("SelectedOpties");
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

        public ICommand OmhoogCommand
        {
            get
            {
                if (_OmhoogCommand == null)
                {
                    _OmhoogCommand = new RelayCommand(OmhoogCommand_Executed, OmhoogCommand_CanExecute);
                }
                return _OmhoogCommand;
            }
        }

        public ICommand OmlaagCommand
        {
            get
            {
                if (_OmlaagCommand == null)
                {
                    _OmlaagCommand = new RelayCommand(OmlaagCommand_Executed, OmlaagCommand_CanExecute);
                }
                return _OmlaagCommand;
            }
        }

        #endregion // Commands

        #region Command functionality

        void AddNewGebruikersOptieCommand_Executed(object prm)
        {
            int index = -1;

            if (SelectedTabIndex == UitgangenConst || SelectedTabIndex == IngangenConst)
            {
                if(SelectedOptie != null)
                    index = ((ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>)_AlleOpties[SelectedTabIndex]).IndexOf(
                        SelectedOptie as GebruikersOptieWithIOViewModel) + 1;

                var o = new GebruikersOptieWithIOViewModel(new GebruikersOptieWithIOModel());
                int i = 1;
                while(string.IsNullOrEmpty(o.Naam))
                {
                    o.Naam = OptiesNames[SelectedTabIndex] + "_" + (((ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>)_AlleOpties[SelectedTabIndex]).Count + i);
                    ++i;
                }

                if(index > 0 && index < ((ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>)_AlleOpties[SelectedTabIndex]).Count)
                    ((ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>)_AlleOpties[SelectedTabIndex]).Insert(index, o);
                else
                    ((ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>)_AlleOpties[SelectedTabIndex]).Add(o);
            }
            else
            {
                if(SelectedOptie != null)
                    index = ((ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>)_AlleOpties[SelectedTabIndex]).IndexOf(
                        SelectedOptie as GebruikersOptieViewModel) + 1;

                var o = new GebruikersOptieViewModel(new GebruikersOptieModel());
                int i = 1;
                while (string.IsNullOrEmpty(o.Naam))
                {
                    o.Naam = OptiesNames[SelectedTabIndex] + "_" + (((ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>)_AlleOpties[SelectedTabIndex]).Count + i);
                    ++i;
                }
                if (OptiesNames[SelectedTabIndex] == "sch" || 
                    OptiesNames[SelectedTabIndex] == "t" || 
                    OptiesNames[SelectedTabIndex] == "prm" || 
                    OptiesNames[SelectedTabIndex] == "c")
                {
                    o.Instelling = 0;
                }
                if (OptiesNames[SelectedTabIndex] == "t")
                {
                    o.Type = CCOLElementTypeEnum.TE_type;
                }
                if (OptiesNames[SelectedTabIndex] == "prm")
                {
                    o.Type = CCOLElementTypeEnum.Geen;
                }

                switch (OptiesNames[SelectedTabIndex])
                {
                    case "us": o.ObjectType = TLCGenObjectTypeEnum.Output; break;
                    case "is": o.ObjectType = TLCGenObjectTypeEnum.Input; break;
                    case "h": o.ObjectType = TLCGenObjectTypeEnum.CCOLHelpElement; break;
                    case "t": o.ObjectType = TLCGenObjectTypeEnum.CCOLTimer; break;
                    case "c": o.ObjectType = TLCGenObjectTypeEnum.CCOLCounter; break;
                    case "sch": o.ObjectType = TLCGenObjectTypeEnum.CCOLSchakelaar; break;
                    case "m": o.ObjectType = TLCGenObjectTypeEnum.CCOLMemoryElement; break;
                    case "prm": o.ObjectType = TLCGenObjectTypeEnum.CCOLParameter; break;
                }

                if (index > 0 && index < ((ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>)_AlleOpties[SelectedTabIndex]).Count)
                    ((ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>)_AlleOpties[SelectedTabIndex]).Insert(index, o);
                else
                    ((ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>)_AlleOpties[SelectedTabIndex]).Add(o);
            }

            if (SelectedTabIndex == UitgangenConst || SelectedTabIndex == IngangenConst)
                ((ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>)_AlleOpties[SelectedTabIndex]).RebuildList();
            else
                ((ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>)_AlleOpties[SelectedTabIndex]).RebuildList();

            Messenger.Default.Send(new Messaging.Messages.ControllerDataChangedMessage());
        }

        bool AddNewGebruikersOptieCommand_CanExecute(object prm)
        {
            return true;
        }

        void RemoveGebruikersOptieCommand_Executed(object prm)
        {
            int index = 0;
            if (SelectedOpties != null && SelectedOpties.Count > 0)
            {
                if (SelectedTabIndex == UitgangenConst || SelectedTabIndex == IngangenConst)
                {
                    var list = ((ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>)_AlleOpties[SelectedTabIndex]);
                    index = list.IndexOf(SelectedOpties[0] as GebruikersOptieWithIOViewModel);
                    var rlist = new List<GebruikersOptieWithIOViewModel>();
                    foreach (var o in SelectedOpties)
                    {
                        rlist.Add(o as GebruikersOptieWithIOViewModel);
                    }
                    foreach(var o in rlist)
                    {
                        list.Remove(o);
                    }
                }
                else
                {
                    var list = ((ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>)_AlleOpties[SelectedTabIndex]);
                    index = list.IndexOf(SelectedOpties[0] as GebruikersOptieViewModel);
                    var rlist = new List<GebruikersOptieViewModel>();
                    foreach (var o in SelectedOpties)
                    {
                        rlist.Add(o as GebruikersOptieViewModel);
                    }
                    foreach (var o in rlist)
                    {
                        list.Remove(o);
                    }
                }
            }
            else 
            if (SelectedOptie != null)
            {
                if (SelectedTabIndex == UitgangenConst || SelectedTabIndex == IngangenConst)
                {
                    var list = ((ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>)_AlleOpties[SelectedTabIndex]);
                    index = list.IndexOf(SelectedOptie as GebruikersOptieWithIOViewModel);
                    list.Remove(SelectedOptie as GebruikersOptieWithIOViewModel);
                }
                else
                {
                    var list = ((ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>)_AlleOpties[SelectedTabIndex]);
                    index = list.IndexOf(SelectedOptie as GebruikersOptieViewModel);
                    list.Remove(SelectedOptie as GebruikersOptieViewModel);
                }        
            }

            if (SelectedTabIndex == UitgangenConst || SelectedTabIndex == IngangenConst)
            {
                var list = ((ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>)_AlleOpties[SelectedTabIndex]);
                int c = list.Count;
                if (index >= c) index = c - 1;
                if(index >= 0)
                    SelectedOptie = list[index];
                else
                    SelectedOptie = null;
            }
            else
            {
                var list = ((ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>)_AlleOpties[SelectedTabIndex]);
                int c = list.Count;
                if (index >= c) index = c - 1;
                if(index >= 0)
                    SelectedOptie = list[index];
                else
                    SelectedOptie = null;
            }

            Messenger.Default.Send(new Messaging.Messages.ControllerDataChangedMessage());
        }

        bool RemoveGebruikersOptieCommand_CanExecute(object prm)
        {
            return SelectedOptie != null;
        }

        void OmhoogCommand_Executed(object prm)
        {
            var optie = SelectedOptie;

            int index = -1;

            if (SelectedTabIndex == UitgangenConst || SelectedTabIndex == IngangenConst)
            {
                var list = ((ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>)_AlleOpties[SelectedTabIndex]);
                if (SelectedOpties != null && SelectedOpties.Count > 0)
                {
                    foreach (var o in SelectedOpties)
                    {
                        index = list.IndexOf(o as GebruikersOptieWithIOViewModel);
                        if (index > 0)
                        {
                            list.Move(index, index - 1);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else if (SelectedOptie != null)
                {
                    index = list.IndexOf(SelectedOptie as GebruikersOptieWithIOViewModel);
                    if (index > 0)
                    {
                        list.Move(index, index - 1);
                    }
                }
                list.RebuildList();
            }
            else
            {
                var list = ((ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>)_AlleOpties[SelectedTabIndex]);
                if (SelectedOpties != null && SelectedOpties.Count > 0)
                {
                    foreach (var o in SelectedOpties)
                    {
                        index = list.IndexOf(o as GebruikersOptieViewModel);
                        if (index > 0)
                        {
                            list.Move(index, index - 1);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else if (SelectedOptie != null)
                {
                    index = list.IndexOf(SelectedOptie as GebruikersOptieViewModel);
                    if (index > 0)
                    {
                        list.Move(index, index - 1);
                    }
                }
                list.RebuildList();
            }
            
            SelectedOptie = optie;
            Messenger.Default.Send(new Messaging.Messages.ControllerDataChangedMessage());
        }

        bool OmhoogCommand_CanExecute(object prm)
        {
            return SelectedOptie != null;
        }

        void OmlaagCommand_Executed(object prm)
        {
            var optie = SelectedOptie;

            int index = -1;
            int max = -1;

            if (SelectedTabIndex == UitgangenConst || SelectedTabIndex == IngangenConst)
            {
                var list = ((ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>)_AlleOpties[SelectedTabIndex]);
                max = list.Count - 1;
                if (SelectedOpties != null && SelectedOpties.Count > 0)
                {
                    for (int i = SelectedOpties.Count - 1; i >= 0; --i)
                    {
                        index = list.IndexOf(SelectedOpties[i] as GebruikersOptieWithIOViewModel);
                        if (index >= 0 && index < max)
                        {
                            list.Move(index, index + 1);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else if (SelectedOptie != null)
                {
                    index = list.IndexOf(SelectedOptie as GebruikersOptieWithIOViewModel);
                    if (index >= 0 && index < max)
                    {
                        list.Move(index, index + 1);
                    }
                }
                list.RebuildList();
            }
            else
            {
                var list = ((ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>)_AlleOpties[SelectedTabIndex]);
                max = list.Count - 1;
                if (SelectedOpties != null && SelectedOpties.Count > 0)
                {
                    for (int i = SelectedOpties.Count - 1; i >= 0; --i)
                    {
                        index = list.IndexOf(SelectedOpties[i] as GebruikersOptieViewModel);
                        if (index >= 0 && index < max)
                        {
                            list.Move(index, index + 1);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else if (SelectedOptie != null)
                {
                    index = list.IndexOf(SelectedOptie as GebruikersOptieViewModel);
                    if (index >= 0 && index < max)
                    {
                        list.Move(index, index + 1);
                    }
                }
                list.RebuildList();
            }

            SelectedOptie = optie;
            Messenger.Default.Send(new Messaging.Messages.ControllerDataChangedMessage());
        }

        bool OmlaagCommand_CanExecute(object prm)
        {
            return SelectedOptie != null;
        }

        #endregion // Command functionality

        #region Private Methods

        private bool CheckCurrentItemNameUnique()
        {
            if (SelectedOptie == null)
                return true;

            var oio = SelectedOptie as GebruikersOptieWithIOViewModel;
            var o = SelectedOptie as GebruikersOptieViewModel;
            if (oio != null)
            {
                return TLCGenIntegrityChecker.IsElementNaamUnique(_Controller, oio.Naam);
            }
            else
            {
                return TLCGenIntegrityChecker.IsElementNaamUnique(_Controller, o.Naam);
            }
        }

        private void ResetMyGebruikersOpties()
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

        #endregion // Private Methods

        #region ITLCGenTabItem

        private ControllerModel _Controller;
        public ControllerModel Controller
        {
            get { return _Controller; }
            set
            {
                _Controller = value;
                if(value == null)
                {
                    ResetMyGebruikersOpties();
                }
                RaisePropertyChanged("");
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
                RaisePropertyChanged("IsEnabled");
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
            return CheckCurrentItemNameUnique();
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

                foreach (var el in Uitgangen) el.ObjectType = TLCGenObjectTypeEnum.Output;
                foreach (var el in Ingangen) el.ObjectType = TLCGenObjectTypeEnum.Input;
                foreach (var el in HulpElementen) el.ObjectType = TLCGenObjectTypeEnum.CCOLHelpElement;
                foreach (var el in Timers) el.ObjectType = TLCGenObjectTypeEnum.CCOLTimer;
                foreach (var el in Counters) el.ObjectType = TLCGenObjectTypeEnum.CCOLCounter;
                foreach (var el in Schakelaars) el.ObjectType = TLCGenObjectTypeEnum.CCOLSchakelaar;
                foreach (var el in GeheugenElementen) el.ObjectType = TLCGenObjectTypeEnum.CCOLMemoryElement;
                foreach (var el in Parameters) el.ObjectType = TLCGenObjectTypeEnum.CCOLParameter;

                RaisePropertyChanged("");
            }
        }

        public void SetXmlInDocument(XmlDocument document)
        {
            XmlDocument doc = TLCGenSerialization.SerializeToXmlDocument(MyGebruikersOpties);
            XmlNode node = document.ImportNode(doc.DocumentElement, true);
            document.DocumentElement.AppendChild(node);
        }

        #endregion // ITLCGenXMLNodeWriter

        #region ITLCGenElementProvider

        public List<IOElementModel> GetOutputItems()
        {
            List<IOElementModel> items = new List<IOElementModel>();
            foreach(var v in Uitgangen)
            {
                items.Add(v.GebruikersOptieWithOI as IOElementModel);
            }
            return items;
        }

        public List<IOElementModel> GetInputItems()
        {
            List<IOElementModel> items = new List<IOElementModel>();
            foreach (var v in Ingangen)
            {
                items.Add(v.GebruikersOptieWithOI as IOElementModel);
            }
            return items;
        }

        public bool IsElementNameUnique(string name)
        {
            foreach(var o in Uitgangen) { if (o.Naam == name) return false; }
            foreach(var o in Ingangen) { if (o.Naam == name) return false; }
            foreach(var o in HulpElementen) { if (o.Naam == name) return false; }
            foreach(var o in Timers) { if (o.Naam == name) return false; }
            foreach(var o in Counters) { if (o.Naam == name) return false; }
            foreach(var o in Schakelaars) { if (o.Naam == name) return false; }
            foreach(var o in GeheugenElementen) { if (o.Naam == name) return false; }
            foreach(var o in Parameters) { if (o.Naam == name) return false; }
            return true;
        }

        public Generators.CCOL.CodeGeneration.CCOLElementTimeTypeEnum ConvertType(CCOLElementTypeEnum type)
        {
            switch(type)
            {
                case CCOLElementTypeEnum.TE_type:
                    return Generators.CCOL.CodeGeneration.CCOLElementTimeTypeEnum.TE_type;
                case CCOLElementTypeEnum.TS_type:
                    return Generators.CCOL.CodeGeneration.CCOLElementTimeTypeEnum.TS_type;
                case CCOLElementTypeEnum.TM_type:
                    return Generators.CCOL.CodeGeneration.CCOLElementTimeTypeEnum.TM_type;
                case CCOLElementTypeEnum.Geen:
                default:
                    return Generators.CCOL.CodeGeneration.CCOLElementTimeTypeEnum.None;
            }
        }

        public List<object> GetAllItems()
        {
            var AllElements = new List<object>();

            foreach(var elem in Uitgangen)
            {
                AllElements.Add(
                    new Generators.CCOL.CodeGeneration.CCOLElement(
                        elem.Naam,
                        Generators.CCOL.CodeGeneration.CCOLElementTypeEnum.Uitgang,
					    elem.Commentaar));
            }
            foreach (var elem in Ingangen)
            {
                AllElements.Add(
                    new Generators.CCOL.CodeGeneration.CCOLElement(
                        elem.Naam,
                        Generators.CCOL.CodeGeneration.CCOLElementTypeEnum.Ingang,
					    elem.Commentaar));
            }
            foreach (var elem in HulpElementen)
            {
                AllElements.Add(
                    new Generators.CCOL.CodeGeneration.CCOLElement(
                        elem.Naam,
                        Generators.CCOL.CodeGeneration.CCOLElementTypeEnum.HulpElement,
					    elem.Commentaar));
            }
            foreach (var elem in Timers)
            {
                AllElements.Add(
                    new Generators.CCOL.CodeGeneration.CCOLElement(
                        elem.Naam,
                        elem.Instelling.Value,
                        ConvertType(elem.Type),
                        Generators.CCOL.CodeGeneration.CCOLElementTypeEnum.Timer,
					    elem.Commentaar));
            }
            foreach (var elem in Counters)
            {
                AllElements.Add(
                    new Generators.CCOL.CodeGeneration.CCOLElement(
                        elem.Naam,
                        elem.Instelling.Value,
                        Generators.CCOL.CodeGeneration.CCOLElementTimeTypeEnum.None,
                        Generators.CCOL.CodeGeneration.CCOLElementTypeEnum.Counter,
					    elem.Commentaar));
            }
            foreach (var elem in Schakelaars)
            {
                AllElements.Add(
                    new Generators.CCOL.CodeGeneration.CCOLElement(
                        elem.Naam,
                        elem.Instelling.Value,
                        Generators.CCOL.CodeGeneration.CCOLElementTimeTypeEnum.SCH_type,
                        Generators.CCOL.CodeGeneration.CCOLElementTypeEnum.Schakelaar,
					    elem.Commentaar));
            }
            foreach (var elem in GeheugenElementen)
            {
                AllElements.Add(
                    new Generators.CCOL.CodeGeneration.CCOLElement(
                        elem.Naam,
                        Generators.CCOL.CodeGeneration.CCOLElementTypeEnum.GeheugenElement,
					    elem.Commentaar));
            }
            foreach (var elem in Parameters)
            {
                AllElements.Add(
                    new Generators.CCOL.CodeGeneration.CCOLElement(
                        elem.Naam,
                        elem.Instelling.Value,
                        ConvertType(elem.Type),
                        Generators.CCOL.CodeGeneration.CCOLElementTypeEnum.Parameter,
					    elem.Commentaar));
            }

            return AllElements;
        }

        #endregion // ITLCGenElementProvider

        #region Constructor

        public GebruikersOptiesTabViewModel()
        {
            ResetMyGebruikersOpties();
        }

        #endregion // Constructor
    }
}
