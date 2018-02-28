using GalaSoft.MvvmLight;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class OVIngreepMassaDetectieMeldingVoorwaardenSetViewModel : ViewModelBase, IViewModelWithItem
    {
        private RelayCommand _removeSetCommand;
        private RelayCommand _addVoorwaardeCommand;

        public OVIngreepMassaDetectieMeldingVoorwaardenSetModel OVIngreepMassaDetectieMeldingVoorwaardenSet { get; }

        public string Omschrijving
        {
            get => OVIngreepMassaDetectieMeldingVoorwaardenSet.Omschrijving;
            set
            {
                OVIngreepMassaDetectieMeldingVoorwaardenSet.Omschrijving = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public ObservableCollectionAroundList<OVIngreepMassaDetectieMeldingVoorwaardeViewModel, OVIngreepMassaDetectieMeldingVoorwaardeModel> Voorwaarden { get; }

        public ICommand RemoveSetCommand => _removeSetCommand ?? (_removeSetCommand = new RelayCommand(RemoveMeldingCommand_Executed));
        public ICommand AddVoorwaardeCommand => _addVoorwaardeCommand ?? (_addVoorwaardeCommand = new RelayCommand(AddVoorwaardenSetCommand_Executed));

        private void RemoveMeldingCommand_Executed(object prm)
        {
            MessengerInstance.Send(new OVIngreepMassaDetectieObjectActionMessage(this, false, true));
        }

        private void AddVoorwaardenSetCommand_Executed(object prm)
        {
            MessengerInstance.Send(new OVIngreepMassaDetectieObjectActionMessage(this, true, false));
        }


        public object GetItem()
        {
            return OVIngreepMassaDetectieMeldingVoorwaardenSet;
        }

        public OVIngreepMassaDetectieMeldingVoorwaardenSetViewModel(OVIngreepMassaDetectieMeldingVoorwaardenSetModel ovIngreepMassaDetectieMeldingVoorwaardenSet)
        {
            OVIngreepMassaDetectieMeldingVoorwaardenSet = ovIngreepMassaDetectieMeldingVoorwaardenSet;
            Voorwaarden = new ObservableCollectionAroundList<OVIngreepMassaDetectieMeldingVoorwaardeViewModel, OVIngreepMassaDetectieMeldingVoorwaardeModel>(ovIngreepMassaDetectieMeldingVoorwaardenSet.Voorwaarden);
        }
    }
}
