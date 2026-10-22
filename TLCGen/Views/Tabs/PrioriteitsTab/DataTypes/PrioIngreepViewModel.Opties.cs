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
    // Prio ingreep view-model: prioriteitsopties, geconditioneerde prioriteit, checks op wagennummer en sirene,
    // and inmelding ook door another signal group.
    public partial class PrioIngreepViewModel
    {
        [Category("Prioriteitsopties")]
        [Description("Afkappen conflicten")]
        public bool AfkappenConflicten
        {
            get => PrioIngreep.AfkappenConflicten;
            set
            {
                PrioIngreep.AfkappenConflicten = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("Afkappen conflicten prio")]
        public bool AfkappenConflictenPrio
        {
            get => PrioIngreep.AfkappenConflictenPrio;
            set
            {
                PrioIngreep.AfkappenConflictenPrio = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("Vasthouden groen")]
        public bool VasthoudenGroen
        {
            get => PrioIngreep.VasthoudenGroen;
            set
            {
                PrioIngreep.VasthoudenGroen = value;
                OnPropertyChanged(nameof(VasthoudenGroen), broadcast: true);
            }
        }

        [Description("Tussendoor realiseren")]
        public bool TussendoorRealiseren
        {
            get => PrioIngreep.TussendoorRealiseren;
            set
            {
                PrioIngreep.TussendoorRealiseren = value;
                OnPropertyChanged(nameof(TussendoorRealiseren), broadcast: true);
            }
        }

        [Description("Prioriteitsniveau")]
        public string PrioriteitsNiveau
        {
            get => PrioIngreep.PrioriteitsNiveau.ToString();
            set
            {
                PrioIngreep.PrioriteitsNiveau = int.Parse(value);
                OnPropertyChanged(nameof(PrioriteitsNiveau), broadcast: true);
            }
        }

        [Browsable(false)]
        public NooitAltijdAanUitEnum GeconditioneerdePrioriteit
        {
            get => PrioIngreep.GeconditioneerdePrioriteit;
            set
            {
                PrioIngreep.GeconditioneerdePrioriteit = value;
                OnPropertyChanged(nameof(GeconditioneerdePrioriteit), broadcast: true);
                OnPropertyChanged(nameof(HasGeconditioneerdePrioriteit));
            }
        }

        [Browsable(false)]
        public bool HasGeconditioneerdePrioriteit => GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit;

        [Browsable(false)]
        public int GeconditioneerdePrioTeVroeg
        {
            get => PrioIngreep.GeconditioneerdePrioTeVroeg;
            set
            {
                PrioIngreep.GeconditioneerdePrioTeVroeg = value;
                OnPropertyChanged(nameof(GeconditioneerdePrioTeVroeg), broadcast: true);
            }
        }

        [Browsable(false)]
        public int GeconditioneerdePrioOpTijd
        {
            get => PrioIngreep.GeconditioneerdePrioOpTijd;
            set
            {
                PrioIngreep.GeconditioneerdePrioOpTijd = value;
                OnPropertyChanged(nameof(GeconditioneerdePrioOpTijd), broadcast: true);
            }
        }

        [Browsable(false)]
        public int GeconditioneerdePrioTeLaat
        {
            get => PrioIngreep.GeconditioneerdePrioTeLaat;
            set
            {
                PrioIngreep.GeconditioneerdePrioTeLaat = value;
                OnPropertyChanged(nameof(GeconditioneerdePrioTeLaat), broadcast: true);
            }
        }

        [Browsable(false)]
        [Description("Check op wagennummers")]
        public bool CheckWagenNummer
        {
            get => PrioIngreep.CheckWagenNummer;
            set
            {
                PrioIngreep.CheckWagenNummer = value;
                OnPropertyChanged(nameof(CheckWagenNummer), broadcast: true);
                if (!value) return;
                PrioIngreep.MeldingenData.AntiJutterVoorAlleInmeldingen = false;
                PrioIngreep.MeldingenData.AntiJutterVoorAlleUitmeldingen = false;
                foreach (var m in PrioIngreep.MeldingenData.Inmeldingen) m.AntiJutterTijdToepassen = false;
                foreach (var m in PrioIngreep.MeldingenData.Uitmeldingen) m.AntiJutterTijdToepassen = false;
            }
        }

        [Browsable(false)]
        [Description("Check op sirene")]
        public bool CheckOpSirene
        {
            get => PrioIngreep.CheckOpSirene;
            set
            {
                PrioIngreep.CheckOpSirene = value;
                OnPropertyChanged(nameof(CheckOpSirene), broadcast: true);
            }
        }

        [Browsable(false)]
        public bool InmeldingOokDoorToepassen
        {
            get => PrioIngreep.InmeldingOokDoorToepassen;
            set
            {
                PrioIngreep.InmeldingOokDoorToepassen = value;
                if (value)
                {
                    if(InmeldingOokDoorFase == 0)
                    {
                        if (int.TryParse(PrioIngreep.FaseCyclus, out var i))
                        {
                            if (PrioIngreep.FaseCyclus.EndsWith("1") || PrioIngreep.FaseCyclus.EndsWith("4") || PrioIngreep.FaseCyclus.EndsWith("7") || PrioIngreep.FaseCyclus.EndsWith("10"))
                            {
                                InmeldingOokDoorFase = i + 1;
                            }
                            else if (PrioIngreep.FaseCyclus.EndsWith("3") || PrioIngreep.FaseCyclus.EndsWith("6") || PrioIngreep.FaseCyclus.EndsWith("9") || PrioIngreep.FaseCyclus.EndsWith("12"))
                            {
                                InmeldingOokDoorFase = i - 1;
                            }
                            else
                            {
                                InmeldingOokDoorFase = i;
                            }
                        }
                    }
                }
                OnPropertyChanged(nameof(InmeldingOokDoorToepassen), broadcast: true);
            }
        }

        [Browsable(false)]
        public int InmeldingOokDoorFase
        {
            get => PrioIngreep.InmeldingOokDoorFase;
            set
            {
                PrioIngreep.InmeldingOokDoorFase = value;
                OnPropertyChanged(nameof(InmeldingOokDoorFase), broadcast: true);
            }
        }
    }
}
