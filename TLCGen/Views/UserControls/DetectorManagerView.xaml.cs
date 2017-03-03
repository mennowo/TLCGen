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

namespace TLCGen.Views
{
    /// <summary>
    /// Interaction logic for DetectorManagerView.xaml
    /// </summary>
    public partial class DetectorManagerView : UserControl
    {
        public DetectorManagerView()
        {
            InitializeComponent();
        }



        public Visibility RemovableDetectorsVisibility
        {
            get { return (Visibility)GetValue(RemovableDetectorsVisibilityProperty); }
            set { SetValue(RemovableDetectorsVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RemovableDetectorsVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RemovableDetectorsVisibilityProperty =
            DependencyProperty.Register("RemovableDetectorsVisibility", typeof(Visibility), typeof(DetectorManagerView), new PropertyMetadata(Visibility.Collapsed));



        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Caption.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register("Caption", typeof(string), typeof(DetectorManagerView), new PropertyMetadata("Detectoren"));



        public bool ShowCaption
        {
            get { return (bool)GetValue(ShowCaptionProperty); }
            set { SetValue(ShowCaptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowCaption.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowCaptionProperty =
            DependencyProperty.Register("ShowCaption", typeof(bool), typeof(DetectorManagerView), new PropertyMetadata(true));


    }
}
