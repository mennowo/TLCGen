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
    /// Interaction logic for AutoBuildToolBarView.xaml
    /// </summary>
    public partial class AutoBuildToolBarView : UserControl
    {
        public AutoBuildToolBarView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var w = Window.GetWindow(this);
            foreach(InputBinding ib in InputBindings)
            {
                w.InputBindings.Add(ib);
            }
        }

		private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}
	}
}
