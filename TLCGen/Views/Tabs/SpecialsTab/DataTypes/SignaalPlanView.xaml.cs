using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using TLCGen.ViewModels;

namespace TLCGen.Views
{
    /// <summary>
    /// Interaction logic for SignaalPlanView.xaml
    /// </summary>
    public partial class SignaalPlanView : UserControl
	{
		public SignaalPlanView()
		{
			InitializeComponent();
            this.DataContextChanged += SignaalPlanView_DataContextChanged;
		}

        private void SignaalPlanView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            draw_all();
        }

        public void draw_all()
        {
            var fasen = (ObservableCollection<SignaalPlanFaseViewModel>)FasenDG.ItemsSource;
            if (fasen == null || fasen.Count == 0) return;

            var i = 0;

            var iXleft = 30;
            var iFCHSize = 14;

            phaseGrid.Children.Clear();
            phaseGrid.Width = FasenDG.ActualWidth;
            phaseGrid.Height = fasen.Count * iFCHSize;
            if (int.TryParse(CycleTimeTB.Text, out var ct))
            {
                var phases = new List<phasereal>();
                foreach (var fc in fasen)
                {
                    var fcA = TLCGen.DataAccess.TLCGenControllerDataProvider.Default.Controller.Fasen.FirstOrDefault(x => x.Naam == fc.FaseCyclus);
                    var p = new phasereal();
                    p.iCycleTime = ct;
                    p.Name = fc.FaseCyclus;
                    p.iStartGreen = fc.B1;
                    p.iMinGreen = fc.C1.HasValue ? fc.C1.Value : 0;
                    p.iEndGreen = fc.D1;
                    p.iEndYellow = fc.D1 + (int)(fcA.TGL / 10.0);
                    if (p.iStartGreen > 0 || p.iEndGreen > 0)
                        phases.Add(p);
                }
                foreach (var p in phases)
                {
                    p.baseline.Stroke = System.Windows.Media.Brushes.Black;
                    p.yellow1.Stroke = System.Windows.Media.Brushes.Yellow;
                    p.yellow2.Stroke = System.Windows.Media.Brushes.Yellow;
                    p.baseline.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    p.green1line.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    p.green2line.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    p.cline.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    p.green1line.StrokeThickness = 1;
                    p.green1line.StrokeThickness = 7;
                    p.green2line.StrokeThickness = 7;
                    p.cline.StrokeThickness = 7;
                    p.yellow1.StrokeThickness = 7;
                    p.yellow2.StrokeThickness = 7;
                    phaseGrid.Children.Add(p.baseline);
                    phaseGrid.Children.Add(p.green1line);
                    phaseGrid.Children.Add(p.green2line);
                    phaseGrid.Children.Add(p.cline);
                    phaseGrid.Children.Add(p.yellow1);
                    phaseGrid.Children.Add(p.yellow2);
                    p.fctextbox.Text = p.Name;
                    Canvas.SetTop(p.fctextbox, i * 14);
                    p.fctextbox.Foreground = Brushes.Black;
                    p.fctextbox.Background = Brushes.Transparent;
                    phaseGrid.Children.Add(p.fctextbox);
                    ++i;
                }

                i = 0;
                foreach (var p in phases)
                {
                    p.fctextbox.Foreground = Brushes.Black;
                    p.green1line.Stroke = System.Windows.Media.Brushes.Green;
                    p.green2line.Stroke = System.Windows.Media.Brushes.Green;
                    p.yellow1.Stroke = System.Windows.Media.Brushes.Yellow;
                    p.yellow2.Stroke = System.Windows.Media.Brushes.Yellow;
                    if (p.iMinGreen != 0)
                        p.cline.Stroke = System.Windows.Media.Brushes.Orange;
                    else
                        p.cline.Stroke = System.Windows.Media.Brushes.Transparent;

                    p.baseline.X1 = iXleft;
                    p.baseline.Y1 = 10 + i * iFCHSize;
                    p.baseline.X2 = phaseGrid.Width - 10;
                    p.baseline.Y2 = 10 + i * iFCHSize;

                    var max = phaseGrid.Width - 10;
                    var start = (((double)(p.iStartGreen - 1) / (double)p.iCycleTime) * (double)(phaseGrid.Width - 10 - iXleft));
                    var end = (((double)(p.iEndGreen - 1) / (double)p.iCycleTime) * (double)(phaseGrid.Width - 10 - iXleft));
                    var endyellow = (((double)(p.iEndYellow) / (double)p.iCycleTime) * (double)(phaseGrid.Width - 10 - iXleft));
                    var cmom = (((double)(p.iMinGreen - 1) / (double)p.iCycleTime) * (double)(phaseGrid.Width - 10 - iXleft));
                    var one = (double)(phaseGrid.Width - 10 - iXleft) / (double)p.iCycleTime;

                    if (start < end)
                    {
                        p.green1line.X1 = iXleft + (int)start;
                        p.green1line.Y1 = 10 + i * iFCHSize;
                        p.green1line.X2 = iXleft + (int)end;
                        p.green1line.Y2 = 10 + i * iFCHSize;
                        p.green2line.X1 = 0;
                        p.green2line.Y1 = 0;
                        p.green2line.X2 = 0; p.green2line.Y2 = 0;
                    }
                    else
                    {
                        p.green1line.X1 = iXleft + (int)start;
                        p.green1line.Y1 = 10 + i * iFCHSize;
                        p.green1line.X2 = phaseGrid.Width - 10;
                        p.green1line.Y2 = 10 + i * iFCHSize;
                        p.green2line.X1 = iXleft;
                        p.green2line.Y1 = 10 + i * iFCHSize;
                        p.green2line.X2 = iXleft + (int)end;
                        p.green2line.Y2 = 10 + i * iFCHSize;
                    }

                    if (p.iMinGreen != 0)
                    {
                        p.cline.X1 = iXleft + (int)cmom - one;
                        p.cline.Y1 = 10 + i * iFCHSize;
                        p.cline.X2 = iXleft + (int)cmom - one + 3;
                        p.cline.Y2 = 10 + i * iFCHSize;
                    }
                    if (end < endyellow)
                    {
                        p.yellow1.X1 = iXleft + (int)end;
                        p.yellow1.Y1 = 10 + i * iFCHSize;
                        p.yellow1.X2 = iXleft + (int)endyellow;
                        p.yellow1.Y2 = 10 + i * iFCHSize;
                        p.yellow2.X1 = 0;
                        p.yellow2.Y1 = 0;
                        p.yellow2.X2 = 0;
                        p.yellow2.Y2 = 0;
                    }
                    else
                    {
                        p.yellow1.X1 = iXleft + (int)end;
                        p.yellow1.Y1 = 10 + i * iFCHSize;
                        p.yellow1.X2 = phaseGrid.Width - 10;
                        p.yellow1.Y2 = 10 + i * iFCHSize;
                        p.yellow2.X1 = iXleft;
                        p.yellow2.Y1 = 10 + i * iFCHSize;
                        p.yellow2.X2 = iXleft + (int)endyellow;
                        p.yellow2.Y2 = 10 + i * iFCHSize;
                    }

                    ++i;
                }
            }
        }

