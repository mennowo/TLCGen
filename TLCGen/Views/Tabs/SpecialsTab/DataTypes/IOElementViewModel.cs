using System;
using GalaSoft.MvvmLight;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
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
}