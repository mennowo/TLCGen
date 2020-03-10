using System.Windows;
using System.Windows.Controls;

namespace TLCGen.Dialogs
{
    /// <summary>
    /// Interaction logic for EnterNameWindow.xaml
    /// </summary>
    public partial class NewTemplateWindow : Window
    {
        public string DialogTitle
        {
            get => this.Title;
            set => this.Title = value;
        }

        public string TemplateNaam
        {
            get => NameTB.Text;
            set => NameTB.Text = value;
        }

        public string TemplateReplace
        {
            get => ReplaceTB.Text;
            set => ReplaceTB.Text = value;
        }

        public NewTemplateWindow()
        {
            InitializeComponent();
            this.Title = "Nieuwe template opslaan";
            OKButton.IsEnabled = false;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            TemplateNaam = "";
            this.Close();
        }

        private void NameTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTB.Text))
                OKButton.IsEnabled = false;
            else
                OKButton.IsEnabled = true;
        }
    }
}
