﻿using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Generators.CCOL.CodeGeneration;
using TLCGen.Generators.CCOL.ProjectGeneration;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Helpers;
using TLCGen.Integrity;
using TLCGen.Messaging.Messages;

namespace TLCGen.Generators.CCOL
{
    public class CCOLGeneratorViewModel : ObservableObject
    {
        #region Fields

        private readonly CCOLCodeGeneratorPlugin _plugin;
        private readonly CCOLGenerator _codeGenerator;
        private readonly CCOLVisualProjectGenerator _projectGenerator;
        private string _SelectedVisualProject;
        private bool _VisualCBEnabled;
        private RelayCommand _generateCodeCommand;
        private RelayCommand _generateVisualProjectCommand;

        #endregion // Fields

        #region Properties

        public CCOLGenerator CodeGenerator => _codeGenerator;

        public ObservableCollection<string> VisualProjects { get; }

        public string SelectedVisualProject
        {
            get => _SelectedVisualProject;
            set
            {
                _SelectedVisualProject = value;
                OnPropertyChanged();
            }
        }

        public bool VisualCBEnabled
        {
            get => _VisualCBEnabled;
            set
            {
                _VisualCBEnabled = value;
                OnPropertyChanged();
            }
        }

        #endregion // Properties

        #region Commands

        public ICommand GenerateCodeCommand
        {
            get
            {
                if (_generateCodeCommand == null)
                {
                    _generateCodeCommand = new RelayCommand(GenerateCodeCommand_Executed, GenerateCodeCommand_CanExecute);
                }
                return _generateCodeCommand;
            }
        }

        public ICommand GenerateVisualProjectCommand
        {
            get
            {
                if (_generateVisualProjectCommand == null)
                {
                    _generateVisualProjectCommand = new RelayCommand(GenerateVisualProjectCommand_Executed, GenerateVisualProjectCommand_CanExecute);
                }
                return _generateVisualProjectCommand;
            }
        }

        #endregion // Commands

        #region Command Functionality

        private void GenerateCodeCommand_Executed()
        {
            if (_plugin.Controller?.Data != null)
            {
                _plugin.Controller.Data.TLCGenVersie = System.Reflection.Assembly.GetEntryAssembly()?.GetName()?.Version?.ToString();
            }
            var prepreq = new Messaging.Requests.PrepareForGenerationRequest(_plugin.Controller);
            WeakReferenceMessenger.Default.Send(prepreq);
            var s = TLCGenIntegrityChecker.IsControllerDataOK(_plugin.Controller);
            if (s == null)
            {
                _codeGenerator.GenerateSourceFiles(_plugin.Controller, Path.GetDirectoryName(_plugin.ControllerFileName));
                WeakReferenceMessenger.Default.Send(new ControllerCodeGeneratedMessage());
            }
            else
            {
                MessageBox.Show(s, "Fout in conflictmatrix");
            }
        }

        private bool GenerateCodeCommand_CanExecute()
        {
            return _plugin?.Controller?.Fasen.Any() == true &&
                   (_plugin.Controller.ModuleMolen.Modules.Any(x2 => x2.Fasen.Any()) ||
                    _plugin.Controller.Data.MultiModuleReeksen && 
                    _plugin.Controller.MultiModuleMolens.Any(x => x.Modules.Any(x2 => x2.Fasen.Any()))) &&
                   _plugin.Controller.GroentijdenSets.Any() &&
                   _plugin.Controller.PeriodenData.DefaultPeriodeGroentijdenSet != null &&
                   !string.IsNullOrWhiteSpace(_plugin.ControllerFileName);
        }

        private void GenerateVisualProjectCommand_Executed()
        {
            var vVer = Regex.Replace(SelectedVisualProject, @"Visual.?([0-9]+).*", "$1");
            if (!int.TryParse(vVer, out var iVer)) return;
            _projectGenerator.GenerateVisualStudioProjectFiles(_plugin, SelectedVisualProject.Replace(" ", "_"), iVer);
            WeakReferenceMessenger.Default.Send(new ControllerProjectGeneratedMessage());
        }

        private bool GenerateVisualProjectCommand_CanExecute()
        {
            var b = _plugin.Controller != null && 
                    !string.IsNullOrWhiteSpace(CCOLGeneratorSettingsProvider.Default.Settings.VisualSettings.CCOLLibsPath) &&
                    !string.IsNullOrWhiteSpace(CCOLGeneratorSettingsProvider.Default.Settings.VisualSettings.CCOLIncludesPaden) &&
                    !string.IsNullOrWhiteSpace(CCOLGeneratorSettingsProvider.Default.Settings.VisualSettings.CCOLResPath) &&
                    !string.IsNullOrWhiteSpace(_plugin.ControllerFileName);
            VisualCBEnabled = b;
            return b && !string.IsNullOrWhiteSpace(SelectedVisualProject);
        }

        #endregion // Command Functionality

        #region Public Methods

        #endregion // Public Methods

        #region Private Methods

        #endregion // Private Methods

        #region Constructor

        public CCOLGeneratorViewModel(CCOLCodeGeneratorPlugin plugin, CCOLGenerator generator)
        {
            _plugin = plugin;
            _codeGenerator = generator;
            _projectGenerator = new CCOLVisualProjectGenerator();

	        VisualProjects = new ObservableCollection<string>();
        }

        #endregion // Constructor
    }
}
