using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TLCGen.Controls
{
    /// <summary>
    /// Interaction logic for InfoElement.xaml
    /// </summary>
    public partial class InfoElement : UserControl
    {
        public InfoElement()
        {
            InitializeComponent();
        }

        public FrameworkElement InfoPopup
        {
            get => (FrameworkElement)GetValue(InfoPopupProperty);
            set => SetValue(InfoPopupProperty, value);
        }

        // Using a DependencyProperty as the backing store for InfoText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InfoPopupProperty =
            DependencyProperty.Register("InfoPopup", typeof(FrameworkElement), typeof(InfoElement), new PropertyMetadata(null, new PropertyChangedCallback(OnInfoPopupChanged)));

        private static void OnInfoPopupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ie = (InfoElement)d;
            var b = new Border();
            b.Padding = new Thickness(2);
            b.Background = Brushes.Beige;
            b.BorderBrush = Brushes.DarkGray;
            b.BorderThickness = new Thickness(0.5);
            var i = e.NewValue as UIElement;
            if (i != null)
            {
                b.Child = i;
            }
            else
            {
                var s = e.NewValue as string;
                if(s != null)
                {
                    b.Child = new TextBlock() { Text = s };
                }
            }
            ie.InfoPopupElem.Child = b;
        }

    }
}
