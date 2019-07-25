using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using GongSolutions.Wpf.DragDrop;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TLCGen.Messaging.Messages;
using TLCGen.ModelManagement;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class KoppelSignaalViewModel : ViewModelBase
    {
        private KoppelSignaalModel KoppelSignaal { get; }

        public string Description => KoppelSignaal.Description;
        public KoppelSignaalRichtingEnum Richting => KoppelSignaal.Richting;
        public int Count
        {
            get => KoppelSignaal.Count;
            set
            {
                KoppelSignaal.Count = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public KoppelSignaalViewModel(KoppelSignaalModel koppelSignaal)
        {
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
                if(_koppelSignaal != null) _koppelSignaal.Count = Count;
                RaisePropertyChanged("");
            }
        }
    }


    public class PTPKoppelingViewModel : ViewModelBase, IDropTarget
    {
        #region IDropTarget

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            //KoppelSignaalModel sourceItem = dropInfo.Data as KoppelSignaalModel;
            if (dropInfo.Data is KoppelSignaalViewModel ks)
            {
                dropInfo.Effects = System.Windows.DragDropEffects.Move;
                GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.DragOver(dropInfo);
            }
            else if (dropInfo.Data is PTPKoppelSignaalViewModel && dropInfo.TargetItem is PTPKoppelSignaalViewModel)
            {
                dropInfo.Effects = System.Windows.DragDropEffects.Move;
                dropInfo.DropTargetAdorner = null;
                GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.DragOver(dropInfo);
            }
            else if (!(dropInfo.TargetItem is PTPKoppelSignaalViewModel))
            {
                dropInfo.NotHandled = true;
            }
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            KoppelSignaalViewModel sourceItem = dropInfo.Data as KoppelSignaalViewModel;
            PTPKoppelSignaalViewModel sourceItem2 = dropInfo.Data as PTPKoppelSignaalViewModel;
            PTPKoppelSignaalViewModel targetItem2 = dropInfo.TargetItem as PTPKoppelSignaalViewModel;
            if (sourceItem == null && sourceItem2 == null || targetItem2 == null) return;
            if (sourceItem != null)
            {
                if (targetItem2.KoppelSignaal != null)
                {
                    targetItem2.KoppelSignaal.Count = 0;
                    switch (targetItem2.KoppelSignaal.Richting)
                    {
                        case KoppelSignaalRichtingEnum.In:
                            KoppelSignalenInBeschikbaar.Add(targetItem2.KoppelSignaal);
                            break;
                        case KoppelSignaalRichtingEnum.Uit:
                            KoppelSignalenUitBeschikbaar.Add(targetItem2.KoppelSignaal);
                            break;
                    }
                }
                targetItem2.KoppelSignaal = sourceItem;
                targetItem2.KoppelSignaal.Count = targetItem2.Count;
                switch (targetItem2.KoppelSignaal.Richting)
                {
                    case KoppelSignaalRichtingEnum.In:
                        KoppelSignalenInBeschikbaar.Remove(sourceItem);
                        break;
                    case KoppelSignaalRichtingEnum.Uit:
                        KoppelSignalenUitBeschikbaar.Remove(sourceItem);
                        break;
                }
            }
            if (sourceItem2 != null)
            {
                if (targetItem2.KoppelSignaal != null)
                {
                    targetItem2.KoppelSignaal.Count = 0;
                    switch (targetItem2.KoppelSignaal.Richting)
                    {
                        case KoppelSignaalRichtingEnum.In:
                            KoppelSignalenInBeschikbaar.Add(targetItem2.KoppelSignaal);
                            break;
                        case KoppelSignaalRichtingEnum.Uit:
                            KoppelSignalenUitBeschikbaar.Add(targetItem2.KoppelSignaal);
                            break;
                    }
                }
                targetItem2.KoppelSignaal = sourceItem2.KoppelSignaal;
                targetItem2.KoppelSignaal.Count = targetItem2.Count;
                sourceItem2.KoppelSignaal = null;
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

        public PTPKoppelingModel PTPKoppeling
        {
            get { return _PTPKoppeling; }
        }

        public string TeKoppelenKruispunt
        {
            get { return _PTPKoppeling.TeKoppelenKruispunt; }
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
            get { return _PTPKoppeling.AantalsignalenIn; }
            set
            {
                _PTPKoppeling.AantalsignalenIn = value;
                RaisePropertyChanged<object>(nameof(AantalsignalenIn), broadcast: true);
                UpdateSignalen();

            }
        }

        public int AantalsignalenUit
        {
            get { return _PTPKoppeling.AantalsignalenUit; }
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
            for (int i = 0; i < AantalsignalenIn; i++)
            {
                KoppelSignalenIn.Add(new PTPKoppelSignaalViewModel { Count = i + 1 });
            }
            for (int i = 0; i < AantalsignalenUit; i++)
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
            get { return _PTPKoppeling.PortnummerSimuatieOmgeving; }
            set
            {
                _PTPKoppeling.PortnummerSimuatieOmgeving = value;
                RaisePropertyChanged<object>(nameof(PortnummerSimuatieOmgeving), broadcast: true);
            }
        }

        public int PortnummerAutomaatOmgeving
        {
            get { return _PTPKoppeling.PortnummerAutomaatOmgeving; }
            set
            {
                _PTPKoppeling.PortnummerAutomaatOmgeving = value;
                RaisePropertyChanged<object>("PortnummerAutomaatOmgeving", broadcast: true);
            }
        }

        public int NummerSource
        {
            get { return _PTPKoppeling.NummerSource; }
            set
            {
                _PTPKoppeling.NummerSource = value;
                RaisePropertyChanged<object>("NummerSource", broadcast: true);
            }
        }

        public int NummerDestination
        {
            get { return _PTPKoppeling.NummerDestination; }
            set
            {
                _PTPKoppeling.NummerDestination = value;
                RaisePropertyChanged<object>("NummerDestination", broadcast: true);
            }
        }

        #endregion // Properties

        #region Constructor

        public PTPKoppelingViewModel(PTPKoppelingModel kop)
        {
            _PTPKoppeling = kop;
        }

        #endregion // Constructor
    }
}
