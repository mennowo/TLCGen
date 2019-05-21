using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.Plugins.Tools
{
    [TLCGenTabItem(-1)]
    [TLCGenPlugin(TLCGenPluginElems.TabControl)]
    public class TLCGenToolsPlugin : ITLCGenTabItem
    {
        private ControllerModel _controller;
        DataTemplate _ContentDataTemplate;
        TLCGenToolsTabViewModel _combinatieTemplateVM;

        #region ITLCGenTabItem

        public string DisplayName => "Tools";

        public ImageSource Icon
        {
            get
            {
                ResourceDictionary dict = new ResourceDictionary();
                Uri u = new Uri("pack://application:,,,/" +
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

        public TLCGenToolsPlugin()
        {
            var templates = new List<CombinatieTemplateViewModel>();
            templates.Add(new CombinatieTemplateViewModel(new CombinatieTemplateModel
            {
                Name = "Voetgangersoversteek",
                Items = new List<CombinatieTemplateItemModel>
                {
                    new CombinatieTemplateItemModel
                    {
                        Type = CombinatieTemplateItemTypeEnum.Naloop,
                        ObjectJson = "{\"FaseVan\":\"VAN\",\"FaseNaar\":\"NAAR\",\"Type\":0,\"VasteNaloop\":true,\"InrijdenTijdensGroen\":false,\"DetectieAfhankelijk\":false,\"MaximaleVoorstart\":null,\"Detectoren\":[],\"Tijden\":[{\"Type\":2,\"Waarde\":-101}]}"
                    },
                    new CombinatieTemplateItemModel
                    {
                        Type = CombinatieTemplateItemTypeEnum.Naloop,
                        ObjectJson = "{\"FaseVan\":\"NAAR\",\"FaseNaar\":\"VAN\",\"Type\":0,\"VasteNaloop\":true,\"InrijdenTijdensGroen\":false,\"DetectieAfhankelijk\":false,\"MaximaleVoorstart\":null,\"Detectoren\":[],\"Tijden\":[{\"Type\":2,\"Waarde\":-102}]}"
                    },
                },
                Opties = new List<CombinatieTemplateOptieModel>
                {
                     new CombinatieTemplateOptieModel{ Type = CombinatieTemplateOptieTypeEnum.Fase, Description = "Fase 1", Search= "VAN", Replace = "31"},
                     new CombinatieTemplateOptieModel{ Type = CombinatieTemplateOptieTypeEnum.Fase, Description = "Fase 2", Search= "NAAR", Replace = "32"},
                     new CombinatieTemplateOptieModel{ Type = CombinatieTemplateOptieTypeEnum.Int, Description = "Naloop tijd 1 > 2", Search= "-101", Replace = "100"},
                     new CombinatieTemplateOptieModel{ Type = CombinatieTemplateOptieTypeEnum.Int, Description = "Naloop tijd 2 > 1", Search= "-102", Replace = "100"},
                }
            }));
            _combinatieTemplateVM = new TLCGenToolsTabViewModel(templates);
        }
    }
}
