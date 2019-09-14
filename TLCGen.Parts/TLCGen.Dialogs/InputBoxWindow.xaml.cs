using System.Windows;
using System.Windows.Controls;

namespace TLCGen.Dialogs
{
    /// <summary>
    /// Interaction logic for InputBoxWindow.xaml
    /// </summary>
    public partial class InputBoxWindow : Window
    {
        public string Explanation
        {
            get { return (string)ExplanationLabel.Content; }
            set { ExplanationLabel.Content = value; }
        }

        public string Text
        {
            get; private set;
        }

        public InputBoxWindow()
        {
            InitializeComponent();
            this.Title = "Geef tekst in";
            OKButton.IsEnabled = false;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Text = TextTB.Text;
            DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Text = null;
            DialogResult = false;
            this.Close();
        }

        private void ElementsTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextTB.Text))
                OKButton.IsEnabled = false;
            else
                OKButton.IsEnabled = true;
        }
    }
}
