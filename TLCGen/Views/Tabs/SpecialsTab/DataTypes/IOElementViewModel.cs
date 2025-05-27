using System;
using CommunityToolkit.Mvvm.ComponentModel;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class IOElementViewModel : ObservableObjectEx, IComparable
    {
        private IOElementRangeerDataModel _savedData;

        public IOElementViewModel(IOElementModel element)
        {
            Element = element;
        }

        public int RangeerIndex
        {
            get => !UseSecondaryIndex ? Element.RangeerIndex : Element.RangeerIndex2;
            set
            {
                if (!UseSecondaryIndex) Element.RangeerIndex = value;
                else Element.RangeerIndex2 = value;
                if (SavedData != null)
                {
                    if (!UseSecondaryIndex) SavedData.RangeerIndex = value;
                    else SavedData.RangeerIndex2 = value;
                }
            }
        }
        
        public string ManualNaam
        {
            get => SavedData.ManualNaam;
            set
            {
                SavedData.ManualNaam = value;
                Element.ManualNaam = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public bool HasManualName
        {
            get => SavedData.HasManualNaam;
            set
            {
                SavedData.HasManualNaam = value;
                if (value && string.IsNullOrWhiteSpace(SavedData.ManualNaam))
                {
                    ManualNaam = SavedData.Naam;
                }
                OnPropertyChanged(broadcast: true);
            }
        }
        
        public bool UseSecondaryIndex { get; set; }

        public IOElementModel Element { get; }

        public IOElementRangeerDataModel SavedData
        {
            get => _savedData;
            set
            {
                _savedData = value;
                OnPropertyChanged(nameof(HasManualName));
                OnPropertyChanged(nameof(ManualNaam));
                OnPropertyChanged(nameof(ManualNaam));
            }
        }

        public int CompareTo(object obj)
        {
            return RangeerIndex.CompareTo(((IOElementViewModel) obj).RangeerIndex);
        }
    }
}