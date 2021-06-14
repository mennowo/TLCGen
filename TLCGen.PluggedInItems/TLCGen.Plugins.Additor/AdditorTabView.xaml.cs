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

namespace TLCGen.Plugins.Additor
{
    /// <summary>
    /// Interaction logic for AdditorTabView.xaml
    /// </summary>
    public partial class AdditorTabView : UserControl
    {
        public AdditorTabView()
        {
            InitializeComponent();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is AdditorTabView ed)
            {
                if (e.NewValue is AdditorTabViewModel edvm)
                {
                    edvm.ViewEditor = ed.textEditor;
                }
            }
        }
    }
}
