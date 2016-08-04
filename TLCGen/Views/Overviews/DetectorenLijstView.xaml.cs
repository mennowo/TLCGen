using System;
using System.Collections;
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

namespace TLCGen.Views
{
    /// <summary>
    /// Interaction logic for DetectorenLijstView.xaml
    /// </summary>
    public partial class DetectorenLijstView : UserControl
    {
        public bool ShowFaseCyclus
        {
            get { return (bool)GetValue(ShowFaseCyclusProperty); }
            set { SetValue(ShowFaseCyclusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowFaseCyclus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowFaseCyclusProperty =
            DependencyProperty.Register("ShowFaseCyclus", typeof(bool), typeof(DetectorenLijstView), new PropertyMetadata(false));

        public bool ShowAanvraagVerlengen
        {
            get { return (bool)GetValue(ShowAanvraagVerlengenProperty); }
            set { SetValue(ShowAanvraagVerlengenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowFaseCyclus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowAanvraagVerlengenProperty =
            DependencyProperty.Register("ShowAanvraagVerlengen", typeof(bool), typeof(DetectorenLijstView), new PropertyMetadata(true));

        public DetectorenLijstView()
        {
            InitializeComponent();
        }
    }
}
