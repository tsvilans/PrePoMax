using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
    public abstract class FieldOutput : NamedClass
    {
        // Variables                                                                                                                
        private bool _lastIterations;
        private bool _contactElements;


        // Properties                                                                                                               
        public bool LastIterations { get { return _lastIterations; } set { _lastIterations = value; } }
        public bool ContactElements { get { return _contactElements; } set { _contactElements = value; } }


        // Constructors                                                                                                             
        public FieldOutput(string name)
            : base(name)
        {
            _lastIterations = false;
            _contactElements = false;
        }


        // Methods                                                                                                                  
    }
}
