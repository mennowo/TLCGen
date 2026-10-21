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
    // Prio ingreep view-model: rijtijden, ondermaximum, groenbewaking, blokkeer-, bezet- en minimale roodtijd.
    public partial class PrioIngreepViewModel
    {
        [Category("Tijden")]
        [Description("Rijtijd ongehinderd")]
        public int RijTijdOngehinderd
        {
            get => PrioIngreep.RijTijdOngehinderd;
            set
            {
                PrioIngreep.RijTijdOngehinderd = value;
                OnPropertyChanged(nameof(RijTijdOngehinderd), broadcast: true);
            }
        }

        [Description("Rijtijd beperkt gehinderd")]
        public int RijTijdBeperktgehinderd
        {
            get => PrioIngreep.RijTijdBeperktgehinderd;
            set
            {
                PrioIngreep.RijTijdBeperktgehinderd = value;
                OnPropertyChanged(nameof(RijTijdBeperktgehinderd), broadcast: true);
            }
        }

        [Description("Rijtijd gehinderd")]
        public int RijTijdGehinderd
        {
            get => PrioIngreep.RijTijdGehinderd;
            set
            {
                PrioIngreep.RijTijdGehinderd = value;
                OnPropertyChanged(nameof(RijTijdGehinderd), broadcast: true);
            }
        }

        [Description("Ondermaximum")]
        public int OnderMaximum
        {
            get => PrioIngreep.OnderMaximum;
            set
            {
                PrioIngreep.OnderMaximum = value;
                OnPropertyChanged(nameof(OnderMaximum), broadcast: true);
            }
        }

        [Description("Groenbewaking")]
        public int GroenBewaking
        {
            get => PrioIngreep.GroenBewaking;
            set
            {
                PrioIngreep.GroenBewaking = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("Blokkeren prio na ingreep")]
        public int BlokkeertijdNaPrioIngreep
        {
            get => PrioIngreep.BlokkeertijdNaPrioIngreep;
            set
            {
                PrioIngreep.BlokkeertijdNaPrioIngreep = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("Bezettijd prio gehinderd")]
        public int BezettijdPrioGehinderd
        {
            get => PrioIngreep.BezettijdPrioGehinderd;
            set
            {
                PrioIngreep.BezettijdPrioGehinderd = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("Minimale roodtijd\n(t.b.v. inmelden)")]
        public int MinimaleRoodtijd
        {
            get => PrioIngreep.MinimaleRoodtijd;
            set
            {
                PrioIngreep.MinimaleRoodtijd = value;
                OnPropertyChanged(broadcast: true);
            }
        }
    }
}
