using System.IO;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Generators.TLCCC.CodeGeneration;
using TLCGen.Helpers;
using TLCGen.Integrity;
using TLCGen.Messaging.Messages;

namespace TLCGen.Generators.TLCCC.GeneratorUI
{
    public class TLCCCGeneratorViewModel : ObservableObject
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
            var prepreq = new Messaging.Requests.PrepareForGenerationRequest(_plugin.Controller);
            WeakReferenceMessengerEx.Default.Send(prepreq);
            var s = TLCGenIntegrityChecker.IsControllerDataOK(_plugin.Controller);
            if (s == null)
            {
                _codeGenerator.GenerateSourceFiles(_plugin.Controller, Path.GetDirectoryName(_plugin.ControllerFileName));
                WeakReferenceMessengerEx.Default.Send(new ControllerCodeGeneratedMessage());
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