using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using TLCGen.Extensions;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class PrioIngreepRISMeldingViewModel : ViewModelBase
    {
        #region Fields
        #endregion // Fields
        
        #region Properties
        
        public PrioIngreepInUitMeldingViewModel Parent { get; }

        public PrioIngreepInUitMeldingTypeEnum InUit => Parent.InUit;

        public int RisStart
        {
            get => Parent.PrioIngreepInUitMelding.RisStart;
            set 
            { 
                Parent.PrioIngreepInUitMelding.RisStart = value; 
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int RisEnd
        {
            get => Parent.PrioIngreepInUitMelding.RisEnd;
            set 
            { 
                Parent.PrioIngreepInUitMelding.RisEnd = value; 
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int? RisEta
        {
            get => Parent.PrioIngreepInUitMelding.RisEta;
            set 
            { 
                Parent.PrioIngreepInUitMelding.RisEta = value; 
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public ObservableCollection<RISVehicleRoleViewModel> AvailableRoles { get; } = new();
        
        public ObservableCollection<RISVehicleSubroleViewModel> AvailableSubroles { get; } = new();

        public ObservableCollection<RISVehicleImportanceViewModel> AvailableImportances { get; } = new();

        #endregion // Properties

        #region Public Methods

        public void UpdateRoles()
        {
            foreach (var role in AvailableRoles)
            {
                role.PropertyChanged -= RvmOnPropertyChanged;
                role.IsSelected = Parent.PrioIngreepInUitMelding.RisRole.HasFlag(role.Role);
                role.PropertyChanged += RvmOnPropertyChanged;
            }
            foreach (var role in AvailableSubroles)
            {
                role.PropertyChanged -= SrvmOnPropertyChanged;
                role.IsSelected = Parent.PrioIngreepInUitMelding.RisSubrole.HasFlag(role.Subrole);
                role.PropertyChanged += SrvmOnPropertyChanged;
            }
            foreach (var role in AvailableImportances)
            {
                role.PropertyChanged -= IvmOnPropertyChanged;
                role.IsSelected = Parent.PrioIngreepInUitMelding.RisSubrole.HasFlag(role.Importance);
                role.PropertyChanged += IvmOnPropertyChanged;
            }
        }

        #endregion // Public Methods
        
        #region Constructor

        public PrioIngreepRISMeldingViewModel(PrioIngreepInUitMeldingViewModel parent)
        {
            Parent = parent;

            foreach (RISVehicleRole role in Enum.GetValues(typeof(RISVehicleRole)))
            {
                var rvm = new RISVehicleRoleViewModel
                {
                    Role = role,
                    IsSelected = Parent.PrioIngreepInUitMelding.RisRole.HasFlag(role)
                };
                rvm.PropertyChanged += RvmOnPropertyChanged;
                AvailableRoles.Add(rvm);
            }

            foreach (RISVehicleSubrole subrole in Enum.GetValues(typeof(RISVehicleSubrole)))
            {
                var srvm = new RISVehicleSubroleViewModel
                {
                    Subrole = subrole,
                    IsSelected = Parent.PrioIngreepInUitMelding.RisSubrole.HasFlag(subrole)
                };
                srvm.PropertyChanged += SrvmOnPropertyChanged;
                AvailableSubroles.Add(srvm);
            }

            foreach (RISVehicleImportance importance in Enum.GetValues(typeof(RISVehicleImportance)))
            {
                var ivm = new RISVehicleImportanceViewModel
                {
                    Importance = importance,
                    IsSelected = Parent.PrioIngreepInUitMelding.RisImportance.HasFlag(importance)
                };
                ivm.PropertyChanged += IvmOnPropertyChanged;
                AvailableImportances.Add(ivm);
            }
        }

        private void SrvmOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Parent.PrioIngreepInUitMelding.RisSubrole = 0;
            foreach (var subrole in AvailableSubroles)
            {
                if (subrole.IsSelected) Parent.PrioIngreepInUitMelding.RisSubrole |= subrole.Subrole;
            }
            RaisePropertyChanged<object>(broadcast: true);
        }

        private void RvmOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Parent.PrioIngreepInUitMelding.RisRole = 0;
            foreach (var role in AvailableRoles)
            {
                if (role.IsSelected) Parent.PrioIngreepInUitMelding.RisRole |= role.Role;
            }
            RaisePropertyChanged<object>(broadcast: true);
        }

        private void IvmOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Parent.PrioIngreepInUitMelding.RisImportance = 0;
            foreach (var impt in AvailableImportances)
            {
                if (impt.IsSelected) Parent.PrioIngreepInUitMelding.RisImportance |= impt.Importance;
            }
            RaisePropertyChanged<object>(broadcast: true);
        }

        #endregion // Constructor
    }

    public class RISVehicleRoleViewModel : ViewModelBase
    {
        private bool _isSelected;
        
        public RISVehicleRole Role { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value; 
                RaisePropertyChanged();
            }
        }
    }
    
    public class RISVehicleSubroleViewModel : ViewModelBase
    {
        private bool _isSelected;
        
        public RISVehicleSubrole Subrole { get; set; }
        
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value; 
                RaisePropertyChanged();
            }
        }
    }

    public class RISVehicleImportanceViewModel : ViewModelBase
    {
        private bool _isSelected;
        private RISVehicleImportance importance;

        public RISVehicleImportance Importance
        {
            get => importance; 
            set
            {
                importance = value;
                ImportanceDescription = importance.GetDescription();
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(ImportanceDescription));
            }
        }

        public string ImportanceDescription { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                RaisePropertyChanged();
            }
        }
    }
}