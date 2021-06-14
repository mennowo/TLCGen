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

namespace TLCGen.Plugins.AutoBuild
{
    /// <summary>
    /// Interaction logic for AutoBuildConsoleTabView.xaml
    /// </summary>
    public partial class AutoBuildConsoleTabView : UserControl
    {
        public AutoBuildConsoleTabView()
        {
            InitializeComponent();
        }

        private Boolean AutoScroll = true;
        private void Scroller_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {

            // User scroll event : set or unset autoscroll mode
            if (e.ExtentHeightChange == 0)
            {   // Content unchanged : user scroll event
                if (LogScroller.VerticalOffset == LogScroller.ScrollableHeight)
                {   // Scroll bar is in bottom
                    // Set autoscroll mode
                    AutoScroll = true;
                }
                else
                {   // Scroll bar isn't in bottom
                    // Unset autoscroll mode
                    AutoScroll = false;
                }
            }

            // Content scroll event : autoscroll eventually
            if (AutoScroll && e.ExtentHeightChange != 0)
            {   // Content changed and autoscroll mode set
                // Autoscroll
                LogScroller.ScrollToVerticalOffset(LogScroller.ExtentHeight);
            }
        }
    }
}
