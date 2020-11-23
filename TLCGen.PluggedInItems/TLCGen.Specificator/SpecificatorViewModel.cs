using System;
using GalaSoft.MvvmLight;
using System.IO;
using System.Windows.Input;
using RelayCommand = GalaSoft.MvvmLight.CommandWpf.RelayCommand;

namespace TLCGen.Specificator
{
    public class SpecificatorViewModel : ViewModelBase
    {
        #region Fields

        private SpecificatorPlugin _plugin;
        private SpecificatorDataModel _data;
        private RelayCommand _generateCommand;

        #endregion // Fields

        #region Properties

        public SpecificatorDataModel Data
        {
            get => _data;
            set
            {
                _data = value;
                RaisePropertyChanged("");
            }
        }

        #endregion // Properties

        #region Commands

        public ICommand GenerateCommand
        {
            get
            {
                if (_generateCommand == null)
                {
                    _generateCommand = new RelayCommand(GenerateCommand_Executed, GenerateCommand_CanExecute);
                }
                return _generateCommand;
            }
        }

        #endregion // Commands

        #region Command Functionality

        private void GenerateCommand_Executed()
        {
            GenerateSpecification();
        }

        private bool GenerateCommand_CanExecute()
        {
            return _plugin.Controller != null &&
                   _plugin.Controller.Fasen != null &&
                   _plugin.Controller.Fasen.Count > 0 &&
                   !string.IsNullOrWhiteSpace(_plugin.ControllerFileName);
        }

        #endregion // Command Functionality

        #region Public Methods

        public void GenerateSpecification()
        {
            _plugin.UpdateCCOLGenData();

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var filename = Path.Combine(Path.GetDirectoryName(_plugin.ControllerFileName), _plugin.Controller.Data.Naam + ".docx");
            try
            {
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }

                using (var stream = assembly.GetManifestResourceStream("TLCGen.Specificator.Resources.specification_template.docx"))
                using (var fileStream = File.Create(filename))
                {
                    stream.CopyTo(fileStream);
                }

                TableGenerator.ClearTables();
                SpecificationGenerator.GenerateSpecification(filename, _plugin.Controller, Data);
            }
            catch (IOException)
            {
                System.Windows.MessageBox.Show("Fout bij wegschrijven data: de specificatie is mogelijk nog geopend in een ander programma.");
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("Fout bij wegschrijven data: " + e);
            }
        }

        #endregion // Public Methods

        #region Constructor

        public SpecificatorViewModel(SpecificatorPlugin plugin)
        {
            _plugin = plugin;
        }

        #endregion // Constructor
    }
}
