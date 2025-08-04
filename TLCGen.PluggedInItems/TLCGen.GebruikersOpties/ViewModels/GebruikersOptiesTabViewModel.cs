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
using System.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using TLCGen.Models.Enumerations;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Messaging.Messages;
using GongSolutions.Wpf.DragDrop;

namespace TLCGen.GebruikersOpties
{
    [TLCGenTabItem(-1, TabItemTypeEnum.MainWindow)]
    [TLCGenPlugin(TLCGenPluginElems.TabControl | TLCGenPluginElems.XMLNodeWriter | TLCGenPluginElems.IOElementProvider)]
    public class GebruikersOptiesTabViewModel : ObservableObject, ITLCGenXMLNodeWriter, ITLCGenTabItem, ITLCGenElementProvider, IDropTarget
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

        readonly string[] _optiesNames = { "us", "is", "h", "t", "c", "sch", "m", "prm" };

        #endregion // Constants

        #region Fields

        private GebruikersOptiesModel _myGebruikersOpties;
        private int _selectedTabIndex;

        private object[] _selectedOptie = new object[OptiesMax];
        private IList[] _selectedOpties = new IList[OptiesMax]
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

        private RelayCommand _addGebruikersOptieCommand;
        private RelayCommand _removeGebruikersOptieCommand;
        private RelayCommand _omhoogCommand;
        private RelayCommand _omlaagCommand;

