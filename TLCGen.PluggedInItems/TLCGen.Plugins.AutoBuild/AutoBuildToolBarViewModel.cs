using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Messaging.Messages;

namespace TLCGen.Plugins.AutoBuild
{
    public class AutoBuildToolBarViewModel : ObservableObject
    {
        #region Fields

        private readonly AutoBuildPlugin _plugin;
        private ObservableCollection<VCXProjectDataViewModel> _vcxProjects;
        private VCXProjectDataViewModel _selectedVcxProject;
        private string _controllerFileName;
        private RelayCommand _buildAndRunCommand;
	    private RelayCommand _buildCommand;
		private RelayCommand _refreshCommand;

        #endregion // Fields

        #region Properties

        public ObservableCollection<VCXProjectDataViewModel> VcxProjects => _vcxProjects ?? (_vcxProjects = new ObservableCollection<VCXProjectDataViewModel>());

        public VCXProjectDataViewModel SelectedVcxProject
        {
            get => _selectedVcxProject;
            set
            {
                _selectedVcxProject = value;
                OnPropertyChanged();
            }
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

        public Visibility ToolBarVisibility => _plugin?.Settings?.ToolBarVisibility == true ? Visibility.Visible : Visibility.Collapsed;

        #endregion // Properties

        #region Commands

        public ICommand BuildAndRunCommand => _buildAndRunCommand ?? (_buildAndRunCommand = new RelayCommand(BuildAndRunCommand_Executed, BuildAndRunCommand_CanExecute));

        public ICommand BuildCommand => _buildCommand ?? (_buildCommand = new RelayCommand(BuildCommand_Executed, BuildAndRunCommand_CanExecute));

	    public ICommand RefreshCommand => _refreshCommand ?? (_refreshCommand = new RelayCommand(RefreshCommand_Executed, RefreshCommand_CanExecute));

        #endregion // Commands

        #region Command Functionality

        private void RefreshCommand_Executed()
        {
            UpdateAvailableProjectsToBuild();
		}

        private bool RefreshCommand_CanExecute()
        {
            return _plugin.Controller != null && !string.IsNullOrWhiteSpace(ControllerFileName);
        }

	    private void BuildCommand_Executed()
	    {
			BuildAndRun(false);
	    }

		private void BuildAndRunCommand_Executed()
        {
			BuildAndRun(true);
        }

        private bool BuildAndRunCommand_CanExecute()
        {
            return _plugin.Controller != null && SelectedVcxProject != null;
        }

        #endregion // Command Functionality

        #region Public Methods

        public void UpdateTLCGenMessaging()
        {
            WeakReferenceMessenger.Default.Register<ControllerFileNameChangedMessage>(this, OnControllerFileNameChanged);
        }

        #endregion // Public Methods

        #region Private Methods

        private void BuildAndRun(bool run)
	    {
		    var myname = SelectedVcxProject.Name;
		    Task.Run(() =>
			{
				try
				{
                    // Prepare the process to run
					ProcessStartInfo start = new ProcessStartInfo();
					Process p = new Process();
					// Enter in the command line arguments, everything you would enter after the executable name itself
					
					start.Arguments = " \"" + SelectedVcxProject.FileName + "\"";
					//start.WorkingDirectory = @"";
					// Enter the executable to run, including the complete path
					start.FileName = Path.Combine(_plugin.Settings.MSBuildPath, "MSBuild.exe");
					// Do you want to show a console window?
					start.WindowStyle = ProcessWindowStyle.Hidden;
					start.CreateNoWindow = true;
					start.UseShellExecute = false;
					start.RedirectStandardOutput = true;
					// Run the external process
					p.StartInfo = start;
					// Start the process.
					p.OutputDataReceived += p_OutputDataReceived;
					Logger.AddMessage(" > ==============================================================");
					Logger.AddMessage("> Building CCOL project using MSBuild.exe");
					Logger.AddMessage("> Project : " + SelectedVcxProject.FileName);
					Logger.AddMessage("> ==============================================================");
					p.Start();
					p.BeginOutputReadLine();
					try
					{
						p.WaitForExit(30000);
						System.Threading.Thread.Sleep(1000);
						if (p.ExitCode == 0)
						{
							if (run)
							{
								Logger.AddMessage("> ==============================================================");
								Logger.AddMessage("> Build succeeded!");
								Logger.AddMessage("> Starting file: " + Path.GetDirectoryName(SelectedVcxProject.FileName) + "\\Debug\\" + myname +
								                  ".exe");
								Logger.AddMessage("> ==============================================================");
								var app = new ProcessStartInfo
								{
									WorkingDirectory = Path.GetDirectoryName(SelectedVcxProject.FileName) ?? "",
									FileName = Path.GetDirectoryName(SelectedVcxProject.FileName) + "\\Debug\\" + myname + ".exe"
								};
								Process.Start(app);
							}
							else
							{
								Logger.AddMessage("> ==============================================================");
								Logger.AddMessage("> Build succeeded!");
								Logger.AddMessage("> ==============================================================");
							}
						}
						else
						{
							Logger.AddErrorMessage("> ==============================================================");
							Logger.AddErrorMessage("> Build FAILED.");
							Logger.AddErrorMessage("> ==============================================================");
						}
					}
					catch
					{
						p.CancelOutputRead();
						p.Kill();
						p.Close();
						Logger.AddErrorMessage("> ==============================================================");
						Logger.AddErrorMessage("> Build took longer than 30 seconds. Aborted.");
						Logger.AddErrorMessage("> ==============================================================");
					}
				}
				catch (Exception e)
				{
					Logger.AddErrorMessage("> Exception while trying to BuildAndRun!");
					Logger.AddErrorMessage("> Namely: " + e);
				}
			});
	    }

        private void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            string l = e.Data;
            if (!string.IsNullOrEmpty(l) && Regex.IsMatch(l, ": [eE]rror [A-Z][0-9][0-9][0-9][0-9]:"))
            {
                Logger.AddErrorMessage("> " + e.Data + "");
            }
            else if (!string.IsNullOrEmpty(l) && Regex.IsMatch(l, ": [wW]arning [A-Z][0-9][0-9][0-9][0-9]:"))
            {
                Logger.AddWarningMessage("> " + e.Data + "");
            }
            else
            {
                Logger.AddMessage("> " + e.Data + "");
            }
        }

