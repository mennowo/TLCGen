﻿using Microsoft.Win32;
using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Windows;
using System.Xml;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Dependencies.Messaging.Messages;
using TLCGen.Helpers;
using TLCGen.Integrity;
using TLCGen.Messaging.Messages;
using TLCGen.Messaging.Requests;
using TLCGen.Models;
using TLCGen.Plugins;
using TLCGen.ModelManagement;

namespace TLCGen.DataAccess
{
    public class TLCGenControllerDataProvider : ITLCGenControllerDataProvider
    {
        #region Fields

        private static readonly object _Locker = new object();
        private static ITLCGenControllerDataProvider _Default;

        private ControllerModel _Controller;

        private XmlDocument _ControllerXml;

	    private Action<object> _setDefaultsAction;
        private bool _controllerHasChanged;

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
            get => _Controller;
	        private set
            {
                _Controller = value;
                WeakReferenceMessengerEx.Default.Send(new ControllerLoadedMessage(Controller));
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

        public XmlDocument ControllerXml => _ControllerXml;

	    /// <summary>
        /// String representation of the currently loaded file.
        /// </summary>
        public string ControllerFileName { get; set; }

        public bool ControllerHasChanged
        {
            get => _controllerHasChanged;
            set
            {
                _controllerHasChanged = value;
                WeakReferenceMessenger.Default.Send(new ControllerDataChangedMessage());
            }
        }

        public ITLCGenGenerator CurrentGenerator { get; set; }

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
	                _setDefaultsAction?.Invoke(Controller.Data);
                    _setDefaultsAction?.Invoke(Controller.PrioData);
                    _setDefaultsAction?.Invoke(Controller.PeriodenData);
				}
                if(Controller.Data.SegmentenDisplayBitmapData.Count == 0)
                {
                    // Force adding the segments; this is because this is a fixed list, 
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
                var lastfilename = ControllerFileName;
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
        public bool OpenController(string controllername = null)
        {
            if (!CheckChanged())
            {
                if (controllername != null)
                {
                    ControllerFileName = controllername;
                }
                else
                {

                    var lastfilename = ControllerFileName;
                    var openFileDialog = new OpenFileDialog
                    {
                        CheckFileExists = true,
                        Filter = "TLCGen files|*.tlc;*.tlcgz"
                    };
                    if (openFileDialog.ShowDialog() == true)
                    {
                        ControllerFileName = openFileDialog.FileName;
                    }
                    else
                    {
                        return false;
                    }
                }
                if(ControllerFileName != null)
                {
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
                        Controller = null; // Must be made null, otherwise plugins handling GetXmlFromDocument()
                                           // may still have a previous controller internally
                        foreach (var pi in TLCGenPluginManager.Default.ApplicationPlugins)
                        {
                            if (pi.Item1.HasFlag(TLCGenPluginElems.XMLNodeWriter))
                            {
                                var writer = (ITLCGenXMLNodeWriter)pi.Item2;
                                writer.GetXmlFromDocument(doc);
                            }
                        }
                        TLCGenModelManager.Default.CorrectXmlDocumentByVersion(doc);
                        var c = TLCGenSerialization.SerializeFromXmlDocument<ControllerModel>(doc);
                        if (!TLCGenModelManager.Default.CheckVersionOrder(c))
                        {
                            return false;
                        }
                        TLCGenModelManager.Default.CorrectModelByVersion(c, ControllerFileName);
                        TLCGenModelManager.Default.PrepareModelForUI(c);
                        Controller = c;
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
                WeakReferenceMessengerEx.Default.Send(new ProcessSynchronisationsRequest());

                // Check data integrity: do not save wrong data
                var s = TLCGenIntegrityChecker.IsControllerDataOK(Controller);
                if (s != null)
                {
                    MessageBox.Show(s + "\n\nRegeling niet opgeslagen.", "Error bij opslaan: fout in regeling");
                    return false;
                }

                // Save data to disk, update saved state
                if (!string.IsNullOrWhiteSpace(ControllerFileName))
                {
                    Controller.Data.TLCGenVersie = Assembly.GetEntryAssembly().GetName().Version.ToString();
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
                    throw new ArgumentNullException("DataProvider filename not set. Cannot save.");
                }

                ControllerHasChanged = false;

                return true;
            }
        }

        public bool SaveControllerAs()
        {
            // Save all changes to model
            // Request to process all synchronisation data from matrix to model
            WeakReferenceMessengerEx.Default.Send(new ProcessSynchronisationsRequest());

            // Check data integrity: do not save wrong data
            var s = TLCGenIntegrityChecker.IsControllerDataOK(Controller);
            if (s != null)
            {
                MessageBox.Show(s + "\n\nRegeling niet opgeslagen.", "Error bij opslaan: fout in regeling");
                return false;
            }

            // Save data to disk, update saved state
            var saveFileDialog = new SaveFileDialog()
            {
                OverwritePrompt = true,
                Filter = "TLCGen files|*.tlc|TLCGen compressed files|*.tlcgz"
            };
            if (!string.IsNullOrWhiteSpace(ControllerFileName))
                saveFileDialog.FileName = ControllerFileName;
            else
            {
                saveFileDialog.FileName = Controller.Data.Naam;
            }
            if (saveFileDialog.ShowDialog() == true)
            {
                var lastfilename = ControllerFileName;
                ControllerFileName = saveFileDialog.FileName;

                SaveController();
                
                ControllerHasChanged = false;
                WeakReferenceMessengerEx.Default.Send(new ControllerFileNameChangedMessage(ControllerFileName, lastfilename ?? ""));

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
            WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
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
                var r = MessageBox.Show(Application.Current.MainWindow ?? throw new InvalidOperationException(), "Wijzigingen opslaan?", "De regeling is gewijzigd. Opslaan?", MessageBoxButton.YesNoCancel);
                if (r == MessageBoxResult.Yes)
                {
                    SaveController();
                    if (ControllerHasChanged)
                        return true;
                }
                else if (r == MessageBoxResult.Cancel)
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

	    public void InjectDefaultAction(Action<object> setDefaultsAction)
	    {
		    _setDefaultsAction = setDefaultsAction;
	    }

#if DEBUG
        public bool OpenDebug()
        {
            ControllerFileName = @"C:\Users\NL33478\Documents\Rotterdam - Vaanweg Oldegaerde\75040.tlc";
            if (!File.Exists(ControllerFileName))
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
            WeakReferenceMessengerEx.Default.Send(new ControllerFileNameChangedMessage(Default.ControllerFileName, null));
            return true;
        }
#endif

        public static void OverrideDefault(ITLCGenControllerDataProvider provider)
        {
            _Default = provider;
        }

        #endregion // Public methods

        #region Static Methods

        public static void Reset()
        {
            _Default = null;
        }

        #endregion // Static Methods

        #region Private Methods
        
        #endregion // Private Methods

        #region Constructor

        static TLCGenControllerDataProvider()
        {
        }

        #endregion // Constructor
    }
}
