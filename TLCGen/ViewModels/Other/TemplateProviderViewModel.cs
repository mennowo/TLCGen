using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Settings;
using System.Collections;

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
                RaisePropertyChanged(nameof(SelectedTemplate));
            }
        }

        private T2 _ApplyToItem;
        public T2 ApplyToItem
        {
            get { return _ApplyToItem; }
            set
            {
                _ApplyToItem = value;
                RaisePropertyChanged(nameof(ApplyToItem));
            }
        }

        private IList<T2> _ApplyToItems;
        public IList<T2> ApplyToItems
        {
            get { return _ApplyToItems; }
            set
            {
                _ApplyToItems = value;
                RaisePropertyChanged(nameof(ApplyToItems));
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

        RelayCommand _AddFromTemplateCommand;
        public ICommand AddFromTemplateCommand
        {
            get
            {
                if (_AddFromTemplateCommand == null)
                {
                    _AddFromTemplateCommand = new RelayCommand(AddFromTemplateCommand_Executed, AddFromTemplateCommand_CanExecute);
                }
                return _AddFromTemplateCommand;
            }
        }

        #endregion // Commands

        #region Commands Functionality

        private bool ApplyTemplateCommand_CanExecute(object obj)
        {
            return
                SelectedTemplate != null &&
                !string.IsNullOrWhiteSpace((SelectedTemplate as TLCGenTemplateModel<T2>).Replace) &&
                SelectedTemplate != null && 
                (ApplyToItem != null || ApplyToItems != null && ApplyToItems.Any());
        }

        private void ApplyTemplateCommand_Executed(object obj)
        {
            if(ApplyToItems != null)
            {
                foreach(var i in ApplyToItems)
                {
                    ApplyTo(i, obj);
                    _SourceVM.UpdateAfterApplyTemplate(i);
                }
            }
            if (ApplyToItem is T2 item)
            {
                ApplyTo(item, obj);
                _SourceVM.UpdateAfterApplyTemplate(item);
            }
        }

        private void ApplyTo(T2 item, object obj)
        {
            TLCGenTemplateModel<T2> template = SelectedTemplate as TLCGenTemplateModel<T2>;
            var tempitem = template.GetItems()?.FirstOrDefault();
            // originalName: name of item if no argument was passed; otherwise, the argument
            // this allows for renaming items in lists belonging to an item
            var orignalName = obj == null ? (item as IHaveName)?.Naam : obj as string;
            // originalItemName: store the item name if an argument was passed
            // this allows to rename potential items in lists of this item while 
            // maintaining the name of the item itself
            var orignalItemName = obj != null ? (item as IHaveName)?.Naam : null;
            if (tempitem != null && orignalName != null)
            {
                var cloneditem = DeepCloner.DeepClone((T2)tempitem);
                var props = typeof(T2).GetProperties().Where(x => x.CanWrite && x.CanRead).ToList();
                foreach (var prop in props)
                {
                    object valueOriginal = prop.GetValue(item);
                    object valueCloned = prop.GetValue(cloneditem);
                    if (prop.PropertyType.IsValueType || prop.PropertyType == typeof(string))
                    {
                        prop.SetValue(item, valueCloned);
                    }
                    else if (valueOriginal is IList elems)
                    {
                        if(elems.Count == 0 || !(elems[0] is IHaveName))
                        {
                            elems.Clear();
                        }
                        else
                        {
                            var l = new List<IHaveName>();
                            foreach(IHaveName i in elems)
                            {
                                l.Add(i);
                            }
                            foreach(var i in l)
                            {
                                Integrity.TLCGenControllerModifier.Default.RemoveModelItemFromController(i.Naam);
                            }
                        }
                        var clonedItems = (IList)valueCloned;
                        foreach (var e in clonedItems)
                        {
                            elems.Add(e);
                        }
                    }
                }
                ModelStringSetter.ReplaceStringInModel(item, template.Replace, orignalName);
                if(orignalItemName != null)
                {
                    ((IHaveName)item).Naam = orignalItemName;
                }
            }
        }

        private bool AddFromTemplateCommand_CanExecute(object obj)
        {
            return SelectedTemplate != null;
        }

        private void AddFromTemplateCommand_Executed(object obj)
        {
            List<T2> items = new List<T2>();
            TLCGenTemplateModel<T2> template = SelectedTemplate as TLCGenTemplateModel<T2>;
            
            // No data provided from view, but a replace value is needed for this template
            if(obj == null && !string.IsNullOrWhiteSpace(template.Replace))
            {
                var dialog = new Dialogs.ApplyTemplateWindow();
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
                            ModelStringSetter.ReplaceStringInModel(cloneditem, template.Replace, elem);
                            items.Add(cloneditem);
                        }
                    }
                    _SourceVM.InsertItemsFromTemplate(items);
                }
            }
            // Data provided from view, for use with replace value
            else if (obj != null && !string.IsNullOrWhiteSpace(template.Replace))
            {
                var elem = obj as string;
                if (!string.IsNullOrWhiteSpace(elem))
                {
                    var tempitems = template.GetItems();
                    foreach (var item in tempitems)
                    {
                        var cloneditem = DeepCloner.DeepClone((T2)item);
                        ModelStringSetter.ReplaceStringInModel(cloneditem, template.Replace, elem);
                        items.Add(cloneditem);
                    }
                    _SourceVM.InsertItemsFromTemplate(items);
                }
            }
            // Fixed template
            else if (string.IsNullOrWhiteSpace(template.Replace))
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

        #region Public Methods

        public void SetSelectedApplyToItem(T2 item)
        {
            ApplyToItem = item;
        }

        public void SetSelectedApplyToItems(List<T2> items)
        {
            if(items != null && items.Any()) ApplyToItem = null;
            ApplyToItems = items;
        }

        public void Update()
        {
            Templates.Clear();
            if (TemplatesProvider.Default.Templates != null)
            {
                if (typeof(T2) == typeof(FaseCyclusModel))
                {
                    foreach (var t in TemplatesProvider.Default.Templates.FasenTemplates)
                    {
                        Templates.Add(t as T1);
                    }
                }
                else if (typeof(T2) == typeof(DetectorModel))
                {
                    foreach (var t in TemplatesProvider.Default.Templates.DetectorenTemplates)
                    {
                        Templates.Add(t as T1);
                    }
                }
                else if (typeof(T2) == typeof(PeriodeModel))
                {
                    foreach (var t in TemplatesProvider.Default.Templates.PeriodenTemplates)
                    {
                        Templates.Add(t as T1);
                    }
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
            if(Templates.Count > 0)
            {
                SelectedTemplate = Templates[0];
            }
        }

        public void OnTemplatesChanged(TemplatesChangedMessage message)
        {
            this.Update();
        }

        #endregion // Public Methods

        #region Constructor

        public TemplateProviderViewModel(IAllowTemplates<T2> vm)
        {
            _SourceVM = vm;
            this.Update();
            Messenger.Default.Register(this, new Action<TemplatesChangedMessage>(OnTemplatesChanged));
        }

        #endregion // Constructor
    }
}
