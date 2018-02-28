using GalaSoft.MvvmLight;
using System.Linq;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class OVIngreepMassaDetectieDataViewModel : ViewModelBase
    {
        #region Fields

        private RelayCommand _addMeldingCommand;
        private object _selectedObject;

        #endregion // Fields

        #region Properties

        public OVIngreepMassaDetectieDataModel OVIngreepMassaDetectieData { get; }

        public ObservableCollectionAroundList<OVIngreepMassaDetectieMeldingViewModel, OVIngreepMassaDetectieMeldingModel> Meldingen { get; }

        public object SelectedObject
        {
            get => _selectedObject;
            set
            {
                _selectedObject = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(SelectedObjectDescription));
            }
        }

        public string SelectedObjectDescription
        {
            get
            {
                switch (SelectedObject)
                {
                    case OVIngreepMassaDetectieMeldingViewModel melding:
                        return "Melding data";
                    case OVIngreepMassaDetectieMeldingVoorwaardenSetViewModel set:
                        return "Voorwaarden set data";
                    case OVIngreepMassaDetectieMeldingVoorwaardeViewModel voorwaarde:
                        return "Voorwaarde data";
                }
                return "Geen selectie";
            }
        }

        #endregion // Properties

        #region Commands

        public ICommand AddMeldingCommand => _addMeldingCommand ?? (_addMeldingCommand = new RelayCommand(AddMeldingCommand_Executed));

        #endregion // Commands

        #region Command functionality

        private void AddMeldingCommand_Executed(object prm)
        {
            Meldingen.Add(new OVIngreepMassaDetectieMeldingViewModel(new OVIngreepMassaDetectieMeldingModel
            {
                Omschrijving = "Massa detectie in/uit melding"
            }));
        }

        private bool AddVoorwaardenSetCommand_CanExecute(object prm)
        {
            return SelectedObject != null && SelectedObject is OVIngreepMassaDetectieMeldingViewModel;
        }

        #endregion // Command functionality

        private void OnObjectAction(OVIngreepMassaDetectieObjectActionMessage msg)
        {
            RaisePropertyChanged<object>(broadcast: true);
            switch (msg.Object)
            {
                case OVIngreepMassaDetectieMeldingViewModel melding:
                    if (msg.Add)
                    {
                        melding.VoorwaardenSets.Add(new OVIngreepMassaDetectieMeldingVoorwaardenSetViewModel(new OVIngreepMassaDetectieMeldingVoorwaardenSetModel
                        {
                            Omschrijving = "Voorwaarden set"
                        }));
                    }
                    if (msg.Remove)
                    {
                        Meldingen.Remove(melding);
                        if (SelectedObject == melding) SelectedObject = null;
                    }
                    break;
                case OVIngreepMassaDetectieMeldingVoorwaardenSetViewModel set:
                    if (msg.Add)
                    {
                        set.Voorwaarden.Add(new OVIngreepMassaDetectieMeldingVoorwaardeViewModel(new OVIngreepMassaDetectieMeldingVoorwaardeModel
                        {
                            Omschrijving = "Voorwaarde"
                        }));
                    }
                    if (msg.Remove)
                    {
                        foreach(var m in Meldingen)
                        {
                            if (m.VoorwaardenSets.Contains(set))
                            {
                                m.VoorwaardenSets.Remove(set);
                                if (SelectedObject == set) SelectedObject = null;
                                return;
                            }
                        }
                    }
                    break;
                case OVIngreepMassaDetectieMeldingVoorwaardeViewModel voorwaarde:
                    if (msg.Remove)
                    {
                        foreach (var m in Meldingen)
                        {
                            foreach(var s in m.VoorwaardenSets)
                            {
                                if (s.Voorwaarden.Contains(voorwaarde))
                                {
                                    s.Voorwaarden.Remove(voorwaarde);
                                    if (SelectedObject == voorwaarde) SelectedObject = null;
                                    return;
                                }
                            }
                        }
                    }
                    break;
            }
        }

        #region Constructor

        public OVIngreepMassaDetectieDataViewModel(OVIngreepMassaDetectieDataModel ovIngreepMassaDetectieData)
        {
            OVIngreepMassaDetectieData = ovIngreepMassaDetectieData;
            Meldingen = new ObservableCollectionAroundList<OVIngreepMassaDetectieMeldingViewModel, OVIngreepMassaDetectieMeldingModel>(ovIngreepMassaDetectieData.Meldingen);

            MessengerInstance.Register<OVIngreepMassaDetectieObjectActionMessage>(this, OnObjectAction);
        }

        #endregion // Constructor
    }
}
