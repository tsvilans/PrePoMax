using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using DynamicTypeDescriptor;
using CaeGlobals;
using System.Drawing;

namespace CaeModel
{
    [Serializable]
    public enum ContactPairMethod
    {
        [StandardValue("Node to surface", "Node to surface",  Description =
                       "Use node-to-face penalty contact method. All contact pairs in the model must use the same contact type.")]
        NodeToSurface,
        [StandardValue("Surface to surface", "Surface to surface", Description =
                       "Use face-to-face penalty contact method. All contact pairs in the model must use the same contact type.")]
        SurfaceToSurface,
        [StandardValue("Mortar", "Mortar", Description =
                       "Use mortar contact method. All contact pairs in the model must use the same contact type.")]
        Mortar,
        [StandardValue("Linear mortar", "Linear mortar", Description =
                       "Use linear mortar contact method. All contact pairs in the model must use the same contact type.")]
        LinMortar,
        [StandardValue("PG linear mortar", "PG linear mortar", Description =
                       "Use Petrov-Galerkin linear mortar contact method. " + 
                       "All contact pairs in the model must use the same contact type.")]
        PGLinMortar
    }

    [Serializable]
    public class ContactPair : NamedClass, IMasterSlaveMultiRegion
    {
        // Variables                                                                                                                
        private static string _positive = "The value must be larger than 0.";
        //
        private string _surfaceInteractionName;
        private ContactPairMethod _method;
        private bool _smallSliding;
        private bool _adjust;
        private double _adjustmentSize;
        //
        private RegionTypeEnum _slaveRegionType;
        private string _slaveSurfaceName;
        private RegionTypeEnum _masterRegionType;
        private string _masterSurfaceName;
        //
        private int[] _slaveCreationIds;
        private Selection _slaveCreationData;
        private int[] _masterCreationIds;
        private Selection _masterCreationData;
        //
        protected Color _masterColor;
        protected Color _slaveColor;


        // Properties                                                                                                               
        public string SurfaceInteractionName { get { return _surfaceInteractionName; } set { _surfaceInteractionName = value; } }
        public ContactPairMethod Method
        {
            get { return _method; }
            set
            {
                _method = value;
                if (_method == ContactPairMethod.SurfaceToSurface) _smallSliding = false;
            }
        }
        public bool SmallSliding
        {
            get { return _smallSliding; }
            set
            {
                _smallSliding = value;
                Method = _method; // check the compatibility
            }
        }
        public bool Adjust { get { return _adjust; } set { _adjust = value; } }
        public double AdjustmentSize 
        {
            get { return _adjustmentSize; }
            set { if (double.IsNaN(value) || value >= 0) _adjustmentSize = value; else throw new CaeException(_positive); }
        }
        //
        public RegionTypeEnum MasterRegionType { get { return _masterRegionType; } set { _masterRegionType = value; } }
        public string MasterRegionName { get { return _masterSurfaceName; } set { _masterSurfaceName = value; } }
        public RegionTypeEnum SlaveRegionType { get { return _slaveRegionType; } set { _slaveRegionType = value; } }
        public string SlaveRegionName { get { return _slaveSurfaceName; } set { _slaveSurfaceName = value; } }                
        //
        public int[] SlaveCreationIds { get { return _slaveCreationIds; } set { _slaveCreationIds = value; } }
        public Selection SlaveCreationData { get { return _slaveCreationData; } set { _slaveCreationData = value; } }
        public int[] MasterCreationIds { get { return _masterCreationIds; } set { _masterCreationIds = value; } }
        public Selection MasterCreationData { get { return _masterCreationData; } set { _masterCreationData = value; } }
        //
        public Color MasterColor
        {
            get
            {
                // Compatibility for version v0.6.0
                if (_masterColor == Color.Empty) _masterColor = Color.Yellow;
                //
                return _masterColor;
            }
            set { _masterColor = value; }
        }
        public Color SlaveColor
        {
            get
            {
                // Compatibility for version v0.6.0
                if (_slaveColor == Color.Empty) _slaveColor = Color.Yellow;
                //
                return _slaveColor;
            }
            set { _slaveColor = value; }
        }


        // Constructors                                                                                                             
        public ContactPair(string name, string surfaceInteractionName, string masterSurfaceName, RegionTypeEnum masterRegionType,
                           string slaveSurfaceName, RegionTypeEnum slaveRegionType)
           : this(name, surfaceInteractionName, ContactPairMethod.SurfaceToSurface, false, true, double.NaN,
                  masterSurfaceName, masterRegionType, slaveSurfaceName, slaveRegionType)
        {
        }
        public ContactPair(string name, string surfaceInteractionName, ContactPairMethod method, bool smallSliding, bool adjust,
                           double adjustmentSize, string masterSurfaceName, RegionTypeEnum masterRegionType, 
                           string slaveSurfaceName, RegionTypeEnum slaveRegionType)
            : base(name)
        {
            // Same name can be used for single surface contact
            //if (masterRegionType == RegionTypeEnum.SurfaceName && slaveRegionType == RegionTypeEnum.SurfaceName &&
            //    slaveSurfaceName == masterSurfaceName) throw new CaeException("The master and slave surface names must be different.");
            //
            _surfaceInteractionName = surfaceInteractionName;
            Method = method;
            SmallSliding = smallSliding;
            _adjust = adjust;
            AdjustmentSize = adjustmentSize;
            //
            _masterRegionType = masterRegionType;
            _masterSurfaceName = masterSurfaceName;
            _slaveRegionType = slaveRegionType;
            _slaveSurfaceName = slaveSurfaceName;
            //
            _slaveCreationIds = null;
            _slaveCreationData = null;
            _masterCreationIds = null;
            _masterCreationData = null;
            //
            _masterColor = Color.Yellow;
            _slaveColor = Color.Yellow;
        }


        // Methods                                                                                                                  
        public void SwapMasterSlave()
        {
            if (_name.Contains(Globals.MasterSlaveSeparator))
            {
                string[] tmp = _name.Split(new string[] { Globals.MasterSlaveSeparator }, StringSplitOptions.None);
                if (tmp.Length == 2) _name = tmp[1] + Globals.MasterSlaveSeparator + tmp[0];
            }
            //
            RegionTypeEnum tmpRegionType = _masterRegionType;
            _masterRegionType = _slaveRegionType;
            _slaveRegionType = tmpRegionType;
            //
            string tmpSurfaceName = _masterSurfaceName;
            _masterSurfaceName = _slaveSurfaceName;
            _slaveSurfaceName = tmpSurfaceName;
            //
            int[] tmpCreationIds = _masterCreationIds;
            _masterCreationIds = _slaveCreationIds;
            _slaveCreationIds = tmpCreationIds;
            //
            Selection tmpCreationData = _masterCreationData;
            _masterCreationData = _slaveCreationData;
            _slaveCreationData = tmpCreationData;
        }

    }
}
