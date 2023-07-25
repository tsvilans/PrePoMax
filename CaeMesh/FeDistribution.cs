using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeMesh
{
    [Serializable]
    public class FeDistribution : FeGroup
    {
        private FeMaterialOrientation _defaultMaterialOrientation;
        public FeMaterialOrientation DefaultMaterialOrientation
        {
            get { return _defaultMaterialOrientation; }
            set { _defaultMaterialOrientation = value; }
        }

        // Constructors                                                                                                             
        public FeDistribution(string name, FeMaterialOrientation defaultMaterialOrientation, int[] labels)
            :base(name, labels)
        {
            _defaultMaterialOrientation = defaultMaterialOrientation;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            for (int i = 0; i < _labels.Length; i++)
            {
                hash ^= _labels[i].GetHashCode();
            }
            return hash;
        }

    }
}
