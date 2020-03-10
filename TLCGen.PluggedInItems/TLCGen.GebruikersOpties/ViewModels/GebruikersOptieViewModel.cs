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
            get => GebruikersOptie.Naam;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && NameSyntaxChecker.IsValidCName(value))
                {
                    if (TLCGenModelManager.Default.IsElementIdentifierUnique(ObjectType, value))
                    {
                        var oldname = GebruikersOptie.Naam;
                        GebruikersOptie.Naam = value;

                        // Notify the messenger
                        Messenger.Default.Send(new NameChangingMessage(ObjectType, oldname, value));
                    }
                }
                RaisePropertyChanged<object>(nameof(Naam), broadcast: true);
            }
        }

        public TLCGenObjectTypeEnum ObjectType { get; set; }

        public CCOLElementTypeEnum Type
        {
            get => GebruikersOptie.Type;
            set
            {
                GebruikersOptie.Type = value;
                RaisePropertyChanged<object>(nameof(Type), broadcast: true);
            }
        }

        public int? Instelling
        {
            get => GebruikersOptie.Instelling;
            set
            {
                GebruikersOptie.Instelling = value;
                RaisePropertyChanged<object>(nameof(Instelling), broadcast: true);
            }
        }

        public bool Dummy
        {
            get => GebruikersOptie.Dummy;
            set
            {
                GebruikersOptie.Dummy = value;
                RaisePropertyChanged<object>(nameof(Dummy), broadcast: true);
            }
        }

        public string Commentaar
        {
            get => GebruikersOptie.Commentaar;
            set
            {
                GebruikersOptie.Commentaar = value;
                RaisePropertyChanged<object>(nameof(Commentaar), broadcast: true);
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
