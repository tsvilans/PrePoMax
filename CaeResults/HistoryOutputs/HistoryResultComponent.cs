using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;

namespace CaeResults
{
    [Serializable]
    public class HistoryResultComponent : NamedClass
    {
        // Variables                                                                                                                
        protected string _unit;
        protected Dictionary<string, HistoryResultEntries> _entries;


        // Properties                                                                                                               
        public string Unit { get { return _unit; } set { _unit = value; } }
        public Dictionary<string, HistoryResultEntries> Entries { get { return _entries; } set { _entries = value; } }


        // Constructor                                                                                                              
        public HistoryResultComponent(string name)
            : base()
        {
            _checkName = false;
            _name = name;
            _unit = null;
            _entries = new Dictionary<string, HistoryResultEntries>();
        }


        // Static methods                                                                                                           


        // Methods                                                                                                                  
       

    }
}
