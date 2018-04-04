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
            get { return (bool)GetValue(EnsurePathExistsProperty); }
            set { SetValue(EnsurePathExistsProperty, value); }
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
	        var ofd = new CommonOpenFileDialog
	        {
		        IsFolderPicker = false,
				Title = ChooseDialogTitle,
		        EnsurePathExists = EnsurePathExists,
		        Multiselect = false
	        };

	        var exts = ChooseDialogFilter.Split('|');
	        if (exts.Length > 0 && exts.Length % 2 == 0)
	        {
		        for (var i = 0; i < exts.Length; i += 2)
		        {
			        ofd.Filters.Add(new CommonFileDialogFilter(exts[i + 1], exts[i]));
		        }
	        }

	        if (ofd.ShowDialog(Window.GetWindow(this)) == CommonFileDialogResult.Ok)
            {
                File = ofd.FileName;
            }
        }
    }
}
