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

namespace TLCGen.Plugins.Tools
{
    /// <summary>
    /// Interaction logic for CombinatieTemplateView.xaml
    /// </summary>
    public partial class CombinatieTemplateView : UserControl
    {
        public CombinatieTemplateView()
        {
            InitializeComponent();
        }
    }

    public class CombinatieTemplateOptieTemplateSelector : DataTemplateSelector
    {
        public DataTemplate IntValueTemplate { get; set; }

        public DataTemplate StringValueTemplate { get; set; }

        public DataTemplate FaseCyclusValueTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var optie = item as CombinatieTemplateOptieViewModel;

            if (optie != null)
            {
                switch (optie.Type)
                {
                    case CombinatieTemplateOptieTypeEnum.Fase:
                        return FaseCyclusValueTemplate;
                    case CombinatieTemplateOptieTypeEnum.Int:
                        return IntValueTemplate;
                    case CombinatieTemplateOptieTypeEnum.String:
                        return StringValueTemplate;
                    default:
                        throw new NotSupportedException();
                }
            }
            else
            {
                return base.SelectTemplate(item, container);
            }
        }
    }

}
