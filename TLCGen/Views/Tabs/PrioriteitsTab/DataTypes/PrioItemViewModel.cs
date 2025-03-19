using System.ComponentModel;
using TLCGen.Helpers;

namespace TLCGen.ViewModels
{
    public class PrioItemViewModel : ObservableObjectEx
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
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }
    }
}