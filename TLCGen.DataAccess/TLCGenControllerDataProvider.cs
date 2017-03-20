using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TLCGen.Helpers;
using TLCGen.Integrity;
using TLCGen.Messaging;
using TLCGen.Messaging.Messages;
using TLCGen.Messaging.Requests;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.DataAccess
{
    public class TLCGenControllerDataProvider : ITLCGenControllerDataProvider
    {
        #region Fields

        private static readonly object _Locker = new object();
        private static TLCGenControllerDataProvider _Default;

        private ControllerModel _Controller;
        private string _ControllerFileName;
        private bool _ControllerHasChanged;

        private XmlDocument _ControllerXml;

        #endregion // Fields

        #region Properties

        public static ITLCGenControllerDataProvider Default
        {
            get
            {
                if (_Default == null)
                {
                    lock (_Locker)
                    {
                        if (_Default == null)
                        {
                            _Default = new TLCGenControllerDataProvider();
                        }
                    }
                }
                return _Default;
            }
        }

        /// <summary>
        /// Holds the Controller model currently loaded in the application.
        /// This property cannot be directly set; use static functions instead.
        /// </summary>
        public ControllerModel Controller
        {
            get { return _Controller; }
            private set
            {
                _Controller = value;
                foreach (var pl in TLCGenPluginManager.Default.ApplicationParts)
                {
                    pl.Item2.Controller = value;
                }
                foreach(var pl in TLCGenPluginManager.Default.ApplicationPlugins)
                {
                    pl.Item2.Controller = value;
                }
            }
        }

        public XmlDocument ControllerXml
        {
            get { return _ControllerXml; }
        }

        /// <summary>
        /// String representation of the currently loaded file.
        /// </summary>
        public string ControllerFileName
        {
            get { return _ControllerFileName; }
            set
            {
                _ControllerFileName = value;
            }
        }

        public bool ControllerHasChanged
        {
            get { return _ControllerHasChanged; }
            set
            {
                _ControllerHasChanged = value;
            }
        }

        #endregion // Properties

        #region Public methods

        /// <summary>
        /// Sets the loaded Controller to the instance of ControllerModel parsed to the function
        /// </summary>
        public bool SetController(ControllerModel cm)
        {
            if (!CheckChanged())
            {
                if (cm != null)
                {
                    Controller = cm;
                }
                else
                {
                    Controller = new ControllerModel();
                    Settings.DefaultsProvider.Default.SetDefaultsOnModel(Controller.Data);
                }
                if(Controller.Data.SegmentenDisplayBitmapData.Count == 0)
                {
                    // Force adding the segments; this is because this is fixed list, 
                    // which we cannot add in the constructor for it will cause double items
                    Controller.Data.SetSegmentOutputs();
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Loads a new, empty Controller
        /// </summary>
        /// <returns>True if succesfull, false if not</returns>
        public bool NewController()
        {
            if(!CheckChanged())
            {
                string lastfilename = ControllerFileName;
                ControllerFileName = null;
                SetController(null);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Loads the data from the file whose location is stored in the FileName property.
        /// </summary>
        /// <returns>True if succesfull, false otherwise</returns>
        public bool OpenController()
        {
            if (!CheckChanged())
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.CheckFileExists = true;
                openFileDialog.Filter = "TLCGen files|*.tlc;*.tlcgz";
                if (openFileDialog.ShowDialog() == true)
                {
                    string lastfilename = ControllerFileName;
                    ControllerFileName = openFileDialog.FileName;
                    if (!string.IsNullOrWhiteSpace(ControllerFileName))
                    {
                        var doc = new XmlDocument();
                        if (Path.GetExtension(ControllerFileName) == ".tlcgz")
                        {
                            using (var fs = File.OpenRead(ControllerFileName))
                            {
                                using (var gz = new GZipStream(fs, CompressionMode.Decompress))
                                {
                                    doc.Load(gz);
                                }
                            }
                        }
                        else
                        {
                            doc.Load(ControllerFileName);
                        }
                        foreach (var pi in TLCGenPluginManager.Default.ApplicationPlugins)
                        {
                            if (pi.Item1.HasFlag(TLCGenPluginElems.XMLNodeWriter))
                            {
                                var writer = (ITLCGenXMLNodeWriter)pi.Item2;
                                writer.GetXmlFromDocument(doc);
                            }
                        }
                        Controller = TLCGenSerialization.SerializeFromXmlDocument<ControllerModel>(doc);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool SaveController()
        {
            if (string.IsNullOrWhiteSpace(ControllerFileName))
            {
                return SaveControllerAs();
            }
            else
            {
                // Save all changes to model
                // Request to process all synchronisation data from matrix to model
                Messenger.Default.Send(new ProcessSynchronisationsRequest());

                // Check data integrity: do not save wrong data
                string s = TLCGenIntegrityChecker.IsControllerDataOK(Controller);
                if (s != null)
                {
                    System.Windows.MessageBox.Show(s + "\n\nRegeling niet opgeslagen.", "Error bij opslaan: fout in regeling");
                    return false;
                }

                // Save data to disk, update saved state
                if (!string.IsNullOrWhiteSpace(ControllerFileName))
                {
                    var doc = TLCGenSerialization.SerializeToXmlDocument(Controller);
                    foreach (var pi in TLCGenPluginManager.Default.ApplicationPlugins)
                    {
                        if (pi.Item1.HasFlag(TLCGenPluginElems.XMLNodeWriter))
                        {
                            var writer = (ITLCGenXMLNodeWriter)pi.Item2;
                            writer.SetXmlInDocument(doc);
                        }
                    }

                    if (Path.GetExtension(ControllerFileName) == ".tlcgz")
                    {
                        using (var fs = File.Create(ControllerFileName))
                        {
                            using (var gz = new GZipStream(fs, CompressionMode.Compress))
                            {
                                doc.Save(gz);
                            }
                        }
                    }
                    else
                    {
                        doc.Save(ControllerFileName);
                    }
                }
                else
                {
                    throw new NotImplementedException("DataProvider filename not set. Cannot save.");
                }

                ControllerHasChanged = false;

                return true;
            }
        }

        public bool SaveControllerAs()
        {
            // Save all changes to model
            // Request to process all synchronisation data from matrix to model
            Messenger.Default.Send(new ProcessSynchronisationsRequest());

            // Check data integrity: do not save wrong data
            string s = TLCGenIntegrityChecker.IsControllerDataOK(Controller);
            if (s != null)
            {
                System.Windows.MessageBox.Show(s + "\n\nRegeling niet opgeslagen.", "Error bij opslaan: fout in regeling");
                return false;
            }

            // Save data to disk, update saved state
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.Filter = "TLCGen files|*.tlc|TLCGen compressed files|*.tlcgz";
            if (!string.IsNullOrWhiteSpace(ControllerFileName))
                saveFileDialog.FileName = ControllerFileName;
            if (saveFileDialog.ShowDialog() == true)
            {
                string lastfilename = ControllerFileName;
                ControllerFileName = saveFileDialog.FileName;

                SaveController();
                
                ControllerHasChanged = false;
                Messenger.Default.Send(new ControllerFileNameChangedMessage(ControllerFileName, lastfilename));

                return true;
            }
            return false;
        }

        /// <summary>
        /// Closes the current Controller, and sets FileName and Controller properties to null.
        /// </summary>
        public bool CloseController()
        {
            if (!CheckChanged())
            {
                Controller = null;
                ControllerFileName = null;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sets the loaded Controller to a new, empty instance of ControllerModel
        /// </summary>
        public void SetControllerChanged()
        {
            Messenger.Default.Send(new ControllerDataChangedMessage());
        }

        /// <summary>
        /// Checks wether or not the currently loaded Controller has changes. If it does,
        /// the method offers the user to save them.
        /// </summary>
        /// <returns>True if there are unsaved changes, false if there are not or if
        /// the user decides to discard them.</returns>
        public bool CheckChanged()
        {
            if (Controller != null && ControllerHasChanged)
            {
                System.Windows.MessageBoxResult r = System.Windows.MessageBox.Show("Wijzigingen opslaan?", "De regeling is gewijzigd. Opslaan?", System.Windows.MessageBoxButton.YesNoCancel);
                if (r == System.Windows.MessageBoxResult.Yes)
                {
                    SaveController();
                    if (ControllerHasChanged)
                        return true;
                }
                else if (r == System.Windows.MessageBoxResult.Cancel)
                {
                    return true;
                }
                else
                {
                    ControllerHasChanged = false;
                }
            }
            return false;
        }

#if DEBUG
        public bool OpenDebug()
        {
            ControllerFileName = @"C:\Users\NL33478\Documents\Rotterdam - Vaanweg Oldegaerde\75040.tlc";
            if (!System.IO.File.Exists(ControllerFileName))
            {
                return false;
            }
            if (!string.IsNullOrWhiteSpace(ControllerFileName))
            {
                var doc = new XmlDocument();
                if (Path.GetExtension(ControllerFileName) == ".tlcgz")
                {
                    using (var fs = File.OpenRead(ControllerFileName))
                    {
                        using (var gz = new GZipStream(fs, CompressionMode.Decompress))
                        {
                            doc.Load(gz);
                        }
                    }
                }
                else
                {
                    doc.Load(ControllerFileName);
                }
                foreach (var pi in TLCGenPluginManager.Default.ApplicationPlugins)
                {
                    if (pi.Item1.HasFlag(TLCGenPluginElems.XMLNodeWriter))
                    {
                        var writer = (ITLCGenXMLNodeWriter)pi.Item2;
                        writer.GetXmlFromDocument(doc);
                    }
                }

                Controller = TLCGenSerialization.SerializeFromXmlDocument<ControllerModel>(doc);

                if (Controller != null)
                {
                    _ControllerXml = new XmlDocument();
                    _ControllerXml.Load(ControllerFileName);
                }
                else
                {
                    return false;
                }

            }
            Messenger.Default.Send(new ControllerFileNameChangedMessage(TLCGenControllerDataProvider.Default.ControllerFileName, null));
            return true;
        }
#endif

        #endregion // Public methods

        #region Static Methods

        public static void Reset()
        {
            _Default = null;
        }

        #endregion // Static Methods

        #region Private Methods

#warning TODO!!!
        // public XmlDocument GetControllerXmlData()
        // {
        //     var doc = TLCGenSerialization.SerializeToXmlDocument(_Controller);
        //     foreach (var v in _LoadedPlugins)
        //     {
        //         if (v is ITLCGenXMLNodeWriter)
        //         {
        //             var writer = (ITLCGenXMLNodeWriter)v;
        //             writer.SetXmlInDocument(doc);
        //         }
        //     }
        //     return doc;
        // }
        //
        // public void LoadPluginDataFromXmlDocument(XmlDocument document)
        // {
        //     if (document == null)
        //         return;
        //
        //     foreach (var v in _LoadedPlugins)
        //     {
        //         if (v is ITLCGenXMLNodeWriter)
        //         {
        //             var writer = (ITLCGenXMLNodeWriter)v;
        //             writer.GetXmlFromDocument(document);
        //         }
        //     }
        // }

        #endregion // Private Methods

        #region Constructor

        static TLCGenControllerDataProvider()
        {
        }

        #endregion // Constructor
    }
}
