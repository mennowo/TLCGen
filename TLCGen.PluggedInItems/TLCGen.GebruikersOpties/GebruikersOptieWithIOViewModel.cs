using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Messaging.Requests;
using TLCGen.Messaging;
using TLCGen.ModelManagement;

namespace TLCGen.GebruikersOpties
{
    public class GebruikersOptieWithIOViewModel : ViewModelBase, IViewModelWithItem
    {
        #region Fields

        private GebruikersOptieWithIOModel _GebruikersOptieWithOI;

        #endregion // Fields

        #region Properties

        public GebruikersOptieWithIOModel GebruikersOptieWithOI
        {
            get
            {
                return _GebruikersOptieWithOI;
            }
        }

        public string Naam
        {
            get { return _GebruikersOptieWithOI.Naam; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && NameSyntaxChecker.IsValidName(value))
                {
                    if (TLCGenModelManager.Default.IsElementIdentifierUnique(ObjectType, value))
                    {
                        string oldname = _GebruikersOptieWithOI.Naam;
                        _GebruikersOptieWithOI.Naam = value;

                        // Notify the messenger
                        Messenger.Default.Send(new NameChangingMessage(ObjectType, oldname, value));
                    }
                }
                RaisePropertyChanged<object>("Naam", broadcast: true);
            }
        }

        public TLCGenObjectTypeEnum ObjectType { get; set; }

        public string Commentaar
        {
            get { return _GebruikersOptieWithOI.Commentaar; }
            set
            {
                _GebruikersOptieWithOI.Commentaar = value;
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
