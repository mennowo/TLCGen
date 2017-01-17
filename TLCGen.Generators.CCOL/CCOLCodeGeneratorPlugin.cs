using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TLCGen.CustomPropertyEditors;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.Generators.CCOL
{
    [TLCGenPlugin(TLCGenPluginElems.Generator)]
    public class CCOLCodeGeneratorPlugin : ITLCGenGenerator, ITLCGenPlugMessaging
    {
        #region ITLCGenGenerator

        private UserControl _GeneratorInterface;
        public UserControl GeneratorView
        {
            get
            {
                if (_GeneratorInterface == null)
                {
                    _GeneratorInterface = new CCOLCodeGeneratorView();
                    _MyVM = new CCOLCodeGeneratorViewModel(this);
                    _GeneratorInterface.DataContext = _MyVM;
                }
                return _GeneratorInterface;
            }
        }

        public string GetGeneratorName()
        {
            return "CCOL";
        }

        public string GetGeneratorVersion()
        {
            return "0.11 (alfa)";
        }

        public string GetPluginName()
        {
            return GetGeneratorName();
        }


        public ControllerModel Controller
        {
            get;
            set;
        }

        #endregion // ITLCGenGenerator

        #region ITLCGenPlugMessaging

        public void UpdateTLCGenMessaging()
        {
            Messenger.Default.Register(this, new Action<Messaging.Messages.ControllerFileNameChangedMessage>(OnControllerFileNameChanged));
            Messenger.Default.Register(this, new Action<Messaging.Messages.ControllerDataChangedMessage>(OnControllerDataChanged));
        }

        #endregion // ITLCGenPlugMessaging

        #region Properties

        public string ControllerFileName { get; set; }

        #endregion // Properties

        #region Fields

        private CCOLCodeGeneratorViewModel _MyVM;

        #endregion // Fields

        #region Static Public Methods

        public static string GetVersion()
        {
            return "0.11 (alfa)";
        }

        #endregion // Static Public Methods

        #region Setting Properties

        private string _CCOLIncludesPaden;
        [DisplayName("CCOL include paden")]
        [Description("CCOL include paden")]
        [Category("Visual project settings")]
        [TLCGenCustomSetting(TLCGenCustomSettingAttribute.SettingTypeEnum.Application)]
        public string CCOLIncludesPaden
        {
            get { return _CCOLIncludesPaden; }
            set
            {
                if(!string.IsNullOrWhiteSpace(value) && !value.EndsWith(";"))
                    _CCOLIncludesPaden = value + ";";
                else
                    _CCOLIncludesPaden = value;
            }
        }

        private string _CCOLLibsPath;
        [DisplayName("CCOL library pad")]
        [Description("CCOL library pad")]
        [Category("Visual project settings")]
        [TLCGenCustomSetting(TLCGenCustomSettingAttribute.SettingTypeEnum.Application)]
        [Editor(typeof(FolderEditor), typeof(FolderEditor))]
        public string CCOLLibsPath
        {
            get { return _CCOLLibsPath; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && !value.EndsWith(";"))
                    _CCOLLibsPath = value + ";";
                else
                    _CCOLLibsPath = value;
            }
        }

        private string _CCOLLibs;
        [DisplayName("CCOL libraries")]
        [Description("CCOL libraries (indien van toepassing)")]
        [Category("Visual project settings")]
        [TLCGenCustomSetting(TLCGenCustomSettingAttribute.SettingTypeEnum.Application)]
        public string CCOLLibs
        {
            get { return _CCOLLibs; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && !value.EndsWith(";"))
                    _CCOLLibs = value + ";";
                else
                    _CCOLLibs = value;
            }
        }

        private string _CCOLResPath;
        [DisplayName("CCOL resources pad")]
        [Description("CCOL resources pad")]
        [Category("Visual project settings")]
        [TLCGenCustomSetting(TLCGenCustomSettingAttribute.SettingTypeEnum.Application)]
        [Editor(typeof(FolderEditor), typeof(FolderEditor))]
        public string CCOLResPath
        {
            get { return _CCOLResPath; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && !value.EndsWith(";"))
                    _CCOLResPath = value + ";";
                else
                    _CCOLResPath = value;
            }
        }

        private string _CCOLPreprocessorDefinitions;
        [DisplayName("Preprocessor definities")]
        [Description("Preprocessor definities")]
        [Category("Visual project settings")]
        [TLCGenCustomSetting(TLCGenCustomSettingAttribute.SettingTypeEnum.Application)]
        public string CCOLPreprocessorDefinitions
        {
            get { return _CCOLPreprocessorDefinitions; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && !value.EndsWith(";"))
                    _CCOLPreprocessorDefinitions = value + ";";
                else
                    _CCOLPreprocessorDefinitions = value;
            }
        }

        #endregion // Setting Properties

        #region TLCGen Events

        private void OnControllerFileNameChanged(TLCGen.Messaging.Messages.ControllerFileNameChangedMessage msg)
        {
            ControllerFileName = msg.NewFileName;
        }

        private void OnControllerDataChanged(TLCGen.Messaging.Messages.ControllerDataChangedMessage msg)
        {
        }

        #endregion // TLCGen Events

        #region Constructor

        public CCOLCodeGeneratorPlugin()
        {
        }

        #endregion // Constructor
    }
}
