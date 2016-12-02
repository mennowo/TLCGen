using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using TLCGen.DataAccess;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Interfaces;
using TLCGen.Messaging;
using TLCGen.Messaging.Messages;
using TLCGen.Messaging.Requests;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Settings;
using TLCGen.ViewModels.Templates;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 1)]
    public class FasenTabViewModel : TLCGenTabItemViewModel, IHaveTemplates<FaseCyclusModel>
    {
        #region Fields

        private FaseCyclusViewModel _SelectedFaseCyclus;
        private GroentijdenSetsLijstViewModel _GroentijdenLijstVM;
        private IList _SelectedFaseCycli = new ArrayList();
        private TabItem _SelectedTab;
        private int _SelectedTabIndex;
        private TemplatesManagerViewModelT<FaseCyclusTemplateViewModel, FaseCyclusModel> _TemplateManagerVM;
        private bool _IsSorting = false;

        #endregion // Fields

        #region Properties

        private ObservableCollection<FaseCyclusViewModel> _Fasen;
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

        public TemplatesManagerViewModelT<FaseCyclusTemplateViewModel, FaseCyclusModel> TemplateManagerVM
        {
            get
            {
                if(_TemplateManagerVM == null)
                {
                    _TemplateManagerVM = new TemplatesManagerViewModelT<FaseCyclusTemplateViewModel, FaseCyclusModel>
                        (System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "templates\\fasecycli\\"),
                         this, $@"{SettingsProvider.Instance.GetFaseCyclusDefinePrefix()}([0-9])");
                }
                return _TemplateManagerVM;
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

        public GroentijdenSetsLijstViewModel GroentijdenLijstVM
        {
            get
            {
                return _GroentijdenLijstVM;
            }
        }

        public TabItem SelectedTab
        {
            get { return _SelectedTab; }
            set
            {
                _SelectedTab = value;
                OnPropertyChanged("SelectedTab");
            }
        }

        public int SelectedTabIndex
        {
            get { return _SelectedTabIndex; }
            set
            {
                _SelectedTabIndex = value;
                if(value != 0)
                {
                    if (!Fasen.IsSorted())
                    {
                        _IsSorting = true;
                        Fasen.BubbleSort();
                        var message = new FasenSortedMessage(_Controller.Fasen);
                        MessageManager.Instance.Send(message);
                        _IsSorting = false;
                    }
                }
                OnPropertyChanged("SelectedTabIndex");
            }
        }

        public string GroentijdenHeader
        {
            get
            {
                switch(_Controller.Data.TypeGroentijden)
                {
                    case Models.Enumerations.GroentijdenTypeEnum.VerlengGroentijden:
                        return "Verlenggroen";
                    default:
                    case Models.Enumerations.GroentijdenTypeEnum.MaxGroentijden:
                        return "Maxgroen";
                }
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
                if(Regex.IsMatch(fcvm.Naam, @"[0-9]+"))
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
                            MessageManager.Instance.SendWithRespons(message);
                            inewname++;
                        }
                        while (!message.IsUnique);
                    }
                }   
            }
            fcm.Naam = newname;
            fcm.Define = SettingsProvider.Instance.GetFaseCyclusDefinePrefix() + newname;
            SettingsProvider.Instance.ApplyDefaultFaseCyclusSettings(fcm, fcm.Define);
            FaseCyclusViewModel fcvm1 = new FaseCyclusViewModel(fcm);
            Fasen.Add(fcvm1);
        }

        bool AddNewFaseCommand_CanExecute(object prm)
        {
            return Fasen != null;
        }

        void RemoveFaseCommand_Executed(object prm)
        {
            if(SelectedFaseCycli != null && SelectedFaseCycli.Count > 0)
            {
                // Create temporary List cause we cannot directly remove the selection,
                // as it will cause the selection to change while we loop it
                List<FaseCyclusViewModel> lfcvm = new List<FaseCyclusViewModel>();
                foreach(FaseCyclusViewModel fcvm in SelectedFaseCycli)
                {
                    lfcvm.Add(fcvm);
                }
                foreach(FaseCyclusViewModel fcvm in lfcvm)
                {
                    Fasen.Remove(fcvm);
                }
                SelectedFaseCycli = null;
            }
            else if(SelectedFaseCyclus != null)
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
                return "Fasen";
            }
        }

        public override bool IsEnabled
        {
            get { return true; }
            set { }
        }

        #endregion // TabItem Overrides

        #region Public Methods

        /// <summary>
        /// Sorts the maxgreen sets according to the order of the phases
        /// </summary>
        public void SortMaxGroenSetsFasen()
        {
            foreach(GroentijdenSetViewModel mgsvm in _GroentijdenLijstVM.GroentijdenSets)
            {
                mgsvm.GroentijdenSetList.BubbleSort();
            }
            _GroentijdenLijstVM.BuildGroentijdenMatrix();
        }

        /// <summary>
        /// Sets the value of the property indicated by propName to the value it has 
        /// for the parsed instance of PhaseCyclusViewModel for all selected phases
        /// </summary>
        /// <param name="o">The instance of PhaseCyclusViewModel to take as the base case</param>
        /// <param name="propName">The property to set</param>
        public void SetAllSelectedFasenValue(FaseCyclusViewModel o, string propName)
        {
            foreach(FaseCyclusViewModel fcvm in SelectedFaseCycli)
            {
                object value = o.GetType().GetProperty(propName).GetValue(o);
                fcvm.GetType().GetProperty(propName).SetValue(fcvm, value);
            }
        }

        #endregion // Public Methods

        #region IHaveTemplates

        public List<object> GetTemplatableItems()
        {
            List<object> items = new List<object>();
            if (SelectedFaseCycli != null)
                foreach (FaseCyclusViewModel fcvm in SelectedFaseCycli)
                    items.Add(fcvm.FaseCyclus);
            return items;
        }

        public void AddFromTemplate(List<FaseCyclusModel> items)
        {
            try
            {
                foreach (FaseCyclusModel fcm in items)
                {
                    var message1 = new IsElementIdentifierUniqueRequest(fcm.Naam, ElementIdentifierType.Naam);
                    var message2 = new IsElementIdentifierUniqueRequest(fcm.Define, ElementIdentifierType.Define);
                    MessageManager.Instance.SendWithRespons(message1);
                    MessageManager.Instance.SendWithRespons(message2);
                    if (message1.Handled && message1.IsUnique &&
                        message2.Handled && message2.IsUnique)
                    {
                        bool IsOK = true;
                        foreach(DetectorModel dm in fcm.Detectoren)
                        {
                            var message3 = new IsElementIdentifierUniqueRequest(dm.Naam, ElementIdentifierType.Naam);
                            var message4 = new IsElementIdentifierUniqueRequest(dm.Define, ElementIdentifierType.Define);
                            MessageManager.Instance.SendWithRespons(message3);
                            MessageManager.Instance.SendWithRespons(message4);
                            if (!(message3.Handled && message3.IsUnique &&
                                  message4.Handled && message4.IsUnique))
                            {
                                IsOK = false;
                                break;
                            }
                        }
                        if(IsOK)
                        {
                            FaseCyclusViewModel fcvm = new FaseCyclusViewModel(fcm);
                            Fasen.Add(fcvm);
                        }
                    }
                }
            }
            catch
            {
                throw new NotImplementedException();
            }
        }

        #endregion // IHaveTemplates

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
            if(e.OldItems != null)
            {
                foreach(FaseCyclusViewModel item in e.OldItems)
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
                MessageManager.Instance.Send(new FasenChangedMessage(_Controller.Fasen, addedfasen, removedfasen));
                MessageManager.Instance.Send(new UpdateTabsEnabledMessage());
                MessageManager.Instance.Send(new ControllerDataChangedMessage());
            }
        }

        #endregion // Collection Changed

        #region TLCGen Event handling

        private void OnGroentijdenTypeChanged(GroentijdenTypeChangedMessage message)
        {
            OnPropertyChanged("GroentijdenHeader");
            int i = 1;
            foreach(GroentijdenSetViewModel setvm in GroentijdenLijstVM.GroentijdenSets)
            {
                switch(message.Type)
                {
                    case GroentijdenTypeEnum.MaxGroentijden:
                        setvm.Naam = "MG" + i.ToString();
                        break;
                    case GroentijdenTypeEnum.VerlengGroentijden:
                        setvm.Naam = "VG" + i.ToString();
                        break;
                }
            }
            GroentijdenLijstVM.BuildGroentijdenMatrix();
        }

        private void OnFaseDetectorTypeChanged(FaseDetectorTypeChangedMessage message)
        {
            foreach(FaseCyclusViewModel fcm in Fasen)
            {
                fcm.UpdateHasKopmax();
            }
        }

        #endregion // TLCGen Event handling

        #region Constructor

        public FasenTabViewModel(ControllerModel controller) : base(controller)
        {
            _GroentijdenLijstVM = new GroentijdenSetsLijstViewModel(_Controller);
            foreach(FaseCyclusModel fcm in _Controller.Fasen)
            {
                Fasen.Add(new FaseCyclusViewModel(fcm));
            }
            Fasen.CollectionChanged += Fasen_CollectionChanged;

            MessageManager.Instance.Subscribe(this, new Action<GroentijdenTypeChangedMessage>(OnGroentijdenTypeChanged));
            MessageManager.Instance.Subscribe(this, new Action<FaseDetectorTypeChangedMessage>(OnFaseDetectorTypeChanged));
        }

        #endregion // Constructor
    }
}
