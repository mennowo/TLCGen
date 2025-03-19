using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace TLCGen.Controls
{
    /// <summary>
    /// Interaction logic for FileTextBox.xaml
    /// </summary>
    public partial class FileTextBox : UserControl
    {

        public string File
        {
            get => (string)GetValue(FileProperty);
	        set => SetValue(FileProperty, value);
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileProperty =
            DependencyProperty.Register("File", typeof(string), typeof(FileTextBox), new PropertyMetadata(""));

        public string ChooseDialogTitle
        {
            get => (string)GetValue(ChooseDialogTitleProperty);
	        set => SetValue(ChooseDialogTitleProperty, value);
        }

        // Using a DependencyProperty as the backing store for ChooseDialogTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChooseDialogTitleProperty =
            DependencyProperty.Register("ChooseDialogTitle", typeof(string), typeof(FileTextBox), new PropertyMetadata("Choose file"));

	    public string ChooseDialogFilter
	    {
		    get => (string)GetValue(ChooseDialogFilterProperty);
		    set => SetValue(ChooseDialogFilterProperty, value);
	    }

	    // Using a DependencyProperty as the backing store for ChooseDialogTitle.  This enables animation, styling, binding, etc...
	    public static readonly DependencyProperty ChooseDialogFilterProperty =
		    DependencyProperty.Register("ChooseDialogFilter", typeof(string), typeof(FileTextBox), new PropertyMetadata("*.*|All files"));

        public bool EnsurePathExists
        {
            get => (bool)GetValue(EnsurePathExistsProperty);
            set => SetValue(EnsurePathExistsProperty, value);
        }

        // Using a DependencyProperty as the backing store for EnsurePathExists.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnsurePathExistsProperty =
            DependencyProperty.Register("EnsurePathExists", typeof(bool), typeof(FileTextBox), new PropertyMetadata(true));

        public FileTextBox()
        {
            InitializeComponent();
        }

        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
	        var ofd = new OpenFileDialog
	        {
				Title = ChooseDialogTitle,
                CheckFileExists = EnsurePathExists,
		        Multiselect = false, Filter = ChooseDialogFilter
	        };

	        if (ofd.ShowDialog(Window.GetWindow(this)) == true)
            {
                File = ofd.FileName;
            }
        }
    }
}
