using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    public class IGeneratorViewModel : ViewModelBase
    {
        private ITLCGenGenerator _Generator;

        public ITLCGenGenerator Generator
        {
            get { return _Generator; }
        }

        public System.Windows.Controls.UserControl GeneratorView
        {
            get { return _Generator.GeneratorView; }
        }


        public string Naam
        {
            get
            {
                return _Generator.GetGeneratorName();
            }
        }

        public IGeneratorViewModel(ITLCGenGenerator generator)
        {
            _Generator = generator;
        }
    }
}
