﻿using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;
using TLCGen.Models.Enumerations;

namespace TLCGen.GebruikersOpties
{
    public class GebruikersOptiesExportModel
    {
        [XmlArray(ElementName = "Item")]
        public List<GebruikersOptieGenericViewModel> Items { get; set; }

        public GebruikersOptiesExportModel()
        {
            Items = new List<GebruikersOptieGenericViewModel>();
        }
    }

    public class GebruikersOptieGenericViewModel : ObservableObject
    {
        #region Fields

        private object _relatedObject;
        private TLCGenObjectTypeEnum _objectType;
        private string _naam;
        private bool _dummy;
        private bool _selected;
        private string _commentaar;
        private int? _instelling;
        private CCOLElementTypeEnum _timeType;

        #endregion // Fields

        #region Properties

        public TLCGenObjectTypeEnum ObjectType
        {
            get => _objectType;
            set
            {
                _objectType = value;
                _relatedObject = null;
                OnPropertyChanged();
            }
        }

        public string Naam
        {
            get => _naam;
            set
            {
                _naam = value;
                _relatedObject = null;
                OnPropertyChanged();
            }
        }

        public string Commentaar
        {
            get => _commentaar;
            set
            {
                _commentaar = value;
                _relatedObject = null;
                OnPropertyChanged();
            }
        }

        public int? Instelling
        {
            get => _instelling;
            set
            {
                _instelling = value;
                _relatedObject = null;
                OnPropertyChanged();
            }
        }

        public CCOLElementTypeEnum TimeType
        {
            get => _timeType;
            set
            {
                _timeType = value;
                _relatedObject = null;
                OnPropertyChanged();
            }
        }

        public bool Dummy
        {
            get => _dummy;
            set
            {
                _dummy = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public bool HasInstelling =>
            ObjectType == TLCGenObjectTypeEnum.CCOLCounter ||
            ObjectType == TLCGenObjectTypeEnum.CCOLParameter ||
            ObjectType == TLCGenObjectTypeEnum.CCOLSchakelaar ||
            ObjectType == TLCGenObjectTypeEnum.CCOLTimer;

        [XmlIgnore]
        public bool HasType =>
            ObjectType == TLCGenObjectTypeEnum.CCOLParameter ||
            ObjectType == TLCGenObjectTypeEnum.CCOLTimer;

        #endregion // Properties

        #region Commands
        #endregion // Commands

        #region Public Methods

        public object GetRelatedObject(bool renew = false)
        {
            if (_relatedObject == null || renew)
            {
                switch (ObjectType)
                {
                    case TLCGenObjectTypeEnum.Output:
                    case TLCGenObjectTypeEnum.Input:
                        _relatedObject = new GebruikersOptieWithIOViewModel(new GebruikersOptieWithIOModel
                        {
                            Naam = _naam,
                            Commentaar = _commentaar,
                            Dummy = _dummy
                        });
                        break;
                    case TLCGenObjectTypeEnum.CCOLHelpElement:
                    case TLCGenObjectTypeEnum.CCOLTimer:
                    case TLCGenObjectTypeEnum.CCOLCounter:
                    case TLCGenObjectTypeEnum.CCOLSchakelaar:
                    case TLCGenObjectTypeEnum.CCOLMemoryElement:
                    case TLCGenObjectTypeEnum.CCOLParameter:
                        _relatedObject = new GebruikersOptieViewModel(new GebruikersOptieModel
                        {
                            Naam = _naam,
                            Commentaar = _commentaar,
                            Type = _timeType,
                            Instelling = _instelling,
                            Dummy = _dummy
                        });
                        break;
                }
            }
            return _relatedObject;
        }

        #endregion // Public Methods

        #region Constructors

        public GebruikersOptieGenericViewModel()
        {

        }

        public GebruikersOptieGenericViewModel(object relatedObject)
        {
            _relatedObject = relatedObject;

            switch (_relatedObject)
            {
                case GebruikersOptieViewModel o:
                    ObjectType = o.ObjectType;
                    Naam = o.Naam;
                    Commentaar = o.Commentaar;
                    TimeType = o.Type;
                    Instelling = o.Instelling;
                    Dummy = o.Dummy;
                    break;
                case GebruikersOptieWithIOViewModel oio:
                    ObjectType = oio.ObjectType;
                    Naam = oio.Naam;
                    Commentaar = oio.Commentaar;
                    Dummy = oio.Dummy;
                    break;
            }
        }

        #endregion // Constructors
    }
}
