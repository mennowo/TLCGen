using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TLCGen.ViewModels.Templates
{
    public abstract class TemplateViewModelBase : ViewModelBase
    {
        private string _Naam;
        private bool _HasReplaceValue;

        public string Naam
        {
            get { return _Naam; }
            set
            {
                _Naam = value;
                OnPropertyChanged("Naam");
            }
        }

        public bool HasReplaceValue
        {
            get { return _HasReplaceValue; }
            set
            {
                _HasReplaceValue = value;
                OnPropertyChanged("HasReplaceValue");
            }
        }

        public abstract List<object> GetItems();

        public override string ToString()
        {
            return Naam;
        }
    }
}
