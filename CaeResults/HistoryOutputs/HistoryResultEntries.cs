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
    public class HistoryResultEntries : NamedClass
    {
        // Variables                                                                                                                
        private List<double> _time;
        private List<double> _values;
        private List<int> _count;
        private bool _local;
        private string _unit;


        // Properties                                                                                                               
        public List<double> Time { get { return _time; } set { _time = value; } }
        public List<double> Values { get { return _values; } set { _values = value; } }
        public List<int> Count { get { return _count; } set { _count = value; } }
        public bool Local { get { return _local; } set { _local = value; } }
        public string Unit { get { return _unit; } set { _unit = value; } }


        // Constructor                                                                                                              
        public HistoryResultEntries(string name, bool local)
            : base()
        {
            _checkName = false;
            _name = name;
            _time = new List<double>();
            _values = new List<double>();
            _count = new List<int>();
            _local = local;
            _unit = null;
        }


        // Methods                                                                                                                  
        public void Add(double time, double value)
        {
            _time.Add(time);
            _values.Add(value);
            _count.Add(1);
        }
        public void Append(HistoryResultEntries historyResultEntry)
        {
            _time.AddRange(historyResultEntry.Time);
            _values.AddRange(historyResultEntry.Values);
        }
        public void ShiftTime(double timeSthift)
        {
            List<double> newTime = new List<double>();
            foreach (var time in _time) newTime.Add(time + timeSthift);
            _time = newTime;
        }
        public void SumValue(double value)
        {
            _values[_values.Count - 1] += value;
            _count[_count.Count - 1]++;
        }
        public void ComputeAverage()
        {
            for (int i = 0; i < _values.Count; i++)
            {
                if (_count[i] > 1) _values[i] /= _count[i];
            }
        }
        public void KeepOnly(double[][] minMax)
        {
            double[] time = _time.ToArray();
            double[] values = _values.ToArray();
            int[] count = _count.ToArray();
            //
            _time.Clear();
            _values.Clear();
            _count.Clear();
            //
            bool add;
            for (int i = 0; i < time.Length; i++)
            {
                add = false;
                foreach (var pair in minMax)
                {
                    if (pair[0] <= time[i] && time[i] <= pair[1])
                    {
                        add = true;
                        break;
                    }
                }
                //
                if (add)
                {
                    _time.Add(time[i]);
                    _values.Add(values[i]);
                    //
                    if (i >= count.Length) _count.Add(1); // error handling
                    else _count.Add(count[i]);
                }
            }
        }

    }
}
