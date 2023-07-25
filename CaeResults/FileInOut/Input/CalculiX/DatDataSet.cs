using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeResults
{
    struct DatDataSet
    {
        public double Time;
        public string FieldName;
        public string SetName;
        public string[] ComponentNames;
        public bool[] Locals;
        /// <summary>
        /// ComponentNames [0 ... num. of ids/values][0...num. of components] -> value
        /// </summary>
        public double[][] Values;

        public string GetHashKey()
        {
            return FieldName + "_" + SetName + "_" + Time;
        }
    }
}
