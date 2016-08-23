using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using System.Xml.Serialization;
using TLCGen.Helpers;
using TLCGen.Interfaces;

namespace TLCGen.ViewModels.Templates
{
    public class TemplatesManagerViewModelT<T1, T2> : ViewModelBase
    {
        private IHaveTemplates<T2> _ContainingTab;
        private string _TemplatesFolder;
        private string _ReplaceRegex;

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

        RelayCommand _SaveTemplateCommand;
        public ICommand SaveTemplateCommand
        {
            get
            {
                if (_SaveTemplateCommand == null)
                {
                    _SaveTemplateCommand = new RelayCommand(SaveTemplateCommand_Executed, SaveTemplateCommand_CanExecute);
                }
                return _SaveTemplateCommand;
            }
        }

        RelayCommand _RemoveTemplateCommand;
        public ICommand RemoveTemplateCommand
        {
            get
            {
                if (_RemoveTemplateCommand == null)
                {
                    _RemoveTemplateCommand = new RelayCommand(RemoveTemplateCommand_Executed, RemoveTemplateCommand_CanExecute);
                }
                return _RemoveTemplateCommand;
            }
        }

        private bool ApplyTemplateCommand_CanExecute(object obj)
        {
            return SelectedTemplate != null;
        }

        private void ApplyTemplateCommand_Executed(object obj)
        {
            TemplateViewModelBase tb = SelectedTemplate as TemplateViewModelBase;

            string ApplyString = (string)obj;
            if(tb.HasReplaceValue && string.IsNullOrWhiteSpace(ApplyString))
            {
                TLCGen.Views.ApplyTemplateWindow dialog = new Views.ApplyTemplateWindow();
                dialog.ShowDialog();
                ApplyString = dialog.TemplateApplyString;
            }

            if (!tb.HasReplaceValue || !string.IsNullOrWhiteSpace(ApplyString))
            {
                if (tb.HasReplaceValue)
                {
                    string list = ApplyString.Replace(" ", "");
                    string[] elems = list.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string s in elems)
                    {
                        List<object> templateitems = tb.GetItems();
                        List<T2> actualitems = new List<T2>();
                        foreach (object o in templateitems)
                        {
                            T2 oc = DeepCloner.DeepClone<T2>((T2)o);
                            ((ITemplatable)oc).SetAllIdentifyingNames("##", s);
                            actualitems.Add(oc);
                        }
                        _ContainingTab.AddFromTemplate(actualitems);
                    }
                }
                else
                {
                    List<object> templateitems = tb.GetItems();
                    List<T2> actualitems = new List<T2>();
                    foreach (object o in templateitems)
                    {
                        T2 oc = DeepCloner.DeepClone<T2>((T2)o);
                        actualitems.Add(oc);
                    }
                    _ContainingTab.AddFromTemplate(actualitems);
                }
            }
        }

        private bool SaveTemplateCommand_CanExecute(object obj)
        {
            List<object> items = _ContainingTab.GetTemplatableItems();
            return items != null && items.Count > 0;
        }

        private void SaveTemplateCommand_Executed(object obj)
        {
            TLCGen.Views.NewTemplateWindow dialog = new Views.NewTemplateWindow();

            List<object> _items = _ContainingTab.GetTemplatableItems();
            List<T2> items = new List<T2>();
            foreach(object o in _items)
            {
                T2 oc = DeepCloner.DeepClone<T2>((T2)o);
                ((ITemplatable)oc).ClearAllReferences();
                items.Add(oc);
            }
            ITemplatable item = (ITemplatable)items[0];

            dialog.TemplateReplace = Regex.Replace(item.GetIdentifyingName(), _ReplaceRegex, "{0}");
            dialog.ShowDialog();

            if(!string.IsNullOrWhiteSpace(dialog.TemplateNaam))
            {
                using (XmlWriter writer = XmlWriter.Create(Path.Combine(_TemplatesFolder, dialog.TemplateNaam + ".xml")))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T1));
                    if (!string.IsNullOrWhiteSpace(dialog.TemplateReplace))
                    {
                        foreach (ITemplatable it in items)
                        {
                            it.SetAllIdentifyingNames(dialog.TemplateReplace, "##");
                        }
                    }
                    T1 template = (T1)Activator.CreateInstance(typeof(T1), items);
                    TemplateViewModelBase tbase = template as TemplateViewModelBase;
                    tbase.Naam = dialog.TemplateNaam;
                    if (!string.IsNullOrWhiteSpace(dialog.TemplateReplace))
                        tbase.HasReplaceValue = true;
                    serializer.Serialize(writer, template);
                    writer.Close();
                    Templates.Add(template);
                }
            }
        }

        private bool RemoveTemplateCommand_CanExecute(object obj)
        {
            return SelectedTemplate != null;
        }

        private void RemoveTemplateCommand_Executed(object obj)
        {
            TemplateViewModelBase tbase = SelectedTemplate as TemplateViewModelBase;
            try
            {
                if (File.Exists(Path.Combine(_TemplatesFolder, tbase.Naam + ".xml")))
                    File.Delete(Path.Combine(_TemplatesFolder, tbase.Naam + ".xml"));
                Templates.Remove(SelectedTemplate);
            }
            catch(Exception e)
            {
                System.Windows.MessageBox.Show($"Fout bij verwijderen template {tbase.Naam}:{Environment.NewLine}{e.ToString()}");
            }
        }

        public TemplatesManagerViewModelT(string templatesfolder, IHaveTemplates<T2> container, string replaceregex)
        {
            _ContainingTab = container;
            _TemplatesFolder = templatesfolder;
            _ReplaceRegex = replaceregex;

            try
            {
                if (Directory.Exists(templatesfolder))
                {
                    foreach (String file in Directory.GetFiles(templatesfolder))
                    {
                        if (Path.GetExtension(file).ToLower() == ".xml")
                        {
                            XmlSerializer deserializer = new XmlSerializer(typeof(T1));
                            try
                            {
                                TextReader reader = new StreamReader(file);
                                T1 template = (T1)deserializer.Deserialize(reader);
                                Templates.Add(template);
                                reader.Close();
                            }
                            catch
                            {

                            }
                        }
                    }
                }
                else
                {
                    Directory.CreateDirectory(templatesfolder);
                }
            }
            catch
            {
                throw new NotImplementedException();
            }
        }
    }
}
