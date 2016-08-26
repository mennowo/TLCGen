using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.DataAccess;
using TLCGen.Helpers;
using TLCGen.Interfaces.Public;
using TLCGen.Models;
using TLCGen.Models.Settings;

namespace TLCGen.ViewModels
{
    public class ControllerDataViewModel : ViewModelBase
    {
        #region Fields

        private ControllerViewModel _ControllerVM;
        private ControllerDataModel _ControllerData;
        private VersieViewModel _SelectedVersie;
        private ObservableCollection<IGenerator> _Generators;

        #endregion // Fields

        #region Properties

        public string Naam
        {
            get { return _ControllerData.Naam; }
            set
            {
                _ControllerData.Naam = value;
                OnMonitoredPropertyChanged("Naam", _ControllerVM);
            }
        }

        public string Stad
        {
            get { return _ControllerData.Stad; }
            set
            {
                _ControllerData.Stad = value;
                OnMonitoredPropertyChanged("Stad", _ControllerVM);
            }
        }

        public string Straat1
        {
            get { return _ControllerData.Straat1; }
            set
            {
                _ControllerData.Straat1 = value;
                OnMonitoredPropertyChanged("Straat1", _ControllerVM);
            }
        }

        public string Straat2
        {
            get { return _ControllerData.Straat2; }
            set
            {
                _ControllerData.Straat2 = value;
                OnMonitoredPropertyChanged("Straat2", _ControllerVM);
            }
        }

        public string BitmapNaam
        {
            get { return _ControllerData.BitmapNaam; }
            set
            {
                _ControllerData.BitmapNaam = value;
                OnMonitoredPropertyChanged("BitmapNaam", _ControllerVM);
                _ControllerVM.UpdateTabsEnabled();
            }
        }

        public VersieViewModel SelectedVersie
        {
            get { return _SelectedVersie; }
            set
            {
                _SelectedVersie = value;
                OnPropertyChanged("SelectedVersie");
            }
        }

        public DefinePrefixSettings PrefixSettings
        {
            get { return _ControllerData.Instellingen.PrefixSettings; }
        }

        private ObservableCollection<SettingModelBase> _PrefixSettingsList;
        public ObservableCollection<SettingModelBase> PrefixSettingsList
        {
            get
            {
                if(_PrefixSettingsList == null)
                {
                    _PrefixSettingsList = new ObservableCollection<SettingModelBase>();
                }
                return _PrefixSettingsList;
            }
        }

        private ObservableCollection<VersieViewModel> _Versies;
        public ObservableCollection<VersieViewModel> Versies
        {
            get
            {
                if (_Versies == null)
                {
                    _Versies = new ObservableCollection<VersieViewModel>();
                }
                return _Versies;
            }
        }

        public ObservableCollection<IGenerator> Generators
        {
            get
            {
                if (_Generators == null)
                {
                    _Generators = new ObservableCollection<IGenerator>();
                }
                return _Generators;
            }
        }

        #endregion // Properties

        #region Public methods

        #endregion // Public methods

        RelayCommand _AddVersieCommand;
        public ICommand AddVersieCommand
        {
            get
            {
                if (_AddVersieCommand == null)
                {
                    _AddVersieCommand = new RelayCommand(AddVersieCommand_Executed, AddVersieCommand_CanExecute);
                }
                return _AddVersieCommand;
            }
        }

        RelayCommand _RemoveVersieCommand;
        public ICommand RemoveVersieCommand
        {
            get
            {
                if (_RemoveVersieCommand == null)
                {
                    _RemoveVersieCommand = new RelayCommand(RemoveVersieCommand_Executed, RemoveVersieCommand_CanExecute);
                }
                return _RemoveVersieCommand;
            }
        }


        void AddVersieCommand_Executed(object prm)
        {
            VersieModel vm = new VersieModel();
            vm.Datum = DateTime.Now;
#warning TODO build intelligence to increase version number
            vm.Versie = "1.0.0";
            vm.Ontwerper = Environment.UserName;
            VersieViewModel vvm = new VersieViewModel(_ControllerVM, vm);
            Versies.Add(vvm);
        }

        bool AddVersieCommand_CanExecute(object prm)
        {
            return Versies != null;
        }

        void RemoveVersieCommand_Executed(object prm)
        {
            Versies.Remove(SelectedVersie);
        }

        bool RemoveVersieCommand_CanExecute(object prm)
        {
            return Versies != null && Versies.Count > 0 && SelectedVersie != null;
        }
        #region Collection Changed

