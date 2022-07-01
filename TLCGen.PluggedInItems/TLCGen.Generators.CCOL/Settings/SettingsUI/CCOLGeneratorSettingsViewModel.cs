using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using TLCGen.Generators.CCOL.CodeGeneration;
using TLCGen.Generators.CCOL.Settings;

namespace TLCGen.Generators.CCOL
{
    public class CCOLGeneratorSettingsViewModel : ViewModelBase
    {
        #region Fields

        private CCOLGeneratorSettingsModel _Settings;
        private CCOLGenerator _Generator;
        private string _generatorOrder;

        #endregion // Fields

        #region Properties

        public CCOLGeneratorVisualSettingsModel VisualSettings => _Settings?.VisualSettings;

        public CCOLGeneratorVisualSettingsModel VisualSettingsCCOL9 => _Settings?.VisualSettingsCCOL9;

        public CCOLGeneratorVisualSettingsModel VisualSettingsCCOL95 => _Settings?.VisualSettingsCCOL95;

        public CCOLGeneratorVisualSettingsModel VisualSettingsCCOL100 => _Settings?.VisualSettingsCCOL100;

        public CCOLGeneratorVisualSettingsModel VisualSettingsCCOL110 => _Settings?.VisualSettingsCCOL110;
        
        public CCOLGeneratorVisualSettingsModel VisualSettingsCCOL120 => _Settings?.VisualSettingsCCOL120;

        public List<CodePieceSettingsTuple<string, CCOLGeneratorClassWithSettingsModel>> CodePieceGeneratorSettings => _Settings?.CodePieceGeneratorSettings;

        public List<CCOLGeneratorCodeStringSettingModel> Prefixes => _Settings?.Prefixes;

        public string TabSpace
        {
            get => _Settings.TabSpace;
            set
            {
                _Settings.TabSpace = value;
                RaisePropertyChanged("TabSpace");
            }
        }

        public string GeneratorOrder
        {
            get => _generatorOrder;
            set
            {
                _generatorOrder = value;
                RaisePropertyChanged();
            }
        }

        #endregion // Properties

        #region Commands

        /* 
		 * For potential future use
		 * 
        RelayCommand _saveSettingsCommand;
        public ICommand SaveSettingsCommand
        {
            get
            {
                if (_saveSettingsCommand == null)
                {
                    _saveSettingsCommand = new RelayCommand(SaveSettingsCommand_Executed, SaveSettingsCommand_CanExecute);
                }
                return _saveSettingsCommand;
            }
        }

        private void SaveSettingsCommand_Executed(object obj)
        {
            throw new NotImplementedException();
        }

        private bool SaveSettingsCommand_CanExecute(object obj)
        {
            throw new NotImplementedException();
        }

		*/

        #endregion // Commands

        #region Command Functionality
        #endregion // Command Functionality

        #region Constructor

        public CCOLGeneratorSettingsViewModel(CCOLGeneratorSettingsModel settings, CCOLGenerator generator)
        {
            _Settings = settings;
            _Generator = generator;
            var t = "";
            foreach (var cpg in CCOLGenerator.OrderedPieceGenerators)
            {
                foreach(var p in cpg.Value)
                {
                    t += $"{cpg.Key.ToString()} [{p.Key}] - {p.Value.GetType().Name}" + Environment.NewLine;
                }
            }
            _generatorOrder = t;
        }

        #endregion // Constructor
    }
}
