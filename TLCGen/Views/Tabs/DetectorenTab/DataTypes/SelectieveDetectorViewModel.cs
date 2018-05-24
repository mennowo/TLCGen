using GalaSoft.MvvmLight;
using System;
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
                if (TLCGenModelManager.Default.IsElementIdentifierUnique(Models.Enumerations.TLCGenObjectTypeEnum.Input, value))
                {
                    SelectieveDetector.Naam = value;
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
            return this.Naam.CompareTo(((SelectieveDetectorViewModel)obj).Naam);
        }
    }
}
