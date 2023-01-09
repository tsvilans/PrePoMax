using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeModel;
using CaeMesh;
using CaeGlobals;

namespace FileInOut.Output.Calculix
{
    [Serializable]
    internal class CalHeatTransferStep : CalculixKeyword
    {
        // Variables                                                                                                                
        private HeatTransferStep _step;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalHeatTransferStep(HeatTransferStep step)
        {
            _step = step;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            string solver = _step.SolverType == SolverTypeEnum.Default ? "" : ", Solver=" + _step.SolverType.GetDisplayedName();
            string direct = _step.IncrementationType == IncrementationTypeEnum.Direct ? ", Direct" : "";
            string steadyState = _step.SteadyState ? ", Steady state" : "";
            string deltmx = double.IsPositiveInfinity(_step.Deltmx) ? "" : ", Deltmx=" + _step.Deltmx.ToCalculiX16String();
            //
            return string.Format("*Heat transfer{0}{1}{2}{3}{4}", solver, direct, steadyState, deltmx, Environment.NewLine);
        }
        public override string GetDataString()
        {
            string data = "";
            if (_step.IncrementationType != IncrementationTypeEnum.Default)
            {
                string minMax = "";
                if (_step.IncrementationType == IncrementationTypeEnum.Automatic)
                    minMax = string.Format(", {0}, {1}", _step.MinTimeIncrement.ToCalculiX16String(),
                                           _step.MaxTimeIncrement.ToCalculiX16String());
                //
                data = string.Format("{0}, {1}{2}{3}", _step.InitialTimeIncrement.ToCalculiX16String(),
                                     _step.TimePeriod.ToCalculiX16String(), minMax, Environment.NewLine);
            }
            //
            if (!_step.RunAnalysis) data += "*No Analysis" + Environment.NewLine;
            return data;
        }
    }
}
