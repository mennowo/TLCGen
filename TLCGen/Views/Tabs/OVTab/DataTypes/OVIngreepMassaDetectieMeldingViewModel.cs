using GalaSoft.MvvmLight;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class OVIngreepMassaDetectieMeldingViewModel : ViewModelBase, IViewModelWithItem
    {
        private RelayCommand _removeMeldingCommand;
        private RelayCommand _addVoorwaardenSetCommand;

        public OVIngreepMassaDetectieMeldingModel OVIngreepMassaDetectieMelding { get; }

        public string Omschrijving
        {
            get => OVIngreepMassaDetectieMelding.Omschrijving;
            set
            {
                OVIngreepMassaDetectieMelding.Omschrijving = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public OVIngreepMassaDetectieMeldingType Type
        {
            get => OVIngreepMassaDetectieMelding.Type;
            set
            {
                OVIngreepMassaDetectieMelding.Type = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public ObservableCollectionAroundList<OVIngreepMassaDetectieMeldingVoorwaardenSetViewModel, OVIngreepMassaDetectieMeldingVoorwaardenSetModel> VoorwaardenSets { get; }

        public ICommand RemoveMeldingCommand => _removeMeldingCommand ?? (_removeMeldingCommand = new RelayCommand(RemoveMeldingCommand_Executed));
        public ICommand AddVoorwaardenSetCommand => _addVoorwaardenSetCommand ?? (_addVoorwaardenSetCommand = new RelayCommand(AddVoorwaardenSetCommand_Executed));

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
            return OVIngreepMassaDetectieMelding;
        }

        public OVIngreepMassaDetectieMeldingViewModel(OVIngreepMassaDetectieMeldingModel oVIngreepMassaDetectieMelding)
        {
            OVIngreepMassaDetectieMelding = oVIngreepMassaDetectieMelding;
            VoorwaardenSets = new ObservableCollectionAroundList<OVIngreepMassaDetectieMeldingVoorwaardenSetViewModel, OVIngreepMassaDetectieMeldingVoorwaardenSetModel>(oVIngreepMassaDetectieMelding.VoorwaardenSets);
        }
    }
}
