
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class VAOntruimenDetectorViewModel : ObservableObjectEx, IViewModelWithItem
    {
        #region Fields

        private VAOntruimenDetectorModel _VAOntruimenDetector;

        #endregion // Fields

        #region Properties

        public string Detector
        {
            get => _VAOntruimenDetector.Detector;
            set
            {
                _VAOntruimenDetector.Detector = value;
                OnPropertyChanged("FaseCyclus", broadcast: true);
            }
        }

        public ObservableCollectionAroundList<VAOntruimenNaarFaseViewModel, VAOntruimenNaarFaseModel> ConflicterendeFasen
        {
            get;
            private set;
        }

        #endregion Properties

        #region Commands

        #endregion // Commands

        #region Command functionality

        #endregion // Command functionality

        #region Private methods

        #endregion // Private methods

        #region Public methods

        #endregion // Public Methods

        #region IViewModelWithItem

        public object GetItem()
        {
            return _VAOntruimenDetector;
        }

        #endregion // IViewModelWithItem

        #region TLCGen Events

        private void OnFasenChanged(object sender, FasenChangedMessage message)
        {
            ConflicterendeFasen.Rebuild();
        }

        #endregion // TLCGen Events

        #region Constructor

        public VAOntruimenDetectorViewModel(VAOntruimenDetectorModel vaontruimend)
        {
            _VAOntruimenDetector = vaontruimend;
            ConflicterendeFasen = new ObservableCollectionAroundList<VAOntruimenNaarFaseViewModel, VAOntruimenNaarFaseModel>(vaontruimend.ConflicterendeFasen);
            WeakReferenceMessengerEx.Default.Register<FasenChangedMessage>(this, OnFasenChanged);
        }

        #endregion // Constructor
    }
}
