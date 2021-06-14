using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace TLCGen.Plugins.AutoBuild
{
    public class VCXProjectDataViewModel : ViewModelBase
    {
        private string _name;
        private string _fileName;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                RaisePropertyChanged();
            }
        }

        public string FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;
                RaisePropertyChanged();
            }
        }

        public VCXProjectDataViewModel(string name, string filename)
        {
            Name = name;
            FileName = filename;
        }
    }
}
