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
                var vm = item as TLCGenTabItemViewModel;
                if (vm != null)
                {
                    var vmname = vm.GetType().Name;
                    var window = System.Windows.Application.Current.MainWindow;
                    var t = new DataTemplate();
                    var s = vmname.Remove(vmname.Length - 5);
                    var uc = window.FindName("TLCGen.Views." + s);
                    t.VisualTree = new FrameworkElementFactory(uc.GetType());
                    return t;
                }
            }

            return null;
        }
    }
}
