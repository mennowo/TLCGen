using System.Windows;
using static TLCGen.Importers.TabC.TabCImportHelper;

namespace TLCGen.Importers.TabC
{
    /// <summary>
    /// Interaction logic for ChooseTabTypeWindow.xaml
    /// </summary>
    public partial class ChooseTabTypeWindow : Window
    {
        private TabCType _tabType;
        private bool _intergroen;
        private bool _hasIntergroen;
        private bool _importGarantie;

        public ChooseTabTypeWindow()
        {
            InitializeComponent();
            ImportDetCheck.IsChecked = 
                ImportTijdCheck.IsChecked =
                    ImportToepDefaultsCheck.IsChecked = true;
            DeelconflictenTypeCB.Items.Add("Voorstarten");
            DeelconflictenTypeCB.Items.Add("Late release");
            DeelconflictenTypeCB.SelectedItem = DeelconflictenTypeCB.Items[0];
        }

        public bool ImportInExisting
        {
            set
            {
                if (value)
                {
                    ImportDetCheck.IsChecked = ImportTijdCheck.IsChecked = false;
                    ImportDetCheck.Visibility = ImportTijdCheck.Visibility = Visibility.Collapsed;
                    OptiesLabel.Content = "In een bestaande regeling kunnen uitsluitend\nontruimingstijden worden geïmporteerd.";
                }
            }
        }

        public TabCType TabType
        {
            get => _tabType;
            set
            {
                _tabType = value;
                ImportDetCheck.IsEnabled = ImportTijdCheck.IsEnabled = true;
                ImportGarantieCheck.IsEnabled = ImportDeelconflictenCheck.IsEnabled = false;
                switch (_tabType)
                {
                    case TabCType.OTTO:
                        ImportDetCheck.IsChecked = ImportTijdCheck.IsChecked = false;
                        ImportDetCheck.IsEnabled = ImportTijdCheck.IsEnabled = false;
                        ImportDeelconflictenCheck.IsEnabled = true;
                        ImportGarantieCheck.IsEnabled = true;
                        break;
                    case TabCType.TPA:
                        break;
                    case TabCType.ATB:
                        ImportGarantieCheck.IsEnabled = true;
                        break;
                    case TabCType.FICK:
                        break;
                    case TabCType.HUIJSKES:
                        break;
                    case TabCType.GC:
                        break;
                    case TabCType.UNKNOWN:
                        break;
                }
            }
        }

        public bool HasIntergroen
        {
            get => _hasIntergroen;
            set
            {
                _hasIntergroen = value;
                IntergroenCheck.IsEnabled = value;
            }
        }

        public bool Intergroen
        {
            get => _intergroen;
            set
            {
                _intergroen = value;
                IntergroenCheck.IsChecked = value;
            }
        }

        public string ImportDeelconflicten { get; private set; }
        public bool ToepassenDefaults { get; private set; } = true;
        public bool ImportDetectoren { get; private set; }
        public bool ImportTijden { get; private set; }
        public bool ImportNalopen { get; private set; }
        public bool ImportGarantie
        {
            get => _importGarantie;
            set
            {
                _importGarantie = value;
                ImportGarantieCheck.IsChecked = value;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ImportDetectoren = ImportDetCheck.IsChecked == true;
            ImportTijden = ImportTijdCheck.IsChecked == true;
            ImportNalopen = ImportNalopenCheck.IsChecked == true;
            Intergroen = IntergroenCheck.IsChecked == true;
            ImportGarantie = ImportGarantieCheck.IsChecked == true;
            ToepassenDefaults = ImportToepDefaultsCheck.IsChecked == true;
            if (ImportDeelconflictenCheck.IsChecked == true)
            {
                ImportDeelconflicten = DeelconflictenTypeCB.SelectedValue.ToString();
            }
            else
            {
                ImportDeelconflicten = null;
            }
            this.DialogResult = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
