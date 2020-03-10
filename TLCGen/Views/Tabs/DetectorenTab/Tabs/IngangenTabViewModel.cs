using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Input;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.ModelManagement;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Plugins;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    /// <summary>
    /// ViewModel for the list of extra detectors, not belonging to a PhaseCyclus
    /// </summary>
    [TLCGenTabItem(index: 50, type: TabItemTypeEnum.DetectieTab)]
    public class IngangenTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields
        
        private IngangViewModel _SelectedIngang;
        private IList _SelectedIngangen = new ArrayList();

        #endregion // Fields

        #region Properties

        private ObservableCollection<IngangViewModel> _Ingangen;
        public ObservableCollection<IngangViewModel> Ingangen
        {
            get
            {
                if(_Ingangen == null)
                {
                    _Ingangen = new ObservableCollection<IngangViewModel>();
                }
                return _Ingangen;
            }
        }

        public IngangViewModel SelectedIngang
        {
            get => _SelectedIngang;
            set
            {
                _SelectedIngang = value;
                RaisePropertyChanged("SelectedIngang");
            }
        }

        public IList SelectedIngangen
        {
            get => _SelectedIngangen;
            set
            {
                _SelectedIngangen = value;
                RaisePropertyChanged("SelectedIngangen");
                if (value != null)
                {
                    var sl = new List<IngangModel>();
                    foreach (var s in value)
                    {
                        sl.Add((s as IngangViewModel).Ingang);
                    }
                }
            }
        }
        
        #endregion // Properties

        #region Commands

        RelayCommand _AddIngangCommand;
        public ICommand AddIngangCommand
        {
            get
            {
                if (_AddIngangCommand == null)
                {
                    _AddIngangCommand = new RelayCommand(AddIngangCommand_Executed, AddIngangCommand_CanExecute);
                }
                return _AddIngangCommand;
            }
        }


        RelayCommand _RemoveIngangCommand;
        public ICommand RemoveIngangCommand
        {
            get
            {
                if (_RemoveIngangCommand == null)
                {
                    _RemoveIngangCommand = new RelayCommand(RemoveIngangCommand_Executed, RemoveIngangCommand_CanExecute);
                }
                return _RemoveIngangCommand;
            }
        }

        #endregion // Commands

        #region Command functionality

        void AddIngangCommand_Executed(object prm)
        {
            var dm = new IngangModel();
            var newname = "i001";
            var inewname = 1;
            foreach (var ivm in Ingangen)
            {
                if (Regex.IsMatch(ivm.Naam, @"[0-9]+"))
                {
                    var m = Regex.Match(ivm.Naam, @"[0-9]+");
                    var next = m.Value;
                    if (Int32.TryParse(next, out inewname))
                    {
                        newname = "i" + inewname.ToString("000");
                        while (!TLCGenModelManager.Default.IsElementIdentifierUnique(TLCGenObjectTypeEnum.Input, newname))
                        {
                            inewname++;
                            newname = "i" + inewname.ToString("000");
                        }
                    }
                }
            }
            dm.Naam = newname;
            DefaultsProvider.Default.SetDefaultsOnModel(dm, dm.Type.ToString());
            var dvm1 = new IngangViewModel(dm);
            Ingangen.Add(dvm1);
            Messenger.Default.Send(new IngangenChangedMessage());
        }

        bool AddIngangCommand_CanExecute(object prm)
        {
            return Ingangen != null;
        }

        void RemoveIngangCommand_Executed(object prm)
        {
            var changed = false;
            if (SelectedIngangen != null && SelectedIngangen.Count > 0)
            {
                changed = true;
                foreach (IngangViewModel ivm in SelectedIngangen)
                {
                    Integrity.TLCGenControllerModifier.Default.RemoveModelItemFromController(ivm.Naam);
                }
            }
            else if (SelectedIngang != null)
            {
                changed = true;
                Integrity.TLCGenControllerModifier.Default.RemoveModelItemFromController(SelectedIngang.Naam);
            }
            RebuildIngangenList();
            MessengerInstance.Send(new ControllerDataChangedMessage());

            if (changed)
            {
                Messenger.Default.Send(new IngangenChangedMessage());
            }
        }

        bool RemoveIngangCommand_CanExecute(object prm)
        {
            return Ingangen != null &&
                (SelectedIngang != null ||
                 SelectedIngangen != null && SelectedIngangen.Count > 0);
        }

        #endregion // Command functionality

        #region Private Methods

        private void RebuildIngangenList()
        {
            Ingangen.CollectionChanged -= Ingangen_CollectionChanged;
            Ingangen.Clear();
            foreach (var dm in base.Controller.Ingangen)
            {
                var dvm = new IngangViewModel(dm);
                dvm.PropertyChanged += Ingang_PropertyChanged;
                Ingangen.Add(dvm);
            }
            Ingangen.CollectionChanged += Ingangen_CollectionChanged;
            RaisePropertyChanged("");
        }

        #endregion // Private Methods

        #region TabItem Overrides

        public override string DisplayName => "Ingangen";

        public override bool IsEnabled
        {
            get => true;
            set { }
        }

        public override void OnSelected()
        {
        }

        public override void OnDeselected()
        {
            this.Ingangen.BubbleSort();
        }

        public override ControllerModel Controller
        {
            get => base.Controller;

            set
            {
                base.Controller = value;
                if (base.Controller != null)
                {
                    RebuildIngangenList();
                }
                else
                {
                    Ingangen.CollectionChanged -= Ingangen_CollectionChanged;
                    Ingangen.Clear();
                }
            }
        }

        #endregion // TabItem Overrides

        #region Event handling

        private bool _SettingMultiple = false;
        private void Ingang_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_SettingMultiple || string.IsNullOrEmpty(e.PropertyName))
                return;

            if (SelectedIngangen != null && SelectedIngangen.Count > 1)
            {
                _SettingMultiple = true;
                MultiPropertySetter.SetPropertyForAllItems<IngangViewModel>(sender, e.PropertyName, SelectedIngangen);
            }
            _SettingMultiple = false;
        }

        #endregion // Event handling

        #region Collection Changed

        private void Ingangen_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (IngangViewModel dvm in e.NewItems)
                {
                    dvm.PropertyChanged += Ingang_PropertyChanged;
                    _Controller.Ingangen.Add(dvm.Ingang);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (IngangViewModel dvm in e.OldItems)
                {
                    _Controller.Ingangen.Remove(dvm.Ingang);
                }
            };
            //Messenger.Default.Send(new IngangenExtraListChangedMessage(_Controller.Ingangen));
            //Messenger.Default.Send(new ControllerDataChangedMessage());
        }

        #endregion // Collection Changed

        #region Constructor

        public IngangenTabViewModel() : base()
        {
        }

        #endregion // Constructor
    }
}
