using GalaSoft.MvvmLight;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    public class IGeneratorViewModel : ViewModelBase
    {
        private ITLCGenGenerator _Generator;

        public ITLCGenGenerator Generator => _Generator;

        public System.Windows.Controls.UserControl GeneratorView => _Generator.GeneratorView;


        public string Naam => _Generator.GetGeneratorName();

        public IGeneratorViewModel(ITLCGenGenerator generator)
        {
            _Generator = generator;
        }
    }
}
