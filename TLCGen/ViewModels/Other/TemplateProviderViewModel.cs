using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Settings;
using System.Collections;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace TLCGen.ViewModels
{
    public class TemplateProviderViewModel<T1,T2> : ObservableObject where T1 : class where T2 : class
    {
        #region Fields

        private readonly IAllowTemplates<T2> _sourceVm;
        private readonly bool _replaceNameOnApply;
        private ObservableCollection<T1> _templates;
        private T1 _selectedTemplate;
        private T2 _applyToItem;
        private IList<T2> _applyToItems;
        RelayCommand<object> _applyTemplateCommand;
        RelayCommand<object> _addFromTemplateCommand;

        #endregion // Fields

        #region Properties

        public ObservableCollection<T1> Templates
        {
            get
            {
                if (_templates == null)
                {
                    _templates = new ObservableCollection<T1>();
                }
                return _templates;
            }
        }

        public T1 SelectedTemplate
        {
            get => _selectedTemplate;
            set
            {
                _selectedTemplate = value;
                OnPropertyChanged(nameof(SelectedTemplate));
            }
        }

        public T2 ApplyToItem
        {
            get => _applyToItem;
            set
            {
                _applyToItem = value;
                OnPropertyChanged(nameof(ApplyToItem));
            }
        }

        public IList<T2> ApplyToItems
        {
            get => _applyToItems;
            set
            {
                _applyToItems = value;
                OnPropertyChanged(nameof(ApplyToItems));
            }
        }

        #endregion // Properties

        #region Commands

        public ICommand ApplyTemplateCommand
        {
            get
            {
                if (_applyTemplateCommand == null)
                {
                    _applyTemplateCommand = new RelayCommand<object>(obj =>
                        {
                            if(ApplyToItems != null)
                            {
                                foreach(var i in ApplyToItems)
                                {
                                    ApplyTo(i, obj);
                                    _sourceVm.UpdateAfterApplyTemplate(i);
                                }
                            }
                            if (ApplyToItem is { } item)
                            {
                                ApplyTo(item, obj);
                                _sourceVm.UpdateAfterApplyTemplate(item);
                            }
                        }, 
                        _ => SelectedTemplate != null &&
                              (!string.IsNullOrWhiteSpace((SelectedTemplate as TLCGenTemplateModel<T2>)?.Replace) || !_replaceNameOnApply) &&
                              (ApplyToItem != null || ApplyToItems != null && ApplyToItems.Any()));
                }
                return _applyTemplateCommand;
            }
        }

        public ICommand AddFromTemplateCommand
        {
            get
            {
                if (_addFromTemplateCommand == null)
                {
                    _addFromTemplateCommand = new RelayCommand<object>(obj =>
                    {
                        var items = new List<T2>();
                        var template = SelectedTemplate as TLCGenTemplateModel<T2>;
                        if (template == null) return;
                        
                        // No data provided from view, but a replace value is needed for this template
                        if(obj == null && !string.IsNullOrWhiteSpace(template.Replace))
                        {
                            var dialog = new Dialogs.ApplyTemplateWindow();
                            dialog.ShowDialog();
                            var applyString = dialog.TemplateApplyString;
                            if(!string.IsNullOrWhiteSpace(applyString))
                            {
                                var list = applyString.Replace(" ", "");
                                var elems = list.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
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
                                _sourceVm.InsertItemsFromTemplate(items);
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
                                _sourceVm.InsertItemsFromTemplate(items);
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
                            _sourceVm.InsertItemsFromTemplate(items);
                        }
                    }, 
                    _ => SelectedTemplate != null);
                }
                return _addFromTemplateCommand;
            }
        }
        
        #endregion // Commands Functionality

        #region Private Methods

        private void ApplyTo(T2 item, object obj)
        {
            var template = SelectedTemplate as TLCGenTemplateModel<T2>;
            if (template == null) return;
            
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
                    var templateApplicableAttr = prop.CustomAttributes.FirstOrDefault(x => x.AttributeType == typeof(NoTemplateApplicableAttribute));
                    if (templateApplicableAttr != null) continue;

                    var valueOriginal = prop.GetValue(item);
                    var valueCloned = prop.GetValue(cloneditem);
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
                                Integrity.TLCGenControllerModifier.Default.RemoveModelItemFromController(i.Naam, i.ObjectType);
                            }
                        }
                        var clonedItems = (IList)valueCloned;
                        foreach (var e in clonedItems)
                        {
                            elems.Add(e);
                        }
                    }
                }
                if (_replaceNameOnApply)
                {
                    ModelStringSetter.ReplaceStringInModel(item, template.Replace, orignalName);
                }
                if(orignalItemName != null)
                {
                    ((IHaveName)item).Naam = orignalItemName;
                }
            }
        }

        #endregion // Private Methods
        
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
                else if (typeof(T2) == typeof(PrioIngreepModel))
                {
                    foreach (var t in TemplatesProvider.Default.Templates.PrioIngreepTemplates)
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

        public void OnTemplatesChanged(object sender, TemplatesChangedMessage message)
        {
            this.Update();
        }

        #endregion // Public Methods

        #region Constructor

        public TemplateProviderViewModel(IAllowTemplates<T2> vm, bool replacenameonapply = true)
        {
            _sourceVm = vm;
            _replaceNameOnApply = replacenameonapply;
            this.Update();
            WeakReferenceMessengerEx.Default.Register<TemplatesChangedMessage>(this, OnTemplatesChanged);
        }

        #endregion // Constructor
    }
}
