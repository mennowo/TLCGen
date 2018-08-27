using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.IO;
using System.Windows.Input;

namespace TLCGen.Specificator
{
    public class SpecificatorViewModel : ViewModelBase
    {
        #region Fields

        private SpecificatorPlugin _plugin;

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

        public string Organisatie
        {
            get => Data.Organisatie;
            set
            {
                Data.Organisatie = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public string Straat
        {
            get => Data.Straat;
            set
            {
                Data.Straat = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public string Postcode
        {
            get => Data.Postcode;
            set
            {
                Data.Postcode = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public string Stad
        {
            get => Data.Stad;
            set
            {
                Data.Stad = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public string TelefoonNummer
        {
            get => Data.TelefoonNummer;
            set
            {
                Data.TelefoonNummer = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public string EMail
        {
            get => Data.EMail;
            set
            {
                Data.EMail = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public string Website
        {
            get => Data.Website;
            set
            {
                Data.Website = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _GenerateCommand;
        private SpecificatorDataModel _data;

        public ICommand GenerateCommand
        {
            get
            {
                if (_GenerateCommand == null)
                {
                    _GenerateCommand = new RelayCommand(GenerateCommand_Executed, GenerateCommand_CanExecute);
                }
                return _GenerateCommand;
            }
        }

        #endregion // Commands

        #region Command Functionality

        private void GenerateCommand_Executed()
        {
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
                SpecificationGenerator.GenerateSpecification(filename, _plugin.Controller, Data);
            }
            catch
            {
                System.Windows.MessageBox.Show("De specificatie is nog geopend in een ander programma.");
            }
        }

        private bool GenerateCommand_CanExecute()
        {
            return _plugin.Controller != null &&
                   _plugin.Controller.Fasen != null &&
                   _plugin.Controller.Fasen.Count > 0 &&
                   !string.IsNullOrWhiteSpace(_plugin.ControllerFileName);
        }

        #endregion // Command Functionality

        #region Constructor

        public SpecificatorViewModel(SpecificatorPlugin plugin)
        {
            _plugin = plugin;
        }

        #endregion // Constructor
    }
}
