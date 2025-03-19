using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TLCGen.Plugins.AutoBuild
{
    public class VCXProjectDataViewModel : ObservableObject
    {
        private string _name;
        private string _fileName;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;
                OnPropertyChanged();
            }
        }

        public VCXProjectDataViewModel(string name, string filename)
        {
            Name = name;
            FileName = filename;
        }
    }
}
