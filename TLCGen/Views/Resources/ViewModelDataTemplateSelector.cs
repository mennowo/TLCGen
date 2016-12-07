using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TLCGen.ViewModels;

namespace TLCGen.Views
{
    public class ViewModelDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null && item is TLCGenTabItemViewModel)
            {
                TLCGenTabItemViewModel vm = item as TLCGenTabItemViewModel;
                if (vm != null)
                {
                    string vmname = vm.GetType().Name;
                    Window window = System.Windows.Application.Current.MainWindow;
                    DataTemplate t = new DataTemplate();
                    string s = vmname.Remove(vmname.Length - 5);
                    var uc = window.FindName("TLCGen.Views." + s);
                    t.VisualTree = new FrameworkElementFactory(uc.GetType());
                    return t;
                }
            }

            return null;
        }
    }
}
