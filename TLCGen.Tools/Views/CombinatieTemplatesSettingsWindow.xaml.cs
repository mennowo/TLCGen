using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System.Reflection;
using System.Windows;
using System.Xml;

namespace TLCGen.Plugins.Tools
{
    /// <summary>
    /// Interaction logic for CombinatieTemplatesSettingsWindow.xaml
    /// </summary>
    public partial class CombinatieTemplatesSettingsWindow : Window
    {
        public CombinatieTemplatesSettingsWindow()
        {
            InitializeComponent();

            using (var s = Assembly.GetExecutingAssembly().GetManifestResourceStream("TLCGen.Plugins.Tools.Resources.json.xshd"))
            {
                using (var reader = new XmlTextReader(s))
                {
                    Editor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
