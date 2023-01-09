using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
    [Flags]
    public enum NodalHistoryVariable
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
    public class NodalHistoryOutput : HistoryOutput
    {
        // Variables                                                                                                                
        private NodalHistoryVariable _variables;


        // Properties                                                                                                               
        public NodalHistoryVariable Variables { get { return _variables; } set { _variables = value; } }


        // Constructors                                                                                                             
        public NodalHistoryOutput(string name, NodalHistoryVariable variables, string regionName, RegionTypeEnum regionType)
            : base(name, regionName, regionType)
        {
            _variables = variables;
        }


        // Methods                                                                                                                  
    }
}
