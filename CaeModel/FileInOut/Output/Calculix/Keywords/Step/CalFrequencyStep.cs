using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeModel;
using CaeGlobals;

namespace FileInOut.Output.Calculix
{
    [Serializable]
    internal class CalFrequencyStep : CalculixKeyword
    {
        // Variables                                                                                                                
        private FrequencyStep _step;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalFrequencyStep(FrequencyStep step)
        {
            _step = step;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            string solver = _step.SolverType == SolverTypeEnum.Default ? "" : ", Solver=" + _step.SolverType.GetDisplayedName();
            string storage = _step.Storage ? ", Storage=Yes" : "";
            return string.Format("*Frequency{0}{1}{2}", solver, storage, Environment.NewLine);
        }
        public override string GetDataString()
        {
            string upperFrequency = double.IsNaN(_step.UpperFrequency) ? "" : ", " + _step.UpperFrequency.ToCalculiX16String();
            string lowerFrequency;
            if (double.IsNaN(_step.LowerFrequency))
            {
                if (upperFrequency == "") lowerFrequency = "";
                else lowerFrequency = ", 0";
            }
            else lowerFrequency = ", " + _step.LowerFrequency.ToCalculiX16String();
            //
            string data = string.Format("{0}{1}{2}{3}", _step.NumOfFrequencies, lowerFrequency, upperFrequency,
                                        Environment.NewLine);
            if (!_step.RunAnalysis) data += "*No Analysis" + Environment.NewLine;
            return data;
        }
    }
}
