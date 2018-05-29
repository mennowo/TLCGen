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
    public partial class ItemsManagerView : UserControl
    {
        public ItemsManagerView()
        {
            InitializeComponent();
        }

        public Visibility RemovableItemsVisibility
        {
            get { return (Visibility)GetValue(RemovableItemsVisibilityProperty); }
            set { SetValue(RemovableItemsVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RemovableDetectorsVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RemovableItemsVisibilityProperty =
            DependencyProperty.Register("RemovableItemsVisibilityProperty", typeof(Visibility), typeof(ItemsManagerView), new PropertyMetadata(Visibility.Collapsed));

        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Caption.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register("Caption", typeof(string), typeof(ItemsManagerView), new PropertyMetadata("Items"));

        public bool ShowCaption
        {
            get { return (bool)GetValue(ShowCaptionProperty); }
            set { SetValue(ShowCaptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowCaption.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowCaptionProperty =
            DependencyProperty.Register("ShowCaption", typeof(bool), typeof(ItemsManagerView), new PropertyMetadata(true));

    }
}
