using System;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GongSolutions.Wpf.DragDrop;
using TLCGen.DataAccess;
using TLCGen.Extensions;
using TLCGen.Messaging.Messages;
using TLCGen.Messaging.Requests;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    public class IOElementModelListDropTarget : IDropTarget
    {
        public void DragOver(IDropInfo dropInfo)
        {
            // Call default DragOver method, cause most stuff should work by default
            DragDrop.DefaultDropHandler.DragOver(dropInfo);
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.InsertIndex == dropInfo.DragInfo.SourceIndex)
                return;

            var changed = false;

            if (dropInfo.Data is IOElementViewModel item)
            {
                var col = (ObservableCollection<IOElementViewModel>)dropInfo.TargetCollection;
                var sourceIndex = col.IndexOf(item);
                var extra = 0;
                // drag up
                if (sourceIndex > dropInfo.InsertIndex)
                {
                    var start = dropInfo.InsertIndex;
                    while (start >= 0)
                    {
                        if (start != dropInfo.InsertIndex && start < col.Count) extra++;
                        start--;
                    }
                    var newIndex = dropInfo.InsertIndex + extra;
                    newIndex -= extra;
                    col.Move(sourceIndex, newIndex);
                    changed = true;
                }
                // drag down
                else
                {
                    var start = 0;
                    while (start <= dropInfo.InsertIndex)
                    {
                        if (start >= 0 && start < col.Count) extra++;
                        start++;
                    }
                    var newIndex = dropInfo.InsertIndex - 1 + extra;
                    newIndex -= extra;
                    if (newIndex >= col.Count) newIndex = col.Count - 1;
                    col.Move(sourceIndex, newIndex);
                    changed = true;
                }
                for (var i = 0; i < col.Count; ++i)
                {
                    col[i].RangeerIndex = i;
                }
            }
            else
            {
                var col = (ObservableCollection<IOElementViewModel>)dropInfo.TargetCollection;

                var insert = 0;
                foreach (var i in dropInfo.DragInfo.SourceItems)
                {
                    var sourceIndex = col.IndexOf(i as IOElementViewModel);
                    var extra = 0;
                    // drag up
                    if (sourceIndex > dropInfo.InsertIndex)
                    {
                        var start = dropInfo.InsertIndex;
                        while (start >= 0)
                        {
                            if (start != dropInfo.InsertIndex && start >= 0 && start < col.Count) extra++;
                            start--;
                        }
                        var newIndex = dropInfo.InsertIndex + extra + insert++;
                        newIndex -= extra;
                        col.Move(sourceIndex, newIndex);
                        changed = true;
                    }
                    // drag down
                    else
                    {
                        var start = 0;
                        while (start <= dropInfo.InsertIndex)
                        {
                            if (start >= 0 && start < col.Count) extra++;
                            start++;
                        }
                        var newIndex = dropInfo.InsertIndex - 1 + extra;
                        newIndex -= extra;
                        if (newIndex >= col.Count) newIndex = col.Count - 1;
                        col.Move(sourceIndex, newIndex);
                       changed = true;
                    }
                }
                for (var i = 0; i < col.Count; ++i)
                {
                    col[i].RangeerIndex = i;
                }
            }
            if (changed) Messenger.Default.Send(new ControllerDataChangedMessage());
        }
    }

    public class IOElementViewModel : ViewModelBase, IComparable
    {
        public IOElementViewModel(IOElementModel element)
        {
            Element = element;
        }

        public int RangeerIndex
        {
            get => Element.RangeerIndex;
            set
            {
                Element.RangeerIndex = value;
                if (SavedData != null) SavedData.RangeerIndex = value;
            }
        }

        public IOElementModel Element { get; }

        public IOElementRangeerDataModel SavedData { get; set; }

        public int CompareTo(object obj)
        {
            return RangeerIndex.CompareTo(((IOElementViewModel) obj).RangeerIndex);
        }
    }

    [TLCGenTabItem(index: 8, type: TabItemTypeEnum.SpecialsTab)]
    public class RangeerElementenTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields
        
        #endregion // Fields

        #region Properties
        
        public bool RangerenFasen
        {
            get => _Controller?.Data?.RangeerData?.RangerenFasen ?? false;
            set
            {
                _Controller.Data.RangeerData.RangerenFasen = value;
                RaisePropertyChanged<object>(broadcast: true);
                UpdateRangeerIndices(_Controller);
            }
        }

        public bool RangerenDetectoren
        {
            get => _Controller?.Data?.RangeerData?.RangerenDetectoren ?? false;
            set
            {
                _Controller.Data.RangeerData.RangerenDetectoren = value;
                RaisePropertyChanged<object>(broadcast: true);
                UpdateRangeerIndices(_Controller);
            }
        }

        public bool RangerenIngangen
        {
            get => _Controller?.Data?.RangeerData?.RangerenIngangen ?? false;
            set
            {
                _Controller.Data.RangeerData.RangerenIngangen = value;
                RaisePropertyChanged<object>(broadcast: true);
                UpdateRangeerIndices(_Controller);
            }
        }

        public bool RangerenUitgangen
        {
            get => _Controller?.Data?.RangeerData?.RangerenUitgangen ?? false;
            set
            {
                _Controller.Data.RangeerData.RangerenUitgangen = value;
                RaisePropertyChanged<object>(broadcast: true);
                UpdateRangeerIndices(_Controller);
            }
        }

        public ObservableCollection<IOElementViewModel> Fasen { get; } = new ObservableCollection<IOElementViewModel>();

        public ObservableCollection<IOElementViewModel> Detectoren { get; } = new ObservableCollection<IOElementViewModel>();

        public ObservableCollection<IOElementViewModel> Ingangen { get; } = new ObservableCollection<IOElementViewModel>();

        public ObservableCollection<IOElementViewModel> Uitgangen { get; } = new ObservableCollection<IOElementViewModel>();

        public IOElementModelListDropTarget DropTarget { get; } = new IOElementModelListDropTarget();

        #endregion // Properties

        #region Commands

        #endregion // Commands

        #region TLCGen TabItem overrides

        public override string DisplayName => "Rangeren IO";

        public override void OnSelected()
        {
            UpdateRangeerIndices(_Controller);
        }

        public override ControllerModel Controller
        {
            get => base.Controller;

            set
            {
                base.Controller = value;
                if (base.Controller != null)
                {
                }
                RaisePropertyChanged("");
            }
        }

        #endregion // TLCGen TabItem overrides

        private void UpdateRangeerIndices(ControllerModel c)
        {
            if (!RangerenFasen && !RangerenDetectoren && !RangerenIngangen && !RangerenIngangen) return;

            var elements = TLCGenControllerDataProvider.Default.CurrentGenerator.GetAllIOElements(c);
            if (elements == null) return;

            var vms = new (ObservableCollection<IOElementViewModel> items, IOElementTypeEnum type)[]
            {
                (Fasen, IOElementTypeEnum.FaseCyclus),
                (Detectoren, IOElementTypeEnum.Detector),
                (Ingangen, IOElementTypeEnum.Input),
                (Uitgangen, IOElementTypeEnum.Output)
            };
            var models = new[]
            {
                c.Data.RangeerData.RangeerFasen,
                c.Data.RangeerData.RangeerDetectoren,
                c.Data.RangeerData.RangeerIngangen,
                c.Data.RangeerData.RangeerUitgangen
            };

            for (var i = 0; i < 4; i++)
            {
                // clear and rebuild viewmodel list
                vms[i].items.Clear();
                foreach (var e in elements.Where(x => x.ElementType == vms[i].type))
                {
                    vms[i].items.Add(new IOElementViewModel(e));
                }

                // for each item, match with saved data
                foreach (var vm in vms[i].items)
                {
                    var model = models[i].FirstOrDefault(x => x.Naam == vm.Element.Naam);
                    if (model != null)
                    {
                        vm.RangeerIndex = model.RangeerIndex;
                        vm.SavedData = model;
                    }
                    else
                    {
                        var ind = models[i].Count;
                        var m = new IOElementRangeerDataModel
                        {
                            Naam = vm.Element.Naam,
                            RangeerIndex = ind,
                        };
                        models[i].Add(m);
                        vm.SavedData = m;
                    }
                }

                // clean up saved model items that are no longer present
                var remModels = models[i].Where(x => vms[i].items.All(x2 => x2.Element.Naam != x.Naam)).ToList();
                foreach (var r in remModels) models[i].Remove(r);

                // sort!
                vms[i].items.BubbleSort();
            }
        }

        private void OnPrepareForGenerationRequestReceived(PrepareForGenerationRequest obj)
        {
            UpdateRangeerIndices(obj.Controller);
        }

        #region Constructor

        public RangeerElementenTabViewModel()
        {
            MessengerInstance.Register<PrepareForGenerationRequest>(this, OnPrepareForGenerationRequestReceived);
        }

        #endregion // Constructor
    }
}
