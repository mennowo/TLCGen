using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.DataAccess
{
    public static class DataProvider
    {
        #region Fields

        private static ControllerModel _Controller;
        private static string _FileName;

        #endregion // Fields

        #region Properties

        /// <summary>
        /// Holds the Controller model currently loaded in the application.
        /// This property cannot be directly set; use static functions instead.
        /// </summary>
        public static ControllerModel Controller
        {
            get { return _Controller; }
            private set
            {
                _Controller = value;
            }
        }

        /// <summary>
        /// String representation of the currently loaded file.
        /// </summary>
        public static string FileName
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

        /// <summary>
        /// Sets the loaded Controller to a new, empty instance of ControllerModel
        /// </summary>
        public static void SetController()
        {
            Controller = new ControllerModel();
            FileName = null;
        }

        /// <summary>
        /// Sets the loaded Controller to the instance of ControllerModel parsed to the function
        /// </summary>
        public static void SetController(ControllerModel cm)
        {
            Controller = cm;
            FileName = null;
        }

        /// <summary>
        /// Loads the data from the file parsed to the function
        /// </summary>
        /// <param name="filename">A string holding the filename of the file to be loaded.</param>
        /// <returns>True if succesfull, false otherwise</returns>
        public static bool LoadController(string filename)
        {
            FileName = filename;
            return LoadController();
        }

        /// <summary>
        /// Loads the data from the file whose location is stored in the FileName property.
        /// </summary>
        /// <returns>True if succesfull, false otherwise</returns>
        public static bool LoadController()
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
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                throw new NotImplementedException("DataProvider filename not set. Cannot load.");
            }
        }

        /// <summary>
        /// Saves the currently loaded Controller to the file pointed to by property FileName.
        /// </summary>
        public static void SaveController()
        {
            if (!string.IsNullOrWhiteSpace(FileName))
            {
                SerializeT<ControllerModel> serializer = new SerializeT<ControllerModel>();
                if (Path.GetExtension(FileName) == ".tlcgz")
                    serializer.SerializeGZip(FileName, Controller);
                else
                    serializer.Serialize(FileName, Controller);
            }
            else
            {
                throw new NotImplementedException("DataProvider filename not set. Cannot save.");
            }
        }

        /// <summary>
        /// Closes the current Controller, and sets FileName and Controller properties to null.
        /// </summary>
        public static void CloseController()
        {
            Controller = null;
            FileName = null;
        }

        #endregion // Public methods

        #region Constructor

        static DataProvider()
        {

        }

        #endregion // Constructor
    }
}
