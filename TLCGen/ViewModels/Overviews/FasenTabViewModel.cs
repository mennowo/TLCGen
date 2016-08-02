using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class FasenTabViewModel : ViewModelBase
    {
        #region Fields

        private ControllerViewModel _ControllerVM;
        private ObservableCollection<string> _Templates;
        private FaseCyclusViewModel _SelectedFaseCyclus;
        private IList _SelectedFaseCycli = new ArrayList();

        #endregion // Fields

        #region Properties

        public ObservableCollection<FaseCyclusViewModel> Fasen
        {
            get
            {
                return _ControllerVM.Fasen;
            }
        }

        public ObservableCollection<string> Templates
        {
            get
            {
                if(_Templates == null)
                {
                    _Templates = new ObservableCollection<string>();
                    _Templates.Add("Template placeholder");
                }
                return _Templates;
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
                if(Regex.IsMatch(fcvm.Naam, @"[0-9]+"))
                {
                    Match m = Regex.Match(fcvm.Naam, @"[0-9]+");
                    string next = m.Value;
                    if (Int32.TryParse(next, out inewname))
                    {
                        inewname++;
                        newname = (inewname < 10 ? "0" : "") + inewname.ToString();
                        while (!_ControllerVM.IsFaseNaamUnique(newname))
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

        #region Constructor

        public FasenTabViewModel(ControllerViewModel controllervm)
        {
            _ControllerVM = controllervm;
        }

        #endregion // Constructor
    }
}
