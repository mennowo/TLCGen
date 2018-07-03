using System.Windows;

namespace TLCGen.Dialogs
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AddNewTemplateFileWindow : Window
    {
        public AddNewTemplateFileWindow()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Name = fileTB.Text;
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Name = null;
            this.DialogResult = false;
            this.Close();
        }
    }
}
