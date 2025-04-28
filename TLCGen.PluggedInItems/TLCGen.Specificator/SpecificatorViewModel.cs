using System;
using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace TLCGen.Specificator
{
    public class SpecificatorViewModel : ObservableObject
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
                OnPropertyChanged("");
            }
        }

        #endregion // Properties

        #region Commands

        public ICommand GenerateCommand => _generateCommand ??= new RelayCommand(
            GenerateSpecification, 
            () => _plugin.Controller is { Fasen.Count: > 0 } &&
                  !string.IsNullOrWhiteSpace(_plugin.ControllerFileName));

        #endregion // Commands

        #region Public Methods

        public void UpdateCommands()
        {
            _generateCommand?.NotifyCanExecuteChanged();
        }

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
