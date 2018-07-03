using System.Windows;
using System.Windows.Controls;

namespace TLCGen.Dialogs
{
    /// <summary>
    /// Interaction logic for ApplyTemplateWindow.xaml
    /// </summary>
    public partial class ApplyTemplateWindow : Window
    {
        public string DialogTitle
        {
            get { return this.Title; }
            set { this.Title = value; }
        }

        public string TemplateApplyString
        {
            get { return ElementsTB.Text; }
            set
            {
                ElementsTB.Text = value;
            }
        }

        public ApplyTemplateWindow()
        {
            InitializeComponent();
            this.Title = "Template toepassen";
            OKButton.IsEnabled = false;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            TemplateApplyString = "";
            this.Close();
        }

        private void ElementsTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ElementsTB.Text))
                OKButton.IsEnabled = false;
            else
                OKButton.IsEnabled = true;
        }
    }
}
