using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Generators.CCOL.CodeGeneration;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Helpers;
using TLCGen.ViewModels;

namespace TLCGen.Generators.CCOL
{
    public class CCOLGeneratorSettingsViewModel : ViewModelBase
    {
        #region Fields

        private CCOLGeneratorSettingsModel _Settings;
        private CCOLGenerator _Generator;

        #endregion // Fields

        #region Properties

        public CCOLGeneratorVisualSettingsModel VisualSettings
        {
            get
            {
                return _Settings?.VisualSettings;
            }
        }

        public List<CodePieceSettingsTuple<string, CCOLGeneratorClassWithSettingsModel>> CodePieceGeneratorSettings
        {
            get
            {
                return _Settings?.CodePieceGeneratorSettings;
            }
        }

        public List<CCOLGeneratorCodeStringSettingModel> Prefixes
        {
            get
            {
                return _Settings?.Prefixes;
            }
        }

        public string TabSpace
        {
            get { return _Settings.TabSpace; }
            set
            {
                _Settings.TabSpace = value;
                OnPropertyChanged("TabSpace");
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _SaveSettingsCommand;
        public ICommand SaveSettingsCommand
        {
            get
            {
                if (_SaveSettingsCommand == null)
                {
                    _SaveSettingsCommand = new RelayCommand(SaveSettingsCommand_Executed, SaveSettingsCommand_CanExecute);
                }
                return _SaveSettingsCommand;
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

        #endregion // Commands

        #region Command Functionality
        #endregion // Command Functionality

        #region Constructor

        public CCOLGeneratorSettingsViewModel(CCOLGeneratorSettingsModel settings, CCOLGenerator generator)
        {
            _Settings = settings;
            _Generator = generator;
        }

        #endregion // Constructor
    }
}
