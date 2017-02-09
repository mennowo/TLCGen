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

namespace TLCGen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MainToolBarTray.DataContextChanged += (s, e) =>
            {
                var vm = e.NewValue as ViewModels.MainWindowViewModel;
                if (vm != null)
                {
                    foreach (var pl in vm.ApplicationPlugins)
                    {
                        var tbpl = pl as Plugins.ITLCGenToolBar;
                        if (tbpl != null)
                        {
                            var tb = new ToolBar();
                            tb.Items.Add(tbpl.ToolBarView);
                            MainToolBarTray.ToolBars.Add(tb);
                        }
                    }
                }
            };

            ViewModels.MainWindowViewModel mvm = new ViewModels.MainWindowViewModel();
            this.DataContext = mvm;
        }
    }
}
