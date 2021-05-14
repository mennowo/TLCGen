using System;
using System.Collections.ObjectModel;
using System.Xml;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ModelManagement
{
    public interface ITLCGenModelManager
    {
        ControllerModel Controller { get; set; }
        ObservableCollection<ControllerAlertMessage> ControllerAlerts { get; }

        bool IsElementIdentifierUnique(TLCGenObjectTypeEnum objectType, string identifier, bool vissim = false);
        void InjectDefaultAction(Action<object, string> setDefaultsAction);
        bool CheckVersionOrder(ControllerModel controller);
        void CorrectModelByVersion(ControllerModel controller, string filename);
        void CorrectXmlDocumentByVersion(XmlDocument doc);
        void PrepareModelForUI(ControllerModel controller);
        int ChangeNameOnObject(object obj, string oldName, string newName, TLCGenObjectTypeEnum objectType);
        void ConvertToIntergroen(ControllerModel controller);
        void ConvertToOntruimingstijden(ControllerModel controller);
        void SetSpecialIOPerSignalGroup(ControllerModel controller);
        void AddControllerAlert(ControllerAlertMessage msg);
        void RemoveControllerAlert(string id);
        void UpdateControllerAlerts();
        event EventHandler ControllerAlertsUpdated;
    }
}
