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
    internal class CalContactPrint : CalculixKeyword
    {
        // Variables                                                                                                                
        private readonly ContactHistoryOutput _contactHistoryOutput;
        private string _masterName;
        private string _slaveName;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalContactPrint(ContactHistoryOutput contactHistoryOutput, string masterName, string slaveName)
        {
            _contactHistoryOutput = contactHistoryOutput;
            _masterName = masterName;
            _slaveName = slaveName;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            string totals = "";
            if (_contactHistoryOutput.TotalsType == TotalsTypeEnum.Yes) totals = ", Totals=Yes";
            else if (_contactHistoryOutput.TotalsType == TotalsTypeEnum.Only) totals = ", Totals=Only";
            string masterSlave = "";
            if (_contactHistoryOutput.Variables.HasFlag(ContactHistoryVariable.CF))
                masterSlave = ", Master=" + _masterName + ", Slave=" + _slaveName;
            return string.Format("*Contact print{0}{1}{2}", totals, masterSlave, Environment.NewLine);
        }
        public override string GetDataString()
        {
            return string.Format("{0}{1}", _contactHistoryOutput.Variables.ToString(), Environment.NewLine);
        }
    }
}
