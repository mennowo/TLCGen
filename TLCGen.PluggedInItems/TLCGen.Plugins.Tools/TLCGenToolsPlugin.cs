using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Plugins;
using TLCGen.Settings;

namespace TLCGen.Plugins.Tools
{
    [TLCGenTabItem(-1)]
    [TLCGenPlugin(TLCGenPluginElems.TabControl | TLCGenPluginElems.HasSettings)]
    public class TLCGenToolsPlugin : ITLCGenTabItem, ITLCGenHasSettings
    {
        #region Fields

        private ControllerModel _controller;
        private DataTemplate _ContentDataTemplate;
        private TLCGenToolsTabViewModel _combinatieTemplateVM;
        private ObservableCollection<CombinatieTemplateViewModel> CombinatieTemplates { get; }
        private CombinatieTemplatesModel _combinatieTemplatesModel;

        #endregion // Fields

        #region Properties

        public string TemplatesLocation
        {
            get
            {
                var cset = SettingsProvider.Default.Settings.CustomData.AddinSettings.FirstOrDefault(x => x.Naam == "TLCGen.Tools.CombinatieTemplates");
                if (cset == null) return "";
                var set = cset.Properties.FirstOrDefault(x => x.Naam == "CombinatieTemplatesLocation");
                if (set == null) return "";
                return set.Setting;
            }
            set
            {
                var cset = SettingsProvider.Default.Settings.CustomData.AddinSettings.FirstOrDefault(x => x.Naam == "TLCGen.Tools.CombinatieTemplates");
                if (cset == null)
                {
                    cset = new AddinSettingsModel { Naam = "TLCGen.Tools.CombinatieTemplates" };
                    SettingsProvider.Default.Settings.CustomData.AddinSettings.Add(cset);
                }
                var set = cset.Properties.FirstOrDefault(x => x.Naam == "CombinatieTemplatesLocation");
                if (set == null)
                {
                    set = new AddinSettingsPropertyModel { Naam = "CombinatieTemplatesLocation" };
                    cset.Properties.Add(set);
                }
                set.Setting = value;

                if (!string.IsNullOrWhiteSpace(set.Setting) &&
                    !File.Exists(set.Setting))
                {
                    try
                    {
                        TLCGenSerialization.Serialize(set.Setting, new CombinatieTemplatesModel());
                    }
                    catch
                    {
                        // ignored
                        return;
                    }
                }
                LoadSettings();
            }
        }

        #endregion // Properties

        #region ITLCGenTabItem

        public string DisplayName => "Tools";

        public ImageSource Icon
        {
            get
            {
                var dict = new ResourceDictionary();
                var u = new Uri("pack://application:,,,/" +
                                System.Reflection.Assembly.GetExecutingAssembly().GetName().Name +
                                ";component/" + "Resources/TabIcons.xaml");
                dict.Source = u;
                return (DrawingImage)dict["ToolsTabDrawingImage"];
            }
        }

        public DataTemplate ContentDataTemplate
        {
            get
            {
                if (_ContentDataTemplate == null)
                {
                    _ContentDataTemplate = new DataTemplate();
                    var tab = new FrameworkElementFactory(typeof(TLCGenToolsTabView));
                    tab.SetValue(TLCGenToolsTabView.DataContextProperty, _combinatieTemplateVM);
                    _ContentDataTemplate.VisualTree = tab;
                }
                return _ContentDataTemplate;
            }
        }

        public bool IsEnabled
        {
            get => true;
            set { }
        }
        public ControllerModel Controller
        {
            get => _controller;
            set
            {
                _controller = value;
                _combinatieTemplateVM.Controller = Controller;
            }
        }

        public bool CanBeEnabled()
        {
            return true;
        }

        public string GetPluginName()
        {
            return "TLCGen.Plugins.Tools";
        }

        public void LoadTabs()
        {
        }

        public void OnDeselected()
        {
        }

        public bool OnDeselectedPreview()
        {
            return true;
        }

        public void OnSelected()
        {
        }

        public bool OnSelectedPreview()
        {
            return true;
        }

        #endregion // ITLCGenTabItem
        
        #region ITLCGenHasSettings

        public void LoadSettings()
        {
            CombinatieTemplates.Clear();
            if (File.Exists(TemplatesLocation))
            {
                try
                {
                    _combinatieTemplatesModel = TLCGenSerialization.DeSerialize<CombinatieTemplatesModel>(TemplatesLocation);
                    CombinatieTemplates.CollectionChanged -= CombinatieTemplates_CollectionChanged;
                    foreach (var t in _combinatieTemplatesModel.CombinatieTemplates)
                    {
                        CombinatieTemplates.Add(new CombinatieTemplateViewModel(t));
                    }
                    CombinatieTemplates.CollectionChanged += CombinatieTemplates_CollectionChanged;
                }
                catch (Exception e)
                {
                    Dependencies.Providers.TLCGenDialogProvider.Default.ShowMessageBox($"Fout bij uitlezen van combinatie templates file:\n\n{e.ToString()}", "Fout in combinatie templates file", MessageBoxButton.OK);
                }
            }
        }

        public void SaveSettings()
        {
            if (_combinatieTemplatesModel == null) return;
            var t = TemplatesLocation;
            if (!string.IsNullOrWhiteSpace(t))
            {
                try
                {
                    TLCGenSerialization.Serialize(t, _combinatieTemplatesModel);
                }
                catch (Exception e)
                {
                    Dependencies.Providers.TLCGenDialogProvider.Default.ShowMessageBox($"Fout bij opslaan van combinatie templates file:\n\n{e.ToString()}", "Fout bij opslaan combinatie templates file", MessageBoxButton.OK);
                }
            }
        }

        #endregion // ITLCGenHasSettings
        #region Constructor

        public TLCGenToolsPlugin()
        {
            CombinatieTemplates = new ObservableCollection<CombinatieTemplateViewModel>();
            CombinatieTemplates.CollectionChanged += CombinatieTemplates_CollectionChanged;
            _combinatieTemplateVM = new TLCGenToolsTabViewModel(CombinatieTemplates, this);
        }

        private void CombinatieTemplates_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (CombinatieTemplateViewModel vvm in e.NewItems)
                {
                    _combinatieTemplatesModel.CombinatieTemplates.Add(vvm.Template);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (CombinatieTemplateViewModel vvm in e.OldItems)
                {
                    _combinatieTemplatesModel.CombinatieTemplates.Remove(vvm.Template);
                }
            }
        }

        #endregion // Constructor
    }
}
