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
    public abstract class SpringConstraint : Constraint, ISerializable // ISerializable must be here
    {
        // Variables                                                                                                                
        private double _k1;             //ISerializable
        private double _k2;             //ISerializable
        private double _k3;             //ISerializable


        // Properties                                                                                                               
        public string RegionName { get { return MasterRegionName; } set { MasterRegionName = value; } }
        public RegionTypeEnum RegionType { get { return MasterRegionType; } set { MasterRegionType = value; } }
        //
        public int[] CreationIds { get { return MasterCreationIds; } set { MasterCreationIds = value; } }
        public Selection CreationData { get { return MasterCreationData; } set { MasterCreationData = value; } }
        //
        
        public double K1 { get { return _k1; } set { _k1 = value; if (_k1 < 0) _k1 = 0; } }
        public double K2 { get { return _k2; } set { _k2 = value; if (_k2 < 0) _k2 = 0; } }
        public double K3 { get { return _k3; } set { _k3 = value; if (_k3 < 0 || TwoD) _k3 = 0; } }


        // Constructors                                                                                                             
        public SpringConstraint(string name, string regionName, RegionTypeEnum regionType, bool twoD)
            : base(name, regionName, regionType, "", RegionTypeEnum.None, twoD)
        {
            K1 = 0;
            K2 = 0;
            K3 = 0;
        }
        public SpringConstraint(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_k1":
                        _k1 = (double)entry.Value; break;
                    case "_k2":
                        _k2 = (double)entry.Value; break;
                    case "_k3":
                        _k3 = (double)entry.Value; break;
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  
        public int[] GetSpringDirections()
        {
            List<int> directions = new List<int>();
            if (_k1 > 0) directions.Add(1);
            if (_k2 > 0) directions.Add(2);
            if (_k3 > 0) directions.Add(3);
            return directions.ToArray();
        }
        public double[] GetSpringStiffnessValues()
        {
            List<double> values = new List<double>();
            if (_k1 > 0) values.Add(_k1);
            if (_k2 > 0) values.Add(_k2);
            if (_k3 > 0) values.Add(_k3);
            return values.ToArray();
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_k1", _k1, typeof(double));
            info.AddValue("_k2", _k2, typeof(double));
            info.AddValue("_k3", _k3, typeof(double));
        }
    }
}
