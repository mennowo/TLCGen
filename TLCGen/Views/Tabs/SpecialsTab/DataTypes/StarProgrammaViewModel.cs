using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.ModelManagement;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class StarProgrammaViewModel : ObservableObjectEx, IViewModelWithItem, IComparable
    {
        #region Properties

        public ObservableCollectionAroundList<StarProgrammaFaseViewModel, StarProgrammaFase> Fasen { get; set; }

        public StarProgrammaModel StarProgramma { get; }

        public string Naam
        {
            get => StarProgramma.Naam;
            set
            {
                var oldName = StarProgramma.Naam;
                if (TLCGenModelManager.Default.IsElementIdentifierUnique(TLCGenObjectTypeEnum.StarProgramma, value))
                {
                    StarProgramma.Naam = value;
                    WeakReferenceMessenger.Default.Send(new NameChangingMessage(TLCGenObjectTypeEnum.StarProgramma, oldName, StarProgramma.Naam));
                }
                OnPropertyChanged(broadcast: true);
            }
        }

        public int Cyclustijd
        {
            get => StarProgramma.Cyclustijd;
            set
            {
                StarProgramma.Cyclustijd = value; 
                OnPropertyChanged(broadcast: true);
            }
        }

        #endregion // Properties

        #region IViewModelWithItem

        public object GetItem() => StarProgramma;

        #endregion // IViewModelWithItem

        #region Constructor

        public StarProgrammaViewModel(StarProgrammaModel starProgramma)
        {
            StarProgramma = starProgramma;
            Fasen = new ObservableCollectionAroundList<StarProgrammaFaseViewModel, StarProgrammaFase>(starProgramma.Fasen);
        }

        #endregion // Constructor

        public int CompareTo(object obj)
        {
            return string.Compare(Naam, ((StarProgrammaViewModel) obj).Naam, StringComparison.Ordinal);
        }
    }
}