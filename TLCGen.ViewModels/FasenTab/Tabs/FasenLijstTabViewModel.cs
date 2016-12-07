using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Messaging.Requests;
using TLCGen.Models;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 0, type: TabItemTypeEnum.FasenTab)]
    public class FasenLijstTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private ObservableCollection<FaseCyclusViewModel> _Fasen;
        private FaseCyclusViewModel _SelectedFaseCyclus;
        private bool _IsSorting = false;
        private IList _SelectedFaseCycli;

        #endregion // Fields

        #region Properties

        public ObservableCollection<FaseCyclusViewModel> Fasen
        {
            get
            {
                if (_Fasen == null)
                {
                    _Fasen = new ObservableCollection<FaseCyclusViewModel>();
                }
                return _Fasen;
            }
        }

        public FaseCyclusViewModel SelectedFaseCyclus
        {
            get { return _SelectedFaseCyclus; }
            set
            {
                _SelectedFaseCyclus = value;
                OnPropertyChanged("SelectedFaseCyclus");
            }
        }

        public IList SelectedFaseCycli
        {
            get { return _SelectedFaseCycli; }
            set
            {
                _SelectedFaseCycli = value;
                OnPropertyChanged("SelectedFaseCycli");
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _AddFaseCommand;
        public ICommand AddFaseCommand
        {
            get
            {
                if (_AddFaseCommand == null)
                {
                    _AddFaseCommand = new RelayCommand(AddNewFaseCommand_Executed, AddNewFaseCommand_CanExecute);
                }
                return _AddFaseCommand;
            }
        }


        RelayCommand _RemoveFaseCommand;
        public ICommand RemoveFaseCommand
        {
            get
            {
                if (_RemoveFaseCommand == null)
                {
                    _RemoveFaseCommand = new RelayCommand(RemoveFaseCommand_Executed, RemoveFaseCommand_CanExecute);
                }
                return _RemoveFaseCommand;
            }
        }

        #endregion // Commands

        #region Command functionality

        void AddNewFaseCommand_Executed(object prm)
        {
            FaseCyclusModel fcm = new FaseCyclusModel();
            string newname = "01";
            int inewname = 1;
            foreach (FaseCyclusViewModel fcvm in Fasen)
            {
                if (Regex.IsMatch(fcvm.Naam, @"[0-9]+"))
                {
                    Match m = Regex.Match(fcvm.Naam, @"[0-9]+");
                    string next = m.Value;
                    if (Int32.TryParse(next, out inewname))
                    {
                        IsElementIdentifierUniqueRequest message;
                        do
                        {
                            newname = (inewname < 10 ? "0" : "") + inewname.ToString();
                            message = new IsElementIdentifierUniqueRequest(newname, ElementIdentifierType.Naam);
                            Messenger.Default.Send(message);
                            inewname++;
                        }
                        while (!message.IsUnique);
                    }
                }
            }
            fcm.Naam = newname;
            fcm.Define = SettingsProvider.Default.GetFaseCyclusDefinePrefix() + newname;
            SettingsProvider.Default.ApplyDefaultFaseCyclusSettings(fcm, fcm.Define);
            FaseCyclusViewModel fcvm1 = new FaseCyclusViewModel(fcm);
            Fasen.Add(fcvm1);
        }

        bool AddNewFaseCommand_CanExecute(object prm)
        {
            return Fasen != null;
        }

        void RemoveFaseCommand_Executed(object prm)
        {
            if (SelectedFaseCycli != null && SelectedFaseCycli.Count > 0)
            {
                // Create temporary List cause we cannot directly remove the selection,
                // as it will cause the selection to change while we loop it
                List<FaseCyclusViewModel> lfcvm = new List<FaseCyclusViewModel>();
                foreach (FaseCyclusViewModel fcvm in SelectedFaseCycli)
                {
                    lfcvm.Add(fcvm);
                }
                foreach (FaseCyclusViewModel fcvm in lfcvm)
                {
                    Fasen.Remove(fcvm);
                }
                SelectedFaseCycli = null;
            }
            else if (SelectedFaseCyclus != null)
            {
                Fasen.Remove(SelectedFaseCyclus);
                SelectedFaseCyclus = null;
            }
        }

        bool RemoveFaseCommand_CanExecute(object prm)
        {
            return Fasen != null &&
                (SelectedFaseCyclus != null ||
                 SelectedFaseCycli != null && SelectedFaseCycli.Count > 0);
        }

        #endregion // Command functionality

        #region TabItem Overrides

        public override string DisplayName
        {
            get
            {
                return "Overzicht";
            }
        }

        public override bool IsEnabled
        {
            get { return true; }
            set { }
        }

        public override void OnSelected()
        {
            
        }

        public override void OnDeselected()
        {
            if (!Fasen.IsSorted())
            {
                _IsSorting = true;
                Fasen.BubbleSort();
                Messenger.Default.Send(new FasenSortedMessage(_Controller.Fasen));
                _IsSorting = false;
            }
        }

        #endregion // TabItem Overrides

        #region TLCGen Event handling

        private void OnFaseDetectorTypeChanged(FaseDetectorTypeChangedMessage message)
        {
            foreach (FaseCyclusViewModel fcm in Fasen)
            {
                fcm.UpdateHasKopmax();
            }
        }

        #endregion // TLCGen Event handling

        #region Collection Changed

        private void Fasen_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (FaseCyclusViewModel fcvm in e.NewItems)
                {
                    _Controller.Fasen.Add(fcvm.FaseCyclus);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (FaseCyclusViewModel fcvm in e.OldItems)
                {
                    _Controller.Fasen.Remove(fcvm.FaseCyclus);
                }
            }

            List<FaseCyclusModel> removedfasen = new List<FaseCyclusModel>();
            if (e.OldItems != null)
            {
                foreach (FaseCyclusViewModel item in e.OldItems)
                {
                    removedfasen.Add(item.FaseCyclus);
                }
            }

            List<FaseCyclusModel> addedfasen = new List<FaseCyclusModel>();
            if (e.NewItems != null)
            {
                foreach (FaseCyclusViewModel item in e.NewItems)
                {
                    addedfasen.Add(item.FaseCyclus);
                }
            }

            if (!_IsSorting)
            {
                Messenger.Default.Send(new FasenChangedMessage(_Controller.Fasen, addedfasen, removedfasen));
                Messenger.Default.Send(new UpdateTabsEnabledMessage());
                Messenger.Default.Send(new ControllerDataChangedMessage());
            }
        }

        #endregion // Collection Changed

        #region Constructor

        public FasenLijstTabViewModel(ControllerModel controller) : base(controller)
        {
            foreach (FaseCyclusModel fcm in _Controller.Fasen)
            {
                Fasen.Add(new FaseCyclusViewModel(fcm));
            }
            Fasen.CollectionChanged += Fasen_CollectionChanged;

            Messenger.Default.Register(this, new Action<FaseDetectorTypeChangedMessage>(OnFaseDetectorTypeChanged));
        }

        #endregion // Constructor
    }
}
