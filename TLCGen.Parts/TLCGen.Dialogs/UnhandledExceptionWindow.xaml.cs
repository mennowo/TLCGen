using System.Windows;

namespace TLCGen.Dialogs
{
    /// <summary>
    /// Interaction logic for EnterNameWindow.xaml
    /// </summary>
    public partial class UnhandledExceptionWindow : Window
    {
        public string DialogTitle
        {
            get { return this.Title; }
            set { this.Title = value; }
        }

        public string DialogMessage
        {
            get { return (string)MessageLabel.Content; }
            set { MessageLabel.Content = value; }
        }

        public string DialogExpceptionText
        {
            get { return (string)ExceptionTextTB.Text; }
            set { ExceptionTextTB.Text = value; }
        }

        public UnhandledExceptionWindow()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}