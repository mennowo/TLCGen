using GalaSoft.MvvmLight;
using System;
using TLCGen.Integrity;
using TLCGen.Messaging.Messages;
using TLCGen.ModelManagement;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class SelectieveDetectorViewModel : ViewModelBase, IComparable
    {
        public SelectieveDetectorModel SelectieveDetector { get; }

        public string Naam
        {
            get => SelectieveDetector.Naam;
            set
            {
                var old = SelectieveDetector.Naam;
                if (TLCGenModelManager.Default.IsElementIdentifierUnique(Models.Enumerations.TLCGenObjectTypeEnum.Input, value))
                {
                    SelectieveDetector.Naam = value;
                    MessengerInstance.Send(new NameChangingMessage(TLCGenObjectTypeEnum.SelectieveDetector, old, value));
                }
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public string Omschrijving
        {
            get => SelectieveDetector.Omschrijving;
            set
            {
                SelectieveDetector.Omschrijving = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public SelectieveDetectorTypeEnum SdType
        {
            get => SelectieveDetector.SdType;
            set
            {
                SelectieveDetector.SdType = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int? TOG
        {
            get => SelectieveDetector.TOG;
            set
            {
                SelectieveDetector.TOG = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int? TBG
        {
            get => SelectieveDetector.TBG;
            set
            {
                SelectieveDetector.TBG = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }


        public int? TFL
        {
            get => SelectieveDetector.TFL;
            set
            {
                SelectieveDetector.TFL = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int? CFL
        {
            get => SelectieveDetector.CFL;
            set
            {
                SelectieveDetector.CFL = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public bool Dummy
        {
            get => SelectieveDetector.Dummy;
            set
            {
                SelectieveDetector.Dummy = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public SelectieveDetectorViewModel(SelectieveDetectorModel ingang)
        {
            SelectieveDetector = ingang;
        }

        public object GetItem()
        {
            return SelectieveDetector;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is SelectieveDetectorViewModel d2)) throw new InvalidCastException();
            return TLCGenIntegrityChecker.CompareDetectors(Naam, d2.Naam, null, null);
        }
    }
}
