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

namespace TLCGen.Updater
{
	class UpdaterViewModel : ViewModelBase
	{
		#region Fields

		private readonly Process _parentProcess;
		private readonly string _parentProcessPath;
		private IEnumerable<Process> _runningProcesses;
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
				p?.WaitForExit();
			}
		}

		public void AbortUpdate()
		{
			_client?.CancelAsync();
		}

		public void CleanUp()
		{
			_client?.Dispose();
			if (File.Exists(_tempfile)) File.Delete(_tempfile);
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
			_runningProcesses = Process.GetProcessesByName("TLCGen").Where(x => x.MainModule.FileName == _parentProcessPath);
			if (!_runningProcesses.Any())
			{
				TLCGenInstanceRunning = false;
				_checkRunningTimer.Enabled = false;
			}
		}
		
		#endregion // Private methods

		#region Constructor

		public UpdaterViewModel()
		{
			var args = Environment.GetCommandLineArgs();

			if (args.Length > 1 && int.TryParse(args[1], out var id))
			{
				_parentProcess = Process.GetProcessById(id);
				_parentProcessPath = _parentProcess.MainModule.FileName;
				_runningProcesses = Process.GetProcessesByName("TLCGen").Where(x => x.MainModule.FileName == _parentProcessPath);
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