        object[] _alleOpties;
        private volatile bool _settingMultiple = false;
        private ControllerModel _controller;
        private DataTemplate _contentDataTemplate;

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
            get => _myGebruikersOpties;
            set
            {
                _myGebruikersOpties = value;
                ImportExportVM.DataModel = value;
                OnPropertyChanged("");
            }
        }

        public bool IsImportExportNotSelected => !_isImportExportSelected;

        public bool IsImportExportSelected
        {
            set
            {
                if (value) ImportExportVM.RebuildAllItems(this);
                _isImportExportSelected = value;
                OnPropertyChanged(nameof(IsImportExportNotSelected));
            }
        }

        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set
            {
                _selectedTabIndex = value;
                OnPropertyChanged(nameof(SelectedOptie));
                OnPropertyChanged();
            }
        }

        public object SelectedOptie
        {
            get
            {
                if (SelectedTabIndex >= 0 && SelectedTabIndex < OptiesMax)
                    return _selectedOptie[SelectedTabIndex];
                else
                    return null;
            }
            set
            {
                if (SelectedTabIndex >= 0 && SelectedTabIndex < OptiesMax)
                {
                    _selectedOptie[SelectedTabIndex] = value;
                }
                _removeGebruikersOptieCommand?.NotifyCanExecuteChanged();
                _omhoogCommand?.NotifyCanExecuteChanged();
                _omlaagCommand?.NotifyCanExecuteChanged();
                OnPropertyChanged();
            }
        }

        public IList SelectedOpties
        {
            get
            {
                if (SelectedTabIndex >= 0 && SelectedTabIndex < OptiesMax)
                {
                    return _selectedOpties[SelectedTabIndex];
                }
                else
                    return null;
            }
            set
            {
                if (SelectedTabIndex >= 0 && SelectedTabIndex < OptiesMax)
                {
                    _selectedOpties[SelectedTabIndex] = value;
                }
                OnPropertyChanged();
            }
        }

        #endregion // Properties

        #region Commands

        public ICommand AddGebruikersOptieCommand => _addGebruikersOptieCommand ??= new RelayCommand(AddNewGebruikersOptieCommand_Executed);
        public ICommand RemoveGebruikersOptieCommand => _removeGebruikersOptieCommand ??= new RelayCommand(RemoveGebruikersOptieCommand_Executed, RemoveGebruikersOptieCommand_CanExecute);
        public ICommand OmhoogCommand => _omhoogCommand ??= new RelayCommand(OmhoogCommand_Executed, OmhoogCommand_CanExecute);
        public ICommand OmlaagCommand => _omlaagCommand ??= new RelayCommand(OmlaagCommand_Executed, OmlaagCommand_CanExecute);

        #endregion // Commands

        #region Command functionality

        void AddNewGebruikersOptieCommand_Executed()
        {
            var index = -1;

            if (SelectedTabIndex == UitgangenConst || SelectedTabIndex == IngangenConst)
            {
                if (SelectedOptie != null)
                    index = ((ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>)_alleOpties[SelectedTabIndex]).IndexOf(
                        SelectedOptie as GebruikersOptieWithIOViewModel) + 1;

                var o = new GebruikersOptieWithIOViewModel(new GebruikersOptieWithIOModel());
                o.PropertyChanged += Optie_PropertyChanged;
                var i = 1;
                while (string.IsNullOrEmpty(o.Naam))
                {
                    o.Naam = _optiesNames[SelectedTabIndex] + "_" + (((ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>)_alleOpties[SelectedTabIndex]).Count + i);
                    ++i;
                }

                switch (_optiesNames[SelectedTabIndex])
                {
                    case "us": o.ObjectType = TLCGenObjectTypeEnum.Output; break;
                    case "is": o.ObjectType = TLCGenObjectTypeEnum.Input; break;
                }

                if (index > 0 && index < ((ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>)_alleOpties[SelectedTabIndex]).Count)
                    ((ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>)_alleOpties[SelectedTabIndex]).Insert(index, o);
                else
                    ((ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>)_alleOpties[SelectedTabIndex]).Add(o);
            }
            else
            {
                if (SelectedOptie != null)
                    index = ((ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>)_alleOpties[SelectedTabIndex]).IndexOf(
                        SelectedOptie as GebruikersOptieViewModel) + 1;

                var o = new GebruikersOptieViewModel(new GebruikersOptieModel());
                o.PropertyChanged += Optie_PropertyChanged;
                var i = 1;
                while (string.IsNullOrEmpty(o.Naam))
                {
                    o.Naam = _optiesNames[SelectedTabIndex] + "_" + (((ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>)_alleOpties[SelectedTabIndex]).Count + i);
                    ++i;
                }
                if (_optiesNames[SelectedTabIndex] == "sch" ||
                    _optiesNames[SelectedTabIndex] == "t" ||
                    _optiesNames[SelectedTabIndex] == "prm" ||
                    _optiesNames[SelectedTabIndex] == "c")
                {
                    o.Instelling = 0;
                }
                if (_optiesNames[SelectedTabIndex] == "t")
                {
                    o.Type = CCOLElementTypeEnum.TE_type;
                }
                if (_optiesNames[SelectedTabIndex] == "prm")
                {
                    o.Type = CCOLElementTypeEnum.Geen;
                }

                switch (_optiesNames[SelectedTabIndex])
                {
                    case "h": o.ObjectType = TLCGenObjectTypeEnum.CCOLHelpElement; break;
                    case "t": o.ObjectType = TLCGenObjectTypeEnum.CCOLTimer; break;
                    case "c": o.ObjectType = TLCGenObjectTypeEnum.CCOLCounter; break;
                    case "sch": o.ObjectType = TLCGenObjectTypeEnum.CCOLSchakelaar; break;
                    case "m": o.ObjectType = TLCGenObjectTypeEnum.CCOLMemoryElement; break;
                    case "prm": o.ObjectType = TLCGenObjectTypeEnum.CCOLParameter; break;
                }

                if (index > 0 && index < ((ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>)_alleOpties[SelectedTabIndex]).Count)
                    ((ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>)_alleOpties[SelectedTabIndex]).Insert(index, o);
                else
                    ((ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>)_alleOpties[SelectedTabIndex]).Add(o);
            }

            if (SelectedTabIndex == UitgangenConst || SelectedTabIndex == IngangenConst)
                ((ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>)_alleOpties[SelectedTabIndex]).RebuildList();
            else
                ((ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>)_alleOpties[SelectedTabIndex]).RebuildList();

            WeakReferenceMessengerEx.Default.Send(new Messaging.Messages.ControllerDataChangedMessage());
        }

        void RemoveGebruikersOptieCommand_Executed()
        {
            var index = 0;
            if (SelectedOpties != null && SelectedOpties.Count > 0)
            {
                if (SelectedTabIndex == UitgangenConst || SelectedTabIndex == IngangenConst)
                {
                    var list = ((ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>)_alleOpties[SelectedTabIndex]);
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
                    var list = ((ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>)_alleOpties[SelectedTabIndex]);
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
                if ((SelectedTabIndex == UitgangenConst || SelectedTabIndex == IngangenConst) &&
                    SelectedOptie is GebruikersOptieWithIOViewModel goIo)
                {
                    var list = ((ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>)_alleOpties[SelectedTabIndex]);
                    index = list.IndexOf(SelectedOptie as GebruikersOptieWithIOViewModel);
                    list.Remove(SelectedOptie as GebruikersOptieWithIOViewModel);
                    goIo.PropertyChanged -= Optie_PropertyChanged;
                }
                else if (SelectedOptie is GebruikersOptieViewModel begrO)
                {
                    var list = ((ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>)_alleOpties[SelectedTabIndex]);
                    index = list.IndexOf(SelectedOptie as GebruikersOptieViewModel);
                    list.Remove(SelectedOptie as GebruikersOptieViewModel);
                    begrO.PropertyChanged -= Optie_PropertyChanged;
                }
            }

            if (SelectedTabIndex == UitgangenConst || SelectedTabIndex == IngangenConst)
            {
                var list = ((ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>)_alleOpties[SelectedTabIndex]);
                var c = list.Count;
                if (index >= c) index = c - 1;
                if (index >= 0)
                    SelectedOptie = list[index];
                else
                    SelectedOptie = null;
            }
            else
            {
                var list = ((ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>)_alleOpties[SelectedTabIndex]);
                var c = list.Count;
                if (index >= c) index = c - 1;
                if (index >= 0)
                    SelectedOptie = list[index];
                else
                    SelectedOptie = null;
            }

            WeakReferenceMessengerEx.Default.Send(new Messaging.Messages.ControllerDataChangedMessage());
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
                var list = ((ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>)_alleOpties[SelectedTabIndex]);
                if (SelectedOpties is { Count: > 0 })
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
                var list = ((ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>)_alleOpties[SelectedTabIndex]);
                if (SelectedOpties is { Count: > 0 })
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
            WeakReferenceMessengerEx.Default.Send(new Messaging.Messages.ControllerDataChangedMessage());
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

            if (SelectedTabIndex is UitgangenConst or IngangenConst)
            {
                var list = ((ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>)_alleOpties[SelectedTabIndex]);
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
                var list = (ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>)_alleOpties[SelectedTabIndex];
                max = list.Count - 1;
                if (SelectedOpties is { Count: > 0 })
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
            WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
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
                return TLCGenIntegrityChecker.IsElementNaamUnique(_controller, oio.Naam, oio.ObjectType);
            }
            else
            {
                return TLCGenIntegrityChecker.IsElementNaamUnique(_controller, o.Naam, o.ObjectType);
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

            Uitgangen = new ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>(_myGebruikersOpties.Uitgangen);
            Ingangen = new ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>(_myGebruikersOpties.Ingangen);
            HulpElementen = new ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>(_myGebruikersOpties.HulpElementen);
            Timers = new ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>(_myGebruikersOpties.Timers);
            Counters = new ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>(_myGebruikersOpties.Counters);
            Schakelaars = new ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>(_myGebruikersOpties.Schakelaars);
            GeheugenElementen = new ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>(_myGebruikersOpties.GeheugenElementen);
            Parameters = new ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>(_myGebruikersOpties.Parameters);

            _alleOpties = new object[8];
            _alleOpties[UitgangenConst] = Uitgangen;
            _alleOpties[IngangenConst] = Ingangen;
            _alleOpties[HulpElementenConst] = HulpElementen;
            _alleOpties[TimersConst] = Timers;
            _alleOpties[CountersConst] = Counters;
            _alleOpties[SchakelaarsConst] = Schakelaars;
            _alleOpties[GeheugenElementenConst] = GeheugenElementen;
            _alleOpties[ParametersConst] = Parameters;

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
            WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
        }

        private void Ingangen_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Ingangen.RebuildList();
            WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
        }

        private void HulpElementen_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            HulpElementen.RebuildList();
            WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
        }

        private void Timers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Timers.RebuildList();
            WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
        }

        private void Counters_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Counters.RebuildList();
            WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
        }

        private void Schakelaars_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Schakelaars.RebuildList();
            WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
        }

        private void GeheugenElementen_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            GeheugenElementen.RebuildList();
            WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
        }

        private void Parameters_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Parameters.RebuildList();
            WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
        }


        #endregion // Private Methods

        #region Event Handling

        private void Optie_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_settingMultiple || string.IsNullOrEmpty(e.PropertyName))
                return;

            if (SelectedOpties != null && SelectedOpties.Count > 1)
            {
                _settingMultiple = true;
                MultiPropertySetter.SetPropertyForAllItems<GebruikersOptieViewModel>(sender, e.PropertyName, SelectedOpties);
            }
            _settingMultiple = false;
        }

        #endregion // Event Handling

        #region ITLCGenTabItem

        public ControllerModel Controller
        {
            get => _controller;
            set
            {
                _controller = value;
                if (value == null)
                {
                    ResetMyGebruikersOpties();
                }
                OnPropertyChanged("");
            }
        }

        public DataTemplate ContentDataTemplate
        {
            get
            {
                if (_contentDataTemplate == null)
                {
                    _contentDataTemplate = new DataTemplate();
                    _contentDataTemplate.VisualTree = new FrameworkElementFactory(typeof(GebruikersOptiesTabView));
                }
                return _contentDataTemplate;
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
                OnPropertyChanged("IsEnabled");
            }
        }

        public bool Visibility { get; set; } = true;

        public ImageSource Icon
        {
            get
            { var dict = new ResourceDictionary();
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
                Uitgangen = new ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>(_myGebruikersOpties.Uitgangen);
                Ingangen = new ObservableCollectionAroundList<GebruikersOptieWithIOViewModel, GebruikersOptieWithIOModel>(_myGebruikersOpties.Ingangen);
                HulpElementen = new ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>(_myGebruikersOpties.HulpElementen);
                Timers = new ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>(_myGebruikersOpties.Timers);
                Counters = new ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>(_myGebruikersOpties.Counters);
                Schakelaars = new ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>(_myGebruikersOpties.Schakelaars);
                GeheugenElementen = new ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>(_myGebruikersOpties.GeheugenElementen);
                Parameters = new ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel>(_myGebruikersOpties.Parameters);

                _alleOpties = new object[8];
                _alleOpties[UitgangenConst] = Uitgangen;
                _alleOpties[IngangenConst] = Ingangen;
                _alleOpties[HulpElementenConst] = HulpElementen;
                _alleOpties[TimersConst] = Timers;
                _alleOpties[CountersConst] = Counters;
                _alleOpties[SchakelaarsConst] = Schakelaars;
                _alleOpties[GeheugenElementenConst] = GeheugenElementen;
                _alleOpties[ParametersConst] = Parameters;

                foreach (var el in Uitgangen) el.ObjectType = TLCGenObjectTypeEnum.Output;
                foreach (var el in Ingangen) el.ObjectType = TLCGenObjectTypeEnum.Input;
                foreach (var el in HulpElementen) el.ObjectType = TLCGenObjectTypeEnum.CCOLHelpElement;
                foreach (var el in Timers) el.ObjectType = TLCGenObjectTypeEnum.CCOLTimer;
                foreach (var el in Counters) el.ObjectType = TLCGenObjectTypeEnum.CCOLCounter;
                foreach (var el in Schakelaars) el.ObjectType = TLCGenObjectTypeEnum.CCOLSchakelaar;
                foreach (var el in GeheugenElementen) el.ObjectType = TLCGenObjectTypeEnum.CCOLMemoryElement;
                foreach (var el in Parameters) el.ObjectType = TLCGenObjectTypeEnum.CCOLParameter;

                OnPropertyChanged("");
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
            if (Uitgangen.Any(o => o.Naam == name && o.ObjectType == type)) return false;
            if (Ingangen.Any(o => o.Naam == name && o.ObjectType == type)) return false;
            if (HulpElementen.Any(o => o.Naam == name && o.ObjectType == type)) return false;
            if (Timers.Any(o => o.Naam == name && o.ObjectType == type)) return false;
            if (Counters.Any(o => o.Naam == name && o.ObjectType == type)) return false;
            if (Schakelaars.Any(o => o.Naam == name && o.ObjectType == type)) return false;
            if (GeheugenElementen.Any(o => o.Naam == name && o.ObjectType == type)) return false;
            return Parameters.All(o => o.Naam != name || o.ObjectType != type);
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
                        null, null,
                        Generators.CCOL.CodeGeneration.CCOLElementTypeEnum.Uitgang,
                        elem.Commentaar, elem.GebruikersOptieWithIO){ Dummy = elem.Dummy });
            }
            foreach (var elem in Ingangen)
            {
                AllElements.Add(
                    new Generators.CCOL.CodeGeneration.CCOLElement(
                        elem.Naam,
                        null, null,
                        Generators.CCOL.CodeGeneration.CCOLElementTypeEnum.Ingang,
                        elem.Commentaar, elem.GebruikersOptieWithIO){ Dummy = elem.Dummy });
            }
            foreach (var elem in HulpElementen)
            {
                AllElements.Add(
                    new Generators.CCOL.CodeGeneration.CCOLElement(
                        elem.Naam,
                        null, null,
                        Generators.CCOL.CodeGeneration.CCOLElementTypeEnum.HulpElement,
                        elem.Commentaar){ Dummy = elem.Dummy });
            }
            foreach (var elem in Timers)
            {
                AllElements.Add(
                    new Generators.CCOL.CodeGeneration.CCOLElement(
                        elem.Naam,
                        "", "",
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
                        "", "",
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
                        "", "",
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
                        null, null,
                        Generators.CCOL.CodeGeneration.CCOLElementTypeEnum.GeheugenElement,
                        elem.Commentaar){ Dummy = elem.Dummy });
            }
            foreach (var elem in Parameters)
            {
                AllElements.Add(
                    new Generators.CCOL.CodeGeneration.CCOLElement(
                        elem.Naam,
                        "", "",
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
                    WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
                    break;
                case ObservableCollectionAroundList<GebruikersOptieViewModel, GebruikersOptieModel> o2:
                    o2.RebuildList();
                    WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
                    break;
            }
        }

        public void DragEnter(IDropInfo dropInfo)
        {
            
        }

        public void DragLeave(IDropInfo dropInfo)
        {
            
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
