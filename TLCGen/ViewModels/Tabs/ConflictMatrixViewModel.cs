using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class ConflictMatrixViewModel : TabViewModel
    {

        #region Fields
        
        private ObservableCollection<string> _FasenNames;
        private bool _MatrixChanged;

        #endregion // Fields

        #region Properties

        /// <summary>
        /// Collection of strings used to display matrix column and row headers
        /// </summary>
        public ObservableCollection<string> FasenNames
        {
            get
            {
                if (_FasenNames == null)
                    _FasenNames = new ObservableCollection<string>();
                return _FasenNames;
            }
        }

        /// <summary>
        /// Returns the collection of FaseCyclusViewModel from the main ControllerViewModel
        /// </summary>
        public ObservableCollection<FaseCyclusViewModel> Fasen
        {
            get
            {
                return _ControllerVM.Fasen;
            }
        }

        private ConflictViewModel _SelectedConflict;
        private ConflictViewModel SelectedConflict
        {
            get
            {
                return _SelectedConflict;
            }
            set
            {
                _SelectedConflict = value;
                OnPropertyChanged("SelectedConflict");
            }
        }

        /// <summary>
        /// Symmetrical, two dimensional matrix used to display phasecycle conflicts.
        /// </summary>
        public ConflictViewModel[,] ConflictMatrix { get; set; }

        /// <summary>
        /// Boolean set by instances of ConflictViewModel when their DisplayWaarde property is 
        /// set by the user. We use this to monitor changes to the model, and to check if we need
        /// to check the matrix for symmetry if the user changes tabs, or tries to save the model.
        /// </summary>
        public bool MatrixChanged
        {
            get { return _MatrixChanged; }
            set
            {
                _MatrixChanged = value;
                if (_MatrixChanged)
                    _ControllerVM.HasChanged = true;
                OnPropertyChanged("MatrixChanged");
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _DeleteValueCommand;
        public ICommand DeleteValueCommand
        {
            get
            {
                if (_DeleteValueCommand == null)
                {
                    _DeleteValueCommand = new RelayCommand(DeleteValueCommand_Executed, DeleteValueCommand_CanExecute);
                }
                return _DeleteValueCommand;
            }
        }

        #endregion // Commands

        #region Command functionality

        void DeleteValueCommand_Executed(object prm)
        {
            if(SelectedConflict != null)
                SelectedConflict.DisplayWaarde = "";
        }

        bool DeleteValueCommand_CanExecute(object prm)
        {
            return SelectedConflict != null;
        }

        #endregion // Command functionality

        #region Private methods

        #endregion // Private methods

        #region Public methods

        /// <summary>
        /// Returns an int to set the value of property Waarde of an instance of
        /// ConflictViewModel, based on the value of the opposite of the parsed instance of 
        /// ConflictViewModel.
        /// This method is designed to be called by an instance of ConflictViewModel.
        /// </summary>
        /// <param name="cvm">Instance of ConflictViewModel that calls the method</param>
        /// <returns>A value to be used to set property Waarde of the parsed instance of ConflictViewModel</returns>
        public int SetBlankConflictFromOppositeConflict(ConflictViewModel cvm)
        {
            foreach (ConflictViewModel cvm2 in ConflictMatrix)
            {
                if (cvm2.FaseVan == cvm.FaseNaar &&
                   cvm2.FaseNaar == cvm.FaseVan)
                {
                    // If the opposite conflict is numeric, we return -6, indicating that the parsed 
                    // instance of ConflictViewModel should also be numeric.
                    int i;
                    if (Int32.TryParse(cvm2.DisplayWaarde, out i))
                    {
                        return -6;
                    }
                    // Otherwise, the SetOppositeConflict() method has already taken care of things.
                    else
                    {
                        return -1;
                    }
                }
            }
            // If the opposite is not found, something is seriously wrong.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the value of the DisplayWaarde property of the ConflictViewModel instance
        /// that is 'opposite' of the one in the argument. Opposite means: the FaseCyclus from
        /// and FaseCyclus to are inversed.
        /// This method is designed to be called by an instance of ConflictViewModel.
        /// </summary>
        /// <param name="cvm">Instance of ConflictViewModel whose opposite should be set</param>
        public bool SetOppositeConflict(ConflictViewModel cvm)
        {
            if(cvm.DisplayWaarde == "*")
            {
                return true;
            }

            foreach(ConflictViewModel cvm2 in ConflictMatrix)
            {
                if(cvm2.FaseVan == cvm.FaseNaar && 
                   cvm2.FaseNaar == cvm.FaseVan)
                {
                    int i;
                    switch (cvm.DisplayWaarde)
                    {
                        case "GKL":
                            if(cvm2.DisplayWaarde != "GK")
                                cvm2.DisplayWaarde = "GK";
                            break;
                        case "GK":
                            if (!(cvm2.DisplayWaarde == "GK" || cvm2.DisplayWaarde == "GKL"))
                                cvm2.DisplayWaarde = "GK";
                            break;
                        case "FK":
                            if(cvm2.DisplayWaarde != "FK")
                                cvm2.DisplayWaarde = "FK";
                            break;
                        case "":
                            if(cvm2.DisplayWaarde == "*")
                                cvm2.DisplayWaarde = "";
                            break;
                        default:
                            if(Int32.TryParse(cvm2.DisplayWaarde, out i))
                            {
                                break;
                            }
                            else
                            {
                                cvm2.DisplayWaarde = "*";
                            }
                            break;
                    }
                    return true;
                }
            }
            // If the opposite is not found, something is seriously wrong.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks if the ConflictMatrix is symmetrical.
        /// </summary>
        /// <returns>null if succesfull, otherwise a string stating the first error found.</returns>
        public string IsMatrixSymmetrical()
        {
            for (int i = 0; i < Fasen.Count; ++i)
            {
                for (int j = 0; j < Fasen.Count; ++j)
                {
                    // Skip from>to self
                    if (i == j || string.IsNullOrWhiteSpace(ConflictMatrix[i, j].DisplayWaarde))
                        continue;

                    switch (ConflictMatrix[i, j].DisplayWaarde)
                    {
                        case "FK":
                            if (ConflictMatrix[j, i].DisplayWaarde != "FK")
                                return "Conflict matrix niet symmetrisch:\nFK van " + Fasen[i].Naam + " naar " + Fasen[j].Naam + " maar niet andersom.";
                            continue;
                        case "GK":
                            if (ConflictMatrix[j, i].DisplayWaarde != "GK" && ConflictMatrix[j, i].DisplayWaarde != "GKL")
                                return "Conflict matrix niet symmetrisch:\nGK van " + Fasen[i].Naam + " naar " + Fasen[j].Naam + " maar niet andersom.";
                            continue;
                        case "GKL":
                            if (ConflictMatrix[j, i].DisplayWaarde != "GK")
                                return "Conflict matrix niet symmetrisch:\nGKL van " + Fasen[i].Naam + " naar " + Fasen[j].Naam + " maar andersom geen GK.";
                            continue;
                        default:
                            int c;
                            if (Int32.TryParse(ConflictMatrix[i, j].DisplayWaarde, out c))
                            {
                                if (Int32.TryParse(ConflictMatrix[j, i].DisplayWaarde, out c))
                                {
                                    continue;
                                }
                                else
                                {
                                    return "Conflict matrix niet symmetrisch:\nwaarde van " + Fasen[j].Naam + " naar " + Fasen[i].Naam + " ontbrekend of onjuist (niet numeriek, FK, GK of GKL).";
                                }
                            }
                            else
                            {
                                return "Conflict matrix not symmetrical:\nwaarde van " + Fasen[i].Naam + " naar " + Fasen[j].Naam + " onjuist (niet numeriek, FK, GK of GKL).";
                            }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Builds a new string[,] to be exposed to the View. The 2D array is filled with data
        /// from the collection of FaseCyclusViewModel, and then from the collection of ConflictViewModel
        /// they contain.
        /// </summary>
        public void BuildConflictMatrix()
        {
            if (_ControllerVM == null ||
                _ControllerVM.Fasen == null ||
                _ControllerVM.Fasen.Count <= 0 ||
                _ControllerVM.IsSortingFasen)
                return;

            int fccount = Fasen.Count;

            _FasenNames = new ObservableCollection<string>();
            foreach (FaseCyclusViewModel fcvm in Fasen)
            {
                FasenNames.Add(fcvm.Naam);
            }
            OnPropertyChanged("FasenNames");


            if (fccount == 0)
            {
                ConflictMatrix = null;
                return;
            }

            ConflictMatrix = new ConflictViewModel[fccount, fccount];
            for(int fcvm_from = 0; fcvm_from < fccount; ++fcvm_from)
            {
                for(int fcvm_to = 0; fcvm_to < fccount; ++fcvm_to)
                {
                    foreach (ConflictViewModel cvm in Fasen[fcvm_from].Conflicten)
                    {
                        if(!string.IsNullOrWhiteSpace(Fasen[fcvm_to].Define) && Fasen[fcvm_to].Define == cvm.FaseNaar)
                        {
                            ConflictMatrix[fcvm_from, fcvm_to] = cvm;
                        }
                    }
                    if(ConflictMatrix[fcvm_from, fcvm_to] == null)
                    {
                        ConflictModel m = new ConflictModel();
                        m.FaseVan = Fasen[fcvm_from].Define;
                        m.FaseNaar = Fasen[fcvm_to].Define;
                        m.Waarde = -1;
                        ConflictMatrix[fcvm_from, fcvm_to] = new ConflictViewModel(_ControllerVM, m);
                    }
                }
            }
            OnPropertyChanged("ConflictMatrix");
        }

        /// <summary>
        /// Reads the property string[,] ConflictMatrix and saves all relevant entries as 
        /// instances of ConflictViewmodel in the collection of the relevant FaseCyclusViewModel.
        /// This also updates the Model data.
        /// </summary>
        public void SaveConflictMatrix()
        {
            if(ConflictMatrix == null || Fasen == null)
            {
                return;
            }

            int fccount = Fasen.Count;

            for (int fcvm_from = 0; fcvm_from < fccount; ++fcvm_from)
            {
                // Call extension method RemoveAll instead of built-in Clear(), see:
                // http://stackoverflow.com/questions/224155/when-clearing-an-observablecollection-there-are-no-items-in-e-olditems
                Fasen[fcvm_from].Conflicten.RemoveAll();
                for (int fcvm_to = 0; fcvm_to < fccount; ++fcvm_to)
                {
                    if (!string.IsNullOrWhiteSpace(ConflictMatrix[fcvm_from, fcvm_to].DisplayWaarde))
                    {                        
                        Fasen[fcvm_from].Conflicten.Add(ConflictMatrix[fcvm_from, fcvm_to]);
                    }
                }
            }
        }

        #endregion // Public methods

        #region Collection Changed

        #endregion // Collection Changed

        #region Constructor

        public ConflictMatrixViewModel(ControllerViewModel controllervm) : base(controllervm)
        {
            BuildConflictMatrix();
        }

        #endregion // Constructor
    }
}
