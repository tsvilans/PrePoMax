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
    public class HistoryResults : NamedClass
    {
        
        // Variables                                                                                                                
        protected Dictionary<string, HistoryResultSet> _sets;


        // Properties                                                                                                               
        public Dictionary<string, HistoryResultSet> Sets { get { return _sets; } set { _sets = value; } }


        // Constructor                                                                                                              
        /// <summary>
        /// time = HistoryResults.Sets.Fields.Components.Entries.Time
        /// values = HistoryResults.Sets.Fields.Components.Entries.Values
        /// </summary>
        /// <param name="name"></param>
        public HistoryResults(string name)
            : base(name)
        {
            _sets = new Dictionary<string, HistoryResultSet>();
        }


        // Static methods                                                                                                           


        // Methods                                                                                                                  
        public void AppendSets(HistoryResults historyResults)
        {
            HistoryResultSet set;
            foreach (var entry in historyResults.Sets)
            {
                if (_sets.TryGetValue(entry.Key, out set)) set.AppendFields(entry.Value);
                else _sets.Add(entry.Key, entry.Value);
            }
        }

    }
}
