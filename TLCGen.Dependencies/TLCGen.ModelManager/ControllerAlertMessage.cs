using System.Windows.Media;
using GalaSoft.MvvmLight;

namespace TLCGen.Models
{
    public enum ControllerAlertType
    {
        WachttijdVoorspeller,
        RealFunc,
        RangerenOldNew,
        TglMinChanged,
        FromPlugin
    }

    public class ControllerAlertMessage : ViewModelBase
    {
        #region Fields

        private bool _shown;

        #endregion // Fields
        

        #region Properties

        public string Id { get; }
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

        #endregion // Properties
        
        #region Constructor

        public ControllerAlertMessage(string id)
        {
            Id = id;
        }

        #endregion // Constructor
    }
}
