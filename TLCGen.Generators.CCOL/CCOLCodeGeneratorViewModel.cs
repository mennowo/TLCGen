using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Generators.CCOL.CodeGeneration;
using TLCGen.Generators.CCOL.ProjectGeneration;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.ViewModels;

namespace TLCGen.Generators.CCOL
{
    public class CCOLCodeGeneratorViewModel : ViewModelBase
    {
        #region Fields

        private CCOLCodeGeneratorPlugin _Plugin;
        private CCOLGenerator _CodeGenerator;
        private CCOLVisualProjectGenerator _ProjectGenerator;
        private VisualProjectTypeEnum _VisualProjectType;
        private bool _VisualCBEnabled;

        #endregion // Fields

        #region Properties

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
            _CodeGenerator.GenerateSourceFiles(_Plugin.Controller, Path.GetDirectoryName(_Plugin.ControllerFileName));
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
            _ProjectGenerator.GenerateVisualStudioProjectFiles(_Plugin);
        }

        private bool GenerateVisualProjectCommand_CanExecute(object prm)
        {
            bool b = _Plugin.Controller != null &&
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

        public CCOLCodeGeneratorViewModel(CCOLCodeGeneratorPlugin plugin)
        {
            _Plugin = plugin;
            _CodeGenerator = new CCOLGenerator();
            _ProjectGenerator = new CCOLVisualProjectGenerator();
        }

        #endregion // Constructor
    }
}
