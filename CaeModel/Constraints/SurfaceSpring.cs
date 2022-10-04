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
    public class SurfaceSpring : SpringConstraint, ISerializable
    {
        // Variables                                                                                                                
        private bool _stiffnessPerArea;          //ISerializable


        // Properties                                                                                                               
        public bool StiffnessPerArea { get { return _stiffnessPerArea; } set { _stiffnessPerArea = value; } }


        // Constructors                                                                                                             
        public SurfaceSpring(string name, string regionName, RegionTypeEnum regionType, bool twoD)
            : base(name, regionName, regionType, twoD)
        {
            _stiffnessPerArea = false;
        }
        public SurfaceSpring(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_stiffnessPerArea":
                        _stiffnessPerArea = (bool)entry.Value; break;
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
            info.AddValue("_stiffnessPerArea", _stiffnessPerArea, typeof(bool));
        }


    }
}
