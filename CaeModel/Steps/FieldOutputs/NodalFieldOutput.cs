using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using System.ComponentModel;

namespace CaeModel
{
    [Serializable]
    [Flags]
    public enum NodalFieldVariable
    {
        // Must start at 1 for the UI to work
        RF = 1,
        U = 2,
        V = 4,
        // Thermal
        NT = 8,
        RFL = 16
    }

    [Serializable]
    public class NodalFieldOutput : FieldOutput
    {
        // Variables                                                                                                                
        private NodalFieldVariable _variables;        


        // Properties                                                                                                               
        public NodalFieldVariable Variables { get { return _variables; } set { _variables = value; } }


        // Constructors                                                                                                             
        public NodalFieldOutput(string name, NodalFieldVariable variables)
            : base(name) 
        {
            _variables |= variables;
        }


        // Methods                                                                                                                  
    }
}
