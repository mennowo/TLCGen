using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GongSolutions.Wpf.DragDrop;
using TLCGen.DataAccess;
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
                    }
                }
                for (var i = 0; i < col.Count; ++i)
                {
                    col[i].RangeerIndex = i;
                }
            }
        }

        public IOElementModelListDropTarget()
        {
        }
    }

    public class IOElementViewModel : ViewModelBase
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
            }
        }

        public IOElementModel Element { get; }
    }

    [TLCGenTabItem(index: 8, type: TabItemTypeEnum.SpecialsTab)]
    public class RangeerElementenTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields
        
        #endregion // Fields

        #region Properties
        
        public ObservableCollection<IOElementViewModel> Fasen { get; } = new ObservableCollection<IOElementViewModel>();

        public IOElementModelListDropTarget DropTarget { get; } = new IOElementModelListDropTarget();

        #endregion // Properties

        #region Commands

        #endregion // Commands

        #region TLCGen TabItem overrides

        public override string DisplayName => "Rangeren IO";

        public override void OnSelected()
        {
            var elements = TLCGenControllerDataProvider.Default.CurrentGenerator.GetAllIOElements(_Controller);
            if (elements == null) return;
            
            Fasen.Clear();
            foreach (var e in elements.Where(x => x.ElementType == IOElementTypeEnum.FaseCyclus).OrderBy(x => x.RangeerIndex))
            {
                Fasen.Add(new IOElementViewModel(e));
            }
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

        private void OnPrepareForGenerationRequestReceived(PrepareForGenerationRequest obj)
        {
            
        }

        #region Constructor

        public RangeerElementenTabViewModel()
        {
            MessengerInstance.Register<PrepareForGenerationRequest>(this, OnPrepareForGenerationRequestReceived);
        }

        #endregion // Constructor
    }
}
