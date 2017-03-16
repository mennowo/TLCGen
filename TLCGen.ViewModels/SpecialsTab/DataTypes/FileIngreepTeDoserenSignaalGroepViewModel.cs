using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
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
                OnPropertyChanged("FaseCyclus");
            }
        }

        public int DoseerPercentage
        {
            get { return _TeDoserenSignaalGroep.DoseerPercentage; }
            set
            {
                _TeDoserenSignaalGroep.DoseerPercentage = value;
                OnMonitoredPropertyChanged("DoseerPercentage");
                Messenger.Default.Send(new FileIngreepTeDoserenSignaalGroepPercentageChangedMessage(_TeDoserenSignaalGroep));
            }
        }

        public int DoseerPercentageNoMessaging
        {
            set
            {
                _TeDoserenSignaalGroep.DoseerPercentage = value;
                OnMonitoredPropertyChanged("DoseerPercentage");
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

        public FileIngreepTeDoserenSignaalGroepViewModel(FileIngreepTeDoserenSignaalGroepModel tedoserensg)
        {
            _TeDoserenSignaalGroep = tedoserensg;
        }

        #endregion // Constructor
    }
}
