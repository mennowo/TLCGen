using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using TLCGen.Generators.CCOL.CodeGeneration;
using TLCGen.Generators.CCOL.ProjectGeneration;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Helpers;
using TLCGen.Integrity;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.ViewModels;

namespace TLCGen.Generators.CCOL
{
    public class CCOLGeneratorViewModel : ViewModelBase
    {
        #region Fields

        private CCOLCodeGeneratorPlugin _Plugin;
        private CCOLGenerator _CodeGenerator;
        private CCOLVisualProjectGenerator _ProjectGenerator;
        private VisualProjectTypeEnum _VisualProjectType;
        private bool _VisualCBEnabled;

        #endregion // Fields

        #region Properties

        public CCOLGenerator CodeGenerator
        {
            get { return _CodeGenerator; }
        }

        public VisualProjectTypeEnum VisualProjectType
        {
            get { return _VisualProjectType; }
            set
            {
                _VisualProjectType = value;
                OnPropertyChanged("VisualProjectType");
            }
        }

        public bool VisualCBEnabled
        {
            get { return _VisualCBEnabled; }
            set
            {
                _VisualCBEnabled = value;
                OnPropertyChanged("VisualCBEnabled");
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _GenerateCodeCommand;
        public ICommand GenerateCodeCommand
        {
            get
            {
                if (_GenerateCodeCommand == null)
                {
                    _GenerateCodeCommand = new RelayCommand(GenerateCodeCommand_Executed, GenerateCodeCommand_CanExecute);
                }
                return _GenerateCodeCommand;
            }
        }

        RelayCommand _GenerateVisualProjectCommand;
        public ICommand GenerateVisualProjectCommand
        {
            get
            {
                if (_GenerateVisualProjectCommand == null)
                {
                    _GenerateVisualProjectCommand = new RelayCommand(GenerateVisualProjectCommand_Executed, GenerateVisualProjectCommand_CanExecute);
                }
                return _GenerateVisualProjectCommand;
            }
        }

        #endregion // Commands

        #region Command Functionality

        private void GenerateCodeCommand_Executed(object prm)
        {
            var prepreq = new Messaging.Requests.PrepareForGenerationRequest();
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(prepreq);
            string s = TLCGenIntegrityChecker.IsControllerDataOK(_Plugin.Controller);
            if (s == null)
            {
                _CodeGenerator.GenerateSourceFiles(_Plugin.Controller, Path.GetDirectoryName(_Plugin.ControllerFileName));
                Messenger.Default.Send(new ControllerCodeGeneratedMessage());
            }
            else
            {
                MessageBox.Show(s, "Fout in conflictmatrix");
            }
        }

        private bool GenerateCodeCommand_CanExecute(object prm)
        {
            return _Plugin.Controller != null &&
                   _Plugin.Controller.Fasen != null &&
                   _Plugin.Controller.Fasen.Count > 0 &&
                   !string.IsNullOrWhiteSpace(_Plugin.ControllerFileName);
        }

        private void GenerateVisualProjectCommand_Executed(object prm)
        {
            _ProjectGenerator.GenerateVisualStudioProjectFiles(_Plugin, VisualProjectType);
            Messenger.Default.Send(new ControllerProjectGeneratedMessage());
        }

        private bool GenerateVisualProjectCommand_CanExecute(object prm)
        {
            bool b = _Plugin.Controller != null &&
                     !string.IsNullOrWhiteSpace(CCOLGeneratorSettingsProvider.Default.Settings.VisualSettings.CCOLLibsPath) &&
                     !string.IsNullOrWhiteSpace(CCOLGeneratorSettingsProvider.Default.Settings.VisualSettings.CCOLIncludesPaden) &&
                     !string.IsNullOrWhiteSpace(CCOLGeneratorSettingsProvider.Default.Settings.VisualSettings.CCOLResPath) &&
                     !string.IsNullOrWhiteSpace(_Plugin.ControllerFileName);
            VisualCBEnabled = b;
            return b;
        }

        #endregion // Command Functionality

        #region Public Methods

        #endregion // Public Methods

        #region Private Methods

        #endregion // Private Methods

        #region Constructor

        public CCOLGeneratorViewModel(CCOLCodeGeneratorPlugin plugin, CCOLGenerator generator)
        {
            _Plugin = plugin;
            _CodeGenerator = generator;
            _ProjectGenerator = new CCOLVisualProjectGenerator();
        }

        #endregion // Constructor
    }
}
