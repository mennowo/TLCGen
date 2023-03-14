using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.ModelManagement;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels;

public class KruispuntArmViewModel : ViewModelBase, IViewModelWithItem
{
    #region Fields

    

    #endregion // Fields

    #region Properties

    public KruispuntArmModel Model { get; }
    
    public string Naam
    {
        get => Model.Naam;
        set
        {
            if (!string.IsNullOrWhiteSpace(value) && NameSyntaxChecker.IsValidCName(value))
            {
                if (TLCGenModelManager.Default.IsElementIdentifierUnique(TLCGenObjectTypeEnum.KruispuntArm, value))
                {
                    var oldname = Model.Naam;
                    Model.Naam = value;

                    // Notify the messenger
                    MessengerInstance.Send(new NameChangingMessage(TLCGenObjectTypeEnum.KruispuntArm, oldname, value));
                }
            }
            RaisePropertyChanged<object>(broadcast: true);
        }
    }

    public string Omschrijving
    {
        get => Model.Omschrijving;
        set
        {
            Model.Omschrijving = value;
            RaisePropertyChanged<object>(broadcast: true);
        }
    }

    #endregion // Properties

    #region IViewModelWithItem
    
    public object GetItem()
    {
        return Model;
    }

    #endregion // IViewModelWithItem

    #region Constructor

    public KruispuntArmViewModel(KruispuntArmModel model)
    {
        Model = model;
    }

    #endregion // Constructor
}
