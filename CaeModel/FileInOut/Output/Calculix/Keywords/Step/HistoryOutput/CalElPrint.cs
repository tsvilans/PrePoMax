using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeModel;
using CaeMesh;

namespace FileInOut.Output.Calculix
{
    [Serializable]
    internal class CalElPrint : CalculixKeyword
    {
        // Variables                                                                                                                
        private readonly ElementHistoryOutput _elementHistoryOutput;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalElPrint(ElementHistoryOutput elementHistoryOutput)
        {
            _elementHistoryOutput = elementHistoryOutput;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            string regionName = ", Elset=" + _elementHistoryOutput.RegionName;
            string totals = "";
            if (_elementHistoryOutput.TotalsType == TotalsTypeEnum.Yes) totals = ", Totals=Yes";
            else if (_elementHistoryOutput.TotalsType == TotalsTypeEnum.Only) totals = ", Totals=Only";
            return string.Format("*El print{0}{1}{2}", regionName, totals, Environment.NewLine);
        }
        public override string GetDataString()
        {
            return string.Format("{0}{1}", _elementHistoryOutput.Variables.ToString(), Environment.NewLine);
        }
    }
}
