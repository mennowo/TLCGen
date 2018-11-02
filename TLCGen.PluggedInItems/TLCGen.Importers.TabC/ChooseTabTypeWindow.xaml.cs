using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static TLCGen.Importers.TabC.TabCImportHelper;

namespace TLCGen.Importers.TabC
{
    /// <summary>
    /// Interaction logic for ChooseTabTypeWindow.xaml
    /// </summary>
    public partial class ChooseTabTypeWindow : Window
    {
        private TabCType _tabType;

        public ChooseTabTypeWindow()
        {
            InitializeComponent();
            ImportDetCheck.IsChecked = ImportTijdCheck.IsChecked = true;
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
                switch (_tabType)
                {
                    case TabCType.OTTO:
                        OttoCheck.IsChecked = true;
                        ImportDetCheck.IsChecked = ImportTijdCheck.IsChecked = false;
                        ImportDetCheck.IsEnabled = ImportTijdCheck.IsEnabled = false;
                        break;
                    case TabCType.TPA:
                        TPACheck.IsChecked = true;
                        break;
                    case TabCType.ATB:
                        ATBCheck.IsChecked = true;
                        break;
                    case TabCType.FICK:
                        FICKCheck.IsChecked = true;
                        break;
                    case TabCType.HUIJSKES:
                        HUIJSKESCheck.IsChecked = true;
                        break;
                    case TabCType.GC:
                        GCCheck.IsChecked = true;
                        break;
                    case TabCType.UNKNOWN:
                        break;
                }
            }
        }
        public bool ImportDetectoren { get; private set; }
        public bool ImportTijden { get; private set; }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (OttoCheck.IsChecked == true) TabType = TabCType.OTTO;
            if (TPACheck.IsChecked == true) TabType = TabCType.TPA;
            if (ATBCheck.IsChecked == true) TabType = TabCType.ATB;
            if (FICKCheck.IsChecked == true) TabType = TabCType.FICK;
            if (HUIJSKESCheck.IsChecked == true) TabType = TabCType.HUIJSKES;
            if (GCCheck.IsChecked == true) TabType = TabCType.GC;

            if (TabType == TabCType.UNKNOWN) OKButton.IsEnabled = false;
            else OKButton.IsEnabled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ImportDetectoren = ImportDetCheck.IsChecked == true;
            ImportTijden = ImportTijdCheck.IsChecked == true;
            this.DialogResult = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
