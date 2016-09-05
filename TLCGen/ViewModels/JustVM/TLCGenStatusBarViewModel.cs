using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.ViewModels
{
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
                OnPropertyChanged("StatusText");
            }
        }

        #endregion // Properties
    }
}
