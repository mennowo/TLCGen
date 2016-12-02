using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.ViewModels
{
    public class GroentijdFaseViewModel : ViewModelBase
    {
        #region Properties

        private string _Define;
        public string Define
        {
            get { return _Define; }
            set
            {
                _Define = value;
                OnPropertyChanged("Define");
            }
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                OnPropertyChanged("Name");
            }
        }

        #endregion // Properties
    }
}
