﻿using CommunityToolkit.Mvvm.ComponentModel;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class FileIngreepDetectorViewModel : ObservableObjectEx, IViewModelWithItem
    {
        #region Fields

        private FileIngreepDetectorModel _Detector;

        #endregion // Fields

        #region Properties

        public string Detector
        {
            get => _Detector.Detector;
            set
            {
                _Detector.Detector = value;
                OnPropertyChanged("Detector");
            }
        }

        public int BezetTijd
        {
            get => _Detector.BezetTijd;
            set
            {
                _Detector.BezetTijd = value;
                OnPropertyChanged(nameof(BezetTijd), broadcast: true);
            }
        }

        public int RijTijd
        {
            get => _Detector.RijTijd;
            set
            {
                _Detector.RijTijd = value;
                OnPropertyChanged(nameof(RijTijd), broadcast: true);
            }
        }

        public int AfvalVertraging
        {
            get => _Detector.AfvalVertraging;
            set
            {
                _Detector.AfvalVertraging = value;
                OnPropertyChanged(nameof(AfvalVertraging), broadcast: true);
            }
        }

        public bool IngreepNaamPerLus
        {
            get => _Detector.IngreepNaamPerLus;
            set
            {
                _Detector.IngreepNaamPerLus = value;
                OnPropertyChanged(nameof(IngreepNaamPerLus), broadcast: true);
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
            return _Detector;
        }

        #endregion // IViewModelWithItem

        #region Constructor

        public FileIngreepDetectorViewModel(FileIngreepDetectorModel detector)
        {
            _Detector = detector;
        }

        #endregion // Constructor
    }
}
