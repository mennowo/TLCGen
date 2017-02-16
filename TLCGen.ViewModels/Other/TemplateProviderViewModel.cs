using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    public class TemplateProviderViewModel<T1,T2> : ViewModelBase where T1 : class where T2 : class
    {
        #region Fields

        private IAllowTemplates<T2> _SourceVM;

        #endregion // Fields

        #region Properties

        private ObservableCollection<T1> _Templates;
        public ObservableCollection<T1> Templates
        {
            get
            {
                if (_Templates == null)
                {
                    _Templates = new ObservableCollection<T1>();
                }
                return _Templates;
            }
        }

        private T1 _SelectedTemplate;
        public T1 SelectedTemplate
        {
            get { return _SelectedTemplate; }
            set
            {
                _SelectedTemplate = value;
                OnPropertyChanged("SelectedTemplate");
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _ApplyTemplateCommand;
        public ICommand ApplyTemplateCommand
        {
            get
            {
                if (_ApplyTemplateCommand == null)
                {
                    _ApplyTemplateCommand = new RelayCommand(ApplyTemplateCommand_Executed, ApplyTemplateCommand_CanExecute);
                }
                return _ApplyTemplateCommand;
            }
        }

        #endregion // Commands

        #region Commands Functionality

        private bool ApplyTemplateCommand_CanExecute(object obj)
        {
            return SelectedTemplate != null;
        }

        private void ApplyTemplateCommand_Executed(object obj)
        {
            List<T2> items = new List<T2>();
            TLCGenTemplateModel<T2> template = SelectedTemplate as TLCGenTemplateModel<T2>;
            if(obj == null && !string.IsNullOrWhiteSpace(template.Replace))
            {
                TLCGen.Dialogs.ApplyTemplateWindow dialog = new TLCGen.Dialogs.ApplyTemplateWindow();
                dialog.ShowDialog();
                string ApplyString = dialog.TemplateApplyString;
                if(!string.IsNullOrWhiteSpace(ApplyString))
                {
                    string list = ApplyString.Replace(" ", "");
                    string[] elems = list.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var elem in elems)
                    {
                        var tempitems = template.GetItems();
                        foreach (var item in tempitems)
                        {
                            var cloneditem = DeepCloner.DeepClone((T2)item);
                            Helpers.ModelStringSetter.SetStringInModel(cloneditem, template.Replace, elem);
                            items.Add(cloneditem);
                        }
                        _SourceVM.InsertItemsFromTemplate(items);
                    }
                }
            }
            else
            {
                var tempitems = template.GetItems();
                foreach(var item in tempitems)
                {
                    var cloneditem = DeepCloner.DeepClone((T2)item);
                    items.Add(cloneditem);
                }
                _SourceVM.InsertItemsFromTemplate(items);
            }
        }

        #endregion // Commands Functionality

        public void Update()
        {
            Templates.Clear();
            if(typeof(T2) == typeof(FaseCyclusModel))
            {
                foreach (var t in TemplatesProvider.Default.Templates.FasenTemplates)
                {
                    Templates.Add(t as T1);
                }
            }
            if (typeof(T2) == typeof(DetectorModel))
            {
                foreach (var t in TemplatesProvider.Default.Templates.DetectorenTemplates)
                {
                    Templates.Add(t as T1);
                }
            }
        }

        public TemplateProviderViewModel(IAllowTemplates<T2> vm)
        {
            _SourceVM = vm;
        }
    }
}
