using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using DynamicTypeDescriptor;
using CaeGlobals;

namespace PrePoMax
{
    [Serializable]
    public class LimitPartDataPoint
    {
        // Variables                                                                                                                
        private string _partName;
        private double _limit;


        // Properties                                                                                                               
        [DisplayName("Part")]
        public string PartName { get { return _partName; } set { _partName = value; } }
        //
        [DisplayName("Limit")]
        [TypeConverter(typeof(StringDoubleConverter))]
        public double Limit { get { return _limit; } set { _limit = value; } }


        // Constructors                                                                                                             
        public LimitPartDataPoint()
            : this(null, 0)
        {
        }
        public LimitPartDataPoint(string partName, double limit)
        {
            _partName = partName;
            _limit = limit;
        }
    }
}
