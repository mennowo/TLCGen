using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using TLCGen.Controls;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using RelayCommand = GalaSoft.MvvmLight.CommandWpf.RelayCommand;

namespace TLCGen.ViewModels
{
    public class PrioIngreepMeldingenListViewModel : PrioItemViewModel
    {
        #region Fields

        private RelayCommand _addMeldingCommand;
        private RelayCommand _removeMeldingCommand;

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

        public ICommand AddMeldingCommand => _addMeldingCommand ?? (_addMeldingCommand = new RelayCommand(() =>
        {
            var m = new PrioIngreepInUitMeldingModel
            {
                Type = PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding
            };
            switch (MeldingType)
            {
                case PrioIngreepInUitMeldingTypeEnum.Inmelding:
                    m.InUit = PrioIngreepInUitMeldingTypeEnum.Inmelding;
                    // TODO do this elsewhere
                    PrioIngreepMeldingenData.Inmeldingen.Add(m);
                    break;
                case PrioIngreepInUitMeldingTypeEnum.Uitmelding:
                    m.InUit = PrioIngreepInUitMeldingTypeEnum.Uitmelding;
                    // TODO do this elsewhere
                    PrioIngreepMeldingenData.Uitmeldingen.Add(m);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Meldingen.Add(new PrioIngreepInUitMeldingViewModel(m, this));
            var msg = new PrioIngreepMassaDetectieObjectNeedsFaseCyclusMessage(this);
            MessengerInstance.Send(msg);
            if (msg.FaseCyclus == null) return;
            MessengerInstance.Send(new PrioIngreepMeldingChangedMessage(msg.FaseCyclus, m));
        }));

        #endregion // Commands

        #region TLCGen events

        private void OnDetectorenChanged(DetectorenChangedMessage dmsg)
        {
            ///var msg = new PrioIngreepMassaDetectieObjectNeedsFaseCyclusMessage(this);
            ///MessengerInstance.Send(msg);
            ///if (msg.FaseCyclus == null) return;
            ///
            ///var sd1 = "";
            ///var sd2 = "";
            ///if (Wissel1 && Wissel1Type == PrioIngreepInUitDataWisselTypeEnum.Detector)
            ///{
            ///    sd1 = Wissel1Detector;
            ///}
            ///if (Wissel2 && Wissel2Type == PrioIngreepInUitDataWisselTypeEnum.Detector)
            ///{
            ///    sd2 = Wissel2Detector;
            ///}
            ///
            ///
            ///WisselDetectoren.Clear();
            ///foreach (var d in DataAccess.TLCGenControllerDataProvider.Default.Controller.Fasen.SelectMany(x => x.Detectoren.Where(x2 => x2.Type == DetectorTypeEnum.WisselStandDetector)))
            ///{
            ///    WisselDetectoren.Add(d.Naam);
            ///}
            ///foreach (var d in DataAccess.TLCGenControllerDataProvider.Default.Controller.Detectoren.Where(x => x.Type == DetectorTypeEnum.WisselStandDetector))
            ///{
            ///    WisselDetectoren.Add(d.Naam);
            ///}
            ///
            ///if (Wissel1 && Wissel1Type == PrioIngreepInUitDataWisselTypeEnum.Detector && WisselDetectoren.Contains(sd1))
            ///{
            ///    Wissel1Detector = sd1;
            ///}
            ///if (Wissel2 && Wissel2Type == PrioIngreepInUitDataWisselTypeEnum.Detector && WisselDetectoren.Contains(sd2))
            ///{
            ///    Wissel2Detector = sd2;
            ///}
        }

        private void OnIngangenChanged(IngangenChangedMessage obj)
        {
            ///var sd1 = "";
            ///var sd2 = "";
            ///if (Wissel1 && Wissel1Type == PrioIngreepInUitDataWisselTypeEnum.Ingang)
            ///{
            ///    sd1 = Wissel1Input;
            ///}
            ///if (Wissel2 && Wissel2Type == PrioIngreepInUitDataWisselTypeEnum.Ingang)
            ///{
            ///    sd2 = Wissel2Input;
            ///}
            ///
            ///WisselInputs.Clear();
            ///foreach (var seld in DataAccess.TLCGenControllerDataProvider.Default.Controller.Ingangen.Where(x => x.Type == IngangTypeEnum.WisselContact))
            ///{
            ///    WisselInputs.Add(seld.Naam);
            ///}
            ///
            ///if (Wissel1 && Wissel1Type == PrioIngreepInUitDataWisselTypeEnum.Ingang && WisselInputs.Contains(sd1))
            ///{
            ///    Wissel1Input = sd1;
            ///}
            ///if (Wissel2 && Wissel2Type == PrioIngreepInUitDataWisselTypeEnum.Ingang && WisselInputs.Contains(sd2))
            ///{
            ///    Wissel2Input = sd2;
            ///}
        }

        #endregion // TLCGen events
        
        #region Constructor

        public PrioIngreepMeldingenListViewModel(string naam, PrioIngreepInUitMeldingTypeEnum type, PrioIngreepMeldingenDataModel prioIngreepMassaDetectieData)
        {
            PrioIngreepMeldingenData = prioIngreepMassaDetectieData;
            switch (type)
            {
                case PrioIngreepInUitMeldingTypeEnum.Inmelding:
                    Meldingen = new ObservableCollection<PrioIngreepInUitMeldingViewModel>(prioIngreepMassaDetectieData.Inmeldingen.Select(x => new PrioIngreepInUitMeldingViewModel(x, this)));
                    break;
                case PrioIngreepInUitMeldingTypeEnum.Uitmelding:
                    Meldingen = new ObservableCollection<PrioIngreepInUitMeldingViewModel>(prioIngreepMassaDetectieData.Uitmeldingen.Select(x => new PrioIngreepInUitMeldingViewModel(x, this)));
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