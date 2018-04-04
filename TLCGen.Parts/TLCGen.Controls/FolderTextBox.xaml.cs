using Microsoft.WindowsAPICodePack.Dialogs;
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

namespace TLCGen.Controls
{
    /// <summary>
    /// Interaction logic for FolderTextBox.xaml
    /// </summary>
    public partial class FolderTextBox : UserControl
    {

        public string Folder
        {
            get { return (string)GetValue(FolderProperty); }
            set { SetValue(FolderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FolderProperty =
            DependencyProperty.Register("Folder", typeof(string), typeof(FolderTextBox), new PropertyMetadata(""));

        public string ChooseDialogTitle
        {
            get { return (string)GetValue(ChooseDialogTitleProperty); }
            set { SetValue(ChooseDialogTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChooseDialogTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChooseDialogTitleProperty =
            DependencyProperty.Register("ChooseDialogTitle", typeof(string), typeof(FolderTextBox), new PropertyMetadata("Choose folder"));

        public bool EnsurePathExists
        {
            get { return (bool)GetValue(EnsurePathExistsProperty); }
            set { SetValue(EnsurePathExistsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnsurePathExists.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnsurePathExistsProperty =
            DependencyProperty.Register("EnsurePathExists", typeof(bool), typeof(FolderTextBox), new PropertyMetadata(true));

        public FolderTextBox()
        {
            InitializeComponent();
        }

        private void SelectStdFolderButton_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog ofd = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = ChooseDialogTitle,
                EnsurePathExists = EnsurePathExists,
                Multiselect = false
            };
            if (ofd.ShowDialog(Window.GetWindow(this)) == CommonFileDialogResult.Ok)
            {
                Folder = ofd.FileName;
            }
        }
    }
}
