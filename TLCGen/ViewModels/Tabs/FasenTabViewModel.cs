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
using TLCGen.DataAccess;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Interfaces;
using TLCGen.Models;
using TLCGen.ViewModels.Templates;

namespace TLCGen.ViewModels
{
    public class FasenTabViewModel : TabViewModel, IHaveTemplates<FaseCyclusModel>
    {
        #region Fields

        private FaseCyclusViewModel _SelectedFaseCyclus;
        private MaxGroentijdenSetsLijstViewModel _MaxGroentijdenLijstVM;
        private IList _SelectedFaseCycli = new ArrayList();
        private TabItem _SelectedTab;
        private int _SelectedTabIndex;
        private TemplatesManagerViewModelT<FaseCyclusTemplateViewModel, FaseCyclusModel> _TemplateManagerVM;

        #endregion // Fields

        #region Properties

        public ObservableCollection<FaseCyclusViewModel> Fasen
        {
            get
            {
                return _ControllerVM.Fasen;
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
                         this, $@"{SettingsProvider.AppSettings.PrefixSettings.FaseCyclusDefinePrefix}([0-9])");
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

        public MaxGroentijdenSetsLijstViewModel MaxGroentijdenLijstVM
        {
            get
            {
                return _MaxGroentijdenLijstVM;
            }
        }

        public TabItem SelectedTab
        {
            get { return _SelectedTab; }
            set
            {
                if (_SelectedTab != null &&
                    _SelectedTab.Header.ToString() == "Overzicht")
                {
                    if (_ControllerVM.DoUpdateFasen())
                    { 
                        MaxGroentijdenLijstVM.BuildMaxGroenMatrix();
                    }
                }
                _SelectedTab = value;
            }
        }

        public int SelectedTabIndex
        {
            get { return _SelectedTabIndex; }
            set
            {
                _SelectedTabIndex = value;
                OnPropertyChanged("SelectedTabIndex");
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
                        inewname++;
                        newname = (inewname < 10 ? "0" : "") + inewname.ToString();
                        while (!_ControllerVM.IsElementNaamUnique(newname))
                        {
                            inewname++;
                            newname = (inewname < 10 ? "0" : "") + inewname.ToString();
                        }
                    }
                }   
            }
            fcm.Naam = newname;
            fcm.Define = _ControllerVM.ControllerDataVM.PrefixSettings.FaseCyclusDefinePrefix + newname;
            FaseCyclusViewModel fcvm1 = new FaseCyclusViewModel(_ControllerVM, fcm);
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
            }
            else if(SelectedFaseCyclus != null)
            {
                Fasen.Remove(SelectedFaseCyclus);
            }
        }

        bool RemoveFaseCommand_CanExecute(object prm)
        {
            return Fasen != null && 
                (SelectedFaseCyclus != null ||
                 SelectedFaseCycli != null && SelectedFaseCycli.Count > 0);
        }

        #endregion // Command functionality

        #region Public Methods

        public void SortMaxGroenSetsFasen()
        {
            foreach(MaxGroentijdenSetViewModel mgsvm in _MaxGroentijdenLijstVM.MaxGroentijdenSets)
            {
                mgsvm.MaxGroentijdenSetList.BubbleSort();
            }
            _MaxGroentijdenLijstVM.BuildMaxGroenMatrix();
        }

        #endregion // Public Methods

        #region IHaveTemplates

        public List<object> GetTemplatableItems()
        {
            List<object> items = new List<object>();
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
                    if(_ControllerVM.IsElementDefineUnique(fcm.Define) &&
                       _ControllerVM.IsElementNaamUnique(fcm.Naam))
                    {
                        bool IsOK = true;
                        foreach(DetectorModel dm in fcm.Detectoren)
                        {
                            if(!(_ControllerVM.IsElementDefineUnique(dm.Define) &&
                                 _ControllerVM.IsElementNaamUnique(dm.Naam)))
                            {
                                IsOK = false;
                                break;
                            }
                        }
                        if(IsOK)
                        {
                            FaseCyclusViewModel fcvm = new FaseCyclusViewModel(_ControllerVM, fcm);
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

        #region Constructor

        public FasenTabViewModel(ControllerViewModel controllervm) : base(controllervm)
        {
            _MaxGroentijdenLijstVM = new MaxGroentijdenSetsLijstViewModel(_ControllerVM);
        }

        #endregion // Constructor
    }
}
