using System;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;
using TLCGen.Helpers;

namespace TLCGen.Controls
{
    public class ZoomViewbox : Viewbox
    {
        private UIElement _child = null;
        private System.Windows.Point _origin;
        private System.Windows.Point _start;
        private Timer _delayedclicktimer = new(200);

        public bool SetReset
        {
            get => false;
            set
            {
                if (value)
                    this.Reset();
            }
        }

        public static readonly DependencyProperty SetResetProperty =
            DependencyProperty.Register(nameof(SetReset), typeof(bool), typeof(ZoomViewbox), new PropertyMetadata(false));

        public ICommand ClickedCommand
        {
            get => (ICommand)GetValue(ClickedCommandProperty);
            set => SetValue(ClickedCommandProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClickedCommandProperty =
            DependencyProperty.Register(nameof(ClickedCommand), typeof(ICommand), typeof(ZoomViewbox), new PropertyMetadata(null));

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
            get => base.Child;
            set
            {
                if (value != null && value != this.Child)
                    this.Initialize(value);
                base.Child = value;
            }
        }

        public void Initialize(UIElement element)
        {
            this._child = element;
            if (_child != null)
            {
                var group = new TransformGroup();
                var st = new ScaleTransform();
                group.Children.Add(st);
                var tt = new TranslateTransform();
                group.Children.Add(tt);
                _child.RenderTransform = group;
                _child.RenderTransformOrigin = new Point(0.0, 0.0);
                MouseWheel += child_MouseWheel;
                MouseLeftButtonDown += child_MouseLeftButtonDown;
                MouseLeftButtonUp += child_MouseLeftButtonUp;
                MouseMove += child_MouseMove;
                PreviewMouseRightButtonDown += child_PreviewMouseRightButtonDown;
                _delayedclicktimer.Elapsed += Delayedclicktimer_Elapsed;
            }
        }

        public void Reset()
        {

            if (_child != null)
            {
                // reset zoom
                var st = GetScaleTransform(_child);
                st.ScaleX = 1.0;
                st.ScaleY = 1.0;

                // reset pan
                var tt = GetTranslateTransform(_child);
                tt.X = 0.0;
                tt.Y = 0.0;
            }
        }

        #region Child Events

        private void child_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_child != null)
            {
                var st = GetScaleTransform(_child);
                var tt = GetTranslateTransform(_child);

                var zoom = e.Delta > 0 ? .2 : -.2;
                if (!(e.Delta > 0) && (st.ScaleX < .4 || st.ScaleY < .4))
                    return;

                var relative = e.GetPosition(_child);

                var absoluteX = relative.X * st.ScaleX + tt.X;
                var absoluteY = relative.Y * st.ScaleY + tt.Y;

                st.ScaleX += zoom;
                st.ScaleY += zoom;

                tt.X = absoluteX - relative.X * st.ScaleX;
                tt.Y = absoluteY - relative.Y * st.ScaleY;
            }
        }

        private void Delayedclicktimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => 
                {
                    _delayedclicktimer.Stop();
                    this.Cursor = Cursors.Hand;
                    _child.CaptureMouse();
                } ));
        }

        private void child_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var tt = GetTranslateTransform(_child);
            _origin = new Point(tt.X, tt.Y);
            _start = e.GetPosition(this);
            _delayedclicktimer.Start();
        }

        private void child_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_child != null)
            {
                if (_delayedclicktimer.Enabled)
                {
                    _delayedclicktimer.Stop();

                    if (ClickedCommand.CanExecute(null))
                    {
                        var source = PresentationSource.FromVisual(this);

                        if (source is { CompositionTarget: not null })
                        {
                            var p = new System.Drawing.Point(
                                (int)(e.GetPosition(_child).X * source.CompositionTarget.TransformToDevice.M11),
                                (int)(e.GetPosition(_child).Y * source.CompositionTarget.TransformToDevice.M22));

                            ClickedCommand.Execute(p);
                        }
                    }
                }
                _child.ReleaseMouseCapture();
                this.Cursor = Cursors.Arrow;
            }
        }

        void child_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Reset();
        }

        private void child_MouseMove(object sender, MouseEventArgs e)
        {
            if (_child is { IsMouseCaptured: true })
            {
                var tt = GetTranslateTransform(_child);
                var v = _start - e.GetPosition(this);
                tt.X = _origin.X - v.X;
                tt.Y = _origin.Y - v.Y;
            }
        }

        #endregion
    }
}
