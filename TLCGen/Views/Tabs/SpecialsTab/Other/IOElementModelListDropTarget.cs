using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Messaging;
using GongSolutions.Wpf.DragDrop;
using TLCGen.Messaging.Messages;

namespace TLCGen.ViewModels
{
    public class IOElementModelListDropTarget : IDropTarget
    {
        public void DragEnter(IDropInfo dropInfo)
        {
            throw new System.NotImplementedException();
        }

        public void DragLeave(IDropInfo dropInfo)
        {
            throw new System.NotImplementedException();
        }

        public void DragOver(IDropInfo dropInfo)
        {
            // Call default DragOver method, cause most stuff should work by default
            DragDrop.DefaultDropHandler.DragOver(dropInfo);
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.InsertIndex == dropInfo.DragInfo.SourceIndex ||
                !ReferenceEquals(dropInfo.DragInfo.SourceCollection, dropInfo.TargetCollection))
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
}