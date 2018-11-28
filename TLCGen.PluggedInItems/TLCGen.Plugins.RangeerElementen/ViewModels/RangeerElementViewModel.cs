using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Plugins.RangeerElementen.Models;

namespace TLCGen.Plugins.RangeerElementen.ViewModels
{
    public class RangeerElementViewModel : ViewModelBase, IViewModelWithItem
    {
        private RangeerElementModel _element;

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

        public RangeerElementViewModel(RangeerElementModel element)
        {
            _element = element;
        }
    }
}
