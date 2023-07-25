using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.Drawing;

namespace CaeModel
{
    [Serializable]
    public class BoundaryCondition : NamedClass, IMultiRegion
    {
        // Variables                                                                                                                
        private string _regionName;
        private RegionTypeEnum _regionType;
        private int[] _creationIds;
        private Selection _creationData;
        protected bool _twoD;
        protected string _amplitudeName;
        protected bool _complex;
        protected double _phaseDeg;
        protected bool _boundaryDisplacementBC;
        protected Color _color;
        public const string DefaultAmplitudeName = "Default";


        // Properties                                                                                                               
        public string RegionName { get { return _regionName; } set { _regionName = value; } }
        public RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public int[] CreationIds { get { return _creationIds; } set { _creationIds = value; } }
        public Selection CreationData { get { return _creationData; } set { _creationData = value; } }
        public bool TwoD { get { return _twoD; } }
        public string AmplitudeName
        {
            get
            {
                if (_amplitudeName == null) return DefaultAmplitudeName;
                else return _amplitudeName;
            }
            set
            {
                _amplitudeName = value;
                if (_amplitudeName == DefaultAmplitudeName) _amplitudeName = null;
            }
        }
        public bool Complex { get { return _complex; } set { _complex = value; } }
        public double PhaseDeg { get { return _phaseDeg; } set { _phaseDeg = Tools.GetPhase360(value); } }
        public Color Color
        {
            get
            {
                // Compatibility for version v0.6.0
                if (_color == Color.Empty) _color = Color.Lime;
                //
                return _color;
            }
            set { _color = value; }
        }


        // Constructors                                                                                                             
        public BoundaryCondition(string name, string regionName, RegionTypeEnum regionType, bool twoD,
                                 bool complex, double phaseDeg)
            : base(name) 
        {
            _regionName = regionName;
            _regionType = regionType;
            _creationIds = null;
            _creationData = null;
            _twoD = twoD;
            _amplitudeName = null;
            _complex = complex;
            PhaseDeg = phaseDeg;    // 360°
            _color = Color.Lime;
        }


        // Methods                                                                                                                  
    }
}
