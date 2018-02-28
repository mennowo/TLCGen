using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class OVIngreepMassaDetectieMeldingVoorwaardeViewModel : ViewModelBase, IViewModelWithItem
    {
        private RelayCommand _removeVoorwaardeCommand;
        
        public OVIngreepMassaDetectieMeldingVoorwaardeModel OVIngreepMassaDetectieMeldingVoorwaarde { get; }

        public ObservableCollection<string> Detectoren { get; }
        public ObservableCollection<string> Wissels { get; }

        public string SelectedDetector
        {
            get => OVIngreepMassaDetectieMeldingVoorwaarde.Detector;
            set
            {
                if(value != null)
                {
                    OVIngreepMassaDetectieMeldingVoorwaarde.Detector = value;
                }
                RaisePropertyChanged();
            }
        }

        public string SelectedWissel
        {
            get => OVIngreepMassaDetectieMeldingVoorwaarde.WisselContact;
            set
            {
                if (value != null)
                {
                    OVIngreepMassaDetectieMeldingVoorwaarde.WisselContact = value;
                }
                RaisePropertyChanged();
            }
        }

        public string Omschrijving
        {
            get => OVIngreepMassaDetectieMeldingVoorwaarde.Omschrijving;
            set
            {
                OVIngreepMassaDetectieMeldingVoorwaarde.Omschrijving = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public OVIngreepMassaDetectieMeldingVoorwaardeTypeEnum Type
        {
            get => OVIngreepMassaDetectieMeldingVoorwaarde.Type;
            set
            {
                OVIngreepMassaDetectieMeldingVoorwaarde.Type = value;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(TypeIsLus));
                RaisePropertyChanged(nameof(TypeIsWissel));
            }
        }

        public bool TypeIsLus =>
            Type != OVIngreepMassaDetectieMeldingVoorwaardeTypeEnum.WisselContactAf &&
            Type != OVIngreepMassaDetectieMeldingVoorwaardeTypeEnum.WisselContactOp;

        public bool TypeIsWissel => !TypeIsLus;

        public ICommand RemoveVoorwaardeCommand => _removeVoorwaardeCommand ?? (_removeVoorwaardeCommand = new RelayCommand(RemoveVoorwaardeCommand_Executed));

        private void RemoveVoorwaardeCommand_Executed(object prm)
        {
            MessengerInstance.Send(new OVIngreepMassaDetectieObjectActionMessage(this, false, true));
        }

        public object GetItem()
        {
            return OVIngreepMassaDetectieMeldingVoorwaarde;
        }

        private void OnDetectorenChanged(DetectorenChangedMessage dmsg)
        {
            var msg = new OVIngreepMassaDetectieObjectNeedsFaseCyclusMessage(this);
            MessengerInstance.Send(msg);
            if (msg.FaseCyclus == null) return;

            var sd = SelectedDetector;

            var fc = DataAccess.TLCGenControllerDataProvider.Default.Controller.Fasen.Where(x => x.Naam == msg.FaseCyclus).FirstOrDefault();
            if (fc != null)
            {
                Detectoren.Clear();
                foreach(var d in fc.Detectoren)
                {
                    Detectoren.Add(d.Naam);
                }
            }

            if (Detectoren.Contains(sd))
            {
                SelectedDetector = sd;
            }
        }

        public OVIngreepMassaDetectieMeldingVoorwaardeViewModel(OVIngreepMassaDetectieMeldingVoorwaardeModel ovIngreepMassaDetectieMeldingVoorwaarde)
        {
            OVIngreepMassaDetectieMeldingVoorwaarde = ovIngreepMassaDetectieMeldingVoorwaarde;
            Detectoren = new ObservableCollection<string>();
            MessengerInstance.Register<DetectorenChangedMessage>(this, OnDetectorenChanged);
            OnDetectorenChanged(null);
        }
    }
}
