using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Interfaces.Public;

namespace TLCGen.ViewModels
{
    public class IGeneratorViewModel : ViewModelBase
    {
        private IGenerator _Generator;

        public IGenerator Generator
        {
            get { return _Generator; }
        }

        public string Naam
        {
            get
            {
                return _Generator.GetGeneratorName();
            }
        }

        public IGeneratorViewModel(IGenerator generator)
        {
            _Generator = generator;
        }
    }
}
