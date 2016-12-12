using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class FileIngreepTeDoserenSignaalGroepViewModel : ViewModelBase, IViewModelWithItem
    {
        #region Fields

        private FileIngreepTeDoserenSignaalGroepModel _TeDoserenSignaalGroep;

        #endregion // Fields

        #region Properties

        public string FaseCyclus
        {
            get { return _TeDoserenSignaalGroep.FaseCyclus; }
            set
            {
                _TeDoserenSignaalGroep.FaseCyclus = value;
                OnPropertyChanged("Detector");
            }
        }

        public int DoseerPercentage
        {
            get { return _TeDoserenSignaalGroep.DoseerPercentage; }
            set
            {
                _TeDoserenSignaalGroep.DoseerPercentage = value;
                OnPropertyChanged("Detector");
            }
        }

        #endregion // Properties

        #region Commands

        #endregion // Commands

        #region Command functionality

        #endregion // Command functionality

        #region Private methods

        #endregion // Private methods

        #region Public methods

        #endregion // Public methods

        #region IViewModelWithItem

        public object GetItem()
        {
            return _TeDoserenSignaalGroep;
        }

        #endregion // IViewModelWithItem

        #region Constructor

        FileIngreepTeDoserenSignaalGroepViewModel(FileIngreepTeDoserenSignaalGroepModel tedoserensg)
        {
            _TeDoserenSignaalGroep = tedoserensg;
        }

        #endregion // Constructor
    }
}
