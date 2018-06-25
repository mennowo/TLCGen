using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Timers;
using System.Windows;
using System.Windows.Documents;
using GalaSoft.MvvmLight;
using Microsoft.Win32;

namespace TLCGen.Updater
{
	class UpdaterViewModel : ViewModelBase
	{
		#region Fields

		private readonly Process _parentProcess;
		private readonly string _parentProcessPath;
		private List<string> _runningProcesses = new List<string>();
		private readonly Timer _checkRunningTimer;
		private bool _tlcGenInstanceRunning;
		private bool _tlcGenDownloaded;
		private int _tlcGenDownloadProgress;
		private string _tempfile;
		private WebClient _client;

		#endregion // Fields

		#region Properties

		public bool TLCGenInstanceRunning
		{
			get => _tlcGenInstanceRunning;
			set
			{
				_tlcGenInstanceRunning = value; 
				RaisePropertyChanged();
				RaisePropertyChanged(nameof(TLCGenMayUpdate));
			}
		}

		public bool TLCGenMayUpdate => !_tlcGenInstanceRunning;

		public bool TLCGenDownloaded
		{
			get => _tlcGenDownloaded;
			set
			{
				_tlcGenDownloaded = value; 
				RaisePropertyChanged();
			}
		}

		public int TLCGenDownloadProgress
		{
			get => _tlcGenDownloadProgress;
			set
			{
				_tlcGenDownloadProgress = value; 
				RaisePropertyChanged();
			}
		}

		#endregion // Properties

		#region Public methods

		public void StartDownload()
		{
			_tempfile = Path.GetTempFileName();
			_tempfile = Path.ChangeExtension(_tempfile, ".msi");
			_client = new WebClient();
			_client.DownloadProgressChanged += ClientOnDownloadProgressChanged;
			_client.DownloadFileCompleted += ClientOnDownloadFileCompleted;
			_client.DownloadFileAsync(new Uri("https://www.codingconnected.eu/download/tlcgen-test/?wpdmdl=1126"), _tempfile);
		}

		public void StartUpdate()
		{
			if (TLCGenDownloaded)
			{
				var p = Process.Start(_tempfile);
			}
		}

		public void AbortUpdate()
		{
			_client?.CancelAsync();
		}

		public void CleanUp()
		{
            if (!string.IsNullOrWhiteSpace(_tempfile))
            {
                var key = Registry.CurrentUser.OpenSubKey("Software", true);
                var sk1 = key?.OpenSubKey("CodingConnected e.U.", true) ?? key.CreateSubKey("CodingConnected e.U.");
                if (sk1 != null)
                {
                    var sk2 = sk1.OpenSubKey("TLCGen", true) ?? key.CreateSubKey("TLCGen");
                    sk2?.SetValue("TempInstallFile", _tempfile, RegistryValueKind.String);
                }
            }
			_client?.Dispose();
		}

		private void ClientOnDownloadFileCompleted(object sender, AsyncCompletedEventArgs asyncCompletedEventArgs)
		{
			TLCGenDownloaded = true;
		}

		private void ClientOnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs downloadProgressChangedEventArgs)
		{
			TLCGenDownloadProgress = downloadProgressChangedEventArgs.ProgressPercentage;
		}

		#endregion // Public methods

		#region Private methods

		private void CheckRunningTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (!string.IsNullOrWhiteSpace(_parentProcessPath))
            {
                _runningProcesses = AppMonitor.GetProcesses(_parentProcessPath);
                if (!_runningProcesses.Any())
                {
                    TLCGenInstanceRunning = false;
                    _checkRunningTimer.Enabled = false;
                }
            }
		}
		
		#endregion // Private methods

		#region Constructor

		public UpdaterViewModel()
		{
#if !DEBUG
            Application.Current.DispatcherUnhandledException += (o, e) =>
            {
                MessageBox.Show("Gelieve dit probleem inclusief onderstaande details doorgeven aan de ontwikkelaar:\n\n" + e.Exception.ToString(), "Er is een onverwachte fout opgetreden.", MessageBoxButton.OK);
            };
#endif

            var args = Environment.GetCommandLineArgs();

			if (args.Length > 1 && int.TryParse(args[1], out var id))
			{
                try
                {
				    _parentProcess = Process.GetProcessById(id);
				    _parentProcessPath = _parentProcess.MainModule.FileName;
				    _runningProcesses = AppMonitor.GetProcesses(_parentProcessPath);
                }
                catch (ArgumentException e)
                {
                    // ignored: process already closed
                }

                if (!_runningProcesses.Any())
				{
					TLCGenInstanceRunning = false;
					return;
				}
				TLCGenInstanceRunning = true;
				_checkRunningTimer = new Timer(1500) {AutoReset = true};
				_checkRunningTimer.Elapsed += CheckRunningTimerOnElapsed;
				_checkRunningTimer.Enabled = true;
			}
			else
			{
				Application.Current.Shutdown(-1);
			}
		}

		#endregion // Constructor
	}
}
