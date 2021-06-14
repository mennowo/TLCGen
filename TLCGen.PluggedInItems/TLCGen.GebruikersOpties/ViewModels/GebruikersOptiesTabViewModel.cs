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
using System.Linq;
using RelayCommand = GalaSoft.MvvmLight.CommandWpf.RelayCommand;
using TLCGen.Messaging.Messages;
using GongSolutions.Wpf.DragDrop;

namespace TLCGen.GebruikersOpties
{
    [TLCGenTabItem(-1, TabItemTypeEnum.MainWindow)]
    [TLCGenPlugin(TLCGenPluginElems.TabControl | TLCGenPluginElems.XMLNodeWriter | TLCGenPluginElems.IOElementProvider)]
    public class GebruikersOptiesTabViewModel : ViewModelBase, ITLCGenXMLNodeWriter, ITLCGenTabItem, ITLCGenElementProvider, IDropTarget
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

        readonly string[] OptiesNames = { "us", "is", "h", "t", "c", "sch", "m", "prm" };

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

        public GebruikersOptiesImportExportTabViewModel ImportExportVM { get; }

        public GebruikersOptiesModel MyGebruikersOpties
        {
            get => _MyGebruikersOpties;
            set
            {
                _MyGebruikersOpties = value;
                ImportExportVM.DataModel = value;
                RaisePropertyChanged("");
            }
        }

        public bool IsImportExportNotSelected => !_isImportExportSelected;

        public bool IsImportExportSelected
        {
            set
            {
                if (value) ImportExportVM.RebuildAllItems(this);
                _isImportExportSelected = value;
                RaisePropertyChanged(nameof(IsImportExportNotSelected));
            }
        }

