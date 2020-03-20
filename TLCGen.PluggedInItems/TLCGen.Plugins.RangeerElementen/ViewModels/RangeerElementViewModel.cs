using System;
using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Integrity;
using TLCGen.Models;
using TLCGen.Plugins.RangeerElementen.Models;

namespace TLCGen.Plugins.RangeerElementen.ViewModels
{
    public class RangeerElementViewModel : ViewModelBase, IViewModelWithItem, IComparable
    {
        private readonly RangeerElementModel _element;

        public string Element
        {

            get => _element.Element;
            set
            {
                _element.Element = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public object GetItem()
        {
            return _element;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is RangeerElementViewModel relem)) throw new InvalidCastException();
            return TLCGenIntegrityChecker.CompareDetectors(Element, relem.Element, null, null);
        }

        public RangeerElementViewModel(RangeerElementModel element)
        {
            _element = element;
        }
    }
}
