using System.Text.RegularExpressions;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.ModelManagement;

namespace TLCGen.ViewModels
{
    public class GroentijdenSetViewModel : ObservableObject
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
WeakReferenceMessenger.Default.Send(new NameChangingMessage(TLCGenObjectTypeEnum.GroenTijdenSet, oldname, value));
                    }
                }
                OnPropertyChanged();
            }
        }

        public GroentijdenTypeEnum Type
        {
            get => _GroentijdenSet.Type;
            set
            {
                _GroentijdenSet.Type = value;
                if (Regex.IsMatch(Naam, @"(M|V)G[0-9]+"))
                {
                    switch (value)
                    {
                        case GroentijdenTypeEnum.VerlengGroentijden:
                            Naam = "VG" + Naam.Substring(2);
                            break;
                        case GroentijdenTypeEnum.MaxGroentijden:
                            Naam = "MG" + Naam.Substring(2);
                            break;
                    }
                }

                OnPropertyChanged(nameof(Naam));
                OnPropertyChanged();
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
