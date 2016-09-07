using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using TLCGen.Interfaces.Public;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL
{

    //Custom editors that are used as attributes MUST implement the ITypeEditor interface.
    public class FolderEditor : Xceed.Wpf.Toolkit.PropertyGrid.Editors.ITypeEditor
    {
        TextBox textBox = new TextBox();
        Binding _binding;

        public FrameworkElement ResolveEditor(Xceed.Wpf.Toolkit.PropertyGrid.PropertyItem propertyItem)
        {
            Grid g = new Grid();
            ColumnDefinition cd1 = new ColumnDefinition();
            ColumnDefinition cd2 = new ColumnDefinition();
            cd1.Width = new GridLength(1, GridUnitType.Star);
            cd2.Width = GridLength.Auto;
            g.ColumnDefinitions.Add(cd1);
            g.ColumnDefinitions.Add(cd2);
            Button b = new Button();
            b.Click += B_Click;
            b.Margin = new Thickness(2);
            b.Content = "...";
            Grid.SetColumn(textBox, 0);
            Grid.SetColumn(b, 1);
            g.Children.Add(textBox);
            g.Children.Add(b);
            
            //create the binding from the bound property item to the editor
            _binding = new Binding("Value"); //bind to the Value property of the PropertyItem
            _binding.Source = propertyItem;
            _binding.ValidatesOnExceptions = true;
            _binding.ValidatesOnDataErrors = true;
            _binding.Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
            BindingOperations.SetBinding(textBox, TextBox.TextProperty, _binding);
            return g;
        }

        private void B_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if(result == System.Windows.Forms.DialogResult.OK)
            {
                textBox.Text = dialog.SelectedPath;
                BindingOperations.GetBindingExpression(textBox, TextBox.TextProperty).UpdateSource();
            }
        }
    }

    [TLCGenGenerator]
    public partial class CCOLCodeGenerator : IGenerator
    {
        #region IGenerator

        public string GenerateSourceFiles(ControllerModel controller, string sourcefilepath)
        {
            if (Directory.Exists(sourcefilepath))
            {
                CollectAllCCOLElements(controller);

                File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}reg.c"), GenerateRegC(controller));
                File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}tab.c"), GenerateTabC(controller));
                File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}dpl.c"), GenerateDplC(controller));
                File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}sim.c"), GenerateSimC(controller));
                File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}sys.h"), GenerateSysH(controller));

                if (!File.Exists(Path.Combine(sourcefilepath, $"{controller.Data.Naam}reg.add")))
                    File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}reg.add"), GenerateRegAdd(controller));
                if (!File.Exists(Path.Combine(sourcefilepath, $"{controller.Data.Naam}tab.add")))
                    File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}tab.add"), GenerateTabAdd(controller));
                if (!File.Exists(Path.Combine(sourcefilepath, $"{controller.Data.Naam}dpl.add")))
                    File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}dpl.add"), GenerateDplAdd(controller));
                if (!File.Exists(Path.Combine(sourcefilepath, $"{controller.Data.Naam}sim.add")))
                    File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}sim.add"), GenerateSimAdd(controller));
                if (!File.Exists(Path.Combine(sourcefilepath, $"{controller.Data.Naam}sys.add")))
                    File.WriteAllText(Path.Combine(sourcefilepath, $"{controller.Data.Naam}sys.add"), GenerateSysAdd(controller));
                return "CCOL source code gegenereerd";
            }
            return $"Map {sourcefilepath} niet gevonden. Niets gegenereerd.";
        }

        public string GenerateProjectFiles(ControllerModel controller, string projectfilepath)
        {
            string result = "test";

            File.WriteAllText(Path.Combine(projectfilepath, $"{controller.Data.Naam}.vcxproj"), GenerateVisualStudioProjectFiles(controller));

            return result;
        }

        public string GenerateSpecification(ControllerModel controller, string specificationfilepath)
        {
            string result = "";

            return result;
        }

        public string GetGeneratorName()
        {
            return "CCOL";
        }

        public string GetGeneratorVersion()
        {
            return "0.1 (alfa)";
        }

        #endregion // IGenerator

        #region Fields

        private CCOLElemListData Uitgangen;
        private CCOLElemListData Ingangen;
        private CCOLElemListData HulpElementen;
        private CCOLElemListData GeheugenElementen;
        private CCOLElemListData Timers;
        private CCOLElemListData Counters;
        private CCOLElemListData Schakelaars;
        private CCOLElemListData Parameters;
        private List<DetectorModel> AlleDetectoren;

        private string tabspace = "    ";

        #endregion // Fields

        #region Setting Properties

        private string _CCOLIncludesPaden;
        [DisplayName("CCOL include paden")]
        [Description("CCOL include paden")]
        [Category("Visual project settings")]
        [TLCGenCustomSetting("application")]
        public string CCOLIncludesPaden
        {
            get { return _CCOLIncludesPaden; }
            set
            {
                if(!string.IsNullOrWhiteSpace(value) && !value.EndsWith(";"))
                    _CCOLIncludesPaden = value + ";";
                else
                    _CCOLIncludesPaden = value;
            }
        }

        private string _CCOLLibsPath;
        [DisplayName("CCOL library pad")]
        [Description("CCOL library pad")]
        [Category("Visual project settings")]
        [TLCGenCustomSetting("application")]
        [Editor(typeof(FolderEditor), typeof(FolderEditor))]
        public string CCOLLibsPath
        {
            get { return _CCOLLibsPath; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && !value.EndsWith(";"))
                    _CCOLLibsPath = value + ";";
                else
                    _CCOLLibsPath = value;
            }
        }

        private string _CCOLResPath;
        [DisplayName("CCOL resources pad")]
        [Description("CCOL resources pad")]
        [Category("Visual project settings")]
        [TLCGenCustomSetting("application")]
        [Editor(typeof(FolderEditor), typeof(FolderEditor))]
        public string CCOLResPath
        {
            get { return _CCOLResPath; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && !value.EndsWith(";"))
                    _CCOLResPath = value + ";";
                else
                    _CCOLResPath = value;
            }
        }

        private string _CCOLPreprocessorDefinitions;
        [DisplayName("Preprocessor definities")]
        [Description("Preprocessor definities")]
        [Category("Visual project settings")]
        [TLCGenCustomSetting("application")]
        public string CCOLPreprocessorDefinitions
        {
            get { return _CCOLPreprocessorDefinitions; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && !value.EndsWith(";"))
                    _CCOLPreprocessorDefinitions = value + ";";
                else
                    _CCOLPreprocessorDefinitions = value;
            }
        }

        #endregion // Setting Properties

    }
}
