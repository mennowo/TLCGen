using System;
using TLCGen.Messaging.Messages;
using ICSharpCode.AvalonEdit;
using System.Windows.Input;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Helpers;

namespace TLCGen.Plugins.Additor
{
    internal class AdditorTabViewModel : ObservableObject
    {
        #region Fields

        private readonly AdditorPlugin _plugin;
        private string _controllerFileName;
        private ObservableCollection<AddFileViewModel> _addFiles;
        private AddFileViewModel _selectedAddFile;
        private bool _selectedAddFileChanged;
        private TextEditor _viewEditor;
        private RelayCommand _refreshFilesListCommand;
        private RelayCommand _saveAddFileCommand;

        #endregion // Fields

        #region Properties

        public TextEditor ViewEditor
        {
            set
            {
                var temp = _viewEditor;
                _viewEditor = value;
                value.Options.IndentationSize = 4;
                value.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(value.Options);
                if(temp != null)
                {
                    _viewEditor.Document = temp.Document;
                    _viewEditor.CaretOffset = temp.CaretOffset;
                    _viewEditor.ScrollToHorizontalOffset(temp.HorizontalOffset);
                    _viewEditor.ScrollToVerticalOffset(temp.VerticalOffset);
                }
                else if (SelectedAddFile != null)
                {
                    LoadAddFile(SelectedAddFile.FullFileName);
                }
                _viewEditor.TextChanged += ViewEditor_TextChanged;
            }
            get => _viewEditor;
        }

        public string ControllerFileName
        {
            get => _controllerFileName;
	        set
            {
                _controllerFileName = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<AddFileViewModel> AddFiles => _addFiles ??= new ObservableCollection<AddFileViewModel>();

	    public AddFileViewModel SelectedAddFile
        {
            get => _selectedAddFile;
	        set
            {
                if(SelectedAddFile != null && _selectedAddFileChanged)
                {
                    var r = MessageBox.Show("Opslaan bestand " + SelectedAddFile.FileName, "Wijzigingen opslaan?", MessageBoxButton.YesNoCancel);
                    if (r == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                    if (r == MessageBoxResult.Yes)
                    {
                        SaveAddFile(SelectedAddFile.FullFileName);
                    }
                }
                _selectedAddFile = value;
                if (_selectedAddFile != null && !string.IsNullOrWhiteSpace(_selectedAddFile.FullFileName) && File.Exists(_selectedAddFile.FullFileName))
                {
                    LoadAddFile(_selectedAddFile.FullFileName);
                }
                else
                {
                    ResetViewEditorText();
                }
                _selectedAddFileChanged = false;
                OnPropertyChanged();
                _saveAddFileCommand?.NotifyCanExecuteChanged();
            }
        }

	    #endregion // Properties

        #region Commands

        public ICommand RefreshFilesListCommand => _refreshFilesListCommand ??= new RelayCommand(() =>
        {
            if (!string.IsNullOrWhiteSpace(ControllerFileName) && File.Exists(ControllerFileName))
            {
                var path = Path.GetDirectoryName(ControllerFileName);
                if (path != null)
                {
                    var files = Directory.GetFiles(path);
                    AddFiles.Clear();
                    if (!string.IsNullOrWhiteSpace(_plugin.Controller.Data.Naam))
                    { 
                        foreach (var f in files)
                        {
                            if (Path.GetFileName(f).StartsWith(_plugin.Controller.Data.Naam) &&
                                (Path.GetExtension(f).ToLower() == ".add" || 
                                 Path.GetExtension(f).ToLower() == ".h" ||
                                 Path.GetExtension(f).ToLower() == ".c"))
                            {
                                var add = new AddFileViewModel {FileName = Path.GetFileName(f), FullFileName = f};
                                AddFiles.Add(add);
                            }
                        }
                    }
                }

                if(AddFiles.Count > 0)
                {
                    SelectedAddFile = AddFiles[0];
                }
            }
        }, () => !string.IsNullOrWhiteSpace(ControllerFileName));

	    public ICommand SaveAddFileCommand => _saveAddFileCommand ??= new RelayCommand(() =>
        {
            try
            {
                SaveAddFile(SelectedAddFile.FullFileName);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error bij schrijven van add file: \n" + e);
            }
        }, () => SelectedAddFile != null);

        #endregion // Commands

        #region Public Methods

        public void UpdateTLCGenMessaging()
        {
            WeakReferenceMessengerEx.Default.Register<ControllerFileNameChangedMessage>(this, OnControllerFileNameChanged);
        }

        #endregion // Public Methods

        #region Private Methods

        private void ViewEditor_TextChanged(object sender, EventArgs e)
        {
            _selectedAddFileChanged = true;
        }

        private void LoadAddFile(string filename)
        {
            if (ViewEditor != null)
            {
                ViewEditor.Load(filename);
            }
        }

        private void SaveAddFile(string filename)
        {
            if (ViewEditor != null)
            {
                ViewEditor.Save(filename);
                _selectedAddFileChanged = false;
            }
        }

        private void ResetViewEditorText()
        {
            if (ViewEditor != null)
            {
                ViewEditor.Clear();
            }
        }

        #endregion // Private Methods

        #region TLCGen Events

        private void OnControllerFileNameChanged(object sender, ControllerFileNameChangedMessage message)
        {
            ControllerFileName = message.NewFileName;
            if (RefreshFilesListCommand?.CanExecute(null) == true)
            {
                RefreshFilesListCommand.Execute(null);
            }

            if (string.IsNullOrWhiteSpace(message.NewFileName))
            {
                SelectedAddFile = null;
            }

            _refreshFilesListCommand?.NotifyCanExecuteChanged();
        }

        #endregion // TLCGen Events

        #region Constructor

        public AdditorTabViewModel(AdditorPlugin plugin)
	    {
            _plugin = plugin;
	    }

        #endregion // Constructor
    }
}