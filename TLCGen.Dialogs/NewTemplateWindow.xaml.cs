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
using System.Windows.Shapes;

namespace TLCGen.Dialogs
{
    /// <summary>
    /// Interaction logic for EnterNameWindow.xaml
    /// </summary>
    public partial class NewTemplateWindow : Window
    {
        public string DialogTitle
        {
            get { return this.Title; }
            set { this.Title = value; }
        }

        public string TemplateNaam
        {
            get { return NameTB.Text; }
            set
            {
                NameTB.Text = value;
            }
        }

        public string TemplateReplace
        {
            get { return ReplaceTB.Text; }
            set { ReplaceTB.Text = value; }
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