        public int SelectedTabIndex
        {
            get => _SelectedTabIndex;
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

        void AddNewGebruikersOptieCommand_Executed()
        {
            var index = -1;

            if (SelectedTabIndex == UitgangenConst || SelectedTabIndex == IngangenConst)
            {
                if (SelectedOptie != null)
                    index = ((ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>)_AlleOpties[SelectedTabIndex]).IndexOf(
                        SelectedOptie as GebruikersOptieWithIOViewModel) + 1;

                var o = new GebruikersOptieWithIOViewModel(new GebruikersOptieWithIOModel());
                o.PropertyChanged += Optie_PropertyChanged;
                var i = 1;
                while (string.IsNullOrEmpty(o.Naam))
                {
                    o.Naam = OptiesNames[SelectedTabIndex] + "_" + (((ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>)_AlleOpties[SelectedTabIndex]).Count + i);
                    ++i;
                }

                switch (OptiesNames[SelectedTabIndex])
                {
                    case "us": o.ObjectType = TLCGenObjectTypeEnum.Output; break;
                    case "is": o.ObjectType = TLCGenObjectTypeEnum.Input; break;
                }

                if (index > 0 && index < ((ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>)_AlleOpties[SelectedTabIndex]).Count)
                    ((ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>)_AlleOpties[SelectedTabIndex]).Insert(index, o);
                else
                    ((ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>)_AlleOpties[SelectedTabIndex]).Add(o);
            }
            else
            {
                if (SelectedOptie != null)
                    index = ((ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>)_AlleOpties[SelectedTabIndex]).IndexOf(
                        SelectedOptie as GebruikersOptieViewModel) + 1;

                var o = new GebruikersOptieViewModel(new GebruikersOptieModel());
                o.PropertyChanged += Optie_PropertyChanged;
                var i = 1;
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

        bool AddNewGebruikersOptieCommand_CanExecute()
        {
            return true;
        }

        void RemoveGebruikersOptieCommand_Executed()
        {
            var index = 0;
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
                    foreach (var o in rlist)
                    {
                        o.PropertyChanged -= Optie_PropertyChanged;
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
                        o.PropertyChanged -= Optie_PropertyChanged;
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
                    (SelectedOptie as GebruikersOptieWithIOViewModel).PropertyChanged -= Optie_PropertyChanged;
                }
                else
                {
                    var list = ((ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>)_AlleOpties[SelectedTabIndex]);
                    index = list.IndexOf(SelectedOptie as GebruikersOptieViewModel);
                    list.Remove(SelectedOptie as GebruikersOptieViewModel);
                    (SelectedOptie as GebruikersOptieViewModel).PropertyChanged -= Optie_PropertyChanged;
                }
            }

            if (SelectedTabIndex == UitgangenConst || SelectedTabIndex == IngangenConst)
            {
                var list = ((ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>)_AlleOpties[SelectedTabIndex]);
                var c = list.Count;
                if (index >= c) index = c - 1;
                if (index >= 0)
                    SelectedOptie = list[index];
                else
                    SelectedOptie = null;
            }
            else
            {
                var list = ((ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>)_AlleOpties[SelectedTabIndex]);
                var c = list.Count;
                if (index >= c) index = c - 1;
                if (index >= 0)
                    SelectedOptie = list[index];
                else
                    SelectedOptie = null;
            }

            Messenger.Default.Send(new Messaging.Messages.ControllerDataChangedMessage());
        }

        bool RemoveGebruikersOptieCommand_CanExecute()
        {
            return SelectedOptie != null;
        }

        void OmhoogCommand_Executed()
        {
            var optie = SelectedOptie;

            var index = -1;

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

        bool OmhoogCommand_CanExecute()
        {
            return SelectedOptie != null;
        }

        void OmlaagCommand_Executed()
        {
            var optie = SelectedOptie;

            var index = -1;
            var max = -1;

            if (SelectedTabIndex == UitgangenConst || SelectedTabIndex == IngangenConst)
            {
                var list = ((ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>)_AlleOpties[SelectedTabIndex]);
                max = list.Count - 1;
                if (SelectedOpties != null && SelectedOpties.Count > 0)
                {
                    for (var i = SelectedOpties.Count - 1; i >= 0; --i)
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
                    for (var i = SelectedOpties.Count - 1; i >= 0; --i)
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

        bool OmlaagCommand_CanExecute()
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
                return TLCGenIntegrityChecker.IsElementNaamUnique(_Controller, oio.Naam, oio.ObjectType);
            }
            else
            {
                return TLCGenIntegrityChecker.IsElementNaamUnique(_Controller, o.Naam, o.ObjectType);
            }
        }

        private void ResetMyGebruikersOpties()
        {
            if (Uitgangen != null) foreach (var op in Uitgangen) op.PropertyChanged -= Optie_PropertyChanged;
            if (Ingangen != null) foreach (var ip in Ingangen) ip.PropertyChanged -= Optie_PropertyChanged;
            if (HulpElementen != null) foreach (var he in HulpElementen) he.PropertyChanged -= Optie_PropertyChanged;
            if (Timers != null) foreach (var ti in Timers) ti.PropertyChanged -= Optie_PropertyChanged;
            if (Counters != null) foreach (var ct in Counters) ct.PropertyChanged -= Optie_PropertyChanged;
            if (Schakelaars != null) foreach (var sch in Schakelaars) sch.PropertyChanged -= Optie_PropertyChanged;
            if (GeheugenElementen != null) foreach (var me in GeheugenElementen) me.PropertyChanged -= Optie_PropertyChanged;
            if (Parameters != null) foreach (var prm in Parameters) prm.PropertyChanged -= Optie_PropertyChanged;

            MyGebruikersOpties = new GebruikersOptiesModel();

            if (Uitgangen != null) Uitgangen.CollectionChanged -= Uitgangen_CollectionChanged;
            if (Ingangen != null) Ingangen.CollectionChanged -= Ingangen_CollectionChanged;
            if (HulpElementen != null) HulpElementen.CollectionChanged -= HulpElementen_CollectionChanged;
            if (Timers != null) Timers.CollectionChanged -= Timers_CollectionChanged;
            if (Counters != null) Counters.CollectionChanged -= Counters_CollectionChanged;
            if (Schakelaars != null) Schakelaars.CollectionChanged -= Schakelaars_CollectionChanged;
            if (GeheugenElementen != null) GeheugenElementen.CollectionChanged -= GeheugenElementen_CollectionChanged;
            if (Parameters != null) Parameters.CollectionChanged -= Parameters_CollectionChanged;

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

            foreach (var op in Uitgangen) { op.PropertyChanged += Optie_PropertyChanged; op.ObjectType = TLCGenObjectTypeEnum.Output; }
            foreach (var ip in Ingangen) { ip.PropertyChanged += Optie_PropertyChanged; ip.ObjectType = TLCGenObjectTypeEnum.Input; }
            foreach (var he in HulpElementen) { he.PropertyChanged += Optie_PropertyChanged; he.ObjectType = TLCGenObjectTypeEnum.CCOLHelpElement; }
            foreach (var ti in Timers) { ti.PropertyChanged += Optie_PropertyChanged; ti.ObjectType = TLCGenObjectTypeEnum.CCOLTimer; }
            foreach (var ct in Counters) { ct.PropertyChanged += Optie_PropertyChanged; ct.ObjectType = TLCGenObjectTypeEnum.CCOLCounter; }
            foreach (var sch in Schakelaars) { sch.PropertyChanged += Optie_PropertyChanged; sch.ObjectType = TLCGenObjectTypeEnum.CCOLSchakelaar; }
            foreach (var me in GeheugenElementen) { me.PropertyChanged += Optie_PropertyChanged; me.ObjectType = TLCGenObjectTypeEnum.CCOLMemoryElement; }
            foreach (var prm in Parameters) { prm.PropertyChanged += Optie_PropertyChanged; prm.ObjectType = TLCGenObjectTypeEnum.CCOLParameter; }

            Uitgangen.CollectionChanged += Uitgangen_CollectionChanged;
            Ingangen.CollectionChanged += Ingangen_CollectionChanged;
            HulpElementen.CollectionChanged += HulpElementen_CollectionChanged;
            Timers.CollectionChanged += Timers_CollectionChanged;
            Counters.CollectionChanged += Counters_CollectionChanged;
            Schakelaars.CollectionChanged += Schakelaars_CollectionChanged;
            GeheugenElementen.CollectionChanged += GeheugenElementen_CollectionChanged;
            Parameters.CollectionChanged += Parameters_CollectionChanged;
        }

        private void Uitgangen_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Uitgangen.RebuildList();
            MessengerInstance.Send(new ControllerDataChangedMessage());
        }

        private void Ingangen_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Ingangen.RebuildList();
            MessengerInstance.Send(new ControllerDataChangedMessage());
        }

        private void HulpElementen_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            HulpElementen.RebuildList();
            MessengerInstance.Send(new ControllerDataChangedMessage());
        }

        private void Timers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Timers.RebuildList();
            MessengerInstance.Send(new ControllerDataChangedMessage());
        }

