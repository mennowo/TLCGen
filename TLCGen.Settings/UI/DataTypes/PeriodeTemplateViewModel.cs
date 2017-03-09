using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Settings
{
    public class PeriodeTemplateViewModel : ViewModelBase, IViewModelWithItem
    {
        #region Fields

        private TLCGenTemplateModel<PeriodeModel> _Template;

        #endregion // Fields

        #region Properties

        public string Naam
        {
            get { return _Template.Naam; }
            set
            {
                _Template.Naam = value;
                RaisePropertyChanged("Naam");
            }
        }
        
        private PeriodeModel _SelectedPeriode;
        public PeriodeModel SelectedPeriode
        {
            get { return _SelectedPeriode; }
            set
            {
                _SelectedPeriode = value;
                RaisePropertyChanged(null);
            }
        }


        private ObservableCollection<PeriodeModel> _Perioden;
        public ObservableCollection<PeriodeModel> Perioden
        {
            get
            {
                if (_Perioden == null)
                {
                    _Perioden = new ObservableCollection<PeriodeModel>();
                }
                return _Perioden;
            }
        }

        private List<string> _DetectorTypeOpties;
        public List<string> DetectorTypeOpties
        {
            get
            {
                if (_DetectorTypeOpties == null)
                {
                    _DetectorTypeOpties = new List<string>();
                }
                return _DetectorTypeOpties;
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _AddPeriodeCommand;
        public ICommand AddPeriodeCommand
        {
            get
            {
                if (_AddPeriodeCommand == null)
                {
                    _AddPeriodeCommand = new RelayCommand(new Action<object>(AddPeriodeCommand_Executed), new Predicate<object>(AddPeriodeCommand_CanExecute));
                }
                return _AddPeriodeCommand;
            }
        }


        RelayCommand _RemovePeriodeCommand;
        public ICommand RemovePeriodeCommand
        {
            get
            {
                if (_RemovePeriodeCommand == null)
                {
                    _RemovePeriodeCommand = new RelayCommand(new Action<object>(RemovePeriodeCommand_Executed), new Predicate<object>(RemovePeriodeCommand_CanExecute));
                }
                return _RemovePeriodeCommand;
            }
        }

        #endregion // Commands

        #region Command Functionality
        
        private void AddPeriodeCommand_Executed(object prm)
        {
            PeriodeModel d = new PeriodeModel();
            d.Naam = "per" + (Perioden.Count + 1);
            Perioden.Add(d);
        }

        bool AddPeriodeCommand_CanExecute(object prm)
        {
            return true;
        }

        void RemovePeriodeCommand_Executed(object prm)
        {
            Perioden.Remove(SelectedPeriode);
            SelectedPeriode = null;
        }

        bool RemovePeriodeCommand_CanExecute(object prm)
        {
            return SelectedPeriode != null && Perioden.Count > 0;
        }

        #endregion // Command Functionality

        #region IViewModelWithItem

        public object GetItem()
        {
            return _Template;
        }

        #endregion // IViewModelWithItem

        #region Collection Changed
        
        private void Perioden_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (PeriodeModel per in e.NewItems)
                {
                    _Template.Items.Add(per);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (PeriodeModel per in e.OldItems)
                {
                    _Template.Items.Remove(per);
                }
            }
        }

        #endregion // Collection Changed

        #region Constructor

        public PeriodeTemplateViewModel(TLCGenTemplateModel<PeriodeModel> template)
        {
            _Template = template;
            foreach (var fc in template.Items)
            {
                Perioden.Add(fc);
            }
            Perioden.CollectionChanged += Perioden_CollectionChanged;
        }

        #endregion // Constructor
    }
}
