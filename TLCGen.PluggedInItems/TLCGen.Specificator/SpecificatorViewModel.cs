using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.IO;
using System.Windows.Input;
using TLCGen.Helpers;
using RelayCommand = GalaSoft.MvvmLight.CommandWpf.RelayCommand;

namespace TLCGen.Specificator
{
    public class SpecificatorViewModel : ViewModelBase
    {
        #region Fields

        private SpecificatorPlugin _plugin;
        private SpecificatorDataModel _data;
        private RelayCommand _generateCommand;
        private RelayCommand _addParagraafCommand;
        private RelayCommand _removeParagraafCommand;
        private RelayCommand _moveParagraafUpCommand;
        private RelayCommand _moveParagraafDownCommand;
        private SpecificatorSpecialsParagraafViewModel _selectedSpecialsParagraaf;

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
            get => Data?.Organisatie;
            set
            {
                Data.Organisatie = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public string Straat
        {
            get => Data?.Straat;
            set
            {
                Data.Straat = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public string Postcode
        {
            get => Data?.Postcode;
            set
            {
                Data.Postcode = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public string Stad
        {
            get => Data?.Stad;
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

        public SpecificatorSpecialsParagraafViewModel SelectedSpecialsParagraaf
        {
            get => _selectedSpecialsParagraaf;
            set
            {
                _selectedSpecialsParagraaf = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollectionAroundList<SpecificatorSpecialsParagraafViewModel, SpecificatorSpecialsParagraaf> SpecialsParagrafen { get; set; }

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

        public ICommand AddParagraafCommand => _addParagraafCommand ?? (_addParagraafCommand = new RelayCommand(() =>
        {
            var par = new SpecificatorSpecialsParagraafViewModel(new SpecificatorSpecialsParagraaf { Titel = "Paragraaf titel", Text = "Paragraaf text" });
            SpecialsParagrafen.Add(par);
            SelectedSpecialsParagraaf = par;
        }));

        public ICommand RemoveParagraafCommand => _removeParagraafCommand ?? (_addParagraafCommand = new RelayCommand(() =>
        {
            int index = SpecialsParagrafen.IndexOf(SelectedSpecialsParagraaf);
            SpecialsParagrafen.Remove(SelectedSpecialsParagraaf);
            SelectedSpecialsParagraaf = null;
            if (SpecialsParagrafen.Count > 0)
            {
                if (index >= SpecialsParagrafen.Count)
                {
                    SelectedSpecialsParagraaf = SpecialsParagrafen[SpecialsParagrafen.Count - 1];
                }
                else
                {
                    SelectedSpecialsParagraaf = SpecialsParagrafen[index];
                }
            }
        },
        () => SelectedSpecialsParagraaf != null));

        public ICommand MoveParagraafUpCommand => _moveParagraafUpCommand ?? (_addParagraafCommand = new RelayCommand(() =>
        {
            int index = -1;
            foreach (SpecificatorSpecialsParagraafViewModel mvm in SpecialsParagrafen)
            {
                ++index;
                if (mvm == SelectedSpecialsParagraaf)
                {
                    break;
                }
            }
            if (index >= 1)
            {
                SpecificatorSpecialsParagraafViewModel mvm = SelectedSpecialsParagraaf;
                SelectedSpecialsParagraaf = null;
                SpecialsParagrafen.Remove(mvm);
                SpecialsParagrafen.Insert(index - 1, mvm);
                SelectedSpecialsParagraaf = mvm;
            }
        },
        () => SelectedSpecialsParagraaf != null));

        public ICommand MoveParagraafDownCommand => _moveParagraafDownCommand ?? (_addParagraafCommand = new RelayCommand(() =>
        {
            int index = -1;
            foreach (SpecificatorSpecialsParagraafViewModel mvm in SpecialsParagrafen)
            {
                ++index;
                if (mvm == SelectedSpecialsParagraaf)
                {
                    break;
                }
            }
            if (index >= 0 && (index <= (SpecialsParagrafen.Count - 2)))
            {
                SpecificatorSpecialsParagraafViewModel mvm = SelectedSpecialsParagraaf;
                SelectedSpecialsParagraaf = null;
                SpecialsParagrafen.Remove(mvm);
                SpecialsParagrafen.Insert(index + 1, mvm);
                SelectedSpecialsParagraaf = mvm;
            }
        },
        () => SelectedSpecialsParagraaf != null));

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
            catch
            {
                System.Windows.MessageBox.Show("De specificatie is nog geopend in een ander programma.");
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

    public class SpecificatorSpecialsParagraafViewModel : ViewModelBase, IViewModelWithItem
    {
        public SpecificatorSpecialsParagraaf Paragraaf { get; }

        public string Titel
        {
            get => Paragraaf.Titel;
            set
            {
                Paragraaf.Titel = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public string Text
        {
            get => Paragraaf.Text;
            set
            {
                Paragraaf.Text = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public SpecificatorSpecialsParagraafViewModel(SpecificatorSpecialsParagraaf paragraaf)
        {
            Paragraaf = paragraaf;
        }

        public object GetItem()
        {
            return Paragraaf;
        }
    }
}
