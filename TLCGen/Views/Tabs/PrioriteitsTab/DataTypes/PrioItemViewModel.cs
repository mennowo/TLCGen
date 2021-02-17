using System.ComponentModel;
using GalaSoft.MvvmLight;

namespace TLCGen.ViewModels
{
    public class PrioItemViewModel : ViewModelBase
    {
        private bool _isExpanded;
        private bool _isSelected;

        [Browsable(false)]
        public virtual bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                _isExpanded = value; 
                RaisePropertyChanged();
            }
        }

        [Browsable(false)]
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                if (value)
                {
                    if (!_isExpanded)
                    {
                        IsExpanded = true;
                    }
                }
                RaisePropertyChanged();
            }
        }
    }
}