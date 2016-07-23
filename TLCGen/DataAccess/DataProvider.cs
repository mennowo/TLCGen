using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.DataAccess
{
    public class DataProvider
    {
        #region Fields

        private ControllerModel _Organizer;
        private string _FileName;

        #endregion // Fields

        #region Properties

        public ControllerModel Controller
        {
            get { return _Organizer; }
            set
            {
                _Organizer = value;
            }
        }

        public string FileName
        {
            get { return _FileName; }
            set
            {
                _FileName = value;
            }
        }

        #endregion // Properties

        #region Private methods

        #endregion // Private methods

        #region Public methods

        public void NewOrganizer()
        {
            Controller = new ControllerModel();
            IDProvider.Controller = Controller;
            FileName = null;
        }

        public void LoadController()
        {
            if (!string.IsNullOrWhiteSpace(FileName))
            {
                DeserializeT<ControllerModel> deserializer = new DeserializeT<ControllerModel>();
                Controller = deserializer.DeSerializeGZip(FileName);
                IDProvider.Controller = Controller;
            }
        }

        public void SaveController()
        {
            if (!string.IsNullOrWhiteSpace(FileName))
            {
                SerializeT<ControllerModel> serializer = new SerializeT<ControllerModel>();
                serializer.SerializeGZip(FileName, Controller);
            }
        }

        public void CloseController()
        {
            Controller = null;
            IDProvider.Controller = null;
            FileName = null;
        }

        #endregion // Public methods

        #region Constructor

        public DataProvider()
        {

        }

        #endregion // Constructor
    }
}
