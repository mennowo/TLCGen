﻿using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class FileIngreepTeDoserenSignaalGroepViewModel : ObservableObjectEx, IViewModelWithItem
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
                OnPropertyChanged();
            }
        }

        public int DoseerPercentage
        {
            get => _TeDoserenSignaalGroep.DoseerPercentage;
            set
            {
                _TeDoserenSignaalGroep.DoseerPercentage = value;
                OnPropertyChanged(broadcast: true);
WeakReferenceMessengerEx.Default.Send(new FileIngreepTeDoserenSignaalGroepPercentageChangedMessage(_TeDoserenSignaalGroep));
            }
        }

        public int DoseerPercentageNoMessaging
        {
            set
            {
                _TeDoserenSignaalGroep.DoseerPercentage = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public bool AfkappenOpStartFile
        {
            get => _TeDoserenSignaalGroep.AfkappenOpStartFile;
            set
            {
                _TeDoserenSignaalGroep.AfkappenOpStartFile = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int AfkappenOpStartFileMinGroentijd
        {
            get => _TeDoserenSignaalGroep.AfkappenOpStartFileMinGroentijd;
            set
            {
                _TeDoserenSignaalGroep.AfkappenOpStartFileMinGroentijd = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public bool MinimaleRoodtijd
        {
            get => _TeDoserenSignaalGroep.MinimaleRoodtijd;
            set
            {
                _TeDoserenSignaalGroep.MinimaleRoodtijd = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int MinimaleRoodtijdTijd
        {
            get => _TeDoserenSignaalGroep.MinimaleRoodtijdTijd;
            set
            {
                _TeDoserenSignaalGroep.MinimaleRoodtijdTijd = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public bool MaximaleGroentijd
        {
            get => _TeDoserenSignaalGroep.MaximaleGroentijd;
            set
            {
                _TeDoserenSignaalGroep.MaximaleGroentijd = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int MaximaleGroentijdTijd
        {
            get => _TeDoserenSignaalGroep.MaximaleGroentijdTijd;
            set
            {
                _TeDoserenSignaalGroep.MaximaleGroentijdTijd = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        #endregion // Properties

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
