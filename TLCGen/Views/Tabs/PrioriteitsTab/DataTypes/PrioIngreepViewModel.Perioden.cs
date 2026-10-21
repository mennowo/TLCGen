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
    // Prio ingreep view-model: periods in which the ingreep is active.
    public partial class PrioIngreepViewModel
    {
        [Browsable(false)]
        [Description("Check op periode actief")]
        public bool CheckPeriode
        {
            get => PrioIngreep.CheckPeriode;
            set
            {
                PrioIngreep.CheckPeriode = value;
                OnPropertyChanged(nameof(CheckPeriode), broadcast: true);
            }
        }

        [Browsable(false)]
        public OVIngreepPeriodeViewModel SelectedPeriode
        {
            get => _selectedPeriode;
            set
            {
                _selectedPeriode = value;
                OnPropertyChanged(nameof(SelectedPeriode));
            }
        }

        [Browsable(false)]
        public ObservableCollectionAroundList<OVIngreepPeriodeViewModel, OVIngreepPeriodeModel> GerelateerdePerioden =>
            _gerelateerdePerioden ??= new ObservableCollectionAroundList<OVIngreepPeriodeViewModel, OVIngreepPeriodeModel>(PrioIngreep.GerelateerdePerioden);

        [Browsable(false)]
        public ItemsManagerViewModel<OVIngreepPeriodeViewModel, string> GerelateerdePeriodenManager => _gerelateerdePeriodenManager ??=
            new ItemsManagerViewModel<OVIngreepPeriodeViewModel, string>(
                GerelateerdePerioden,
                ControllerAccessProvider.Default.AllePerioden.Where(x => x.Type == PeriodeTypeEnum.Overig).Select(x => x.Naam),
                model => new OVIngreepPeriodeViewModel(new OVIngreepPeriodeModel {Periode = model}),
                model => GerelateerdePerioden.All(x => x.PeriodeNaam != model));

        [Browsable(false)]
        public ICollectionView OverigePerioden =>
            ControllerAccessProvider.Default.GetCollectionView(PeriodeTypeEnum.Overig);

        private void OnPeriodenChanged(object sender, PeriodenChangedMessage obj)
        {
            GerelateerdePerioden.Rebuild();
            _gerelateerdePeriodenManager?.Refresh();
        }
    }
}
