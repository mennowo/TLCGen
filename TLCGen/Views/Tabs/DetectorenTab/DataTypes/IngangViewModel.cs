using CommunityToolkit.Mvvm.ComponentModel;
using System;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.ModelManagement;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class IngangViewModel : ObservableObjectEx, IViewModelWithItem, IComparable
    {
        public IngangModel Ingang { get; }

        public string Naam
        {
            get => Ingang.Naam;
            set
            {
                if(TLCGenModelManager.Default.IsElementIdentifierUnique(TLCGenObjectTypeEnum.Input, value))
                {
                    var oldname = Ingang.Naam;
                    Ingang.Naam = value;
                    WeakReferenceMessenger.Default.Send(new NameChangingMessage(TLCGenObjectTypeEnum.Input, oldname, value));
                }
                OnPropertyChanged(broadcast: true);
            }
        }

        public string Omschrijving
        {
            get => Ingang.Omschrijving;
            set
            {
                Ingang.Omschrijving = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public IngangTypeEnum Type
        {
            get => Ingang.Type;
            set
            {
                Ingang.Type = value;
                OnPropertyChanged(broadcast: true);
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
            return string.Compare(this.Naam, ((IngangViewModel)obj).Naam, StringComparison.Ordinal);
        }
    }
}
