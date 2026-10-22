using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TLCGen.Controls;
using TLCGen.Dependencies.Providers;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    // Prio ingreep view-model: meerealiserende ingrepen (other ingrepen realised together with this one).
    public partial class PrioIngreepViewModel
    {
        [Browsable(false)]
        public ObservableCollection<string> Fasen
        {
            get
            {
                if (_Fasen == null)
                {
                    _Fasen = new ObservableCollection<string>();
                }
                return _Fasen;
            }
        }

        [Browsable(false)]
        public ObservableCollection<MeerealiserendeIngreepItemViewModel> Ingrepen
        {
            get
            {
                if (_Ingrepen == null)
                {
                    _Ingrepen = new ObservableCollection<MeerealiserendeIngreepItemViewModel>();
                }
                return _Ingrepen;
            }
        }

        [Browsable(false)]
        public string SelectedFase
        {
            get => _SelectedFase;
            set
            {
                _SelectedFase = value;
                OnPropertyChanged("SelectedFase");
                _AddMeerealiserendeFaseCommand?.NotifyCanExecuteChanged();
            }
        }

        [Browsable(false)]
        public MeerealiserendeIngreepItemViewModel SelectedIngreep
        {
            get => _SelectedIngreep;
            set
            {
                _SelectedIngreep = value;
                OnPropertyChanged("SelectedIngreep");
                _AddMeerealiserendeFaseCommand?.NotifyCanExecuteChanged();
            }
        }

        [Browsable(false)]
        public PrioIngreepMeerealiserendeIngreepViewModel SelectedMeerealiserendeIngreep
        {
            get => _SelectedMeerealiserendeIngreep;
            set
            {
                _SelectedMeerealiserendeIngreep = value;
                OnPropertyChanged("SelectedMeerealiserendeFase");
                _RemoveMeerealiserendeFaseCommand?.NotifyCanExecuteChanged();
            }
        }

        [Browsable(false)]
        public ObservableCollectionAroundList<PrioIngreepMeerealiserendeIngreepViewModel, PrioIngreepMeerealiserendeIngreepModel> MeerealiserendeIngrepen
        {
            get;
            private set;
        }

        RelayCommand _AddMeerealiserendeFaseCommand;
        RelayCommand _RemoveMeerealiserendeFaseCommand;
        private ObservableCollection<MeerealiserendeIngreepItemViewModel> _Ingrepen;
        private MeerealiserendeIngreepItemViewModel _SelectedIngreep;

        public ICommand AddMeerealiserendeFaseCommand
        {
            get
            {
                if (_AddMeerealiserendeFaseCommand == null)
                {
                    _AddMeerealiserendeFaseCommand = new RelayCommand(AddNewMeerealiserendeFaseCommand_Executed, AddNewMeerealiserendeFaseCommand_CanExecute);
                }
                return _AddMeerealiserendeFaseCommand;
            }
        }

        public ICommand RemoveMeerealiserendeFaseCommand
        {
            get
            {
                if (_RemoveMeerealiserendeFaseCommand == null)
                {
                    _RemoveMeerealiserendeFaseCommand = new RelayCommand(RemoveMeerealiserendeFaseCommand_Executed, RemoveMeerealiserendeFaseCommand_CanExecute);
                }
                return _RemoveMeerealiserendeFaseCommand;
            }
        }

        void AddNewMeerealiserendeFaseCommand_Executed()
        {
            if (!(MeerealiserendeIngrepen.Where(x => x.Ingreep.PrioIngreep == SelectedFase).Count() > 0))
            {
                MeerealiserendeIngrepen.Add(
                    new PrioIngreepMeerealiserendeIngreepViewModel(
                        new PrioIngreepMeerealiserendeIngreepModel() { PrioIngreep = SelectedIngreep.PrioIngreep, FaseCyclus = SelectedIngreep.FaseCyclus }));
            }

            BuildIngrepenList();

            PrioIngreep.MeerealiserendeIngrepen.BubbleSort();
            MeerealiserendeIngrepen.Rebuild();

            if (MeerealiserendeIngrepen.Count > 0)
                SelectedMeerealiserendeIngreep = MeerealiserendeIngrepen[MeerealiserendeIngrepen.Count - 1];

            OnPropertyChanged(broadcast: true);
        }

        bool AddNewMeerealiserendeFaseCommand_CanExecute()
        {
            return MeerealiserendeIngrepen != null && SelectedIngreep != null;
        }

        void RemoveMeerealiserendeFaseCommand_Executed()
        {
            MeerealiserendeIngrepen.Remove(SelectedMeerealiserendeIngreep);

            BuildIngrepenList();

            PrioIngreep.MeerealiserendeIngrepen.BubbleSort();
            MeerealiserendeIngrepen.Rebuild();

            if (MeerealiserendeIngrepen.Count > 0)
                SelectedMeerealiserendeIngreep = MeerealiserendeIngrepen[MeerealiserendeIngrepen.Count - 1];
            else
                SelectedMeerealiserendeIngreep = null;

            OnPropertyChanged(broadcast: true);
        }

        bool RemoveMeerealiserendeFaseCommand_CanExecute()
        {
            return SelectedMeerealiserendeIngreep != null && MeerealiserendeIngrepen != null && MeerealiserendeIngrepen.Count > 0;
        }

        private void OnFasenChanged(object sender, FasenChangedMessage fmsg)
        {
            BuildFasenList();
        }

        private void BuildFasenList()
        {
            Fasen.Clear();
            foreach (var f in DataAccess.TLCGenControllerDataProvider.Default.Controller.Fasen)
            {
                Fasen.Add(f.Naam);
            }
        }

        private void BuildIngrepenList()
        {
            Ingrepen.Clear();
            foreach (var f in DataAccess.TLCGenControllerDataProvider.Default.Controller.PrioData.PrioIngrepen)
            {
                if (ReferenceEquals(this, f)) continue;
                Ingrepen.Add(new MeerealiserendeIngreepItemViewModel { FaseCyclus = f.FaseCyclus, PrioIngreep = f.Naam });
            }
        }

        private void OnPrioIngrepenChanged(object recipient, PrioIngrepenChangedMessage message)
        {
            BuildIngrepenList();
        }
    }
}