        private void UpdateAvailableProjectsToBuild()
        {
            VcxProjects.Clear();

            if(string.IsNullOrWhiteSpace(ControllerFileName) || !File.Exists(ControllerFileName))
            {
                return;
            }

            var path = Path.GetDirectoryName(ControllerFileName);
            if (path != null)
            {
	            var files = Directory.GetFiles(path);
	            foreach (var f in files)
	            {
		            if (Path.GetExtension(f) == ".vcxproj")
		            {
			            VcxProjects.Add(new VCXProjectDataViewModel(Path.GetFileNameWithoutExtension(f), f));
		            }
	            }
            }

            if (VcxProjects.Count > 0)
            {
                SelectedVcxProject = VcxProjects[0];
            }
        }
        
        #endregion // Private Methods

        #region TLCGen Events

        private void OnControllerFileNameChanged(object sender, ControllerFileNameChangedMessage message)
        {
            VcxProjects.Clear();

            if (!string.IsNullOrWhiteSpace(message.NewFileName) && File.Exists(message.NewFileName))
            {
                ControllerFileName = message.NewFileName;
                UpdateAvailableProjectsToBuild();
            }
            else
            {
                ControllerFileName = null;
            }

            OnPropertyChanged("");

            _buildAndRunCommand?.NotifyCanExecuteChanged();
            _buildCommand?.NotifyCanExecuteChanged();
			_refreshCommand?.NotifyCanExecuteChanged();
        }

        #endregion // TLCGen Events

        #region Constructor

        public AutoBuildToolBarViewModel(AutoBuildPlugin plugin)
        {
            _plugin = plugin;
        }

        #endregion // Constructor
    }
}
