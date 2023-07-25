using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeMesh
{
    [Serializable]
    public class FeOrientation : NamedClass
    {
        private FeDistribution _distribution;

        public FeDistribution Distribution
        {
            get { return _distribution; }
            set { _distribution = value; }
        }

        // Constructors                                                                                                             
        public FeOrientation(string name, FeDistribution distribution)
            : base(name)
        {
            _distribution = distribution;
        }
    }
}
