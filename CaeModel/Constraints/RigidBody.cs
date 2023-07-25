using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.Runtime.Serialization;


namespace CaeModel
{
    [Serializable]
    public class RigidBody : Constraint, ISerializable // ISerializable must be here
    {
        // Variables                                                                                                                


        // Properties                                                                                                               
        public string ReferencePointName { get { return MasterRegionName; } set { MasterRegionName = value; } }
        public string RegionName { get { return SlaveRegionName; } set { SlaveRegionName = value; } }
        public RegionTypeEnum RegionType { get { return SlaveRegionType; } set { SlaveRegionType = value; } }
        //
        public int[] CreationIds { get { return SlaveCreationIds; } set { SlaveCreationIds = value; } }
        public Selection CreationData { get { return SlaveCreationData; } set { SlaveCreationData = value; } }


        // Constructors                                                                                                             
        public RigidBody(string name, string referencePointName, string regionName, RegionTypeEnum regionType, bool twoD)
            : base(name, referencePointName, RegionTypeEnum.ReferencePointName, regionName, regionType, twoD)
        {
        }
        public RigidBody(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    // Compatibility for version v1.1.1
                    case "_referencePointName":
                        MasterRegionName = (string)entry.Value; break;
                    // Compatibility for version v1.1.1
                    case "_regionName":
                        RegionName = (string)entry.Value; break;
                    // Compatibility for version v1.1.1
                    case "_regionType":
                        RegionType = (RegionTypeEnum)entry.Value; break;
                    // Compatibility for version v1.1.1
                    case "_creationIds":
                        CreationIds = (int[])entry.Value; break;
                    // Compatibility for version v1.1.1
                    case "_creationData":
                        CreationData = (Selection)entry.Value; break;
                    //
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  

        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
        }
    }
}
