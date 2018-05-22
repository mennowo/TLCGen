using GalaSoft.MvvmLight;
using System;
using TLCGen.Helpers;
using TLCGen.ModelManagement;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class IngangViewModel : ViewModelBase, IViewModelWithItem, IComparable
    {
        public IngangModel Ingang { get; }

        public string Naam
        {
            get => Ingang.Naam;
            set
            {
                if(TLCGenModelManager.Default.IsElementIdentifierUnique(Models.Enumerations.TLCGenObjectTypeEnum.Input, value))
                {
                    Ingang.Naam = value;
                }
                RaisePropertyChanged();
            }
        }

        public string Omschrijving
        {
            get => Ingang.Omschrijving;
            set
            {
                Ingang.Omschrijving = value;
                RaisePropertyChanged();
            }
        }

        public IngangTypeEnum Type
        {
            get => Ingang.Type;
            set
            {
                Ingang.Type = value;
                RaisePropertyChanged();
            }
        }

        public IngangViewModel(IngangModel ingang)
        {
            Ingang = ingang;
        }

        public object GetItem()
        {
            return Ingang;
        }

        public int CompareTo(object obj)
        {
            return this.Naam.CompareTo(((IngangViewModel)obj).Naam);
        }
    }
}
