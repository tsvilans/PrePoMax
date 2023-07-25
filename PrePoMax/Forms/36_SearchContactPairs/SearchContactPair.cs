using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;
using DynamicTypeDescriptor;


namespace PrePoMax.Forms
{
    [Serializable]
    public enum SearchContactPairType
    {
        Tie,
        Contact
    }
    [Serializable]
    public enum SearchContactPairAdjust
    {
        Yes,
        No
    }
    [Serializable]
    public class SearchContactPair
    {
        // Variables                                                                                                                
        private string _name;
        private SearchContactPairType _type;
        private string _surfaceInteractionName;
        private string _contactPairMethod;
        private SearchContactPairAdjust _adjust;
        private double _distance;
        private bool _multiView;
        private CaeMesh.MasterSlaveItem _masterSlaveItem;
        private DynamicCustomTypeDescriptor _dctd = null;           // needed for sorting properties


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 10, "Name")]
        [DescriptionAttribute("Name of the contact pair.")]
        public string Name
        {
            get { return _name; }
            set
            {
                if (NamedClass.CheckName(value)) _name = value;
            }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Geometry")]
        [DescriptionAttribute("Geometry type of the contact pair.")]
        public string GeometryTypeName
        {
            get
            {
                if (_masterSlaveItem != null) return _masterSlaveItem.GeometryTypeName;
                else return "";
            }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Type")]
        [DescriptionAttribute("Select the type of the contact pair.")]
        public SearchContactPairType Type
        {
            get { return _type; }
            set
            {
                if (_type != value)
                {
                    _type = value;
                    UpdateVisibility();
                }
            }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(3, 10, "Surface interaction")]
        [DescriptionAttribute("Select the surface interaction defining the properties of the contact pair.")]
        public string SurfaceInteractionName
        {
            get
            {
                if (_type == SearchContactPairType.Tie) return "";
                else return _surfaceInteractionName;
            }
            set { _surfaceInteractionName = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(4, 10, "Method")]
        [DescriptionAttribute("Select the method defining the properties of the contact pair.")]
        public string ContactPairMethod
        {
            get
            {
                if (_type == SearchContactPairType.Tie) return "";
                else return _contactPairMethod;
            }
            set { _contactPairMethod = value; }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(5, 10, "Adjust")]
        [DescriptionAttribute("Set adjust to No to prevent the projection of the slave nodes on the master surface.")]
        public SearchContactPairAdjust Adjust { get { return _adjust; } set { _adjust = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(6, 10, "Distance")]
        [DescriptionAttribute("Set the distance inside which the slave nodes will be included/projected.")]
        [TypeConverter(typeof(StringLengthConverter))]
        public double Distance { get { return _distance; } set { _distance = value; } }
        //
        [Browsable(false)]
        public bool MultiView
        {
            get { return _multiView; }
            set
            {
                if (_multiView != value)
                {
                    _multiView = value;
                    UpdateVisibility();
                }
            }
        }
        //
        [Browsable(false)]
        public CaeMesh.MasterSlaveItem MasterSlaveItem { get { return _masterSlaveItem; } set { _masterSlaveItem = value; } }


        // Constructors                                                                                                             
        public SearchContactPair(string name, bool adjust, double adjustmentSize)
        {
            _name = name;
            _type = SearchContactPairType.Tie;
            if (adjust) _adjust = SearchContactPairAdjust.Yes;
            else _adjust = SearchContactPairAdjust.No;
            _distance = adjustmentSize;
            //
            _dctd = ProviderInstaller.Install(this);
            //
            UpdateVisibility();
        }


        // Methods                                                                                                                  
        public void PopulateDropDownLists(string[] surfaceInteractionNames, string[] conctactPairMethodNames)
        {
            _dctd.PopulateProperty(nameof(SurfaceInteractionName), surfaceInteractionNames);
            _dctd.PopulateProperty(nameof(ContactPairMethod), conctactPairMethodNames);
            //
            _dctd.GetProperty(nameof(GeometryTypeName)).SetIsBrowsable(false);
            //
            UpdateVisibility();
        }
        public void SetProperty(string propertyName, SearchContactPair source)
        {
            Type type = typeof(SearchContactPair);
            PropertyInfo[] propertyInfos = type.GetProperties();
            foreach (var property in propertyInfos)
            {
                if (property.Name == propertyName)
                {
                    var o = property.GetValue(source);
                    var p = this.GetType().GetProperty(property.Name);
                    if (p != null)
                    {
                        Type t = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                        object safeValue = (o == null) ? null : Convert.ChangeType(o, t);
                        p.SetValue(this, safeValue);
                    }
                }
            }
        }
        public void UpdateVisibility()
        {
            _dctd.GetProperty(nameof(SurfaceInteractionName)).SetIsBrowsable(_type == SearchContactPairType.Contact);
            _dctd.GetProperty(nameof(ContactPairMethod)).SetIsBrowsable(_type == SearchContactPairType.Contact);
            //
            _dctd.GetProperty(nameof(Name)).SetIsBrowsable(!_multiView);
        }
    }
}
