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
    public class GebruikersOptieViewModel : ViewModelBase, IViewModelWithItem
    {
        #region Fields

        private GebruikersOptieModel _GebruikersOptie;

        #endregion // Fields

        #region Properties

        public string Naam
        {
            get { return _GebruikersOptie.Naam; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && NameSyntaxChecker.IsValidName(value))
                {
                    if (TLCGenModelManager.Default.IsElementIdentifierUnique(ObjectType, value))
                    {
                        string oldname = _GebruikersOptie.Naam;
                        _GebruikersOptie.Naam = value;

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
            get { return _GebruikersOptie.Type; }
            set
            {
                _GebruikersOptie.Type = value;
                RaisePropertyChanged<object>("Type", broadcast: true);
            }
        }

        public int? Instelling
        {
            get { return _GebruikersOptie.Instelling; }
            set
            {
                _GebruikersOptie.Instelling = value;
                RaisePropertyChanged<object>("Instelling", broadcast: true);
            }
        }
        public string Commentaar
        {
            get { return _GebruikersOptie.Commentaar; }
            set
            {
                _GebruikersOptie.Commentaar = value;
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
            return _GebruikersOptie;
        }

        #endregion // IViewModelWithItem

        #region Constructor

        public GebruikersOptieViewModel(GebruikersOptieModel gebruikersoptie)
        {
            _GebruikersOptie = gebruikersoptie;
        }

        #endregion // Constructor
    }
}
