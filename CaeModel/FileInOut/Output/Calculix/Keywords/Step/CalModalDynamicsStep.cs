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
    internal class CalModalDynamicsStep : CalculixKeyword
    {
        // Variables                                                                                                                
        private ModalDynamicsStep _step;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalModalDynamicsStep(ModalDynamicsStep step)
        {
            _step = step;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            string solver = _step.SolverType == SolverTypeEnum.Default ? "" : ", Solver=" + _step.SolverType.GetDisplayedName();
            string steadyState = _step.SteadyState ? ", Steady state" : "";
            return string.Format("*Modal dynamics{0}{1}{2}", solver, steadyState, Environment.NewLine);
        }
        public override string GetDataString()
        {
            string data;
            if (_step.SteadyState)
            {
                data = string.Format("{0}, {1}{2}", _step.InitialTimeIncrement.ToCalculiX16String(),
                                                    _step.RelativeError.ToCalculiX16String(),
                                                    Environment.NewLine);
            }
            else
            {
                data = string.Format("{0}, {1}{2}", _step.InitialTimeIncrement.ToCalculiX16String(),
                                                    _step.TimePeriod.ToCalculiX16String(),
                                                    Environment.NewLine);
            }
            
            //
            if (!_step.RunAnalysis) data += "*No Analysis" + Environment.NewLine;
            return data;
        }
    }
}
