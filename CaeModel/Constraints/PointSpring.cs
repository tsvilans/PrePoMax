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
    public class PointSpring : SpringConstraint, ISerializable
    {
        // Variables                                                                                                                
        private int _nodeId;                  //ISerializable


        // Properties                                                                                                               
        public int NodeId { get { return _nodeId; } set { _nodeId = value; } }


        // Constructors                                                                                                             
        public PointSpring(string name, string regionName, RegionTypeEnum regionType, bool twoD)
            : base(name, regionName, regionType, twoD)
        {
        }
        public PointSpring(string name, int nodeId, double k1, double k2, double k3, bool twoD)
           : base(name, null, RegionTypeEnum.NodeId, twoD)
        {
            _nodeId = nodeId;
            K1 = k1;
            K2 = k2;
            K3 = k3;
        }

        public PointSpring(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_nodeId":
                        _nodeId = (int)entry.Value; break;
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
            //
            info.AddValue("_nodeId", _nodeId, typeof(int));
        }
    }
}
