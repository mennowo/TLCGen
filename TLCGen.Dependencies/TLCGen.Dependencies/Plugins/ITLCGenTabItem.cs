using System.Windows;
using System.Windows.Media;
using TLCGen.Models;

namespace TLCGen.Plugins
{
    public interface ITLCGenTabItem : ITLCGenPlugin
    {
        string DisplayName { get; }
        ImageSource Icon { get; }
        DataTemplate ContentDataTemplate { get; }
        
        bool IsEnabled { get; set; }
        bool Visibility { get; set; }

        void LoadTabs();
        bool CanBeEnabled();
        void OnSelected();
        bool OnSelectedPreview();
        void OnDeselected();
        bool OnDeselectedPreview();
    }
}
