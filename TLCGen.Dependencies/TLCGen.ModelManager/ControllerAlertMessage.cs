using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;

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

    public class ControllerAlertMessage : ObservableObject
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
                OnPropertyChanged();
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
