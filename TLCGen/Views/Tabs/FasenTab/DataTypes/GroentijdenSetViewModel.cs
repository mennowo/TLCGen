using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.ModelManagement;

namespace TLCGen.ViewModels
{
    public class GroentijdenSetViewModel : ViewModelBase
    {
        #region Fields
        
        private GroentijdenSetModel _GroentijdenSet;

        #endregion // Fields

        #region Properties

        public string Naam
        {
            get => _GroentijdenSet.Naam;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && NameSyntaxChecker.IsValidCName(value))
                {
                    if (TLCGenModelManager.Default.IsElementIdentifierUnique(TLCGenObjectTypeEnum.GroenTijdenSet, value))
                    {
                        var oldname = _GroentijdenSet.Naam;
                        _GroentijdenSet.Naam = value;

                        // Notify the messenger
                        Messenger.Default.Send(new NameChangingMessage(TLCGenObjectTypeEnum.GroenTijdenSet, oldname, value));
                    }
                }
                _GroentijdenSet.Naam = value;
                RaisePropertyChanged("Naam");
            }
        }

        public GroentijdenTypeEnum Type
        {
            get => _GroentijdenSet.Type;
            set
            {
                _GroentijdenSet.Type = value;
                switch (value)
                {
                    case GroentijdenTypeEnum.VerlengGroentijden:
                        this.Naam = "VG" + this.Naam.Substring(2);
                        break;
                    case GroentijdenTypeEnum.MaxGroentijden:
                        this.Naam = "MG" + this.Naam.Substring(2); 
                        break;
                }
                RaisePropertyChanged("Naam");
                RaisePropertyChanged("Type");
            }
        }

        public GroentijdenSetModel GroentijdenSet => _GroentijdenSet;

        public ObservableCollectionAroundList<GroentijdViewModel, GroentijdModel> Groentijden
        {
            get;
            private set;
        }

        #endregion // Properties

        #region Public methods

        #endregion // Public methods

        #region TLCGen Messaging

        #endregion // TLCGen Messaging

        #region Constructor

        public GroentijdenSetViewModel(GroentijdenSetModel mgsm)
        {
            _GroentijdenSet = mgsm;
            Groentijden = new ObservableCollectionAroundList<GroentijdViewModel, GroentijdModel>(mgsm.Groentijden);
        }

        #endregion // Constructor
    }
}
