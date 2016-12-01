using System.Windows;
using TLCGen.Models;

namespace TLCGen.Plugins
{
    public interface ITLCGenTabItem : ITLCGenPlugin
    {
        string DisplayName { get; }
        bool IsEnabled { get; set; }
        DataTemplate ContentDataTemplate { get; }
        ControllerModel Controller { get; set; }

        bool CanBeEnabled();
        void OnSelected();
        bool OnSelectedPreview();
        void OnDeselected();
        bool OnDeselectedPreview();

    }
}
