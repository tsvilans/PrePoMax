using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;
using CaeMesh;

namespace CaeModel
{
    [Serializable]
    public enum SolidSectionType
    {
        ThreeDimensional,
        TwoDimensional
    }

    [Serializable]
    public class SolidSection : Section
    {
        // Variables                                                                                                                
        private double _thickness;
        private FeOrientation _orientation = null;


        // Properties                                                                                                               
        public double Thickness { get { return _thickness; } set { _thickness = value; } }
        public FeOrientation Orientation { get { return _orientation; } set { _orientation = value; } }


        // Constructors                                                                                                             
        public SolidSection(string name, string materialName, string regionName, RegionTypeEnum regionType, double thickness,
                            bool twoD)
            : base(name, materialName, regionName, regionType, twoD)
        {
            _thickness = thickness;
        }
    }
}
