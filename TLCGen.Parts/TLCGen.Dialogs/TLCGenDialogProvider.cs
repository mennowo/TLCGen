using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TLCGen.Dialogs
{
    public interface ITLCGenDialogProvider
    {
        bool ShowDialogs { get; }
        MessageBoxResult ShowMessageBox(string content, string caption, MessageBoxButton buttons = MessageBoxButton.OK);
        bool ShowOpenFileDialog(string title, string filter, bool checkFileExists, out string filename);
    }

    public class TLCGenDialogProvider : ITLCGenDialogProvider
    {
        private static ITLCGenDialogProvider _default;

        public static ITLCGenDialogProvider Default
        {
            get => _default ?? (_default = new TLCGenDialogProvider(true));
            set
            {
                _default = value;
            }
        }

        public bool ShowDialogs { get; }

        public TLCGenDialogProvider(bool showDialogs)
        {
            ShowDialogs = showDialogs;
        }

        public bool ShowOpenFileDialog(string title, string filter, bool checkFileExists, out string filename)
        {
            var openFileDialog = new OpenFileDialog
            {
                CheckFileExists = true,
                Title = "Selecteer tab.c file voor importeren",
                Filter = "tab.c files|*tab.c,*.ccol|Alle files|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                filename = openFileDialog.FileName;
            }
            else
            {
                filename = null;
            }
            return openFileDialog.ShowDialog() == true;
        }

        public MessageBoxResult ShowMessageBox(string content, string caption, MessageBoxButton buttons = MessageBoxButton.OK)
        {
            return MessageBox.Show(content, caption, buttons);
        }
    }
}