        private void Versies_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (VersieViewModel vvm in e.NewItems)
                {
                    _ControllerData.Versies.Add(vvm.VersieEntry);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (VersieViewModel vvm in e.OldItems)
                {
                    _ControllerData.Versies.Remove(vvm.VersieEntry);
                }
            }
            _ControllerVM.HasChanged = true;
        }

        #endregion // Collection Changed

        #region Constructor

        public ControllerDataViewModel(ControllerViewModel controllervm, ControllerDataModel controllerdata)
        {
            _ControllerVM = controllervm;
            _ControllerData = controllerdata;

            foreach(SettingModelBase setting in SettingsProvider.AppSettings.PrefixSettingsList)
            {
                PrefixSettingsList.Add(setting);
            }

            foreach(VersieModel vm in _ControllerData.Versies)
            {
                VersieViewModel vvm = new VersieViewModel(_ControllerVM, vm);
                Versies.Add(vvm);
            }
            Versies.CollectionChanged += Versies_CollectionChanged;


            try
            {
                string path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Generators\\");
                if (Directory.Exists(path))
                {
                    // Find all Generator DLL's
                    foreach (String file in Directory.GetFiles(path))
                    {
                        if (Path.GetExtension(file).ToLower() == ".dll")
                        {
                            // Find and loop all types from the Generators
                            Assembly assemblyInstance = Assembly.LoadFrom(file);
                            Type[] types = assemblyInstance.GetTypes();
                            foreach(Type t in types)
                            {
                                // Find TLCGenGenerator attribute, and if found, continue
                                TLCGenGeneratorAttribute genattr = (TLCGenGeneratorAttribute)Attribute.GetCustomAttribute(t, typeof(TLCGenGeneratorAttribute));
                                if(genattr != null)
                                {
                                    // Cast the Generator to IGenerator so we can read its name
                                    var generator = Activator.CreateInstance(t);
                                    var igenerator = generator as IGenerator;
                                    // Loop the settings data, to see if we have settings for this Generator
                                    foreach (GeneratorDataModel gendata in _ControllerVM.Controller.CustomData.GeneratorsData)
                                    {
                                        if (gendata.Naam == igenerator.GetGeneratorName())
                                        {
                                            // From the Generator, real all properties attributed with [TLCGenGeneratorSetting]
                                            var dllprops = t.GetProperties().Where(
                                                prop => Attribute.IsDefined(prop, typeof(TLCGenGeneratorSettingAttribute)));
                                            // Loop the saved settings, and load if applicable
                                            foreach (GeneratorDataPropertyModel dataprop in gendata.Properties)
                                            {
                                                foreach (var propinfo in dllprops)
                                                {
                                                    // Only load here, if it is a controller specific setting
                                                    TLCGenGeneratorSettingAttribute propattr = (TLCGenGeneratorSettingAttribute)Attribute.GetCustomAttribute(propinfo, typeof(TLCGenGeneratorSettingAttribute));
                                                    if(propinfo.Name == dataprop.Naam)
                                                    {
                                                        if (propattr != null && propattr.SettingType == "controller")
                                                        {
                                                            try
                                                            {
                                                                string type = propinfo.PropertyType.ToString();
                                                                switch (type)
                                                                {
                                                                    case "System.Double":
                                                                        double d;
                                                                        if (Double.TryParse(dataprop.Setting, out d))
                                                                            propinfo.SetValue(generator, d);
                                                                        break;
                                                                    case "System.Int32":
                                                                        int i32;
                                                                        if (Int32.TryParse(dataprop.Setting, out i32))
                                                                            propinfo.SetValue(generator, i32);
                                                                        break;
                                                                    case "System.String":
                                                                        propinfo.SetValue(generator, dataprop.Setting);
                                                                        break;
                                                                    default:
                                                                        throw new NotImplementedException();
                                                                }
                                                            }
                                                            catch
                                                            {
                                                                System.Windows.MessageBox.Show("Error load generator settings.");
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    // Add the generator to our list of available generators
                                    Generators.Add(igenerator);
                                    break;
                                }
                                else
                                {
                                    System.Windows.MessageBox.Show($"Library {file} wordt niet herkend als TLCGen genrator module.");
                                }
                            }
                        }
                    }
                }
                else
                {
                }
            }
            catch
            {
                throw new NotImplementedException();
            }
        }

        #endregion // Constructor
    }
}
