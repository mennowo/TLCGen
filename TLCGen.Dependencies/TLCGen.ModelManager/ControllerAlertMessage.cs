using System.Windows.Media;
using GalaSoft.MvvmLight;

namespace TLCGen.Models
{
    public enum ControllerAlertType
    {
        WachttijdVoorspeller
    }

    public class ControllerAlertMessage : ViewModelBase
    {
        private bool _shown;
        public ControllerAlertType Type { get; set; }
        public string Message { get; set; }
        public Brush Background { get; set; }

        public bool Shown
        {
            get => _shown;
            set
            {
                _shown = value; 
                RaisePropertyChanged();
            }
        }
    }
}
