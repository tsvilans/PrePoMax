using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using System.ComponentModel;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
    public abstract class InitialCondition : NamedClass, IMultiRegion
    {
        // Variables                                                                                                                
        private RegionTypeEnum _regionType;
        private string _regionName;
        private int[] _creationIds;
        private Selection _creationData;
        protected bool _twoD;


        // Properties                                                                                                               
        public string RegionName { get { return _regionName; } set { _regionName = value; } }
        public RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public int[] CreationIds { get { return _creationIds; } set { _creationIds = value; } }
        public Selection CreationData { get { return _creationData; } set { _creationData = value; } }
        public bool TwoD { get { return _twoD; } }


        // Constructors                                                                                                             
        public InitialCondition(string name, string regionName, RegionTypeEnum regionType, bool twoD)
            : base(name)
        {
            _regionName = regionName;
            _regionType = regionType;
            _creationIds = null;
            _creationData = null;
            _twoD = twoD;
        }


        // Methods                                                                                                                  
    }
}