        private void VisualisatieExpender_Expanded(object sender, RoutedEventArgs e)
        {
            draw_all();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            draw_all();
        }
    }

    public class phasereal : IComparable
    {

        public Line baseline { get; set; }
        public Line green1line { get; set; }
        public Line green2line { get; set; }
        public Line cline { get; set; }
        public Line yellow1 { get; set; }
        public Line yellow2 { get; set; }
        public TextBlock fctextbox { get; set; }

        public int iCycleTime { get; set; }
        public string Name { get; set; }
        public int iStartGreen { get; set; }
        public int iMinGreen { get; set; }
        private int _iEndGreen;
        public int iEndGreen
        {
            get => _iEndGreen;
            set
            {
                _iEndGreen = value;
                if (iStartGreen != 0 && iCycleTime != 0)
                {
                    if (iStartGreen < _iEndGreen) iGreenTime = _iEndGreen - iStartGreen;
                    else iGreenTime = _iEndGreen - iStartGreen + iCycleTime;
                }
            }
        }
        private int _iEndYellow;
        public int iEndYellow
        {
            get => _iEndYellow;
            set
            {
                if (value > iCycleTime)
                    _iEndYellow = value - iCycleTime;
                else
                    _iEndYellow = value;
            }
        }
        public int iGreenTime { get; set; }

        public phasereal()
        {
            baseline = new Line();
            green1line = new Line();
            green2line = new Line();
            cline = new Line();
            yellow1 = new Line();
            yellow2 = new Line();
            fctextbox = new TextBlock();
        }

        public int CompareTo(object obj)
        {
            var p = (phasereal)obj;
            return Name.CompareTo(p.Name);
        }
    }
}
