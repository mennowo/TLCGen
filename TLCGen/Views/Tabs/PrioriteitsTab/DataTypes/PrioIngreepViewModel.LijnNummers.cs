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
    // Prio ingreep view-model: checks on line numbers and ritcategorie, and the list of line numbers.
    public partial class PrioIngreepViewModel
    {
        [Browsable(false)]
        [Description("Check op lijnnummers")]
        public bool CheckLijnNummer
        {
            get => PrioIngreep.CheckLijnNummer;
            set
            {
                PrioIngreep.CheckLijnNummer = value;
				if(value && !LijnNummers.Any())
				{
					Add10LijnNummersCommand.Execute(null);
				}
                OnPropertyChanged(nameof(CheckLijnNummer), broadcast: true);
            }
        }

        [Browsable(false)]
        [Description("Check op ritcategorie")]
        public bool CheckRitCategorie
        {
            get => PrioIngreep.CheckRitCategorie;
            set
            {
                PrioIngreep.CheckRitCategorie = value;
                OnPropertyChanged(nameof(CheckRitCategorie), broadcast: true);
            }
        }

        [Browsable(false)]
        [EnabledCondition(nameof(CheckLijnNummer))]
        [Description("Prioriteit voor alle lijnen")]
        public bool AlleLijnen
        {
            get => PrioIngreep.AlleLijnen;
            set
            {
                PrioIngreep.AlleLijnen = value;
                OnPropertyChanged(nameof(AlleLijnen), broadcast: true);
            }
        }

        [Browsable(false)]
        public OVIngreepLijnNummerViewModel SelectedLijnNummer
        {
            get => _selectedLijnNummer;
            set
            {
                _selectedLijnNummer = value;
                OnPropertyChanged(nameof(SelectedLijnNummer));
            }
        }

        [Browsable(false)]
        public string NewLijnNummer
        {
            get => _newLijnNummer;
            set
            {
                _newLijnNummer = value;
                OnPropertyChanged(nameof(NewLijnNummer));
            }
        }

        [Browsable(false)]
        public ObservableCollectionAroundList<OVIngreepLijnNummerViewModel, OVIngreepLijnNummerModel> LijnNummers =>
            _lijnNummers ??= new ObservableCollectionAroundList<OVIngreepLijnNummerViewModel, OVIngreepLijnNummerModel>(PrioIngreep.LijnNummers);

        public ICommand AddLijnNummerCommand => _addLijnNummerCommand ??= new RelayCommand(() =>
            {
                if (!string.IsNullOrWhiteSpace(NewLijnNummer))
                {
                    var nummer = new OVIngreepLijnNummerModel()
                    {
                        Nummer = NewLijnNummer, RitCategorie = "999"
                    };
                    LijnNummers.Add(new OVIngreepLijnNummerViewModel(nummer));
                }
                else
                {
                    var nummer = new OVIngreepLijnNummerModel()
                    {
                        Nummer = "0", RitCategorie = "999"
                    };
                    LijnNummers.Add(new OVIngreepLijnNummerViewModel(nummer));
                }
                NewLijnNummer = "";
                WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
                _removeLijnNummerCommand?.NotifyCanExecuteChanged();
            });

        public ICommand Add10LijnNummersCommand => _add10LijnNummersCommand ??= new RelayCommand(
            () =>
            {
                for (var i = 0; i < 10; ++i) AddLijnNummerCommand.Execute(null);
            }, () => LijnNummers != null);

        public ICommand RemoveLijnNummerCommand => _removeLijnNummerCommand ??= new RelayCommand(() =>
            {
                if (SelectedLijnNummer != null)
                {
                    LijnNummers.Remove(SelectedLijnNummer);
                    SelectedLijnNummer = null;
                }
                else
                {
                    LijnNummers.RemoveAt(LijnNummers.Count - 1);
                }

                WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
                _removeLijnNummerCommand?.NotifyCanExecuteChanged();
            }, () => LijnNummers is { Count: > 0 });
    }
}
