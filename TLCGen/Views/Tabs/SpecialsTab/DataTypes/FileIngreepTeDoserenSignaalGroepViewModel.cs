using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight;
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
            get => _TeDoserenSignaalGroep.FaseCyclus;
            set
            {
                _TeDoserenSignaalGroep.FaseCyclus = value;
                RaisePropertyChanged();
            }
        }

        public int DoseerPercentage
        {
            get => _TeDoserenSignaalGroep.DoseerPercentage;
            set
            {
                _TeDoserenSignaalGroep.DoseerPercentage = value;
                RaisePropertyChanged<object>(broadcast: true);
                Messenger.Default.Send(new FileIngreepTeDoserenSignaalGroepPercentageChangedMessage(_TeDoserenSignaalGroep));
            }
        }

        public int DoseerPercentageNoMessaging
        {
            set
            {
                _TeDoserenSignaalGroep.DoseerPercentage = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public bool AfkappenOpStartFile
        {
            get => _TeDoserenSignaalGroep.AfkappenOpStartFile;
            set
            {
                _TeDoserenSignaalGroep.AfkappenOpStartFile = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int AfkappenOpStartFileMinGroentijd
        {
            get => _TeDoserenSignaalGroep.AfkappenOpStartFileMinGroentijd;
            set
            {
                _TeDoserenSignaalGroep.AfkappenOpStartFileMinGroentijd = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public bool MinimaleRoodtijd
        {
            get => _TeDoserenSignaalGroep.MinimaleRoodtijd;
            set
            {
                _TeDoserenSignaalGroep.MinimaleRoodtijd = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int MinimaleRoodtijdTijd
        {
            get => _TeDoserenSignaalGroep.MinimaleRoodtijdTijd;
            set
            {
                _TeDoserenSignaalGroep.MinimaleRoodtijdTijd = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public bool MaximaleGroentijd
        {
            get => _TeDoserenSignaalGroep.MaximaleGroentijd;
            set
            {
                _TeDoserenSignaalGroep.MaximaleGroentijd = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int MaximaleGroentijdTijd
        {
            get => _TeDoserenSignaalGroep.MaximaleGroentijdTijd;
            set
            {
                _TeDoserenSignaalGroep.MaximaleGroentijdTijd = value;
                RaisePropertyChanged<object>(broadcast: true);
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
