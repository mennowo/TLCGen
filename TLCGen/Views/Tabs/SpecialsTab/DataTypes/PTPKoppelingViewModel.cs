using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using TLCGen.Messaging.Messages;
using TLCGen.ModelManagement;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class KoppelSignaalViewModel : ViewModelBase
    {
        public KoppelSignaalModel KoppelSignaal { get; }

        public string Description => KoppelSignaal.Description;
        public KoppelSignaalRichtingEnum Richting => KoppelSignaal.Richting;
        public int Id { get; set; }
        public int Count
        {
            get => KoppelSignaal.Count;
            set
            {
                KoppelSignaal.Count = value;
                RaisePropertyChanged("");
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public KoppelSignaalViewModel(KoppelSignaalModel koppelSignaal, int id)
        {
            Id = id;
            KoppelSignaal = koppelSignaal;
        }
    }

    public class PTPKoppelSignaalViewModel : ViewModelBase
    {
        private KoppelSignaalViewModel _koppelSignaal;

        public int Count { get; set; }

        public string Description => _koppelSignaal?.Description;

        public KoppelSignaalViewModel KoppelSignaal
        {
            get => _koppelSignaal;
            set
            {
                _koppelSignaal = value;
                if (_koppelSignaal != null && _koppelSignaal.Count != Count)
                {
                    _koppelSignaal.Count = Count;
                    RaisePropertyChanged<object>(broadcast: true);
                }
                RaisePropertyChanged("");
            }
        }
    }


    public class PTPKoppelingViewModel : ViewModelBase, IDropTarget
    {
        #region IDropTarget

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            // Van PTP lijst naar zichzelf
            if ((dropInfo.Data is List<PTPKoppelSignaalViewModel> ||
                 dropInfo.Data is PTPKoppelSignaalViewModel) && 
                dropInfo.TargetCollection is ObservableCollection<PTPKoppelSignaalViewModel>)
            {
                dropInfo.Effects = System.Windows.DragDropEffects.Move;
                DragDrop.DefaultDropHandler.DragOver(dropInfo);
            }
            // Van PTP lijst terug naar beschikbaar
            else if ((dropInfo.Data is List<PTPKoppelSignaalViewModel> ||
                dropInfo.Data is PTPKoppelSignaalViewModel) && 
                dropInfo.TargetCollection is ObservableCollection<KoppelSignaalViewModel>)
            {
                dropInfo.Effects = System.Windows.DragDropEffects.Move;
                DragDrop.DefaultDropHandler.DragOver(dropInfo);
            }
            // Van beschikbaar naar PTP lijst
            else if ((dropInfo.Data is List<KoppelSignaalViewModel> ||
                dropInfo.Data is KoppelSignaalViewModel) && 
                dropInfo.TargetCollection is ObservableCollection<PTPKoppelSignaalViewModel>)
            {
                dropInfo.Effects = System.Windows.DragDropEffects.Move;
                DragDrop.DefaultDropHandler.DragOver(dropInfo);
            }
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            // drag single item from assigned items list to somewhere else on the list
            if (dropInfo.Data is PTPKoppelSignaalViewModel sourcePTPKopItem &&
                dropInfo.TargetCollection is ObservableCollection<PTPKoppelSignaalViewModel> tc1)
            {
                var i = tc1.IndexOf(dropInfo.TargetItem as PTPKoppelSignaalViewModel);
                if (i == -1) i = dropInfo.InsertIndex;
                var ti = tc1[i];
                var oks = ti.KoppelSignaal;
                ti.KoppelSignaal = sourcePTPKopItem.KoppelSignaal;
                if (ti.KoppelSignaal != null) ti.KoppelSignaal.Count = ti.Count;
                if (oks != null)
                {
                    oks.Count = sourcePTPKopItem.Count;
                    sourcePTPKopItem.KoppelSignaal = oks;
                }
                else
                {
                    sourcePTPKopItem.KoppelSignaal = null;
                }
            }
            // drag multiple items from assigned items list to somewhere else on the list
            else if (dropInfo.Data is List<PTPKoppelSignaalViewModel> sourcePTPKopItems && 
                dropInfo.TargetCollection is ObservableCollection<PTPKoppelSignaalViewModel> tc2)
            {
                var i = tc2.IndexOf(dropInfo.TargetItem as PTPKoppelSignaalViewModel);
                if (i == -1) i = dropInfo.InsertIndex;
                var startI = i;
                var moving = new Dictionary<int, KoppelSignaalViewModel>();
                var handled = new List<KoppelSignaalViewModel>();
                // check each signal in the list of signals being moved
                foreach (var s in sourcePTPKopItems.OrderBy(x => x.Count))
                {
                    if (i < 0 || i >= tc2.Count) break;
                    
                    // skip if this item has been handled 
                    if (handled.Any(x => ReferenceEquals(x, s.KoppelSignaal))) continue;

                    var targetItem = tc2[i];
                    ++i;
                    // remove existing items that are currently in this place, unless they are being moved also
                    if (targetItem.KoppelSignaal != null && !sourcePTPKopItems.Any(x => x.KoppelSignaal != null && ReferenceEquals(x.KoppelSignaal, targetItem.KoppelSignaal)))
                    {
                        targetItem.KoppelSignaal.Count = 0;
                        switch (targetItem.KoppelSignaal.Richting)
                        {
                            case KoppelSignaalRichtingEnum.In:
                                KoppelSignalenInBeschikbaar.Add(targetItem.KoppelSignaal);
                                break;
                            case KoppelSignaalRichtingEnum.Uit:
                                KoppelSignalenUitBeschikbaar.Add(targetItem.KoppelSignaal);
                                break;
                        }
                    }
                    // If the target item is also being moved, store it
                    if (sourcePTPKopItems.Any(x => x.KoppelSignaal != null && ReferenceEquals(x.KoppelSignaal, targetItem.KoppelSignaal)))
                    {
                        var movingItem = sourcePTPKopItems.First(x => x.KoppelSignaal != null && ReferenceEquals(x.KoppelSignaal, targetItem.KoppelSignaal));
                        moving.Add(startI + sourcePTPKopItems.IndexOf(movingItem), movingItem.KoppelSignaal);
                        handled.Add(movingItem.KoppelSignaal);
                        targetItem.KoppelSignaal = null;
                    }
                    targetItem.KoppelSignaal = s.KoppelSignaal;
                    handled.Add(targetItem.KoppelSignaal);
                    if (targetItem.KoppelSignaal != null) targetItem.KoppelSignaal.Count = targetItem.Count;
                    s.KoppelSignaal = null;
                }
                foreach(var m in moving)
                {
                    if (m.Key >= tc2.Count) continue;
                    // remove existing items that are currently in this place, unless they are being moved also
                    if (tc2[m.Key].KoppelSignaal != null)
                    {
                        tc2[m.Key].KoppelSignaal.Count = 0;
                        switch (tc2[m.Key].KoppelSignaal.Richting)
                        {
                            case KoppelSignaalRichtingEnum.In:
                                KoppelSignalenInBeschikbaar.Add(tc2[m.Key].KoppelSignaal);
                                break;
                            case KoppelSignaalRichtingEnum.Uit:
                                KoppelSignalenUitBeschikbaar.Add(tc2[m.Key].KoppelSignaal);
                                break;
                        }
                    }
                    tc2[m.Key].KoppelSignaal = m.Value;
                    if (tc2[m.Key].KoppelSignaal != null) tc2[m.Key].KoppelSignaal.Count = tc2[m.Key].Count;
                }
            }
            // drag single item or multiple items from non-assigned items list to somewhere else on the assigned items list
            else if ((dropInfo.Data is List<KoppelSignaalViewModel> || dropInfo.Data is KoppelSignaalViewModel ) && 
                dropInfo.TargetCollection is ObservableCollection<PTPKoppelSignaalViewModel> tc3)
            {
                var ksSourceItems = dropInfo.Data as List<KoppelSignaalViewModel>;
                if (dropInfo.Data is KoppelSignaalViewModel ksSourceItem) ksSourceItems = new List<KoppelSignaalViewModel> { ksSourceItem };
                var i = tc3.IndexOf(dropInfo.TargetItem as PTPKoppelSignaalViewModel);
                if (i == -1) i = dropInfo.InsertIndex;
                foreach (var s in ksSourceItems.OrderBy(x => x.Id))
                {
                    if (s == null || s.KoppelSignaal == null) continue;
                    if (i < 0 || i >= tc3.Count) break;
                    var ti = tc3[i];
                    ++i;
                    if (ti.KoppelSignaal != null)
                    {
                        ti.KoppelSignaal.Count = 0;
                        switch (ti.KoppelSignaal.Richting)
                        {
                            case KoppelSignaalRichtingEnum.In:
                                KoppelSignalenInBeschikbaar.Add(ti.KoppelSignaal);
                                break;
                            case KoppelSignaalRichtingEnum.Uit:
                                KoppelSignalenUitBeschikbaar.Add(ti.KoppelSignaal);
                                break;
                        }
                    }
                    ti.KoppelSignaal = s;
                    ti.KoppelSignaal.Count = ti.Count;
                    switch (ti.KoppelSignaal.Richting)
                    {
                        case KoppelSignaalRichtingEnum.In:
                            KoppelSignalenInBeschikbaar.Remove(s);
                            break;
                        case KoppelSignaalRichtingEnum.Uit:
                            KoppelSignalenUitBeschikbaar.Remove(s);
                            break;
                    }
                }
            }
            // drag single or more items from assigned to non-assigned
            else if ((dropInfo.Data is List<PTPKoppelSignaalViewModel> || dropInfo.Data is PTPKoppelSignaalViewModel) &&
               dropInfo.TargetCollection is ObservableCollection<KoppelSignaalViewModel> tc4)
            {
                var ksSourceItems = dropInfo.Data as List<PTPKoppelSignaalViewModel>;
                if (dropInfo.Data is PTPKoppelSignaalViewModel ksSourceItem) ksSourceItems = new List<PTPKoppelSignaalViewModel> { ksSourceItem };
                foreach (var s in ksSourceItems)
                {
                    if (s == null || s.KoppelSignaal == null) continue;
                    s.KoppelSignaal.Count = 0;
                    switch (s.KoppelSignaal.Richting)
                    {
                        case KoppelSignaalRichtingEnum.In:
                            KoppelSignalenInBeschikbaar.Add(s.KoppelSignaal);
                            break;
                        case KoppelSignaalRichtingEnum.Uit:
                            KoppelSignalenUitBeschikbaar.Add(s.KoppelSignaal);
                            break;
                    }
                    s.KoppelSignaal = null;
                }
            }
        }

        #endregion

        #region Fields

        private PTPKoppelingModel _PTPKoppeling;
        private PTPKoppelSignaalViewModel _selectedKoppelSignaalIn;
        private PTPKoppelSignaalViewModel _selectedKoppelSignaalUit;
        private RelayCommand _removeKoppelSignaalInCommand;
        private RelayCommand _removeKoppelSignaalUitCommand;

        #endregion // Fields

        #region Properties

        public ICommand RemoveKoppelSignaalInCommand => _removeKoppelSignaalInCommand ?? (_removeKoppelSignaalInCommand = new RelayCommand(
            () =>
            {
                SelectedKoppelSignaalIn.KoppelSignaal.Count = 0;
                KoppelSignalenInBeschikbaar.Add(SelectedKoppelSignaalIn.KoppelSignaal);
                SelectedKoppelSignaalIn.KoppelSignaal = null;
            },
            () => SelectedKoppelSignaalIn != null));

        public ICommand RemoveKoppelSignaalUitCommand => _removeKoppelSignaalUitCommand ?? (_removeKoppelSignaalUitCommand = new RelayCommand(
            () =>
            {
                SelectedKoppelSignaalUit.KoppelSignaal.Count = 0;
                KoppelSignalenUitBeschikbaar.Add(SelectedKoppelSignaalUit.KoppelSignaal);
                SelectedKoppelSignaalUit.KoppelSignaal = null;
            },
            () => SelectedKoppelSignaalUit != null));

        public PTPKoppelingModel PTPKoppeling => _PTPKoppeling;

        public string TeKoppelenKruispunt
        {
            get => _PTPKoppeling.TeKoppelenKruispunt;
            set
            {
                if (TLCGenModelManager.Default.IsElementIdentifierUnique(TLCGenObjectTypeEnum.PTPKruising, value))
                {
                    var oldname = _PTPKoppeling.TeKoppelenKruispunt;
                    _PTPKoppeling.TeKoppelenKruispunt = value;
                    RaisePropertyChanged<object>(nameof(TeKoppelenKruispunt), broadcast: true);
                    Messenger.Default.Send(new NameChangingMessage(TLCGenObjectTypeEnum.PTPKruising, oldname, value));
                }
            }
        }

        public List<KoppelSignaalViewModel> KoppelSignalenAlles { get; } = new List<KoppelSignaalViewModel>();
        public ObservableCollection<KoppelSignaalViewModel> KoppelSignalenInBeschikbaar { get; } = new ObservableCollection<KoppelSignaalViewModel>();
        public ObservableCollection<KoppelSignaalViewModel> KoppelSignalenUitBeschikbaar { get; } = new ObservableCollection<KoppelSignaalViewModel>();
        public ObservableCollection<PTPKoppelSignaalViewModel> KoppelSignalenIn { get; } = new ObservableCollection<PTPKoppelSignaalViewModel>();
        public ObservableCollection<PTPKoppelSignaalViewModel> KoppelSignalenUit { get; } = new ObservableCollection<PTPKoppelSignaalViewModel>();

        public PTPKoppelSignaalViewModel SelectedKoppelSignaalIn
        {
            get => _selectedKoppelSignaalIn;
            set
            {
                _selectedKoppelSignaalIn = value;
                RaisePropertyChanged();
            }
        }

        public PTPKoppelSignaalViewModel SelectedKoppelSignaalUit
        {
            get => _selectedKoppelSignaalUit;
            set
            {
                _selectedKoppelSignaalUit = value;
                RaisePropertyChanged();
            }
        }

        public int AantalsignalenIn
        {
            get => _PTPKoppeling.AantalsignalenIn;
            set
            {
                _PTPKoppeling.AantalsignalenIn = value;
                RaisePropertyChanged<object>(nameof(AantalsignalenIn), broadcast: true);
                UpdateSignalen();

            }
        }

        public int AantalsignalenUit
        {
            get => _PTPKoppeling.AantalsignalenUit;
            set
            {
                _PTPKoppeling.AantalsignalenUit = value;
                RaisePropertyChanged<object>(nameof(AantalsignalenUit), broadcast: true);
                UpdateSignalen();
            }
        }

        public void UpdateSignalen()
        {
            KoppelSignalenInBeschikbaar.Clear();
            KoppelSignalenUitBeschikbaar.Clear();
            KoppelSignalenIn.Clear();
            KoppelSignalenUit.Clear();
            for (var i = 0; i < AantalsignalenIn; i++)
            {
                KoppelSignalenIn.Add(new PTPKoppelSignaalViewModel { Count = i + 1 });
            }
            for (var i = 0; i < AantalsignalenUit; i++)
            {
                KoppelSignalenUit.Add(new PTPKoppelSignaalViewModel { Count = i + 1 });
            }
            foreach (var s in KoppelSignalenAlles)
            {
                switch (s.Richting)
                {
                    case KoppelSignaalRichtingEnum.In:
                        if (s.Count != 0 && KoppelSignalenIn.Count >= s.Count && KoppelSignalenIn[s.Count - 1].KoppelSignaal == null)
                        {
                            KoppelSignalenIn[s.Count - 1].KoppelSignaal = s;
                        }
                        else
                        {
                            KoppelSignalenInBeschikbaar.Add(s);
                        }
                        break;
                    case KoppelSignaalRichtingEnum.Uit:
                        if (s.Count != 0 && KoppelSignalenUit.Count >= s.Count && KoppelSignalenUit[s.Count - 1].KoppelSignaal == null)
                        {
                            KoppelSignalenUit[s.Count - 1].KoppelSignaal = s;
                        }
                        else
                        {
                            KoppelSignalenUitBeschikbaar.Add(s);
                        }
                        break;
                }
            }
        }

        public int PortnummerSimuatieOmgeving
        {
            get => _PTPKoppeling.PortnummerSimuatieOmgeving;
            set
            {
                _PTPKoppeling.PortnummerSimuatieOmgeving = value;
                RaisePropertyChanged<object>(nameof(PortnummerSimuatieOmgeving), broadcast: true);
            }
        }

        public int PortnummerAutomaatOmgeving
        {
            get => _PTPKoppeling.PortnummerAutomaatOmgeving;
            set
            {
                _PTPKoppeling.PortnummerAutomaatOmgeving = value;
                RaisePropertyChanged<object>(nameof(PortnummerAutomaatOmgeving), broadcast: true);
            }
        }

        public int NummerSource
        {
            get => _PTPKoppeling.NummerSource;
            set
            {
                _PTPKoppeling.NummerSource = value;
                RaisePropertyChanged<object>(nameof(NummerSource), broadcast: true);
            }
        }

        public int NummerDestination
        {
            get => _PTPKoppeling.NummerDestination;
            set
            {
                _PTPKoppeling.NummerDestination = value;
                RaisePropertyChanged<object>(nameof(NummerDestination), broadcast: true);
            }
        }

        #endregion // Properties

        public int KoppelSignalenAllesId { get; set; }

        #region Constructor

        public PTPKoppelingViewModel(PTPKoppelingModel kop)
        {
            _PTPKoppeling = kop;
            var ksIn = CollectionViewSource.GetDefaultView(KoppelSignalenInBeschikbaar);
            if (ksIn != null && ksIn.CanSort == true)
            {
                ksIn.SortDescriptions.Clear();
                ksIn.SortDescriptions.Add(new SortDescription("Id", ListSortDirection.Ascending));
            }
            var ksUit = CollectionViewSource.GetDefaultView(KoppelSignalenUitBeschikbaar);
            if (ksUit != null && ksUit.CanSort == true)
            {
                ksUit.SortDescriptions.Clear();
                ksUit.SortDescriptions.Add(new SortDescription("Id", ListSortDirection.Ascending));
            }
        }

        #endregion // Constructor
    }
}
