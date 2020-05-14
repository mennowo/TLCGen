using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TLCGen.Dependencies.Providers
{
    public interface ITLCGenDialogProvider
    {
        bool ShowDialogs { get; }
        MessageBoxResult ShowMessageBox(string content, string caption, MessageBoxButton buttons);
        bool ShowOpenFileDialog(string title, string filter, bool checkFileExists, out string filename);
    }

    public class TLCGenDialogProvider : ITLCGenDialogProvider
    {
        private static ITLCGenDialogProvider _default;

        public static ITLCGenDialogProvider Default
        {
            get => _default ?? (_default = new TLCGenDialogProvider(true));
            set => _default = value;
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
                Title = title,
                Filter = filter
            };

            var result = openFileDialog.ShowDialog() == true;
            if (result)
            {
                filename = openFileDialog.FileName;
            }
            else
            {
                filename = null;
            }
            return result;
        }

        public MessageBoxResult ShowMessageBox(string content, string caption, MessageBoxButton buttons)
        {
            return MessageBox.Show(content, caption, buttons);
        }
    }
}
