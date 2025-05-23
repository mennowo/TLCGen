﻿using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    public class PrioIngreepMeldingenListViewModel : PrioItemViewModel
    {
        #region Fields

        private RelayCommand _addMeldingCommand;
        private readonly PrioIngreepViewModel _ingreep;

        #endregion // Fields

        #region Properties

        [Browsable(false)]
        public PrioIngreepMeldingenDataModel PrioIngreepMeldingenData { get; }

        public ObservableCollection<PrioIngreepInUitMeldingViewModel> Meldingen { get; }

        [Browsable(false)]
        public string Naam { get; }

        [Browsable(false)]
        public PrioIngreepInUitMeldingTypeEnum MeldingType { get; }

        [Browsable(false)] public bool IsInmeldingenList => MeldingType == PrioIngreepInUitMeldingTypeEnum.Inmelding;
        [Browsable(false)] public bool IsUitmeldingenList => MeldingType == PrioIngreepInUitMeldingTypeEnum.Uitmelding;

        #endregion // Properties

        #region Commands

        public ICommand AddMeldingCommand => _addMeldingCommand ??= new RelayCommand(() =>
        {
            var m = new PrioIngreepInUitMeldingModel
            {
                Type = PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding
            };
            switch (MeldingType)
            {
                case PrioIngreepInUitMeldingTypeEnum.Inmelding:
                    m.InUit = PrioIngreepInUitMeldingTypeEnum.Inmelding;
                    m.InUit = PrioIngreepInUitMeldingTypeEnum.Inmelding;
                    PrioIngreepMeldingenData.Inmeldingen.Add(m);
                    break;
                case PrioIngreepInUitMeldingTypeEnum.Uitmelding:
                    m.InUit = PrioIngreepInUitMeldingTypeEnum.Uitmelding;
                    PrioIngreepMeldingenData.Uitmeldingen.Add(m);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Meldingen.Add(new PrioIngreepInUitMeldingViewModel(m, this, _ingreep));
            m.Naam = _ingreep.FaseCyclus
                     + _ingreep.PrioIngreep.Naam
                     + DefaultsProvider.Default.GetMeldingShortcode(m)
                     + (MeldingType == PrioIngreepInUitMeldingTypeEnum.Inmelding ? "in" : "uit");
            WeakReferenceMessengerEx.Default.Send(new PrioIngreepMeldingChangedMessage(_ingreep.FaseCyclus, m));
        });

        #endregion // Commands

        #region TLCGen events
        
        #endregion // TLCGen events
        
        #region Constructor

        public PrioIngreepMeldingenListViewModel(string naam, PrioIngreepInUitMeldingTypeEnum type, PrioIngreepMeldingenDataModel prioIngreepMassaDetectieData, PrioIngreepViewModel ingreep)
        {
            _ingreep = ingreep;
            PrioIngreepMeldingenData = prioIngreepMassaDetectieData;
            switch (type)
            {
                case PrioIngreepInUitMeldingTypeEnum.Inmelding:
                    Meldingen = new ObservableCollection<PrioIngreepInUitMeldingViewModel>(prioIngreepMassaDetectieData.Inmeldingen.Select(x => new PrioIngreepInUitMeldingViewModel(x, this, ingreep)));
                    break;
                case PrioIngreepInUitMeldingTypeEnum.Uitmelding:
                    Meldingen = new ObservableCollection<PrioIngreepInUitMeldingViewModel>(prioIngreepMassaDetectieData.Uitmeldingen.Select(x => new PrioIngreepInUitMeldingViewModel(x, this, ingreep)));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            Meldingen.CollectionChanged += MeldingenOnCollectionChanged;

            Naam = naam;

            MeldingType = type;
        }

        private void MeldingenOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                // TODO do this elsewhere
                foreach (PrioIngreepInUitMeldingViewModel melding in e.OldItems)
                {
                    switch (MeldingType)
                    {
                        case PrioIngreepInUitMeldingTypeEnum.Inmelding:
                            PrioIngreepMeldingenData.Inmeldingen.Remove(melding.PrioIngreepInUitMelding);
                            break;
                        case PrioIngreepInUitMeldingTypeEnum.Uitmelding:
                            PrioIngreepMeldingenData.Uitmeldingen.Remove(melding.PrioIngreepInUitMelding);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        #endregion // Constructor
    }
}