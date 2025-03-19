using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace TLCGen.Controls
{
    /// <summary>
    /// Interaction logic for FolderTextBox.xaml
    /// </summary>
    public partial class FolderTextBox : UserControl
    {

        public string Folder
        {
            get => (string)GetValue(FolderProperty);
            set => SetValue(FolderProperty, value);
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FolderProperty =
            DependencyProperty.Register(nameof(Folder), typeof(string), typeof(FolderTextBox), new PropertyMetadata(""));

        public string ChooseDialogTitle
        {
            get => (string)GetValue(ChooseDialogTitleProperty);
            set => SetValue(ChooseDialogTitleProperty, value);
        }

        // Using a DependencyProperty as the backing store for ChooseDialogTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChooseDialogTitleProperty =
            DependencyProperty.Register(nameof(ChooseDialogTitle), typeof(string), typeof(FolderTextBox), new PropertyMetadata("Choose folder"));

        public bool EnsurePathExists
        {
            get => (bool)GetValue(EnsurePathExistsProperty);
            set => SetValue(EnsurePathExistsProperty, value);
        }

        // Using a DependencyProperty as the backing store for EnsurePathExists.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnsurePathExistsProperty =
            DependencyProperty.Register(nameof(EnsurePathExists), typeof(bool), typeof(FolderTextBox), new PropertyMetadata(true));

        public FolderTextBox()
        {
            InitializeComponent();
        }

        private void SelectStdFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFolderDialog
            {
                Title = ChooseDialogTitle,
                Multiselect = false
            };
            if (ofd.ShowDialog(Window.GetWindow(this)) == true)
            {
                Folder = ofd.FolderName;
            }
        }
    }
}
