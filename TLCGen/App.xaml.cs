using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shell;
using TLCGen.Dialogs;
using TLCGen.Extensions;

namespace TLCGen
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnJumpItemsRejected(object sender, JumpItemsRejectedEventArgs e) { }
        private void OnJumpItemsRemoved(object sender, JumpItemsRemovedEventArgs e) { }

        public App()
        {
#if !DEBUG
            SetupExceptionHandling();
#endif
        }

#if !DEBUG
        private void SetupExceptionHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

            Current.DispatcherUnhandledException += (s, e) =>
            {
                LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
                e.Handled = true;
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                LogUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException");
                e.SetObserved();
            };
        }

        private static void LogUnhandledException(Exception exception, string source)
        {
            string message;
            string details;
            try
            {
                var assemblyName = Assembly.GetExecutingAssembly().GetName();
                message = "Er is een onverwachte fout opgetreden." + Environment.NewLine;
                message += "Gelieve dit probleem inclusief onderstaande details doorgeven aan de ontwikkelaar.";
                details = $"Fout in TLCGen (versie {Assembly.GetCallingAssembly().GetName().Version})." + Environment.NewLine + Environment.NewLine;
                details += $"Bron: {source}, in {assemblyName.Name} v{assemblyName.Version}." + Environment.NewLine + Environment.NewLine;
                details += exception;
            }
            catch (Exception ex)
            {
                message = "Er is een onverwachte fout opgetreden in LogUnhandledException." + Environment.NewLine;
                message += "Gelieve dit probleem inclusief onderstaande details doorgeven aan de ontwikkelaar.";
                details = "Fout in LogUnhandledException." + Environment.NewLine + Environment.NewLine;
                details += ex + Environment.NewLine + Environment.NewLine;
                details += "Oorspronkelijke fout:" + Environment.NewLine + Environment.NewLine + exception;
            }

            if (GalaSoft.MvvmLight.Threading.DispatcherHelper.UIDispatcher != null)
            {
                GalaSoft.MvvmLight.Threading.DispatcherHelper.UIDispatcher.Invoke(() =>
                {
                    var win = new UnhandledExceptionWindow
                    {
                        DialogTitle = "Onverwachte fout in TLCGen",
                        DialogMessage = message,
                        DialogExpceptionText = details
                    };
                    win.ShowDialog();
                });
            }
            else
            {
                try
                {
                    var win = new UnhandledExceptionWindow
                    {
                        DialogTitle = "Onverwachte fout in TLCGen",
                        DialogMessage = message,
                        DialogExpceptionText = details
                    };
                    win.ShowDialog();
                }
                catch
                {
                    MessageBox.Show("Onverwachte fout in TLCGen", message + Environment.NewLine + details);
                }
            }
        }
#endif
    }
}