        private void Counters_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Counters.RebuildList();
            MessengerInstance.Send(new ControllerDataChangedMessage());
        }

        private void Schakelaars_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Schakelaars.RebuildList();
            MessengerInstance.Send(new ControllerDataChangedMessage());
        }

        private void GeheugenElementen_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            GeheugenElementen.RebuildList();
            MessengerInstance.Send(new ControllerDataChangedMessage());
        }

        private void Parameters_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Parameters.RebuildList();
            MessengerInstance.Send(new ControllerDataChangedMessage());
        }


        #endregion // Private Methods

        #region Event Handling

        private volatile bool _SettingMultiple = false;
        private void Optie_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_SettingMultiple || string.IsNullOrEmpty(e.PropertyName))
                return;

            if (SelectedOpties != null && SelectedOpties.Count > 1)
            {
                _SettingMultiple = true;
                MultiPropertySetter.SetPropertyForAllItems<GebruikersOptieViewModel>(sender, e.PropertyName, SelectedOpties);
            }
            _SettingMultiple = false;
        }

        #endregion // Event Handling

        #region ITLCGenTabItem

        private ControllerModel _Controller;
        public ControllerModel Controller
        {
            get => _Controller;
            set
            {
                _Controller = value;
                if (value == null)
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

        public string DisplayName => "Gebruikersopties";

        public string GetPluginName()
        {
            return "CCOLGebruikersOpties";
        }

        private bool _IsEnabled;
        private bool _isImportExportSelected;

        public bool IsEnabled
        {
            get => _IsEnabled;
            set
            {
                _IsEnabled = value;
                RaisePropertyChanged("IsEnabled");
            }
        }

        public bool Visibility { get; set; } = true;

        public ImageSource Icon
        {
            get
            {
                var dict = new ResourceDictionary();
                var u = new Uri("pack://application:,,,/" +
                                System.Reflection.Assembly.GetExecutingAssembly().GetName().Name +
                                ";component/" + "Resources/GebruikersOptiesIcon.xaml");
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
            // TODO check why do this? and why does it sometimes return false?
            // seems like this plugin returns false if two items of different types have the same name (PRM p1 and MM p1...)
            return true; // CheckCurrentItemNameUnique();
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
                if (node.LocalName == "GebruikersOpties")
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
            var doc = TLCGenSerialization.SerializeToXmlDocument(MyGebruikersOpties);
            var node = document.ImportNode(doc.DocumentElement, true);
            document.DocumentElement.AppendChild(node);
        }

        #endregion // ITLCGenXMLNodeWriter

        #region ITLCGenElementProvider

        public List<IOElementModel> GetOutputItems()
        {
            return Uitgangen.Select(v => v.GebruikersOptieWithIO).Cast<IOElementModel>().ToList();
        }

        public List<IOElementModel> GetInputItems()
        {
            return Ingangen.Select(v => v.GebruikersOptieWithIO).Cast<IOElementModel>().ToList();
        }

        public bool IsElementNameUnique(string name, TLCGenObjectTypeEnum type)
        {
            foreach (var o in Uitgangen) { if (o.Naam == name && o.ObjectType == type) return false; }
            foreach (var o in Ingangen) { if (o.Naam == name && o.ObjectType == type) return false; }
            foreach (var o in HulpElementen) { if (o.Naam == name && o.ObjectType == type) return false; }
            foreach (var o in Timers) { if (o.Naam == name && o.ObjectType == type) return false; }
            foreach (var o in Counters) { if (o.Naam == name && o.ObjectType == type) return false; }
            foreach (var o in Schakelaars) { if (o.Naam == name && o.ObjectType == type) return false; }
            foreach (var o in GeheugenElementen) { if (o.Naam == name && o.ObjectType == type) return false; }
            foreach (var o in Parameters) { if (o.Naam == name && o.ObjectType == type) return false; }
            return true;
        }

        public Generators.CCOL.CodeGeneration.CCOLElementTimeTypeEnum ConvertType(CCOLElementTypeEnum type)
        {
            switch (type)
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

            foreach (var elem in Uitgangen)
            {
                AllElements.Add(
                    new Generators.CCOL.CodeGeneration.CCOLElement(
                        elem.Naam,
                        Generators.CCOL.CodeGeneration.CCOLElementTypeEnum.Uitgang,
                        elem.Commentaar, elem.GebruikersOptieWithIO){ Dummy = elem.Dummy });
            }
            foreach (var elem in Ingangen)
            {
                AllElements.Add(
                    new Generators.CCOL.CodeGeneration.CCOLElement(
                        elem.Naam,
                        Generators.CCOL.CodeGeneration.CCOLElementTypeEnum.Ingang,
                        elem.Commentaar, elem.GebruikersOptieWithIO){ Dummy = elem.Dummy });
            }
            foreach (var elem in HulpElementen)
            {
                AllElements.Add(
                    new Generators.CCOL.CodeGeneration.CCOLElement(
                        elem.Naam,
                        Generators.CCOL.CodeGeneration.CCOLElementTypeEnum.HulpElement,
                        elem.Commentaar){ Dummy = elem.Dummy });
            }
            foreach (var elem in Timers)
            {
                AllElements.Add(
                    new Generators.CCOL.CodeGeneration.CCOLElement(
                        elem.Naam,
                        elem.Instelling.Value,
                        ConvertType(elem.Type),
                        Generators.CCOL.CodeGeneration.CCOLElementTypeEnum.Timer,
                        elem.Commentaar){ Dummy = elem.Dummy });
            }
            foreach (var elem in Counters)
            {
                AllElements.Add(
                    new Generators.CCOL.CodeGeneration.CCOLElement(
                        elem.Naam,
                        elem.Instelling.Value,
                        Generators.CCOL.CodeGeneration.CCOLElementTimeTypeEnum.None,
                        Generators.CCOL.CodeGeneration.CCOLElementTypeEnum.Counter,
                        elem.Commentaar){ Dummy = elem.Dummy });
            }
            foreach (var elem in Schakelaars)
            {
                AllElements.Add(
                    new Generators.CCOL.CodeGeneration.CCOLElement(
                        elem.Naam,
                        elem.Instelling.Value,
                        Generators.CCOL.CodeGeneration.CCOLElementTimeTypeEnum.SCH_type,
                        Generators.CCOL.CodeGeneration.CCOLElementTypeEnum.Schakelaar,
                        elem.Commentaar){ Dummy = elem.Dummy });
            }
            foreach (var elem in GeheugenElementen)
            {
                AllElements.Add(
                    new Generators.CCOL.CodeGeneration.CCOLElement(
                        elem.Naam,
                        Generators.CCOL.CodeGeneration.CCOLElementTypeEnum.GeheugenElement,
                        elem.Commentaar){ Dummy = elem.Dummy });
            }
            foreach (var elem in Parameters)
            {
                AllElements.Add(
                    new Generators.CCOL.CodeGeneration.CCOLElement(
                        elem.Naam,
                        elem.Instelling.Value,
                        ConvertType(elem.Type),
                        Generators.CCOL.CodeGeneration.CCOLElementTypeEnum.Parameter,
                        elem.Commentaar){ Dummy = elem.Dummy });
            }

            return AllElements;
        }

        public void DragOver(IDropInfo dropInfo)
        {
            GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.DragOver(dropInfo);
        }

        public void Drop(IDropInfo dropInfo)
        {
            GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.Drop(dropInfo);
            switch (dropInfo.TargetCollection)
            {
                case ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel> o1:
                    o1.RebuildList();
                    MessengerInstance.Send(new ControllerDataChangedMessage());
                    break;
                case ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel> o2:
                    o2.RebuildList();
                    MessengerInstance.Send(new ControllerDataChangedMessage());
                    break;
            }
        }

        #endregion // ITLCGenElementProvider

        #region Constructor

        public GebruikersOptiesTabViewModel()
        {
            ImportExportVM = new GebruikersOptiesImportExportTabViewModel(this);
            ResetMyGebruikersOpties();
        }

        #endregion // Constructor
    }
}
