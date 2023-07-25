using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeResults
{
    [Serializable]
    public class ResultsCollection
    {
        // Variables                                                                                                                
        private FeResults _currentResult;
        private OrderedDictionary<string, FeResults> _results;


        // Properties                                                                                                               
        public int Count { get { return _results.Count; } }
        public FeResults CurrentResult { get { return _currentResult; } }


        //  Constructors                                                                                                            
        public ResultsCollection()
        {
            _results = new OrderedDictionary<string, FeResults>("Results Collection", StringComparer.OrdinalIgnoreCase);
            //
            Clear();
        }


        // Metods
        public static void PrepareForSavig(ResultsCollection resultsCollection)
        {
            foreach (var entry in resultsCollection._results)
            {
                if (entry.Value != null) CaeMesh.FeMesh.PrepareForSavig(entry.Value.Mesh);
            }
        }
        public static void ResetAfterSavig(ResultsCollection resultsCollection)
        {
            foreach (var entry in resultsCollection._results)
            {
                if (entry.Value != null) CaeMesh.FeMesh.ResetAfterSavig(entry.Value.Mesh);
            }
        }
        public static void WriteToFile(ResultsCollection allResults, System.IO.BinaryWriter bw)
        {
            bw.Write(allResults.Count);
            //
            foreach (var entry in allResults._results)
            {
                bw.Write(entry.Key);
                FeResults.WriteToFile(entry.Value, bw);
            }
        }
        public static void ReadFromFile(ResultsCollection allResults, System.IO.BinaryReader br, int version)
        {
            string resultsName;
            int numResults = br.ReadInt32();
            //
            if (numResults > 0)
            {
                for (int i = 0; i < numResults; i++)
                {
                    resultsName = br.ReadString();
                    allResults.SetCurrentResult(resultsName);
                    if (allResults.CurrentResult != null) FeResults.ReadFromFile(allResults.CurrentResult, br, version);
                }
            }
        }
        //
        public void Clear()
        {
            _currentResult = null;
            _results.Clear();
        }
        public string[] GetResultNames()
        {
            return _results.Keys.ToArray();
        }
        public string GetCurrentResultName()
        {
            foreach (var entry in _results)
            {
                if (entry.Value == _currentResult) return entry.Key;
            }
            return null;
        }
        public FeResults GetResult(string name)
        {
            FeResults result;
            _results.TryGetValue(name, out result);
            return result;
        }
        public bool ContainsResult(string name)
        {
            return _results.ContainsKey(name);
        }
        public bool ContainsComplexResults()
        {
            foreach (var entry in _results)
            {
                if (entry.Value.ContainsComplexResults()) return true;
            }
            return false;
        }
        //
        public void Add(string name, FeResults result)
        {
            if (_results.ContainsKey(name)) _results.Replace(name, name, result);
            else _results.Add(name, result);
            //
            _currentResult = result;
        }
        //
        public void SetCurrentResult(string name)
        {
            _results.TryGetValue(name, out _currentResult);
        }
        public void RemoveCurrentResult()
        {
            if (_results.Count == 0) { }
            else if (_results.Count == 1) Clear();
            else if (_currentResult != null)
            {
                int count = 0;
                string name = null;
                foreach (var entry in _results)
                {
                    if (entry.Value == _currentResult)
                    {
                        name = entry.Key;
                        break;
                    }
                    //
                    count++;
                }
                //
                if (name != null) _results.Remove(name);
                //
                string[] remainingNames = _results.Keys.ToArray();
                if (count >= remainingNames.Length) count = remainingNames.Length - 1;
                SetCurrentResult(remainingNames[count]);
            }
        }


    }
}
