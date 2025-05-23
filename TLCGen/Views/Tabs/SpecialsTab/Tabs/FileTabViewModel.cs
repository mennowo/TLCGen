﻿
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Plugins;
using TLCGen.Settings;


namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 1, type: TabItemTypeEnum.SpecialsTab)]
    public class FileTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private FileIngreepViewModel _SelectedFileIngreep;

        private List<string> _ControllerFasen;

        private RelayCommand _AddFileIngreepCommand;
        private RelayCommand _RemoveFileIngreepCommand;
        private ObservableCollection<string> _GroentijdenSets;

        #endregion // Fields

        #region Properties

        public FileIngreepViewModel SelectedFileIngreep
        {
            get => _SelectedFileIngreep;
            set
            {
                _SelectedFileIngreep = value;
                _SelectedFileIngreep?.OnSelected(_ControllerFasen);
                OnPropertyChanged();
                _RemoveFileIngreepCommand?.NotifyCanExecuteChanged();
            }
        }

        public ObservableCollection<string> GroentijdenSets
        {
            get
            {
                if (_GroentijdenSets == null)
                {
                    _GroentijdenSets = new ObservableCollection<string>();
                }
                return _GroentijdenSets;
            }
        }

        public ObservableCollectionAroundList<FileIngreepViewModel, FileIngreepModel> FileIngrepen
        {
            get;
            private set;
        }

        #endregion // Properties

        #region Commands

        public ICommand AddFileIngreepCommand => _AddFileIngreepCommand ??= new RelayCommand(() =>
        {
            var fim = new FileIngreepModel();
            DefaultsProvider.Default.SetDefaultsOnModel(fim);
            var i = FileIngrepen.Count + 1;
            fim.Naam = "File" + i.ToString();
            while(!Integrity.TLCGenIntegrityChecker.IsElementNaamUnique(_Controller, fim.Naam, TLCGenObjectTypeEnum.FileIngreep))
            {
                ++i;
                fim.Naam = "File" + i.ToString();
            }
            var fivm = new FileIngreepViewModel(fim);
            FileIngrepen.Add(fivm);

            WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
        });

        public ICommand RemoveFileIngreepCommand => _RemoveFileIngreepCommand ??= new RelayCommand(() =>
        {
            FileIngrepen.Remove(SelectedFileIngreep);
            SelectedFileIngreep = null;
            WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
        }, () => SelectedFileIngreep != null);

        #endregion // Commands

        #region Private methods

        #endregion // Private methods

        #region Public methods

        #endregion // Public methods

        #region TLCGen TabItem overrides

        public override string DisplayName => "File";

        public override void OnSelected()
        {
            _ControllerFasen = new List<string>();
            foreach (var fcm in _Controller.Fasen)
            {
                _ControllerFasen.Add(fcm.Naam);
            }

            var v = "";
            
            if (_SelectedFileIngreep != null)
            {
                _SelectedFileIngreep.OnSelected(_ControllerFasen);
                v = SelectedFileIngreep.AlternatieveGroentijdenSet;
            }

            GroentijdenSets.Clear();
            GroentijdenSets.Add("NG");
            foreach (var gsm in _Controller.GroentijdenSets)
            {
                GroentijdenSets.Add(gsm.Naam);
            }

            if (_SelectedFileIngreep != null && _SelectedFileIngreep.AlternatieveGroentijdenSet != v)
            {
                _SelectedFileIngreep.AlternatieveGroentijdenSet = v;
            }
        }

        public override ControllerModel Controller
        {
            get => base.Controller;

            set
            {
                base.Controller = value;
                if (base.Controller != null)
                {
                    FileIngrepen = new ObservableCollectionAroundList<FileIngreepViewModel, FileIngreepModel>(_Controller.FileIngrepen);
                }
                else
                {
                    FileIngrepen = null;
                }
                OnPropertyChanged("FileIngrepen");
            }
        }

        #endregion // TLCGen TabItem overrides

        #region TLCGen Events

        private void OnFasenChanged(object sender, FasenChangedMessage message)
        {
            FileIngrepen?.Rebuild();
        }

        private void OnDetectorenChanged(object sender, DetectorenChangedMessage message)
        {
            FileIngrepen?.Rebuild();
        }

        public void OnFileIngreepTeDoserenSignaalPercentageChanged(object sender, FileIngreepTeDoserenSignaalGroepPercentageChangedMessage message)
        {

            foreach (var fivm in FileIngrepen)
            {
                if (fivm.EerlijkDoseren)
                {
                    foreach (var tdsgvm in fivm.TeDoserenSignaalGroepen)
                    {
                        var tdsgm = (FileIngreepTeDoserenSignaalGroepModel)tdsgvm.GetItem();
                        if (tdsgm == message.TeDoserenSignaalGroep)
                        {
                            foreach (var _tdsgvm in fivm.TeDoserenSignaalGroepen)
                            {
                                _tdsgvm.DoseerPercentageNoMessaging = tdsgm.DoseerPercentage;
                            }
                            return;
                        }
                    }
                }
            }
        }

        #endregion // TLCGen Events

        #region Constructor

        public FileTabViewModel() : base()
        {
            WeakReferenceMessengerEx.Default.Register<FasenChangedMessage>(this, OnFasenChanged);
            WeakReferenceMessengerEx.Default.Register<DetectorenChangedMessage>(this, OnDetectorenChanged);
            WeakReferenceMessengerEx.Default.Register<FileIngreepTeDoserenSignaalGroepPercentageChangedMessage>(this, OnFileIngreepTeDoserenSignaalPercentageChanged);
        }

        #endregion // Constructor
    }
}
