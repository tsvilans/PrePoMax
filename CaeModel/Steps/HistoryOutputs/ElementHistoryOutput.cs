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
    public enum ElementHistoryVariable
    {
        // must start at 1 for the UI to work
        S = 1,
        E = 2,
        ME = 4,
        PEEQ = 8,
        // Thermal
        HFL = 16,
        // Whole element
        ENER = 32,
        ELSE = 64,
        ELKE = 128,
        EVOL = 256,
        EBHE = 512
    }

    [Serializable]
    public class ElementHistoryOutput : HistoryOutput
    {
        // Variables                                                                                                                
        private ElementHistoryVariable _variables;


        // Properties                                                                                                               
        public ElementHistoryVariable Variables { get { return _variables; } set { _variables = value; } }


        // Constructors                                                                                                             
        public ElementHistoryOutput(string name, ElementHistoryVariable variables, string regionName, RegionTypeEnum regionType)
            : base(name, regionName, regionType)
        {
            _variables = variables;
        }


        // Methods                                                                                                                  
    }
}
