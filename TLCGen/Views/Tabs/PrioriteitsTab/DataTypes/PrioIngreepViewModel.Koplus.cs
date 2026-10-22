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
    // Prio ingreep view-model: koplus and versnelde inmelding via de koplus.
    public partial class PrioIngreepViewModel
    {
        [Browsable(false)]
        public NooitAltijdAanUitEnum VersneldeInmeldingKoplus
        {
            get => PrioIngreep.VersneldeInmeldingKoplus;
            set
            {
                PrioIngreep.VersneldeInmeldingKoplus = value;
                OnPropertyChanged(nameof(VersneldeInmeldingKoplus), broadcast: true);
            }
        }

        [Browsable(false)]
        public string Koplus
        {
            get => PrioIngreep.Koplus;
            set
            {
                // Do not set if value is NULL but the existing value is valid
                if (value == null &&
                    PrioIngreep.Koplus != null &&
                    DataAccess.TLCGenControllerDataProvider.Default.Controller.GetAllDetectors(x => x.Type == DetectorTypeEnum.Kop).Any(x => x.Naam == PrioIngreep.Koplus))
                {
                    return;
                }

                PrioIngreep.Koplus = value;
                if (value == null) PrioIngreep.Koplus = "NG";
                OnPropertyChanged(nameof(Koplus), broadcast: true);
                OnPropertyChanged(nameof(HasKoplus));
                OnPropertyChanged(nameof(HasWisselstand));
            }
        }

        [Browsable(false)]
        public bool NoodaanvraagKoplus
        {
            get => PrioIngreep.NoodaanvraagKoplus;
            set
            {
                PrioIngreep.NoodaanvraagKoplus = value;
                OnPropertyChanged(nameof(NoodaanvraagKoplus), broadcast: true);
            }
        }

        [Browsable(false)]
        public bool KoplusKijkNaarWisselstand
        {
            get => PrioIngreep.KoplusKijkNaarWisselstand;
            set
            {
                PrioIngreep.KoplusKijkNaarWisselstand = value;
                OnPropertyChanged(nameof(KoplusKijkNaarWisselstand), broadcast: true);
            }
        }

        [Browsable(false)]
        public bool HasKoplus => !string.IsNullOrWhiteSpace(Koplus) && Koplus != "NG";

        [Browsable(false)]
        public bool HasWisselstand => HasKoplus && PrioIngreep.MeldingenData.Wissel1 || PrioIngreep.MeldingenData.Wissel2;

        [Browsable(false)]
        public ObservableCollection<string> Detectoren { get; }

        private void OnDetectorenChanged(object sender, DetectorenChangedMessage dmsg)
        {
            var sd1 = Koplus;

            Detectoren.Clear();
            Detectoren.Add("NG");
            foreach (var d in DataAccess.TLCGenControllerDataProvider.Default.Controller.GetAllDetectors(x => x.Type == DetectorTypeEnum.Kop))
            {
                Detectoren.Add(d.Naam);
            }

            if (!string.IsNullOrWhiteSpace(sd1) && Detectoren.Contains(sd1))
            {
                PrioIngreep.Koplus = sd1;
                OnPropertyChanged(nameof(Koplus));
            }
            else
            {
                PrioIngreep.Koplus = "NG";
                OnPropertyChanged(nameof(Koplus));
            }
        }
    }
}
