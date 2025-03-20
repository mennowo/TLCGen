using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.ModelManagement;
using TLCGen.Models.Enumerations;

namespace TLCGen.GebruikersOpties
{

    public class GebruikersOptieWithIOViewModel : ObservableObjectEx, IViewModelWithItem
    {
        #region Fields

        private GebruikersOptieWithIOModel _GebruikersOptieWithOI;

        #endregion // Fields

        #region Properties

        public GebruikersOptieWithIOModel GebruikersOptieWithIO => _GebruikersOptieWithOI;

        public string Naam
        {
            get => _GebruikersOptieWithOI.Naam;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && NameSyntaxChecker.IsValidCName(value))
                {
                    if (TLCGenModelManager.Default.IsElementIdentifierUnique(ObjectType, value))
                    {
                        var oldname = _GebruikersOptieWithOI.Naam;
                        _GebruikersOptieWithOI.Naam = value;

                        // Notify the messenger
                        WeakReferenceMessengerEx.Default.Send(new NameChangingMessage(ObjectType, oldname, value));
                    }
                }
                OnPropertyChanged(broadcast: true);
            }
        }

        public bool Multivalent
        {
            get => _GebruikersOptieWithOI.Multivalent;
            set
            {
                _GebruikersOptieWithOI.Multivalent = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public bool Dummy
        {
            get => _GebruikersOptieWithOI.Dummy;
            set
            {
                _GebruikersOptieWithOI.Dummy = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public TLCGenObjectTypeEnum ObjectType { get; set; }

        public string Commentaar
        {
            get => _GebruikersOptieWithOI.Commentaar;
            set
            {
                _GebruikersOptieWithOI.Commentaar = value;
                OnPropertyChanged(broadcast: true);
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
            return _GebruikersOptieWithOI;
        }

        #endregion // IViewModelWithItem

        #region Constructor

        public GebruikersOptieWithIOViewModel(GebruikersOptieWithIOModel gebruikersoptiewithoi)
        {
            _GebruikersOptieWithOI = gebruikersoptiewithoi;
        }

        #endregion // Constructor
    }
}
