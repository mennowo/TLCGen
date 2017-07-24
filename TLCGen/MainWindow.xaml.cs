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
using GalaSoft.MvvmLight.Threading;
using TLCGen.Plugins;

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

            DispatcherHelper.Initialize();

            MainToolBarTray.DataContextChanged += (s, e) =>
            {
                var vm = e.NewValue as ViewModels.MainWindowViewModel;
                if (vm != null)
                {
                    foreach (var pl in vm.ApplicationParts)
                    {
                        if ((pl.Item1 & TLCGenPluginElems.ToolBarControl) == TLCGenPluginElems.ToolBarControl)
                        {
                            var tb = new ToolBar();
                            tb.Items.Add((pl.Item2 as ITLCGenToolBar).ToolBarView);
                            MainToolBarTray.ToolBars.Add(tb);
                        }
                    }
                }
            };

            ViewModels.MainWindowViewModel mvm = new ViewModels.MainWindowViewModel();
            this.DataContext = mvm;
        }

        private void MainWindow_OnPreviewDragEnter(object sender, DragEventArgs e)
        {
            int o = 0;
        }
    }
}
