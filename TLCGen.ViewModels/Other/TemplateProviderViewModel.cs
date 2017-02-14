using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Helpers;
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
            TemplateModelBase template = SelectedTemplate as TemplateModelBase;
            if(!string.IsNullOrWhiteSpace(template.Replace))
            {
#warning TODO: replace! use reflection...
            }
            var tempitems = template.GetItems();
            foreach(var item in tempitems)
            {
                items.Add(item as T2);
            }
            _SourceVM.InsertItems(items);
            //TemplateViewModelBase tb = SelectedTemplate as TemplateViewModelBase;
            //
            //string ApplyString = (string)obj;
            //if (tb.HasReplaceValue && string.IsNullOrWhiteSpace(ApplyString))
            //{
            //    TLCGen.Dialogs.ApplyTemplateWindow dialog = new TLCGen.Dialogs.ApplyTemplateWindow();
            //    dialog.ShowDialog();
            //    ApplyString = dialog.TemplateApplyString;
            //}
            //
            //if (!tb.HasReplaceValue || !string.IsNullOrWhiteSpace(ApplyString))
            //{
            //    if (tb.HasReplaceValue)
            //    {
            //        string list = ApplyString.Replace(" ", "");
            //        string[] elems = list.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            //        foreach (string s in elems)
            //        {
            //            List<object> templateitems = tb.GetItems();
            //            List<T2> actualitems = new List<T2>();
            //            foreach (object o in templateitems)
            //            {
            //                T2 oc = DeepCloner.DeepClone<T2>((T2)o);
            //                ((ITemplatable)oc).SetAllIdentifyingNames("##", s);
            //                actualitems.Add(oc);
            //            }
            //            _ContainingTab.AddFromTemplate(actualitems);
            //        }
            //    }
            //    else
            //    {
            //        List<object> templateitems = tb.GetItems();
            //        List<T2> actualitems = new List<T2>();
            //        foreach (object o in templateitems)
            //        {
            //            T2 oc = DeepCloner.DeepClone<T2>((T2)o);
            //            actualitems.Add(oc);
            //        }
            //        _ContainingTab.AddFromTemplate(actualitems);
            //    }
            //}
        }

        #endregion // Commands Functionality

        public void Update()
        {
            Templates.Clear();
            if (typeof(T1) == typeof(FaseCyclusTemplateModel))
            {
                foreach (var t in TemplatesProvider.Default.Templates.FasenTemplates)
                {
                    Templates.Add(t as T1);
                }
            }
            else if (typeof(T1) == typeof(DetectorTemplateModel))
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
