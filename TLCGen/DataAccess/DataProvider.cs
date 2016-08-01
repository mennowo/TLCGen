using System;
using System.Collections.Generic;
using System.IO;
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

        public bool LoadController()
        {
            if (!string.IsNullOrWhiteSpace(FileName))
            {
                DeserializeT<ControllerModel> deserializer = new DeserializeT<ControllerModel>();
                if(Path.GetExtension(FileName) == ".tlcgz")
                    Controller = deserializer.DeSerializeGZip(FileName);
                else
                    Controller = deserializer.DeSerialize(FileName);
                if (Controller != null)
                {
                    IDProvider.Controller = Controller;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public void SaveController()
        {
            if (!string.IsNullOrWhiteSpace(FileName))
            {
                SerializeT<ControllerModel> serializer = new SerializeT<ControllerModel>();
                if (Path.GetExtension(FileName) == ".tlcgz")
                    serializer.SerializeGZip(FileName, Controller);
                else
                    serializer.Serialize(FileName, Controller);
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
