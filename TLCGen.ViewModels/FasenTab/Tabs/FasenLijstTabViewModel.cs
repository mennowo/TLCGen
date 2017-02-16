using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Integrity;
using TLCGen.Messaging.Messages;
using TLCGen.Messaging.Requests;
using TLCGen.Models;
using TLCGen.Models.Operations;
using TLCGen.Plugins;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 0, type: TabItemTypeEnum.FasenTab)]
    public class FasenLijstTabViewModel : TLCGenTabItemViewModel, IAllowTemplates<FaseCyclusModel>
    {
        #region Fields

        private ObservableCollection<FaseCyclusViewModel> _Fasen;
        private FaseCyclusViewModel _SelectedFaseCyclus;
        private bool _IsSorting = false;
        private IList _SelectedFaseCycli = new ArrayList();

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
                _SettingMultiple = false;
                OnPropertyChanged("SelectedFaseCycli");
            }
        }

        private TemplateProviderViewModel<TLCGenTemplateModel<FaseCyclusModel>, FaseCyclusModel> _TemplatesProviderVM;
        public TemplateProviderViewModel<TLCGenTemplateModel<FaseCyclusModel>, FaseCyclusModel> TemplatesProviderVM
        {
            get
            {
                if (_TemplatesProviderVM == null)
                {
                    _TemplatesProviderVM = new TemplateProviderViewModel<TLCGenTemplateModel<FaseCyclusModel>, FaseCyclusModel>(this);
                }
                return _TemplatesProviderVM;
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
            fcm.Type = Settings.Utilities.FaseCyclusUtilities.GetFaseTypeFromNaam(fcm.Naam);
            DefaultsProvider.Default.SetDefaultsOnModel(fcm);
            FaseCyclusViewModel fcvm1 = new FaseCyclusViewModel(fcm);
            Fasen.Add(fcvm1);
        }

        bool AddNewFaseCommand_CanExecute(object prm)
        {
            return Fasen != null;
        }

        void RemoveFaseCommand_Executed(object prm)
        {
            bool changed = false;
            List<FaseCyclusModel> remfcs = new List<FaseCyclusModel>();
            if (SelectedFaseCycli != null && SelectedFaseCycli.Count > 0)
            {
                changed = true;
                foreach (FaseCyclusViewModel fcvm in SelectedFaseCycli)
                {
                    ControllerModifier.RemoveSignalGroupFromController(_Controller, fcvm.Naam);
                    remfcs.Add(fcvm.FaseCyclus);
                }

                SelectedFaseCycli = null;
            }
            else if (SelectedFaseCyclus != null)
            {
                changed = true;
                remfcs.Add(SelectedFaseCyclus.FaseCyclus);
                ControllerModifier.RemoveSignalGroupFromController(_Controller, SelectedFaseCyclus.Naam);
                SelectedFaseCyclus = null;
            }

            if(changed)
            {
                Fasen.CollectionChanged -= Fasen_CollectionChanged;
                Fasen.Clear();
                foreach (var fc in _Controller.Fasen)
                {
                    Fasen.Add(new FaseCyclusViewModel(fc));
                }
                Fasen.CollectionChanged += Fasen_CollectionChanged;
                Messenger.Default.Send(new FasenChangedMessage(_Controller.Fasen, null, remfcs));
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
            TemplatesProviderVM.Update();
        }

        public override bool OnDeselectedPreview()
        {
            if (!Fasen.IsSorted())
            {
                _IsSorting = true;
                Fasen.BubbleSort();
                _Controller.Fasen.Clear();
                foreach(var fcvm in Fasen)
                {
                    _Controller.Fasen.Add(fcvm.FaseCyclus);
                }
                Messenger.Default.Send(new FasenSortedMessage(_Controller.Fasen));
                _IsSorting = false;
            }
            return true;
        }

        public override ControllerModel Controller
        {
            get
            {
                return base.Controller;
            }

            set
            {
                base.Controller = value;
                Fasen.CollectionChanged -= Fasen_CollectionChanged;
                Fasen.Clear();
                if (base.Controller != null)
                {
                    foreach (FaseCyclusModel fcm in base.Controller.Fasen)
                    {
                        var fcvm = new FaseCyclusViewModel(fcm);
                        fcvm.PropertyChanged += FaseCyclus_PropertyChanged;
                        Fasen.Add(fcvm);
                    }
                    Fasen.CollectionChanged += Fasen_CollectionChanged;
                }
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

        #region Event handling

        private bool _SettingMultiple = false;
        private void FaseCyclus_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_SettingMultiple || string.IsNullOrEmpty(e.PropertyName))
                return;

            if (SelectedFaseCycli != null && SelectedFaseCycli.Count > 1)
            {
                _SettingMultiple = true;
                MultiPropertySetter.SetPropertyForAllItems<FaseCyclusViewModel>(sender, e.PropertyName, SelectedFaseCycli);
            }
            _SettingMultiple = false;
        }

        #endregion // Event handling

        #region Collection Changed

        private void Fasen_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (FaseCyclusViewModel fcvm in e.NewItems)
                {
                    _Controller.Fasen.Add(fcvm.FaseCyclus);
                    fcvm.PropertyChanged += FaseCyclus_PropertyChanged;
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

        #region IAllowTemplates

        public void InsertItemsFromTemplate(List<FaseCyclusModel> items)
        {
            foreach (var fc in items)
            {
                if (!(IntegrityChecker.IsElementNaamUnique(_Controller, fc.Naam) &&
                     (fc.Detectoren.Count == 0 || !fc.Detectoren.Where(x => IntegrityChecker.IsElementNaamUnique(_Controller, x.Naam) == false).Any())))
                {
                    MessageBox.Show("Error bij toevoegen van fase met naam " + fc.Naam + ".\nFase en/of bijbehorende detectie is niet uniek in de regeling.", "Error bij toepassen template");
                    return;
                }
            }
            foreach(var fc in items)
            {
                Fasen.Add(new FaseCyclusViewModel(fc));
            }
        }

        #endregion // IAllowTemplates

        #region Constructor

        public FasenLijstTabViewModel() : base()
        {
            Messenger.Default.Register(this, new Action<FaseDetectorTypeChangedMessage>(OnFaseDetectorTypeChanged));
        }

        #endregion // Constructor
    }
}
