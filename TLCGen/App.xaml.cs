using System.Windows;
using System.Windows.Shell;

namespace TLCGen
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnJumpItemsRejected(object sender, JumpItemsRejectedEventArgs e) { }
        private void OnJumpItemsRemoved(object sender, JumpItemsRemovedEventArgs e) { }
    }
}
