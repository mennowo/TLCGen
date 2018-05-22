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
                RaisePropertyChanged();
            }
        }

        public string Omschrijving
        {
            get => SelectieveDetector.Omschrijving;
            set
            {
                SelectieveDetector.Omschrijving = value;
                RaisePropertyChanged();
            }
        }

        public SelectieveDetectorTypeEnum Type
        {
            get => SelectieveDetector.Type;
            set
            {
                SelectieveDetector.Type = value;
                RaisePropertyChanged();
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
