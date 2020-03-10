using System.Windows;
using System.Windows.Controls;

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

        public Visibility InsertableItemsVisibility
        {
            get => (Visibility)GetValue(InsertableItemsVisibilityProperty);
            set => SetValue(InsertableItemsVisibilityProperty, value);
        }

        // Using a DependencyProperty as the backing store for InsertableDetectorsVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InsertableItemsVisibilityProperty =
            DependencyProperty.Register("InsertableItemsVisibilityProperty", typeof(Visibility), typeof(ItemsManagerView), new PropertyMetadata(Visibility.Collapsed));

        public Visibility RemovableItemsVisibility
        {
            get => (Visibility)GetValue(RemovableItemsVisibilityProperty);
            set => SetValue(RemovableItemsVisibilityProperty, value);
        }

        // Using a DependencyProperty as the backing store for RemovableDetectorsVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RemovableItemsVisibilityProperty =
            DependencyProperty.Register("RemovableItemsVisibilityProperty", typeof(Visibility), typeof(ItemsManagerView), new PropertyMetadata(Visibility.Collapsed));

        public string Caption
        {
            get => (string)GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }

        // Using a DependencyProperty as the backing store for Caption.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register("Caption", typeof(string), typeof(ItemsManagerView), new PropertyMetadata("Items"));

        public bool ShowCaption
        {
            get => (bool)GetValue(ShowCaptionProperty);
            set => SetValue(ShowCaptionProperty, value);
        }

        // Using a DependencyProperty as the backing store for ShowCaption.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowCaptionProperty =
            DependencyProperty.Register("ShowCaption", typeof(bool), typeof(ItemsManagerView), new PropertyMetadata(true));

    }
}
