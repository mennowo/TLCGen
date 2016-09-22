using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TLCGen.Helpers;

namespace TLCGen.Views
{
    public class ZoomBorder : Border
    {
        private UIElement child = null;
        private System.Windows.Point origin;
        private System.Windows.Point start;
        private Timer delayedclicktimer = new Timer(200);

        public bool SetReset
        {
            get { return false; }
            set
            {
                if (value)
                    this.Reset();
            }
        }

        public static readonly DependencyProperty SetResetProperty =
            DependencyProperty.Register("SetReset", typeof(bool), typeof(ZoomBorder), new PropertyMetadata(false));

        public RelayCommand ClickedCommand
        {
            get { return (RelayCommand)GetValue(ClickedCommandProperty); }
            set { SetValue(ClickedCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClickedCommandProperty =
            DependencyProperty.Register("ClickedCommand", typeof(RelayCommand), typeof(ZoomBorder), new PropertyMetadata(null));

        private TranslateTransform GetTranslateTransform(UIElement element)
        {
            return (TranslateTransform)((TransformGroup)element.RenderTransform)
              .Children.First(tr => tr is TranslateTransform);
        }

        private ScaleTransform GetScaleTransform(UIElement element)
        {
            return (ScaleTransform)((TransformGroup)element.RenderTransform)
              .Children.First(tr => tr is ScaleTransform);
        }

        public override UIElement Child
        {
            get { return base.Child; }
            set
            {
                if (value != null && value != this.Child)
                    this.Initialize(value);
                base.Child = value;
            }
        }

        public void Initialize(UIElement element)
        {
            this.child = element;
            if (child != null)
            {
                TransformGroup group = new TransformGroup();
                ScaleTransform st = new ScaleTransform();
                group.Children.Add(st);
                TranslateTransform tt = new TranslateTransform();
                group.Children.Add(tt);
                child.RenderTransform = group;
                child.RenderTransformOrigin = new System.Windows.Point(0.0, 0.0);
                this.MouseWheel += child_MouseWheel;
                this.MouseLeftButtonDown += child_MouseLeftButtonDown;
                this.MouseLeftButtonUp += child_MouseLeftButtonUp;
                this.MouseMove += child_MouseMove;
                this.PreviewMouseRightButtonDown += new MouseButtonEventHandler(
                  child_PreviewMouseRightButtonDown);
                delayedclicktimer.Elapsed += Delayedclicktimer_Elapsed;
            }
        }

        public void Reset()
        {

            if (child != null)
            {
                // reset zoom
                var st = GetScaleTransform(child);
                st.ScaleX = 1.0;
                st.ScaleY = 1.0;

                // reset pan
                var tt = GetTranslateTransform(child);
                tt.X = 0.0;
                tt.Y = 0.0;
            }
        }

        #region Child Events

        private void child_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (child != null)
            {
                var st = GetScaleTransform(child);
                var tt = GetTranslateTransform(child);

                double zoom = e.Delta > 0 ? .2 : -.2;
                if (!(e.Delta > 0) && (st.ScaleX < .4 || st.ScaleY < .4))
                    return;

                System.Windows.Point relative = e.GetPosition(child);
                double abosuluteX;
                double abosuluteY;

                abosuluteX = relative.X * st.ScaleX + tt.X;
                abosuluteY = relative.Y * st.ScaleY + tt.Y;

                st.ScaleX += zoom;
                st.ScaleY += zoom;

                tt.X = abosuluteX - relative.X * st.ScaleX;
                tt.Y = abosuluteY - relative.Y * st.ScaleY;
            }
        }

        private void Delayedclicktimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => 
                {
                    delayedclicktimer.Stop();
                    this.Cursor = Cursors.Hand;
                    child.CaptureMouse();
                } ));
        }

        private void child_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var tt = GetTranslateTransform(child);
            origin = new System.Windows.Point(tt.X, tt.Y);
            start = e.GetPosition(this);
            delayedclicktimer.Start();
        }

        private void child_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (child != null)
            {
                if (delayedclicktimer.Enabled)
                {
                    delayedclicktimer.Stop();

                    if (ClickedCommand.CanExecute(null))
                    {
                        PresentationSource source = PresentationSource.FromVisual(this);

                        System.Drawing.Point p = new System.Drawing.Point();

                        if (source != null)
                        {
                            p = new System.Drawing.Point(
                                (int)(e.GetPosition(child).X * source.CompositionTarget.TransformToDevice.M11),
                                (int)(e.GetPosition(child).Y * source.CompositionTarget.TransformToDevice.M22));

                            ClickedCommand.Execute(p);
                        }
                    }
                }
                child.ReleaseMouseCapture();
                this.Cursor = Cursors.Arrow;
            }
        }

        void child_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Reset();
        }

        private void child_MouseMove(object sender, MouseEventArgs e)
        {
            if (child != null)
            {
                if (child.IsMouseCaptured)
                {
                    var tt = GetTranslateTransform(child);
                    Vector v = start - e.GetPosition(this);
                    tt.X = origin.X - v.X;
                    tt.Y = origin.Y - v.Y;
                }
            }
        }

        #endregion
    }
}
