using System.IO;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using TLCGen.Generators.TLCCC.CodeGeneration;
using TLCGen.Integrity;
using TLCGen.Messaging.Messages;

namespace TLCGen.Generators.TLCCC.GeneratorUI
{
    public class TLCCCGeneratorViewModel : ViewModelBase
    {
        #region Fields

        private TLCCCCodeGeneratorPlugin _plugin;
        private TLCCCGenerator _codeGenerator;

        #endregion // Fields

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

        #endregion // Commands

        #region Command Functionality

        private void GenerateCodeCommand_Executed()
        {
            var prepreq = new Messaging.Requests.PrepareForGenerationRequest();
            Messenger.Default.Send(prepreq);
            var s = TLCGenIntegrityChecker.IsControllerDataOK(_plugin.Controller);
            if (s == null)
            {
                _codeGenerator.GenerateSourceFiles(_plugin.Controller, Path.GetDirectoryName(_plugin.ControllerFileName));
                Messenger.Default.Send(new ControllerCodeGeneratedMessage());
            }
            else
            {
                MessageBox.Show(s, "Fout in controller");
            }
        }

        private bool GenerateCodeCommand_CanExecute()
        {
            return _plugin.Controller != null &&
                   _plugin.Controller.Fasen != null &&
                   _plugin.Controller.Fasen.Count > 0 &&
                   !string.IsNullOrWhiteSpace(_plugin.ControllerFileName);
        }

        #endregion // Command Functionality

        #region Constructor

        public TLCCCGeneratorViewModel(TLCCCCodeGeneratorPlugin plugin, TLCCCGenerator generator)
        {
            _plugin = plugin;
            _codeGenerator = generator;
        }

        #endregion // Constructor
    }
}