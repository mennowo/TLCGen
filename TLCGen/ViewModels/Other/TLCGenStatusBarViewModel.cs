using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace TLCGen.ViewModels
{
    /// <summary>
    /// ViewModel for the statusbar of the application.
    /// The statusbar is held in a seperate View.
    /// </summary>
    public class TLCGenStatusBarViewModel : ViewModelBase
    {
        #region Fields

        private string _StatusText;

        #endregion // Fields

        #region Properties

        public string StatusText
        {
            get { return _StatusText; }
            set
            {
                _StatusText = value;
                RaisePropertyChanged("StatusText");
            }
        }

        #endregion // Properties
    }
}
