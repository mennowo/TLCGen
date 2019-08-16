using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.ModelManagement;
using TLCGen.Models.Enumerations;

namespace TLCGen.GebruikersOpties
{
    public class GebruikersOptieViewModel : ViewModelBase, IViewModelWithItem
    {
        #region Fields
        #endregion // Fields

        #region Properties

        public GebruikersOptieModel GebruikersOptie { get; }

        public string Naam
        {
            get { return GebruikersOptie.Naam; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && NameSyntaxChecker.IsValidCName(value))
                {
                    if (TLCGenModelManager.Default.IsElementIdentifierUnique(ObjectType, value))
                    {
                        string oldname = GebruikersOptie.Naam;
                        GebruikersOptie.Naam = value;

                        // Notify the messenger
                        Messenger.Default.Send(new NameChangingMessage(ObjectType, oldname, value));
                    }
                }
                RaisePropertyChanged<object>("Naam", broadcast: true);
            }
        }

        public TLCGenObjectTypeEnum ObjectType { get; set; }

        public CCOLElementTypeEnum Type
        {
            get { return GebruikersOptie.Type; }
            set
            {
                GebruikersOptie.Type = value;
                RaisePropertyChanged<object>("Type", broadcast: true);
            }
        }

        public int? Instelling
        {
            get { return GebruikersOptie.Instelling; }
            set
            {
                GebruikersOptie.Instelling = value;
                RaisePropertyChanged<object>("Instelling", broadcast: true);
            }
        }
        public string Commentaar
        {
            get { return GebruikersOptie.Commentaar; }
            set
            {
                GebruikersOptie.Commentaar = value;
                RaisePropertyChanged<object>("Commentaar", broadcast: true);
            }
        }

        #endregion Properties

        #region Commands

        #endregion // Commands

        #region Command functionality

        #endregion // Command functionality

        #region Private methods

        #endregion // Private methods

        #region Public methods

        #endregion // Public Methods

        #region IViewModelWithItem

        public object GetItem()
        {
            return GebruikersOptie;
        }

        #endregion // IViewModelWithItem

        #region Constructor

        public GebruikersOptieViewModel(GebruikersOptieModel gebruikersoptie)
        {
            GebruikersOptie = gebruikersoptie;
        }

        #endregion // Constructor
    }
}
